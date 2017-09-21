/*  Copyright (C) 2017 MatthiWare
 *  Copyright (C) 2017 Matthias Beerens
 *  For the full notice see <https://github.com/MatthiWare/RUDP.Net/blob/master/LICENSE>. 
 */
using MatthiWare.Net.Sockets.Base;
using System;
using System.Collections.Concurrent;

namespace MatthiWare.Net.Sockets.Packets
{
    public static class PacketReader
    {
        private static ConcurrentDictionary<byte, Type> PacketTypes = new ConcurrentDictionary<byte, Type>();

        public static void RegisterPacket(byte id, Type packetType)
        {
            PacketTypes.AddOrUpdate(id, packetType, (b, t) => packetType);
        }

        public static Packet PacketFromType(Type packetType)
        {
            if (!typeof(Packet).IsAssignableFrom(packetType))
                throw new InvalidCastException("Type must inherit from IPacket");

            return (Packet)Activator.CreateInstance(packetType);
        }

        public static Packet GetPacket(RawPacket data)
        {
            byte id = data.ReadUInt8();
            Type type = PacketTypes[id];

            if (type == null)
                throw new InvalidOperationException($"Invalid packet id: 0x{id.ToString("X2")}");

            Packet packet = PacketFromType(type);
            packet.ReadPacket(ref data);

            return packet;
        }
    }
}
