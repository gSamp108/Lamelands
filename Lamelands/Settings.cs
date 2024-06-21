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
            public int TickTime { get; } = 100;
            public int TickTimeVariance { get; } = 50;
            public int ProgressRequirementCurve { get; } = 100;
        }
        public CitySettings City { get; } = new CitySettings();
    }
}
