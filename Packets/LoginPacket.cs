using MatthiWare.Net.Sockets.Packets;
using System;
using MatthiWare.Net.Sockets.Base;

namespace Packets
{
    public class LoginPacket : Packet
    {
        public override short Id => 0;

        public override bool IsReliable => true;

        public bool Authenticated { get; set; }
        public string Username { get; set; }

        public Guid ClientID { get; set; }

        public override void ReadPacket(ref RawPacket data)
        {
            base.ReadPacket(ref data);

            Authenticated = data.ReadBool();
            Username = data.ReadString();
            ClientID = data.ReadGuid();
        }

        public override void WritePacket(ref RawPacket data)
        {
            base.WritePacket(ref data);

            data.Write(Authenticated);
            data.Write(Username);
            data.Write(ClientID);
        }
    }
}
