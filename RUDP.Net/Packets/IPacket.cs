/*  Copyright (C) 2017 MatthiWare
 *  Copyright (C) 2017 Matthias Beerens
 *  For the full notice see <https://github.com/MatthiWare/RUDP.Net/blob/master/LICENSE>. 
 */
using MatthiWare.Net.Sockets.Base;

namespace MatthiWare.Net.Sockets.Packets
{
    public interface IPacket
    {
        byte Id { get; }
        void ReadPacket(RawPacket data);
        void WritePacket(RawPacket data);
    }
}
