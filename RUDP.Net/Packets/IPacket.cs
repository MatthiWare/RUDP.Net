/*  Copyright (C) 2017 MatthiWare
 *  Copyright (C) 2017 Matthias Beerens
 *  For the full notice see <https://github.com/MatthiWare/RUDP.Net/blob/master/LICENSE>. 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatthiWare.Net.Sockets.Packets
{
    public interface IPacket
    {
        byte Id { get; }
        void ReadPacket();
        void SendPacket();
    }
}
