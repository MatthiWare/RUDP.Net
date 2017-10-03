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
        private static ConcurrentDictionary<short, Type> PacketTypes = new ConcurrentDictionary<short, Type>();

        public static void RegisterPacket(short id, Type packetType)
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
            short id = data.ReadInt16();
            Type type = null; 

            if (!PacketTypes.TryGetValue(id, out type))
                throw new InvalidOperationException($"Invalid packet id {id}");

            Packet packet = PacketFromType(type);
            packet.ReadPacket(ref data);

            return packet;
        }
    }
}
