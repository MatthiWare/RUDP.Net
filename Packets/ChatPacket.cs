using MatthiWare.Net.Sockets.Base;
using MatthiWare.Net.Sockets.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Packets
{
    public class ChatPacket : IPacket
    {
        public const byte UniqueID = 0x01;

        public byte Id => UniqueID;

        public string Username { get; set; }
        public string Message { get; set; }

        public void ReadPacket(RawPacket data)
        {
            Username = data.ReadString();
            Message = data.ReadString();
        }

        public void WritePacket(RawPacket data)
        {
            data.WriteByte(UniqueID);
            data.WriteString(Username);
            data.WriteString(Message);
        }
    }
}
