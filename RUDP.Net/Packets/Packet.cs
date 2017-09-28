using System;
using MatthiWare.Net.Sockets.Base;
using MatthiWare.Net.Sockets.Utils;

namespace MatthiWare.Net.Sockets.Packets
{
    public abstract class Packet 
    {
        public short Id { get; }

        public bool IsReliable { get; }

        internal long Seq { get; set; }

        internal DateTime ResendTime { get; set; }

        public Packet(short id, bool reliable)
        {
            if (!Helper.IsValidPacketId(id)) throw new ArgumentException("Given id is not valid", nameof(id));

            Id = id;
            IsReliable = reliable;
        }

        public virtual void ReadPacket(ref RawPacket data)
        {
            Seq = data.ReadInt64();
        }

        public virtual void WritePacket(ref RawPacket data)
        {
            data.Write(Id);
            data.Write(Seq);
        }

        public override string ToString()
        {
            return $"{GetType().Name}, Id: 0x{Id.ToString("X2")}, Reliable: {IsReliable}, Magic: {Seq}";
        }

        public override bool Equals(object obj)
        {
            var packet = obj as Packet;

            if (packet == null) return false;

            return packet.Id == Id && packet.IsReliable == IsReliable && packet.Seq == Seq;
        }
    }
}
