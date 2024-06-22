using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lamelands
{
    public partial class MainForm : Form
    {
        private Timer timer;
        private World world;
        private int tileSize = 16;

        public MainForm()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint, true);
            this.timer = new Timer();
            this.timer.Interval = 1;
            this.timer.Tick += Timer_Tick;
            this.timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (this.world!=null) this.world.Tick();
            this.Invalidate();
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);
            if (e.KeyCode == Keys.Q )
            {
                var width = this.ClientRectangle.Width / this.tileSize;
                var height = this.ClientRectangle.Height / this.tileSize;
                this.world = new World(width, height);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var canvas = e.Graphics;
            if (this.world != null)
            {
                foreach (var tile in this.world.Tiles)
                {
                    var tileRect = new Rectangle(tile.Position.X * this.tileSize, tile.Position.Y * this.tileSize, this.tileSize-1, this.tileSize-1);
                    canvas.DrawRectangle(Pens.DarkGray , tileRect); 
                    if (tile.City!= null)
                    {
                        canvas.FillRectangle(Brushes.White, tileRect);
                    }
                }
            }
        }
    }
}
