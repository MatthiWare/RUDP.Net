using MatthiWare.Net.Sockets.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MatthiWare.Net.Sockets.Base;

namespace Packets
{
    public class PingPacket : Packet
    {
        public override short Id => 2;

        public override bool IsReliable => true;

        public long SendTime { get; set; }

        public override void ReadPacket(ref RawPacket data)
        {
            base.ReadPacket(ref data);

            SendTime = data.ReadInt64();
        }

        public override void WritePacket(ref RawPacket data)
        {
            base.WritePacket(ref data);

            data.Write(SendTime);
        }
    }
}
