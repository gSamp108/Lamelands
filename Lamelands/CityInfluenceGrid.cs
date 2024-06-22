using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lamelands
{
    public sealed class CityInfluenceGrid
    {
        public sealed class CityInfluenceTile
        {
            public Tile Tile { get; private set; }
            public CityInfluenceGrid Grid { get; private set; }
            public CityInfluenceTile ParentTile { get; private set; }
            public List<CityInfluenceTile> ChildTiles { get; } = new List<CityInfluenceTile>();

            public CityInfluenceTile(CityInfluenceGrid grid, Tile tile, CityInfluenceTile parentTile)
            {
                this.Grid = grid;
                this.Tile = tile;
                this.ParentTile = parentTile;
            }
        }

        public City City { get; private set; }
        public World World => this.City.World;

        public CityInfluenceTile SourceTile { get; private set; }
        public Dictionary<Tile, CityInfluenceTile> ClaimedTiles { get; } = new Dictionary<Tile, CityInfluenceTile>();
        public Dictionary<Tile, CityInfluenceTile> KnownTiles { get; } = new Dictionary<Tile, CityInfluenceTile>();

        public CityInfluenceGrid(City city)
        {
            this.City = city;
            this.ClaimTile(city.Tile);
        }

        public void ClaimTile(Tile tile)
        {
            if (this.KnownTiles.ContainsKey(tile))
            {
                var knownTile = this.KnownTiles[tile];
                this.KnownTiles.Remove(tile);
                this.ClaimedTiles.Add(tile, knownTile);
            }
            else
            {
                this.ClaimedTiles.Add(tile, new CityInfluenceTile(this, tile, null));
                this.SourceTile = this.ClaimedTiles[tile];
            }

            var iTile = this.ClaimedTiles[tile];
            foreach (var adjacent in iTile.Tile.Adjacent)
            {
                if (!this.ClaimedTiles.ContainsKey(adjacent) && !this.KnownTiles.ContainsKey(adjacent))
                {
                    this.KnownTiles.Add(adjacent, new CityInfluenceTile(this, adjacent, iTile));
                }
            }
        }
    }
}
