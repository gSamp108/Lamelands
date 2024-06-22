using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lamelands
{
    public sealed class Forces
    {
        public World World { get; private set; }
        public Random Rng => this.World.Rng;
        public Position Position => (this.Tile != null ? this.Tile.Position : Position.Zero);
        public Settings Settings => this.World.Settings;

        private Tile tile;
        public Tile Tile
        {
            get { return this.tile; }
            set
            {
                if (this.tile != null) this.tile.Forces.Remove(this);
                this.tile = value;
                if (this.tile != null) this.tile.Forces.Add(this);
            }
        }
        private Empire empire;
        public Empire Empire
        {
            get { return this.empire; }
            set
            {
                if (this.empire != null) this.empire.Forces.Remove(this);
                this.empire = value;
                if (this.empire != null) this.empire.Forces.Add(this);
            }
        }
        private City city;
        public City City
        {
            get { return this.city; }
            set
            {
                if (this.city != null) this.city.Forces.Remove(this);
                this.city = value;
                if (this.city != null) this.city.Forces.Add(this);
            }
        }

        public int Reserves { get; set; }
        public int Units { get; set; }

        public Forces(World world, Tile tile, Empire empire, City city)
        {
            this.World = world;
            this.World.Forces.Add(this);
            this.Tile = tile;
            this.Empire = empire;
            this.City = city;
        }
    }
}
