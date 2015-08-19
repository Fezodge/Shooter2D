using System;
using System.Collections.Generic;
using EzGame.Collision;
using Lidgren.Network;
using Microsoft.Xna.Framework;

namespace EzGame
{
    public static class MultiPlayer
    {
        public static Dictionary<string, NetPeer> Peers;
        private static string LastName;

        public static NetPeerConfiguration DefaultConfiguration
        {
            get
            {
                var Configuration = new NetPeerConfiguration("Game");
                Configuration.EnableMessageType(NetIncomingMessageType.ConnectionApproval);
                Configuration.DisableMessageType(NetIncomingMessageType.ConnectionLatencyUpdated);
                Configuration.DisableMessageType(NetIncomingMessageType.DebugMessage);
                Configuration.DisableMessageType(NetIncomingMessageType.DiscoveryRequest);
                Configuration.DisableMessageType(NetIncomingMessageType.DiscoveryResponse);
                Configuration.DisableMessageType(NetIncomingMessageType.Error);
                Configuration.DisableMessageType(NetIncomingMessageType.ErrorMessage);
                Configuration.DisableMessageType(NetIncomingMessageType.NatIntroductionSuccess);
                Configuration.DisableMessageType(NetIncomingMessageType.Receipt);
                Configuration.DisableMessageType(NetIncomingMessageType.UnconnectedData);
                Configuration.DisableMessageType(NetIncomingMessageType.VerboseDebugMessage);
                Configuration.DisableMessageType(NetIncomingMessageType.WarningMessage);
                return Configuration;
            }
        }

        public static NetPeer Peer(string Name)
        {
            if (!string.IsNullOrEmpty(Name) && (Peers != null) && Peers.ContainsKey(Name)) return Peers[Name];
            return null;
        }

        public static Types? Type()
        {
            return Type(LastName);
        }

        public static bool IsNullOrServer()
        {
            return IsNullOrServer(LastName);
        }

        public static Types? Type(string Name)
        {
            if (Peer(Name) is NetServer) return Types.Server;
            if (Peer(Name) is NetClient) return Types.Client;
            return null;
        }

        public static bool IsNullOrServer(string Name)
        {
            var Type = MultiPlayer.Type(Name);
            return ((Type == null) || (Type == Types.Server));
        }

        public static void Initialize()
        {
            Peers = new Dictionary<string, NetPeer>();
        }

        /// <summary>
        /// Start a server on a peer.
        /// </summary>
        /// <param name="Name">The name of the peer.</param>
        /// <param name="Configuration">The configuration for the server.</param>
        /// <returns></returns>
        public static NetPeer Start(string Name, NetPeerConfiguration Configuration)
        {
            LastName = Name;
            if (!Peers.ContainsKey(Name)) Peers.Add(Name, new NetServer(Configuration));
            else throw new Exception("Peer already started!");
            Peer(Name).Start();
            return Peer(Name);
        }

        /// <summary>
        /// Start a server on a peer.
        /// </summary>
        /// <param name="Name">The name of the peer.</param>
        /// <param name="Port">The port of the server.</param>
        /// <param name="MaximumConnections">The maximum connections on the server.</param>
        /// <returns></returns>
        public static NetPeer Start(string Name, int Port, int MaximumConnections)
        {
            LastName = Name;
            var Configuration = DefaultConfiguration;
            Configuration.Port = Port;
            Configuration.MaximumConnections = MaximumConnections;
            return Start(Name, Configuration);
        }

        public static NetPeer Connect(string Name, string Host, int Port, NetPeerConfiguration Configuration,
            params object[] Variables)
        {
            LastName = Name;
            if (!Peers.ContainsKey(Name)) Peers.Add(Name, new NetClient(Configuration));
            else throw new Exception("Peer already started!");
            Peer(Name).Start();
            if (Variables == null) Peer(Name).Connect(Host, Port, Construct(Name, null, Globe.Version));
            else Peer(Name).Connect(Host, Port, Construct(Name, Globe.Version, Variables));
            return Peer(Name);
        }

        public static NetPeer Connect(string Name, string Host, int Port, params object[] Variables)
        {
            return Connect(Name, Host, Port, DefaultConfiguration, Variables);
        }

        public static NetIncomingMessage Read(string Name)
        {
            if (Peers.ContainsKey(Name) && (Peer(Name).Status == NetPeerStatus.Running))
                return Peer(Name).ReadMessage();
            return null;
        }

        public static void Send(NetOutgoingMessage Message, NetDeliveryMethod Method = NetDeliveryMethod.ReliableOrdered,
            int Channel = 0)
        {
            if (Type(LastName) == Types.Server)
            {
                if (Peer(LastName).ConnectionsCount > 0)
                    Peer(LastName).SendMessage(Message, Peer(LastName).Connections, Method, Channel);
            }
            else if (Type(LastName) == Types.Client) (Peer(LastName) as NetClient).SendMessage(Message, Method, Channel);
        }

        public static void Send(NetOutgoingMessage Message, NetConnection Except,
            NetDeliveryMethod Method = NetDeliveryMethod.ReliableOrdered, int Channel = 0)
        {
            if ((Except != null) && (Type(LastName) == Types.Server))
            {
                if (Peer(LastName).ConnectionsCount > 0)
                    (Peer(LastName) as NetServer).SendToAll(Message, Except, Method, Channel);
            }
            else Send(LastName, Message, Method, Channel);
        }

        public static void Send(string Name, NetOutgoingMessage Message,
            NetDeliveryMethod Method = NetDeliveryMethod.ReliableOrdered, int Channel = 0)
        {
            LastName = Name;
            if (Type(Name) == Types.Server)
            {
                if (Peer(Name).ConnectionsCount > 0)
                    Peer(Name).SendMessage(Message, Peer(Name).Connections, Method, Channel);
            }
            else if (Type(Name) == Types.Client) (Peer(Name) as NetClient).SendMessage(Message, Method, Channel);
        }

        public static void Send(string Name, NetOutgoingMessage Message, NetConnection Except,
            NetDeliveryMethod Method = NetDeliveryMethod.ReliableOrdered, int Channel = 0)
        {
            LastName = Name;
            if ((Except != null) && (Type(Name) == Types.Server))
            {
                if (Peer(Name).ConnectionsCount > 0)
                    (Peer(Name) as NetServer).SendToAll(Message, Except, Method, Channel);
            }
            else Send(Name, Message, Method, Channel);
        }

        public static void SendTo(NetOutgoingMessage Message, NetConnection To,
            NetDeliveryMethod Method = NetDeliveryMethod.ReliableOrdered, int Channel = 0)
        {
            if (Type(LastName) == Types.Server) (Peer(LastName) as NetServer).SendMessage(Message, To, Method, Channel);
            else Send(LastName, Message, Method, Channel);
        }

        public static void SendTo(string Name, NetOutgoingMessage Message, NetConnection To,
            NetDeliveryMethod Method = NetDeliveryMethod.ReliableOrdered, int Channel = 0)
        {
            if (Type(Name) == Types.Server) (Peer(Name) as NetServer).SendMessage(Message, To, Method, Channel);
            else Send(Name, Message, Method, Channel);
        }

        public static void Flush(string Name)
        {
            if (Peer(Name) != null) Peer(Name).FlushSendQueue();
        }

        public static void Shutdown(string Name, string Reason)
        {
            if (Peer(Name) != null)
            {
                Peer(Name).Shutdown(Reason);
                Peers.Remove(Name);
            }
        }

        /// <summary>
        /// Construct an outbound message without any built variables.
        /// </summary>
        /// <returns></returns>
        public static NetOutgoingMessage Construct()
        {
            return Peer(LastName).CreateMessage();
        }

        /// <summary>
        /// Construct an outbound message with the following variables.
        /// </summary>
        /// <param name="Variables"># of variables to add to the message.</param>
        /// <returns></returns>
        public static NetOutgoingMessage Construct(params object[] Variables)
        {
            var Message = Peer(LastName).CreateMessage();
            foreach (var Variable in Variables) Message.Write(Variable);
            return Message;
        }

        /// <summary>
        /// Construct an outbound message with the following packet (byte).
        /// </summary>
        /// <param name="Packet">The packet to send at the start of the message (will be converted to a byte).</param>
        /// <returns></returns>
        public static NetOutgoingMessage Construct(Enum Packet)
        {
            var Message = Peer(LastName).CreateMessage();
            Message.Write(Convert.ToByte(Packet));
            return Message;
        }

        /// <summary>
        /// Construct an outbound message with the following packet (byte) and variables.
        /// </summary>
        /// <param name="Packet">The packet to send at the start of the message (will be converted to a byte).</param>
        /// <param name="Variables"># of variables to add to the message after the packet (byte).</param>
        /// <returns></returns>
        public static NetOutgoingMessage Construct(Enum Packet, params object[] Variables)
        {
            var Message = Peer(LastName).CreateMessage();
            Message.Write(Convert.ToByte(Packet));
            foreach (var Variable in Variables) Message.Write(Variable);
            return Message;
        }

        /// <summary>
        /// Construct an outbound message without any built variables.
        /// </summary>
        /// <param name="Name">The name of the peer to construct the message from.</param>
        /// <returns></returns>
        public static NetOutgoingMessage Construct(string Name)
        {
            LastName = Name;
            return Peer(Name).CreateMessage();
        }

        /// <summary>
        /// Construct an outbound message with the following variables.
        /// </summary>
        /// <param name="Name">The name of the peer to construct the message from.</param>
        /// <param name="Variables"># of variables to add to the message.</param>
        /// <returns></returns>
        public static NetOutgoingMessage Construct(string Name, params object[] Variables)
        {
            LastName = Name;
            var Message = Peer(Name).CreateMessage();
            foreach (var Variable in Variables) Message.Write(Variable);
            return Message;
        }

        /// <summary>
        /// Construct an outbound message with the following packet (byte).
        /// </summary>
        /// <param name="Name">The name of the peer to construct the message from.</param>
        /// <param name="Packet">The packet to send at the start of the message (will be converted to a byte).</param>
        /// <returns></returns>
        public static NetOutgoingMessage Construct(string Name, Enum Packet)
        {
            LastName = Name;
            var Message = Peer(Name).CreateMessage();
            Message.Write(Convert.ToByte(Packet));
            return Message;
        }

        /// <summary>
        /// Construct an outbound message with the following packet (byte) and variables.
        /// </summary>
        /// <param name="Name">The name of the peer to construct the message from.</param>
        /// <param name="Packet">The packet to send at the start of the message (will be converted to a byte).</param>
        /// <param name="Variables"># of variables to add to the message after the packet (byte).</param>
        /// <returns></returns>
        public static NetOutgoingMessage Construct(string Name, Enum Packet, params object[] Variables)
        {
            LastName = Name;
            var Message = Peer(Name).CreateMessage();
            Message.Write(Convert.ToByte(Packet));
            foreach (var Variable in Variables) Message.Write(Variable);
            return Message;
        }

        public static void Write(this NetBuffer Message, object Variable)
        {
            if (Variable is Array)
                for (var i = 0; i < (Variable as Array).Length; i++)
                {
                    if (Variable is byte[]) Message.Write((Variable as byte[])[i]);
                    else if (Variable is ushort[]) Message.Write((Variable as ushort[])[i]);
                    else if (Variable is float[]) Message.Write((Variable as float[])[i]);
                    else if (Variable is Vector2[]) Message.Write((Variable as Vector2[])[i]);
                    else if (Variable is Vector3[]) Message.Write((Variable as Vector3[])[i]);
                    else if (Variable is Vector4[]) Message.Write((Variable as Vector4[])[i]);
                    else Message.Write((Variable as object[])[i]);
                }
            else if (Variable is List<object>)
                for (var i = 0; i < (Variable as List<object>).Count; i++)
                    Message.Write((Variable as List<object>)[i]);
            else if (Variable is List<byte>)
                for (var i = 0; i < (Variable as List<byte>).Count; i++)
                    Message.Write((Variable as List<byte>)[i]);
            else if (Variable.GetType() == typeof (bool)) Message.Write((bool) Variable); // 1 byte, 8 bits
            else if (Variable.GetType() == typeof (sbyte)) Message.Write((sbyte) Variable); // 1 byte, 8 bits
            else if (Variable.GetType() == typeof (byte)) Message.Write((byte) Variable); // 1 byte, 8 bits
            else if (Variable.GetType() == typeof (char)) Message.Write((char) Variable); // 2 bytes, 16 bits
            else if (Variable.GetType() == typeof (short)) Message.Write((short) Variable); // 2 bytes, 16 bits
            else if (Variable.GetType() == typeof (ushort)) Message.Write((ushort) Variable); // 2 bytes, 16 bits
            else if (Variable.GetType() == typeof (float)) Message.Write((float) Variable); // 4 bytes, 32 bits
            else if (Variable.GetType() == typeof (int)) Message.Write((int) Variable); // 4 bytes, 32 bits
            else if (Variable.GetType() == typeof (uint)) Message.Write((uint) Variable); // 4 bytes, 32 bits
            else if (Variable.GetType() == typeof (Vector2)) Message.Write((Vector2) Variable); // 8 bytes, 64 bits
            else if (Variable.GetType() == typeof (Point)) Message.Write((Point) Variable); // 8 bytes, 64 bits
            else if (Variable.GetType() == typeof (double)) Message.Write((double) Variable); // 8 bytes, 64 bits
            else if (Variable.GetType() == typeof (long)) Message.Write((long) Variable); // 8 bytes, 64 bits
            else if (Variable.GetType() == typeof (ulong)) Message.Write((ulong) Variable); // 8 bytes, 64 bits
            else if (Variable.GetType() == typeof (Vector3)) Message.Write((Vector3) Variable); // 12 bytes, 96 bits
            //else if (Variable.GetType() == typeof(decimal))
            //    Message.Write((decimal)Variable);                                           // 16 bytes, 128 bits
            else if (Variable.GetType() == typeof (Vector4)) Message.Write((Vector4) Variable); // 16 bytes, 128 bits
            else if (Variable.GetType() == typeof (Line)) Message.Write((Line) Variable); // 16 bytes, 128 bits
            else if (Variable.GetType() == typeof (Rectangle))
                Message.Write((Rectangle) Variable); // 16 bytes, 128 bits
            else if (Variable.GetType() == typeof (Quaternion))
                Message.Write((Quaternion) Variable, 24); // 16 bytes, 128 bits
            else if (Variable.GetType() == typeof (BoundingSphere))
                Message.Write((BoundingSphere) Variable); // 16 bytes, 128 bits
            else if (Variable.GetType() == typeof (Matrix))
                Message.Write((Matrix) Variable); // 28 bytes, 224 bits
            else if (Variable.GetType() == typeof (string)) Message.Write((string) Variable); // ~~
            else if (Variable.GetType() == typeof (Polygon)) Message.Write((Polygon) Variable); // ~~
        }

        public enum Types
        {
            Server,
            Client
        }

        #region Lidgren Extensions

        public static void Write(this NetBuffer Message, Point Variable)
        {
            Message.Write(Variable.X);
            Message.Write(Variable.Y);
        }

        public static Point ReadPoint(this NetBuffer Message)
        {
            return new Point(Message.ReadInt32(), Message.ReadInt32());
        }

        public static void Write(this NetBuffer Message, Vector2 Variable)
        {
            Message.Write(Variable.X);
            Message.Write(Variable.Y);
        }

        public static Vector2 ReadVector2(this NetBuffer Message)
        {
            return new Vector2(Message.ReadFloat(), Message.ReadFloat());
        }

        public static void Write(this NetBuffer Message, Vector3 Variable)
        {
            Message.Write(Variable.X);
            Message.Write(Variable.Y);
            Message.Write(Variable.Z);
        }

        public static Vector3 ReadVector3(this NetBuffer Message)
        {
            return new Vector3(ReadVector2(Message), Message.ReadFloat());
        }

        public static void Write(this NetBuffer Message, Vector4 Variable)
        {
            Message.Write(Variable.X);
            Message.Write(Variable.Y);
            Message.Write(Variable.Z);
            Message.Write(Variable.W);
        }

        public static Vector4 ReadVector4(this NetBuffer Message)
        {
            return new Vector4(ReadVector3(Message), Message.ReadFloat());
        }

        public static void Write(this NetBuffer Message, Line Variable)
        {
            Write(Message, Variable.Start);
            Write(Message, Variable.End);
        }

        public static Line ReadLine(this NetBuffer Message)
        {
            return new Line(ReadVector2(Message), ReadVector2(Message));
        }

        public static void Write(this NetBuffer Message, Rectangle Variable)
        {
            Message.Write(Variable.X);
            Message.Write(Variable.Y);
            Message.Write(Variable.Width);
            Message.Write(Variable.Height);
        }

        public static Rectangle ReadRectangle(this NetBuffer Message)
        {
            return new Rectangle(Message.ReadInt32(), Message.ReadInt32(), Message.ReadInt32(), Message.ReadInt32());
        }

        public static void Write(this NetBuffer Message, BoundingSphere BoundingSphere)
        {
            Write(Message, BoundingSphere.Center);
            Write(Message, BoundingSphere.Radius);
        }

        public static BoundingSphere ReadBoundingSphere(this NetBuffer Message)
        {
            return new BoundingSphere(ReadVector3(Message), Message.ReadFloat());
        }

        public static void Write(this NetBuffer Message, Quaternion Quaternion, int Bits)
        {
            if (Quaternion.X > 1.0f) Quaternion.X = 1.0f;
            if (Quaternion.Y > 1.0f) Quaternion.Y = 1.0f;
            if (Quaternion.Z > 1.0f) Quaternion.Z = 1.0f;
            if (Quaternion.W > 1.0f) Quaternion.W = 1.0f;
            if (Quaternion.X < -1.0f) Quaternion.X = -1.0f;
            if (Quaternion.Y < -1.0f) Quaternion.Y = -1.0f;
            if (Quaternion.Z < -1.0f) Quaternion.Z = -1.0f;
            if (Quaternion.W < -1.0f) Quaternion.W = -1.0f;
            Message.WriteSignedSingle(Quaternion.X, Bits);
            Message.WriteSignedSingle(Quaternion.Y, Bits);
            Message.WriteSignedSingle(Quaternion.Z, Bits);
            Message.WriteSignedSingle(Quaternion.W, Bits);
        }

        public static Quaternion ReadQuaternion(this NetBuffer Message, int Bits)
        {
            return new Quaternion(Message.ReadSignedSingle(Bits), Message.ReadSignedSingle(Bits),
                Message.ReadSignedSingle(Bits), Message.ReadSignedSingle(Bits));
        }

        public static void Write(this NetBuffer Message, Matrix Variable)
        {
            var Quaternion = Microsoft.Xna.Framework.Quaternion.CreateFromRotationMatrix(Variable);
            Write(Message, Quaternion, 24);
            Message.Write(Variable.M41);
            Message.Write(Variable.M42);
            Message.Write(Variable.M43);
        }

        public static Matrix ReadMatrix(this NetBuffer Message)
        {
            var Quaternion = ReadQuaternion(Message, 24);
            var Matrix = Microsoft.Xna.Framework.Matrix.CreateFromQuaternion(Quaternion);
            Matrix.M41 = Message.ReadFloat();
            Matrix.M42 = Message.ReadFloat();
            Matrix.M43 = Message.ReadFloat();
            return Matrix;
        }

        public static void Write(this NetBuffer Message, Polygon Variable)
        {
            Message.Write((byte) Variable.Lines.Length);
            for (byte i = 0; i < Variable.Lines.Length; i++)
                Message.Write(Variable.Lines[i]);
            //Message.Write(Variable.Origin);
            //Message.Write(Variable.Position);
            //Message.Write(Variable.Angle);
        }

        public static Polygon ReadPolygon(this NetBuffer Message)
        {
            var Polygon = new Polygon(new Line[Message.ReadByte()]);
            for (byte i = 0; i < Polygon.Lines.Length; i++)
                Polygon.Lines[i] = ReadLine(Message);
            //Polygon.Origin = ReadVector2(Message);
            //Polygon.Position = ReadVector2(Message);
            //Polygon.Angle = Message.ReadFloat();
            return Polygon;
        }

        #endregion
    }
}