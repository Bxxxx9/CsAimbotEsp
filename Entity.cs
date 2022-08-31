using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GameHack
{
    public class Entity
    {
        public IntPtr baseAddress;
        public Vector3 feet, head;
        public Vector2 ViewAngles;
        public float mag, ViewOffset, Jump;
        public int health, team, Ammo, dead;
        public string name;
    }
    public class ViewMatrix
    {
        public float m11, m12, m13, m14;
        public float m21, m22, m23, m24;
        public float m31, m32, m33, m34;
        public float m41, m42, m43, m44;
    }
}
