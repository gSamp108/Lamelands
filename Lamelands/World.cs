using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lamelands
{
    public sealed class World
    {
        public Random Rng { get; } = new Random();
        public Dictionary<Position, Tile> TilesByPosition { get; } = new Dictionary<Position, Tile>();
        public IEnumerable<Tile> Tiles => this.TilesByPosition.Values;
        public HashSet<Empire> Empires { get; } = new HashSet<Empire>();
        public HashSet<City> Cities { get; } = new HashSet<City>();
        public Settings Settings { get; } = new Settings();
        public HashSet<Forces> Forces { get; } = new HashSet<Forces>();

        public World(int width, int height)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    var position = new Position(x, y);
                    var tile = new Tile(this, position);
                    this.TilesByPosition.Add(position, tile);
                }
            }

            var openSpawnTiles = this.Tiles.ToList();
            while (openSpawnTiles.Count>0)
            {
                var selectedTile = openSpawnTiles.Random();
                openSpawnTiles.Remove(selectedTile);
                foreach(var nearbyTile in selectedTile.NearbyRange(2))
                {
                    openSpawnTiles.Remove(nearbyTile);
                }
                if (this.Rng.Next(this.Settings.WorldGeneration.CitySpawnChance) == 0)
                {
                    var empire = new Empire(this);
                    var city = new City(this, selectedTile, empire);
                }
            }
        }

        public TickLog Tick()
        {
            var log = new TickLog();
            var currentCities = this.Cities.ToList();
            foreach(var city in currentCities)
            {
                city.Tick(log);
            }

            var activeForceTiles = this.Tiles.Where(o => o.Forces.Count > 0).ToList();
            foreach (var tile in activeForceTiles)
            {
                var empires = tile.Forces.Select(o => o.Empire).Distinct().Count();
                if (empires > 1) this.TileCombatTick(tile);
            }

            return log;
        }

        private void TileCombatTick(Tile tile)
        {
            
        }

        public Tile GetTile(Position position)
        {
            if (this.TilesByPosition.ContainsKey(position)) return this.TilesByPosition[position];
            return null;
        }
    }
}
