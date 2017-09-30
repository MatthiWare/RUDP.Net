using MatthiWare.Net.Sockets.Base;
using MatthiWare.Net.Sockets.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Packets
{
    public class ChatPacket : Packet
    {
        public override short Id => 1;

        public override bool IsReliable => true;

        public Guid ClientID { get; set; }
        public string Message { get; set; }

        public override void ReadPacket(ref RawPacket data)
        {
            base.ReadPacket(ref data);

             ClientID = data.ReadGuid();
            Message = data.ReadString();
        }

        public override void WritePacket(ref RawPacket data)
        {
            base.WritePacket(ref data);

            data.Write(ClientID);
            data.Write(Message);
        }
    }
}
