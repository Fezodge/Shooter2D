using System;
using System.Collections.Generic;
using System.Windows.Forms;
using EzGame;
using EzGame.Input;
using EzGame.Perspective.Planar;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Screen = EzGame.Screen;

namespace Shooter2D
{
    public class Game : Microsoft.Xna.Framework.Game
    {
        public static float Speed = 1;
        public static States State = States.MainMenu;
        public static Map Map;
        private static Camera Camera { get { return Map.Camera; } }
        public static Player Self;
        public static Player[] Players;
        public static string MpName = "Guest";

        public static ulong Version
        {
            get { return Globe.Version; }
            set { Globe.Version = value; }
        }

        public Game()
        {
            Globe.GraphicsDeviceManager = new GraphicsDeviceManager(this)
            {PreferredBackBufferWidth = 960, PreferredBackBufferHeight = 540};
        }

        protected override void LoadContent()
        {
            #region Globalization

            Globe.Batches = new Batch[1] {new Batch(GraphicsDevice)};
            Globe.Form = (Form) Control.FromHandle(Window.Handle);
            Globe.GameWindow = Window;
            Globe.ContentManager = Content;
            Globe.GraphicsDevice = GraphicsDevice;
            
            Globe.Viewport = GraphicsDevice.Viewport;
            Globe.GraphicsAdapter = GraphicsDevice.Adapter;
            Globe.TextureLoader = new TextureLoader(Globe.GraphicsDevice);
            Textures.LoadTextures(System.IO.Path.Combine(Globe.ExecutableDirectory, "Content"));

            #endregion

            Screen.Expand(true);
            Sound.Initialize(256);
            Performance.UpdateFramesPerSecondBuffer = new float[180];
            Performance.DrawFramesPerSecondBuffer = new float[3];
            MultiPlayer.Initialize();
            IsMouseVisible = true;

            Mod.Fore = Mod.Load(@".\Content\Fore.xml");
            Mod.Back = Mod.Load(@".\Content\Back.xml");
        }

        protected override void Update(GameTime Time)
        {
            Mouse.Update();
            Keyboard.Update(Time);
            XboxPad.Update(Time);
            Timers.Update(Time);
            Performance.Update(Time);

            Globe.Active = IsActive;
            if (XboxPad.Pressed(XboxPad.Buttons.Back) || Keyboard.Pressed(Keyboard.Keys.Escape)) Exit();

            Profiler.Start("Game Update");
            switch (State)
            {
                #region MainMenu

                case States.MainMenu:
                    if (Keyboard.Pressed(Keyboard.Keys.F1))
                    {
                        CreateLobby("Server");
                        State = States.Game;
                    }
                    else if (Keyboard.Pressed(Keyboard.Keys.F2))
                    {
                        MultiPlayer.Connect("Game", "127.0.0.1", 6121, Globe.Version, MpName);
                    }
                    else if (Keyboard.Pressed(Keyboard.Keys.D3))
                    {
                        //Map = new Map(60, 34);
                        Map = Map.Load(@".\map.dat");
                        State = States.MapEditor;
                    }
                    break;

                #endregion

                #region MapEditor
                case States.MapEditor:
                    Point MousePoint = new Point((int)(Mouse.CameraPosition.X / Tile.Width), (int)(Mouse.CameraPosition.Y / Tile.Height));
                    #region Camera Movement
                    if (Keyboard.Holding(Keyboard.Keys.W)) Camera.Position.Y -= (float)(Map.Speed.Y * Time.ElapsedGameTime.TotalSeconds);
                    if (Keyboard.Holding(Keyboard.Keys.S)) Camera.Position.Y += (float)(Map.Speed.Y * Time.ElapsedGameTime.TotalSeconds);
                    if (Keyboard.Holding(Keyboard.Keys.A)) Camera.Position.X -= (float)(Map.Speed.X * Time.ElapsedGameTime.TotalSeconds);
                    if (Keyboard.Holding(Keyboard.Keys.D)) Camera.Position.X += (float)(Map.Speed.X * Time.ElapsedGameTime.TotalSeconds);
                    #endregion
                    if (Keyboard.Holding(Keyboard.Keys.D1)) Map.ClearFore(MousePoint.X, MousePoint.Y, true);
                    if (Keyboard.Holding(Keyboard.Keys.D2)) Map.PlaceFore(1, MousePoint.X, MousePoint.Y, null, true);
                    if (Keyboard.Holding(Keyboard.Keys.D3)) Map.PlaceBack(1, MousePoint.X, MousePoint.Y, true);
                    break;
                #endregion

                #region Game

                case States.Game:
                    Map.Update(Time);
                    for (byte i = 0; i < Players.Length; i++)
                        if (Players[i] != null)
                        {
                            Players[i].Update(Time);
                        }
                    if (Timers.Tick("Positions") && (MultiPlayer.Type("Game") == MultiPlayer.Types.Server))
                        foreach (var Player1 in Players)
                            if (Player1 != null && (Player1.Connection != null))
                            {
                                var O = MultiPlayer.Construct("Game", Packets.Position);
                                foreach (var Player2 in Players)
                                    if ((Player2 != Player1) && (Player2 != null))
                                    {
                                        O.Write(Player2.Slot);
                                        O.Write(Player2.Position);
                                        O.Write(Player2.Angle);
                                    }
                                MultiPlayer.SendTo("Game", O, Player1.Connection, NetDeliveryMethod.UnreliableSequenced, 1);
                            }
                    break;

                    #endregion
            }
            Profiler.Stop("Game Update");

            #region Networking

            MultiPlayer.Flush("Game");
            NetIncomingMessage I;
            while ((I = MultiPlayer.Read("Game")) != null)
            {
                var Break = false;
                switch (I.MessageType)
                {
                    case NetIncomingMessageType.ConnectionApproval:
                        Read(Packets.Connection, I);
                        break;
                    case NetIncomingMessageType.Data:
                        Read((Packets) I.ReadByte(), I);
                        break;
                    case NetIncomingMessageType.StatusChanged:
                        var Status = ((MultiPlayer.Type("Game") == MultiPlayer.Types.Client)
                            ? (NetConnectionStatus) I.ReadByte()
                            : ((MultiPlayer.Type("Game") == MultiPlayer.Types.Server)
                                ? I.SenderConnection.Status
                                : NetConnectionStatus.None));
                        switch (Status)
                        {
                            case NetConnectionStatus.Connected:
                                if (MultiPlayer.Type("Game") == MultiPlayer.Types.Client)
                                {
                                    var Hail = I.SenderConnection.RemoteHailMessage;
                                    Read((Packets) Hail.ReadByte(), Hail);
                                }
                                break;
                            case NetConnectionStatus.Disconnected:
                                if ((MultiPlayer.Type("Game") == MultiPlayer.Types.Client) &&
                                    (I.SenderConnection.Status == NetConnectionStatus.Disconnected))
                                {
                                    QuitLobby();
                                    Break = true;
                                }
                                else Read(Packets.Disconnection, I);
                                break;
                        }
                        break;
                }
                if (Break) break;
            }

            #endregion

            base.Update(Time);
        }

        protected override void Draw(GameTime Time)
        {
            Performance.Draw(Time);
            GraphicsDevice.Clear(Color.CornflowerBlue);

            Profiler.Start("Game Draw");
            switch (State)
            {
                #region MainMenu
                case States.MainMenu:
                    Globe.Batches[0].Draw(Textures.Get("cog"), Screen.ViewportBounds);
                    break;
                #endregion

                #region MapEditor
                case States.MapEditor:
                    Globe.Batches[0].Begin(SpriteSortMode.BackToFront, Camera.View);
                    Map.Draw();
                    Globe.Batches[0].End();
                    break;
                #endregion

                #region Game
                case States.Game:
                    Globe.Batches[0].Begin(SpriteSortMode.BackToFront, Camera.View);
                    Map.Draw();
                    for (byte i = 0; i < Players.Length; i++)
                        if (Players[i] != null)
                        {
                            Players[i].Draw();
                        }
                    Globe.Batches[0].End();
                    break;
                    #endregion
            }
            Profiler.Stop("Game Draw");

            //Performance.Draw(Fonts.Get("Default/ExtraSmall"), new Vector2(5, (Screen.ViewportHeight - 35)), Origin.None, Color.White, Color.Black);
            //Profiler.Draw(430);

            Batch.EndAll();
            base.Draw(Time);
        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            if ((Map != null) && (State == States.MapEditor) && MultiPlayer.IsNullOrServer()) Map.Save(@".\map.dat");
            QuitLobby();
            base.OnExiting(sender, args);
        }

        public static bool CreateLobby(string Name)
        {
            if (MultiPlayer.Type("Game") == null)
            {
                Map = Map.Load(@".\map.dat");
                Players = new Player[10];
                Self = Player.Add(new Player(Name));
                MultiPlayer.Start("Game", 6121, Players.Length);
                Timers.Add("Positions", (1/30d));
                return true;
            }
            return false;
        }
        public static void QuitLobby()
        {
            MultiPlayer.Shutdown("Game", string.Empty);
            Players = null;
            Map = null;
            Timers.Remove("Positions");
            State = States.MainMenu;
        }
        public static void Read(Packets Packet, NetIncomingMessage I)
        {
            switch (Packet)
            {
                case Packets.Connection:
                    if (MultiPlayer.Type("Game") == MultiPlayer.Types.Server)
                    {
                        var ClientVersion = I.ReadUInt64();
                        if (ClientVersion == Globe.Version)
                        {
                            var Connector = Player.Add(new Player(I.ReadString()) {Connection = I.SenderConnection});
                            if (Connector != null)
                            {
                                MultiPlayer.Send("Game",
                                    MultiPlayer.Construct("Game", Packet, Connector.Slot, Connector.Name),
                                    I.SenderConnection);
                                var Details = new List<object>();
                                for (byte i = 0; i < Players.Length; i++)
                                    if ((Players[i] != null) && (Players[i] != Connector))
                                    {
                                        Details.Add(true);
                                        Details.Add(Players[i].Name);
                                    }
                                    else Details.Add(false);
                                I.SenderConnection.Approve(MultiPlayer.Construct("Game", Packets.Initial,
                                    (byte) Players.Length, Connector.Slot, Details));
                            }
                            else I.SenderConnection.Deny("Full");
                        }
                        else
                            I.SenderConnection.Deny("Version indifference, Client: " + ClientVersion + " - Server: " +
                                                    Globe.Version);
                    }
                    else if (MultiPlayer.Type("Game") == MultiPlayer.Types.Client)
                    {
                        var Slot = I.ReadByte();
                        Player.Set(Slot, new Player(I.ReadString()));
                    }
                    break;
                case Packets.Disconnection:
                    var Disconnector = ((MultiPlayer.Type("Game") == MultiPlayer.Types.Server)
                        ? Player.Get(I.SenderConnection)
                        : ((MultiPlayer.Type("Game") == MultiPlayer.Types.Client) ? Players[I.ReadByte()] : null));
                    if (Disconnector != null) Player.Remove(Disconnector);
                    if (MultiPlayer.Type("Game") == MultiPlayer.Types.Server)
                        MultiPlayer.Send("Game", MultiPlayer.Construct("Game", Packets.Disconnection, Disconnector.Slot),
                            I.SenderConnection);
                    break;
                case Packets.Initial:
                    if (MultiPlayer.Type("Game") == MultiPlayer.Types.Client)
                    {
                        Players = new Player[I.ReadByte()];
                        Self = Player.Set(I.ReadByte(), new Player(MpName));
                        for (byte i = 0; i < Players.Length; i++)
                            if (I.ReadBoolean())
                            {
                                Players[i] = new Player(i, I.ReadString());
                            }
                        Timers.Add("Positions", (1/30d));
                        State = States.Game;
                    }
                    break;
                case Packets.Position:
                    var Sender = ((MultiPlayer.Type("Game") == MultiPlayer.Types.Server)
                        ? Player.Get(I.SenderConnection)
                        : null);
                    Vector2 Position;
                    float Angle;
                    if (MultiPlayer.Type("Game") == MultiPlayer.Types.Server)
                    {
                        if (Sender != null)
                        {
                            Sender.Position = I.ReadVector2();
                            Sender.Angle = I.ReadFloat();
                        }
                    }
                    else if (MultiPlayer.Type("Game") == MultiPlayer.Types.Client)
                    {
                        var Count = (byte) ((I.LengthBytes - 1)/12);
                        for (byte i = 0; i < Count; i++)
                        {
                            Sender = Players[I.ReadByte()];
                            Position = I.ReadVector2();
                            Angle = I.ReadFloat();
                            if (Sender != null)
                            {
                                Sender.Position = Position;
                                Sender.Angle = Angle;
                            }
                        }
                    }
                    break;
                case Packets.PlaceFore:
                    ushort ID = I.ReadUInt16(), x = I.ReadUInt16(), y = I.ReadUInt16(); byte TAngle = I.ReadByte();
                    Map.PlaceFore(ID, x, y, TAngle);
                    if (MultiPlayer.Type("Game") == MultiPlayer.Types.Server) MultiPlayer.Send(MultiPlayer.Construct(Packet, ID, x, y, TAngle), I.SenderConnection);
                    break;
                case Packets.ClearFore:
                    x = I.ReadUInt16(); y = I.ReadUInt16();
                    Map.ClearFore(x, y);
                    if (MultiPlayer.Type("Game") == MultiPlayer.Types.Server) MultiPlayer.Send(MultiPlayer.Construct(Packet, x, y), I.SenderConnection);
                    break;
                case Packets.PlaceBack:
                    ID = I.ReadUInt16(); x = I.ReadUInt16(); y = I.ReadUInt16();
                    Map.PlaceBack(ID, x, y);
                    if (MultiPlayer.Type("Game") == MultiPlayer.Types.Server) MultiPlayer.Send(MultiPlayer.Construct(Packet, ID, x, y), I.SenderConnection);
                    break;
                case Packets.ClearBack:
                    x = I.ReadUInt16(); y = I.ReadUInt16();
                    Map.ClearBack(x, y);
                    if (MultiPlayer.Type("Game") == MultiPlayer.Types.Server) MultiPlayer.Send(MultiPlayer.Construct(Packet, x, y), I.SenderConnection);
                    break;
            }
        }

        public enum Packets
        {
            Connection,
            Disconnection,
            Initial,
            Position,
            PlaceFore,
            ClearFore,
            PlaceBack,
            ClearBack
        }

        public enum States
        {
            MainMenu,
            Game,
            MapEditor
        }
    }
}