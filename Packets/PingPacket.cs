using MatthiWare.Net.Sockets.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MatthiWare.Net.Sockets.Base;

namespace Packets
{
    public class PingPacket : IPacket
    {

        public const byte UniqueID = 0x02;
        public byte Id => UniqueID;

        public long SendTime { get; set; }

        public void ReadPacket(RawPacket data)
        {
            SendTime = data.ReadInt64();
        }

        public void WritePacket(RawPacket data)
        {
            data.WriteByte(UniqueID);
            data.WriteInt64(SendTime);
        }
    }
}
