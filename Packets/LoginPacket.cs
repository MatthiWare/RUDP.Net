using MatthiWare.Net.Sockets.Packets;
using System;
using MatthiWare.Net.Sockets.Base;

namespace Packets
{
    public class LoginPacket : IPacket
    {
        public const byte UniqueID = 0x00;

        public byte Id => UniqueID;

        public string Username { get; set; }

        public void ReadPacket(RawPacket data)
        {
            Username = data.ReadString();
        }

        public void WritePacket(RawPacket data)
        {
            data.WriteByte(UniqueID);
            data.WriteString(Username);
        }
    }
}
