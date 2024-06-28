using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lamelands
{
    public sealed class Settings
    {
        public sealed class CitySettings
        {
            public enum TickActions { Idle, BuildReserves, BuildLeadership, BuildWealth, TrainUnit, DeployForces, ReinforceForces, ExpandClaim }

            public int TickTime { get; } = 100;
            public int TickTimeVariance { get; } = 50;
            public int ReservesRequirementCurve { get; } = 1;
            public int LeadershipRequirementCurve { get; } = 2;
            public int WealthRequirementCurve { get; } = 2;
            public WeightedSelection<TickActions> TickActionSelector { get; } = new WeightedSelection<TickActions>();
            public int DeployOutsideClaimChance { get; } = 5;

            public CitySettings()
            {
                this.TickActionSelector.AddEntry(10, TickActions.BuildLeadership);
                this.TickActionSelector.AddEntry(10, TickActions.BuildReserves);
                this.TickActionSelector.AddEntry(10, TickActions.BuildWealth);
                this.TickActionSelector.AddEntry(1, TickActions.DeployForces);
                this.TickActionSelector.AddEntry(1, TickActions.Idle);
                this.TickActionSelector.AddEntry(1, TickActions.ReinforceForces);
                this.TickActionSelector.AddEntry(10, TickActions.TrainUnit);
                this.TickActionSelector.AddEntry(1, TickActions.ExpandClaim);
            }
        }
        public CitySettings City { get; } = new CitySettings();

        public sealed class ForcesSettings
        {
            public int ProgressRequiredToClaimTile = 1000;
        }
        public ForcesSettings Forces { get; }= new ForcesSettings();    

        public sealed class WorldGenerationSettings
        {
            public int CitySpawnChance = 5;
        }
        public WorldGenerationSettings WorldGeneration { get; } = new WorldGenerationSettings();

    }
}
