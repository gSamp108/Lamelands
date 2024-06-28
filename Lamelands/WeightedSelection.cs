using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lamelands
{
    public sealed  class WeightedSelection<type>
    {
        public sealed class WeightedSelectionEntry
        {
            public WeightedSelectionEntry(int weight, type value)
            {
                Weight = weight;
                Value = value;
            }

            public int Weight { get; private set; }
            public type Value { get; private set; }
        }

        public List<WeightedSelectionEntry> Entries { get; } = new List<WeightedSelectionEntry>();
        public int TotalWeight { get;private set; }

        public void AddEntry(int weight, type value)
        {
            this.Entries.Add(new WeightedSelectionEntry(weight, value));
            this.TotalWeight += weight;
        }

        public type Select(Random rng)
        {
            var roll = rng.Next(this.TotalWeight);
            var rolling = 0;
            foreach (var entry in this.Entries)
            {
                rolling += entry.Weight;
                if (roll < rolling) return entry.Value;
            }
            return default(type);
        }
    }
}
