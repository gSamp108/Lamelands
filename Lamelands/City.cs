using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lamelands
{
    public sealed class City
    {
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
            this.Tile.ClaimingCity = this;
        }

        public void Tick(TickLog log)
        {
            this.TickTimer -= 1;
            if (this.TickTimer < 1)
            {
                this.TickTimer = this.TickTimerReset;

                var action = this.Settings.City.TickActionSelector.Select(this.Rng);
                switch (action)
                {
                    case Settings.CitySettings.TickActions.Idle:
                        {
                            break;
                        }
                    case Settings.CitySettings.TickActions.BuildReserves:
                        {
                            this.Reserves.ChangeProgress(1);
                            log.Entires.Add(new TickLog.TickLogEntry() { Type = TickLog.TickLogEntryTypes.Build, FromTile = this.Tile });
                            break;
                        }
                    case Settings.CitySettings.TickActions.BuildLeadership:
                        {
                            this.Leadership.ChangeProgress(1);
                            log.Entires.Add(new TickLog.TickLogEntry() { Type = TickLog.TickLogEntryTypes.Build, FromTile = this.Tile });
                            break;
                        }
                    case Settings.CitySettings.TickActions.BuildWealth:
                        {
                            this.Wealth.ChangeProgress(1);
                            log.Entires.Add(new TickLog.TickLogEntry() { Type = TickLog.TickLogEntryTypes.Build, FromTile = this.Tile });
                            break;
                        }
                    case Settings.CitySettings.TickActions.TrainUnit:
                        {
                            if (this.Reserves.Level > 0 && this.Wealth.Level > 0)
                            {
                                this.Reserves.ChangeLevel(-1);
                                this.Wealth.ChangeLevel(-1);
                                this.Units += 1;
                            }
                            log.Entires.Add(new TickLog.TickLogEntry() { Type = TickLog.TickLogEntryTypes.Build, FromTile = this.Tile });
                            break;
                        }
                    case Settings.CitySettings.TickActions.DeployForces:
                        {
                            var unitsToDeploy = this.Rng.Next(this.Units + 1);
                            var reservesToDeploy = this.Rng.Next(this.Reserves.Level + 1);
                            var totalDeployment = unitsToDeploy+reservesToDeploy;
                            if (totalDeployment > 0)
                            {
                                var deploymentTile = this.Tile;
                                if (this.Rng.Next(this.Settings.City.DeployOutsideClaimChance) > 0) deploymentTile = this.InfluenceGrid.RandomKnownTile();
                                else deploymentTile = this.InfluenceGrid.RandomClaimedTile();
                                if (deploymentTile!= null)
                                {
                                    var force = new Forces(this.World, deploymentTile, this.empire, this);
                                    force.Units = unitsToDeploy;
                                    force.Reserves= reservesToDeploy;
                                    this.Units -= unitsToDeploy;
                                    this.Reserves.ChangeLevel(-reservesToDeploy);
                                    log.Entires.Add(new TickLog.TickLogEntry() { Type = TickLog.TickLogEntryTypes.Move, FromTile = this.Tile, ToTile = deploymentTile });
                                }
                            }
                            break;
                        }
                    case Settings.CitySettings.TickActions.ReinforceForces:
                        {
                            var unitsToDeploy = this.Rng.Next(this.Units + 1);
                            var reservesToDeploy = this.Rng.Next(this.Reserves.Level + 1);
                            var totalDeployment = unitsToDeploy + reservesToDeploy;
                            if (totalDeployment > 0)
                            {
                                var selectedForce = this.Forces.ToList().Random();
                                if (selectedForce != null)
                                {
                                    selectedForce.Units += unitsToDeploy;
                                    selectedForce.Reserves += reservesToDeploy;
                                    this.Units -= unitsToDeploy;
                                    this.Reserves.ChangeLevel(-reservesToDeploy);
                                    log.Entires.Add(new TickLog.TickLogEntry() { Type = TickLog.TickLogEntryTypes.Move, FromTile = this.Tile, ToTile = selectedForce.Tile });
                                }
                            }
                            break;
                        }
                    case Settings.CitySettings.TickActions.ExpandClaim:
                        {
                            break;
                        }
                }
            }
        }
    }
}
