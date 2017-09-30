using System;
using MatthiWare.Net.Sockets.Base;
using MatthiWare.Net.Sockets.Utils;

namespace MatthiWare.Net.Sockets.Packets
{
    public abstract class Packet
    {
        public abstract short Id { get; }

        public abstract bool IsReliable { get; }

        internal long Seq { get; set; }

        internal DateTime ResendTime { get; set; }

        public Packet() { }

        public virtual void ReadPacket(ref RawPacket data)
        {
            if (IsReliable)
                Seq = data.ReadInt64();
        }

        public virtual void WritePacket(ref RawPacket data)
        {
            data.Write(Id);
            if (IsReliable)
                data.Write(Seq);
        }

        public override string ToString()
        {
            return $"[{GetType().Name}, Id: {Id}, Reliable: {IsReliable}, Magic: {Seq}]";
        }

        public override bool Equals(object obj)
        {
            var packet = obj as Packet;

            if (packet == null) return false;

            return packet.Id == Id && packet.IsReliable == IsReliable && packet.Seq == Seq;
        }
    }
}
