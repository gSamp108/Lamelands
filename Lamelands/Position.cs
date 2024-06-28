using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lamelands
{
    public struct Position
    {
        public static readonly Position Zero = new Position();

        public int X { get; private set; }
        public int Y { get; private set; }

        public Position(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        public override bool Equals(object obj)
        {
            return obj is Position && ((Position)obj).X == this.X && ((Position)obj).Y == this.Y;
        }

        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                int hash = 17;
                // Suitable nullity checks etc, of course :)
                hash = hash * 23 + this.X.GetHashCode();
                hash = hash * 23 + this.Y.GetHashCode();
                return hash;
            }
        }

        public override string ToString()
        {
            return "(" + this.X.ToString() + ", " + this.Y.ToString() + ")";
        }

        public IEnumerable<Position> Adjacent
        {
            get
            {
                yield return new Position(this.X + 0, this.Y - 1);
                yield return new Position(this.X + 1, this.Y + 0);
                yield return new Position(this.X + 0, this.Y + 1);
                yield return new Position(this.X - 1, this.Y + 0);
            }
        }
        public IEnumerable<Position> Nearby
        {
            get
            {
                return this.NearbyRange(1);
            }
        }
        public IEnumerable<Position> NearbyRange(int range)
        {
            for (int x = -range; x <= range; x++)
            {
                for (int y = -range; y <= range; y++)
                {
                    if (!(x == 0 && y == 0)) yield return new Position(this.X + x, this.Y + y);
                }
            }
        }
    }
}
