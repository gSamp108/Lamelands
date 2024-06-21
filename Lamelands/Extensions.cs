using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lamelands
{
    public static class Extensions
    {
        private static readonly Random rng = new Random();

        public static type Random<type>(this List<type> list)
        {
            if (list == null) return default(type);
            if (list.Count == 0) return default(type);
            return list[Extensions.rng.Next(list.Count)];
        }
        public static type Random<type>(this HashSet<type> list)
        {
            return list.ToList().Random();
        }
        public static int Variance(this Random rng, int amount)
        {
            return (rng.Next(amount * 2 + 1) - amount);
        }
    }
}
