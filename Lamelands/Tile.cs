using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lamelands
{
    public sealed class Tile
    {
        public World World { get; private set; }
        public Random Rng => this.World.Rng;
        public Settings Settings => this.World.Settings;

        public Position Position { get; private set; }
        public City City { get; set; }
        public City ClaimingCity { get; set; }
        public HashSet<Forces> Forces { get; } = new HashSet<Forces>();

        public Tile(World world, Position position)
        {
            this.World = world;
            this.Position = position;
        }

        public IEnumerable<Tile> Adjacent
        {
            get
            {
                foreach(var adjacent in this.Position.Adjacent)
                {
                    var tile = this.World.GetTile(adjacent);
                    if (tile != null) yield return tile;
                }
            }
        }
        public IEnumerable<Tile> Nearby
        {
            get
            {
                return this.NearbyRange(1);
            }
        }
        public IEnumerable<Tile> NearbyRange(int range)
        {
            foreach (var nearby in this.Position.NearbyRange(range))
            {
                var tile = this.World.GetTile(nearby);
                if (tile != null) yield return tile;
            }
        }
    }
}
