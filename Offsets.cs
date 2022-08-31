using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameHack
{
    public class Offsets
    {
        public static int
            ViewMatrix = 0x17DFFC-0x6C+ 0x4*16,
            LocalPlayer = 0x0018AC00,
            EntityList = 0x00191FCC, //0x0018AC04,
            Head = 0x4,
            Feet = 0x28,
            dead = 0xB4,
            vAngles = 0x28,
            Angles = 0x34,
            Health = 0xEC,
            Ammo = 0x140,
            Name = 0x205,
            Jump = 0x30,
            Team = 0x30C;
    }
}
