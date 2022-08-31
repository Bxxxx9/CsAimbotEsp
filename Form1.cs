using GameHack;
using System.Runtime.InteropServices;
using ezOverLay;

namespace CsAimbotEsp
{
   // ogto Project -> proerties -> build -> use{ x86 }
    public partial class Form1 : Form
    {
        ez ez = new ez();

        [DllImport("user32.dll")]
        static extern short GetAsyncKeyState(System.Windows.Forms.Keys keys);
        methods? m;
        Entity localplayer = new Entity();
        List<Entity> entities = new List<Entity>();
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            CheckForIllegalCrossThreadCalls = false;
            m = new methods();
            if (m != null)
            {
                ez.SetInvi(this);
                ez.DoStuff("AssaultCube", this);
                Thread thread = new Thread(Main) { IsBackground = true };
                thread.Start();
            }
        }
        void Main()
        {
            while (true)
            {
                localplayer = m.ReadLocalPlayer();
                entities = m.ReadEntities(localplayer);

                entities = entities.OrderBy(o => o.mag).ToList();
                if (GetAsyncKeyState(Keys.LButton) < 0)
                {
                    if (entities.Count > 0)
                    {
                        foreach (var ent in entities)
                        {
                            if (ent.team != localplayer.team)
                            {

                                var angles = m.CalcAngles(localplayer, ent);
                                m.Aim(localplayer, angles.X, angles.Y);
                                break;
                            }
                        }
                    }

                }
                Form1 f = this;
                f.Refresh();

                Thread.Sleep(20);
            }
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Pen red = new Pen(Color.Red, (float) 0.9);
            Pen green = new Pen(Color.Green,(float) 0.5);

            foreach (var ent in entities.ToList())
            {
                var mtsfeet = m.worldToScreen(m.ReadMatrix(), ent.feet, this.Width, this.Height);
                var mtshead = m.worldToScreen(m.ReadMatrix(), ent.head, this.Width, this.Height);

                if (mtsfeet.X > 0)
                {
                    if (localplayer.team == ent.team)
                    {
                        g.DrawLine(green, new Point(Width / 2, Height), mtsfeet); //ESP Line
                        g.DrawRectangle(green, m.CalcRect(mtsfeet, mtshead)); // Esp box
                    }
                    else
                    {
                        g.DrawLine(red, new Point(Width / 2, Height), mtsfeet);//ESP Line
                        g.DrawRectangle(red, m.CalcRect(mtsfeet, mtshead));// Esp box
                    }
                }
            }
        }
    }
}