using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lamelands
{
    public sealed class Empire
    {
        public World World { get; private set; }
        public Random Rng => this.World.Rng;
        public HashSet<City> Cities { get; } = new HashSet<City>();
        public Settings Settings => this.World.Settings;
        public HashSet<Forces> Forces = new HashSet<Forces>();

        public Empire(World world)
        {
            this.World = world;
            this.World.Empires.Add(this);
        }
    }
}
