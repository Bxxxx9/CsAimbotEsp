using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using swed32;


namespace GameHack
{
    public class methods
    {
        public swed mem;
        public IntPtr moduleBase;

        public Entity ReadLocalPlayer() // #2 
        {
            var localplayer = ReadEntity(mem.ReadPointer(moduleBase, Offsets.LocalPlayer));
            localplayer.ViewAngles.X = mem.ReadFloat(localplayer.baseAddress, Offsets.Angles);
            localplayer.ViewAngles.Y = mem.ReadFloat(localplayer.baseAddress, Offsets.Angles + 0x4);
           
            return localplayer;
        }
        public Entity ReadEntity(IntPtr EntityBase) // #1  تخزين البينات
        {
            var ent = new Entity();
            ent.baseAddress = EntityBase;
            
            ent.Ammo = mem.ReadInt(ent.baseAddress,Offsets.Ammo);
            ent.Jump = mem.ReadFloat(ent.baseAddress, Offsets.Jump);
            ent.health = mem.ReadInt(ent.baseAddress,Offsets.Health);
            ent.team = mem.ReadInt(ent.baseAddress, Offsets.Team);
            ent.feet = mem.ReadVector3(ent.baseAddress, Offsets.Feet);
            ent.head = mem.ReadVector3(ent.baseAddress, Offsets.Head);
            ent.name = Encoding.UTF8.GetString(mem.ReadBytes(ent.baseAddress, Offsets.Name,11));

            return ent;

        }
        public List<Entity> ReadEntities(Entity localplayer) // aimbot 1
        {
            var entities = new List<Entity>();
            var entityList = mem.ReadPointer(moduleBase, Offsets.EntityList);

            for (int i = 0; i < 21; i++) // 4 إذا تستخدم المزيد من البوتات
            {
                var currentEntityBase = mem.ReadPointer(entityList, i * 0x4);
                var ent = ReadEntity(currentEntityBase);
                ent.mag = CalcMag(localplayer, ent);
                if (ent.health > 0 && ent.health < 101)
                {
                    entities.Add(ent);
                }
            }
            return entities;
        }
        public static float CalcMag(Entity localplayer , Entity destEnt) //aimbot 2
        {
            return (float) 
                Math.Sqrt(Math.Pow(destEnt.feet.X - localplayer.feet.X,2)
                + Math.Pow(destEnt.feet.Y - localplayer.feet.Y,2)
                + Math.Pow(destEnt.feet.Z - localplayer.feet.Z,2));
        } 
        public Vector2 CalcAngles(Entity localplayer, Entity destEnt)
        {
            float x, y;
            var deltax = destEnt.head.X - localplayer.head.X;
            var deltay = destEnt.head.Y - localplayer.head.Y;

            x = (float)(Math.Atan2(deltay, deltax) * 180 / Math.PI) + 90;

            float deltaz = destEnt.head.Z - localplayer.head.Z;
            float dist = CalcDist(localplayer, destEnt);

            y = (float)(Math.Atan2(deltaz, dist) * 180 / Math.PI);
            return new Vector2(x,y);
        }
        public static float CalcDist(Entity localplayer, Entity destEnt)
        {
            return (float)
                Math.Sqrt(Math.Pow(destEnt.feet.X - localplayer.feet.X, 2)
                + Math.Pow(destEnt.feet.Y - localplayer.feet.Y, 2));
        }
        public void Aim(Entity ent, float x, float y)
        {
            mem.WriteFloat(ent.baseAddress, Offsets.Angles, x);
            mem.WriteFloat(ent.baseAddress, Offsets.Angles + 0x4, y);

        }
        public void ReadJump(Entity ent, float newvalue)
        {
             
            float oldvalue = mem.ReadFloat(ent.baseAddress, Offsets.Jump);
            newvalue = oldvalue + 5;
            mem.WriteFloat(ent.baseAddress, Offsets.Jump,newvalue);
            Thread.Sleep(20);
        }
        public Point worldToScreen(ViewMatrix mtx, Vector3 pos, int Width, int height) // ESP 2
        {
            var twoD = new Point();
            float screenW = (mtx.m14 * pos.X) + (mtx.m24 * pos.Y) + (mtx.m34 * pos.Z) + mtx.m44;
            if (screenW > 0.001f)
            {
                float screenX = (mtx.m11 * pos.X) + (mtx.m21 * pos.Y) + (mtx.m31 * pos.Z) + mtx.m41;
                float screenY = (mtx.m12 * pos.X) + (mtx.m22 * pos.Y) + (mtx.m32 * pos.Z) + mtx.m42;

                float camX = Width / 2f;
                float camY = height / 2f;

                float X = camX + (camX * screenX / screenW);
                float Y = camY - (camY * screenY / screenW);

                twoD.X = (int)X;
                twoD.Y = (int)Y;

                return twoD;
            }
            else
            {
                return new Point(-99,-99);
            }
        }
        public ViewMatrix ReadMatrix()  //ESP 1
        {
            var viewMatrix = new ViewMatrix();
            var mtx = mem.ReadMatrix(moduleBase + Offsets.ViewMatrix);
            viewMatrix.m11 = mtx[0];
            viewMatrix.m12 = mtx[1];
            viewMatrix.m13 = mtx[2];
            viewMatrix.m14 = mtx[3];

            viewMatrix.m21 = mtx[4];
            viewMatrix.m22 = mtx[5];
            viewMatrix.m23 = mtx[6];
            viewMatrix.m24 = mtx[7];

            viewMatrix.m31 = mtx[8];
            viewMatrix.m32 = mtx[9];
            viewMatrix.m33 = mtx[10];
            viewMatrix.m34 = mtx[11];

            viewMatrix.m41 = mtx[12];
            viewMatrix.m42 = mtx[13];
            viewMatrix.m43 = mtx[14];
            viewMatrix.m44 = mtx[15];

            return viewMatrix;
        }
        public Rectangle CalcRect(Point feet, Point head) //ESP BOX
        {
            var rect = new Rectangle();
            rect.X = head.X - (feet.X - head.Y) / 4;
            rect.Y = head.Y;

            rect.Width = (feet.Y - head.Y) /2;
            rect.Height = feet.Y - head.Y ;

            return rect;
        }
        public methods()
        {
            mem = new swed();
            mem.GetProcess("ac_client");
            moduleBase = mem.GetModuleBase(".exe");

        }
    }
}
