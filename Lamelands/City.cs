using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lamelands
{
    public sealed class City
    {
        public enum TickActions { Idle, BuildReserves, BuildLeadership, BuildWealth, TrainUnit, DeployForces, ReinforceForces }
        public static readonly List<TickActions> PossibleTickActions = Enum.GetValues(typeof(TickActions)).Cast<TickActions>().ToList();

        public World World { get; private set; }
        public Random Rng => this.World.Rng;
        public Position Position => this.Tile.Position;
        public int TickTimer { get; private set; }
        public Settings Settings => this.World.Settings;

        public int Units { get; private set; }
        public ProgressionStat Reserves { get; private set; }
        public ProgressionStat Leadership { get; private set; }
        public ProgressionStat Wealth { get; private set; }
        public CityInfluenceGrid InfluenceGrid { get; private set; }
        public HashSet<Forces> Forces { get; } = new HashSet<Forces>();

        private Tile tile;
        public Tile Tile
        {
            get { return this.tile; }
            set
            {
                if (this.tile != null) this.tile.City = null;
                this.tile = value;
                this.tile.City = this;
            }
        }
        private Empire empire;
        public Empire Empire
        {
            get { return this.empire; }
            set
            {
                if (this.empire != null) this.empire.Cities.Remove(this);
                this.empire = value;
                if (this.empire != null) this.empire.Cities.Add(this);
            }
        }

        public int TickTimerReset => this.Settings.City.TickTime + this.Rng.Variance(this.Settings.City.TickTimeVariance);

        public City(World world, Tile tile, Empire empire)
        {
            this.World = world;
            this.Tile = tile;
            this.Empire = empire;
            this.World.Cities.Add(this);
            this.TickTimer = this.TickTimerReset;
            this.Reserves = new ProgressionStat(this.Settings.City.ReservesRequirementCurve, true);
            this.Leadership = new ProgressionStat(this.Settings.City.LeadershipRequirementCurve);
            this.Wealth = new ProgressionStat(this.Settings.City.WealthRequirementCurve);
            this.InfluenceGrid = new CityInfluenceGrid(this);
        }

        public void Tick()
        {
            this.TickTimer -= 1;
            if (this.TickTimer < 1)
            {
                this.TickTimer = this.TickTimerReset;

                var action = City.PossibleTickActions.Random();
                switch (action)
                {
                    case TickActions.Idle:
                        {
                            break;
                        }
                    case TickActions.BuildReserves:
                        {
                            this.Reserves.ChangeProgress(1);
                            break;
                        }
                    case TickActions.BuildLeadership:
                        {
                            this.Leadership.ChangeProgress(1);
                            break;
                        }
                    case TickActions.BuildWealth:
                        {
                            this.Wealth.ChangeProgress(1);
                            break;
                        }
                    case TickActions.TrainUnit:
                        {
                            if (this.Reserves.Level > 0 && this.Wealth.Level > 0)
                            {
                                this.Reserves.ChangeLevel(-1);
                                this.Wealth.ChangeLevel(-1);
                                this.Units += 1;
                            }
                            break;
                        }
                    case TickActions.DeployForces:
                        {
                            
                            break;
                        }
                }
            }
        }
    }
}
