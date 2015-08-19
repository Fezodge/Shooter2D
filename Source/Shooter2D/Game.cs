using Lidgren.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using EzGame;
using EzGame.Input;
using EzGame.Perspective.Planar;

namespace Shooter2D
{
    public class Game : Microsoft.Xna.Framework.Game
    {
        public static ulong Version { get { return Globe.Version; } set { Globe.Version = value; } }
        public static float Speed = 1;
        public enum States { MainMenu, Game }
        public static States State = States.MainMenu;

        public enum Packets { Connection, Disconnection, Initial, Position }

        public static Player Self;
        public static Player[] Players;

        public Game()
        {
            Globe.GraphicsDeviceManager = new GraphicsDeviceManager(this)
            { PreferredBackBufferWidth = 960, PreferredBackBufferHeight = 540, SynchronizeWithVerticalRetrace = false };
            Content.RootDirectory = "Content";
        }

        protected override void LoadContent()
        {
            #region Globalization
            Globe.Batches = new Batch[1] { new Batch(GraphicsDevice) };
            Globe.Form = (System.Windows.Forms.Form)System.Windows.Forms.Form.FromHandle(Window.Handle);
            Globe.GameWindow = Window;
            Globe.ContentManager = Content;
            Globe.GraphicsDevice = GraphicsDevice;
            Globe.Viewport = GraphicsDevice.Viewport;
            Globe.GraphicsAdapter = GraphicsDevice.Adapter;
            #endregion
            Sound.Initialize(256);
            Performance.UpdateFramesPerSecondBuffer = new float[180];
            Performance.DrawFramesPerSecondBuffer = new float[3];
            MultiPlayer.Initialize();
            IsMouseVisible = true;
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
                    if (Keyboard.Pressed(Keyboard.Keys.F1)) { CreateLobby("Server"); State = States.Game; }
                    else if (Keyboard.Pressed(Keyboard.Keys.F2)) { MultiPlayer.Connect("Game", "127.0.0.1", 6121, Globe.Version, MpName); }
                    break;
                #endregion
                #region Game
                case States.Game:
                    for (byte i = 0; i < Players.Length; i++)
                        if (Players[i] != null)
                        {
                            Players[i].Update(Time);
                        }

                    if (Timers.Tick("Positions") && (MultiPlayer.Type("Game") == MultiPlayer.Types.Server))
                        foreach (Player Player1 in Players)
                            if (Player1 != null && (Player1.Connection != null))
                            {
                                NetOutgoingMessage O = MultiPlayer.Construct("Game", Packets.Position);
                                foreach (Player Player2 in Players)
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
                bool Break = false;
                switch (I.MessageType)
                {
                    case NetIncomingMessageType.ConnectionApproval: Read(Packets.Connection, I); break;
                    case NetIncomingMessageType.Data: Read((Packets)I.ReadByte(), I); break;
                    case NetIncomingMessageType.StatusChanged:
                        NetConnectionStatus Status = ((MultiPlayer.Type("Game") == MultiPlayer.Types.Client) ? (NetConnectionStatus)I.ReadByte() :
                            ((MultiPlayer.Type("Game") == MultiPlayer.Types.Server) ? I.SenderConnection.Status : NetConnectionStatus.None));
                        switch (Status)
                        {
                            case NetConnectionStatus.Connected:
                                if (MultiPlayer.Type("Game") == MultiPlayer.Types.Client)
                                {
                                    NetIncomingMessage Hail = I.SenderConnection.RemoteHailMessage;
                                    Read((Packets)Hail.ReadByte(), Hail);
                                }
                                break;
                            case NetConnectionStatus.Disconnected:
                                if ((MultiPlayer.Type("Game") == MultiPlayer.Types.Client) && (I.SenderConnection.Status == NetConnectionStatus.Disconnected))
                                { QuitLobby(); Break = true; }
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
                #region MainMenun
                case States.MainMenu:
                    break;
                #endregion
                #region Game
                case States.Game:
                    break;
                    #endregion
            }
            Profiler.Stop("Game Draw");

            //Performance.Draw(Fonts.Get("Default/ExtraSmall"), new Vector2(5, (Screen.ViewportHeight - 35)), Origin.None, Color.White, Color.Black);
            //Profiler.Draw(430);

            Batch.EndAll();
            base.Draw(Time);
        }

        public static string MpName = "Guest";
        protected override void OnExiting(object sender, EventArgs args) { QuitLobby(); base.OnExiting(sender, args); }
        public static bool CreateLobby(string Name)
        {
            if (MultiPlayer.Type("Game") == null)
            {
                Players = new Player[10];
                Self = Player.Add(new Player(Name));
                MultiPlayer.Start("Game", 6121, Players.Length);
                Timers.Add("Positions", (1 / 30d));
                return true;
            }
            else return false;
        }
        public static void QuitLobby()
        {
            MultiPlayer.Shutdown("Game", string.Empty);
            Players = null;
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
                        ulong ClientVersion = I.ReadUInt64();
                        if (ClientVersion == Globe.Version)
                        {
                            Player Connector = Player.Add(new Player(I.ReadString()) { Connection = I.SenderConnection });
                            if (Connector != null)
                            {
                                MultiPlayer.Send("Game", MultiPlayer.Construct("Game", Packet, Connector.Slot, Connector.Name), I.SenderConnection);
                                List<object> Details = new List<object>();
                                for (byte i = 0; i < Players.Length; i++)
                                    if ((Players[i] != null) && (Players[i] != Connector))
                                    {
                                        Details.Add(true);
                                        Details.Add(Players[i].Name);
                                    }
                                    else Details.Add(false);
                                I.SenderConnection.Approve(MultiPlayer.Construct("Game", Packets.Initial, (byte)Players.Length, Connector.Slot, Details));
                            }
                            else I.SenderConnection.Deny("Full");
                        }
                        else I.SenderConnection.Deny("Version indifference, Client: " + ClientVersion + " - Server: " + Globe.Version);
                    }
                    else if (MultiPlayer.Type("Game") == MultiPlayer.Types.Client) { byte Slot = I.ReadByte(); Player.Set(Slot, new Player(I.ReadString())); }
                    break;
                case Packets.Disconnection:
                    Player Disconnector = ((MultiPlayer.Type("Game") == MultiPlayer.Types.Server) ? Player.Get(I.SenderConnection) :
                        ((MultiPlayer.Type("Game") == MultiPlayer.Types.Client) ? Players[I.ReadByte()] : null));
                    if (Disconnector != null) Player.Remove(Disconnector);
                    if (MultiPlayer.Type("Game") == MultiPlayer.Types.Server) MultiPlayer.Send("Game", MultiPlayer.Construct("Game", Packets.Disconnection, Disconnector.Slot), I.SenderConnection);
                    break;
                case Packets.Initial:
                    if (MultiPlayer.Type("Game") == MultiPlayer.Types.Client)
                    {
                        Players = new Player[I.ReadByte()];
                        Self = Player.Set(I.ReadByte(), new Player(MpName) { });
                        for (byte i = 0; i < Players.Length; i++)
                            if (I.ReadBoolean())
                            {
                                Players[i] = new Player(i, I.ReadString());
                            }
                        Timers.Add("Positions", (1 / 30d));
                        State = States.Game;
                    }
                    break;
                case Packets.Position:
                    Player Sender = ((MultiPlayer.Type("Game") == MultiPlayer.Types.Server) ? Player.Get(I.SenderConnection) : null);
                    Vector2 Position; float Angle;
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
                        byte Count = (byte)((I.LengthBytes - 1) / 12);
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
            }
        }
    }
}