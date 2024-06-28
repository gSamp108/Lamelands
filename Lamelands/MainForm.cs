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
        public sealed class Animation
        {
            public TickLog.TickLogEntry Data;
            public int TickTime = 0;
        }

        private Timer timer;
        private World world;
        private int tileSize = 16;
        private Dictionary<Empire,Color> EmpireColors = new Dictionary<Empire,Color>();
        private Random rng = new Random();
        private int citySizeModifier = -4;
        private int forceSizeModifier = -4;
        private bool showDataPanel = false;
        private Position mouseHoverPosition = new Position();
        private List<string> dataPanelData = new List<string>();
        private int dataPanelStart = 10;
        private int dataPanelLineSize = 12;
        private List<Animation> currentAnimations = new List<Animation>();
        private Dictionary<Tile, int> forceRenderColorIndex = new Dictionary<Tile, int>();

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
            if (this.world != null)
            {
                var tickLog = this.world.Tick();
                foreach(var entry in tickLog.Entires )
                {
                    this.currentAnimations.Add(new Animation() { TickTime = 10, Data = entry });
                }
            }
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
            else if (e.KeyCode == Keys.W )
            {
                this.showDataPanel = !this.showDataPanel;
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            this.mouseHoverPosition = new Position(e.Location.X / this.tileSize, e.Location.Y / this.tileSize);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var canvas = e.Graphics;
            if (this.world != null)
            {
                using (var brush = new SolidBrush(Color.Black))
                using (var pen = new Pen(brush))
                {
                    foreach (var tile in this.world.Tiles)
                    {
                        var tileRect = new Rectangle(tile.Position.X * this.tileSize, tile.Position.Y * this.tileSize, this.tileSize - 1, this.tileSize - 1);
                        var cityRect = tileRect.Resize(this.citySizeModifier);
                        var forceRect = tileRect.Resize(this.forceSizeModifier);

                        if (tile.ClaimingCity != null)
                        {
                            brush.Color = this.GetEmpireColor(tile.ClaimingCity.Empire);
                            canvas.FillRectangle(brush, tileRect);
                            canvas.DrawRectangle(Pens.Black, tileRect);
                        }

                        if (tile.City != null)
                        {
                            brush.Color = this.GetEmpireColor(tile.City.Empire);
                            canvas.FillRectangle(brush, cityRect);
                            canvas.DrawRectangle(Pens.Black, cityRect);
                        }

                        if (tile.Forces.Count > 0)
                        {
                            brush.Color = this.GetTileForceColor(tile);
                            canvas.FillEllipse(brush, forceRect);
                            canvas.DrawEllipse(Pens.Black, forceRect);
                        }
                    }

                    foreach(var entry in this.currentAnimations.ToList())
                    {
                        entry.TickTime -= 1;
                        if (entry.TickTime < 1) this.currentAnimations.Remove(entry);
                        var fromTileRect = new Rectangle(entry.Data.FromTile.Position.X * this.tileSize, entry.Data.FromTile.Position.Y * this.tileSize, this.tileSize - 1, this.tileSize - 1);
                        var fromCityRect = fromTileRect.Resize(this.citySizeModifier);
                        if (entry.Data.Type == TickLog.TickLogEntryTypes.Build) canvas.DrawCross(fromCityRect, Pens.White);
                        else if (entry.Data.Type== TickLog.TickLogEntryTypes.Move )
                        {
                            var toTileRect = new Rectangle(entry.Data.ToTile.Position.X * this.tileSize, entry.Data.ToTile.Position.Y * this.tileSize, this.tileSize - 1, this.tileSize - 1);
                            var toCityRect = fromTileRect.Resize(this.citySizeModifier);
                            canvas.DrawLine(Pens.White, fromTileRect.Center(), toTileRect.Center());
                        }
                    }

                    if (this.showDataPanel)
                    {
                        this.dataPanelData.Clear();
                        var mouseTile = this.world.GetTile(this.mouseHoverPosition);
                        if (mouseTile != null) 
                        {
                            this.dataPanelData.Add(mouseTile.Position.ToString());
                            this.dataPanelData.Add(string.Empty);
                            if (mouseTile.City!=null)
                            {
                                this.dataPanelData.Add("City [" + this.GetEmpireName(mouseTile.City.Empire) + "]");
                                this.dataPanelData.Add("----TickTimer:" + mouseTile.City.TickTimer.ToString());
                                this.dataPanelData.Add("----Wealth:" + mouseTile.City.Wealth.ToString());
                                this.dataPanelData.Add("----Leadership:" + mouseTile.City.Leadership.ToString());
                                this.dataPanelData.Add("----Reserves:" + mouseTile.City.Reserves.ToString());
                                this.dataPanelData.Add("----Units:" + mouseTile.City.Units.ToString());
                                this.dataPanelData.Add(string.Empty);
                            }
                            if (mouseTile.Forces.Count>0)
                            {
                                foreach(var force in mouseTile.Forces)
                                {
                                    this.dataPanelData.Add("Force [" + this.GetEmpireName(force.Empire) + "]");
                                    this.dataPanelData.Add("----Reserves:" + force.Reserves.ToString());
                                    this.dataPanelData.Add("----Units:" + force.Units.ToString());
                                    this.dataPanelData.Add(string.Empty);
                                }
                            }
                        }

                        var dataPanelLine = this.dataPanelStart;
                        foreach(var line in this.dataPanelData)
                        {
                            var lineMeasure = canvas.MeasureString(line, this.Font);
                            canvas.FillRectangle(Brushes.Black, new RectangleF(this.dataPanelStart, dataPanelLine, lineMeasure.Width, this.dataPanelLineSize));
                            canvas.DrawString(line, this.Font, Brushes.White, new Point(this.dataPanelStart, dataPanelLine));
                            dataPanelLine += this.dataPanelLineSize;
                        }
                    }
                }
            }
        }

        private Color GetTileForceColor(Tile tile)
        {
            if (!this.forceRenderColorIndex.ContainsKey(tile)) this.forceRenderColorIndex.Add(tile, -1);
            this.forceRenderColorIndex[tile]++;
            if (this.forceRenderColorIndex[tile] >= tile.Forces.Count) this.forceRenderColorIndex[tile] = 0;
            var selectedForce = tile.Forces.ToList()[this.forceRenderColorIndex[tile]];
            return this.GetEmpireColor(selectedForce.Empire);
        }

        private Color GetEmpireColor(Empire empire)
        {
            if (!this.EmpireColors.ContainsKey(empire)) this.EmpireColors.Add(empire, Color.FromArgb(this.rng.Next(256), this.rng.Next(256), this.rng.Next(256)));
            return this.EmpireColors[empire];
        }

        private string GetEmpireName(Empire empire)
        {
            var color = this.GetEmpireColor(empire);
            return color.Name;
        }
    }
}
