using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
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
        public static Rectangle Resize(this Rectangle rect, int amount)
        {
            return new Rectangle(rect.X - amount, rect.Y - amount, rect.Width + (amount*2), rect.Height + (amount*2));
        }
        public static void DrawCross(this Graphics canvas, Rectangle rect, Pen pen)
        {
            canvas.DrawLine(pen, new Point(rect.X + rect.Width / 2, rect.Y), new Point(rect.X + rect.Width / 2, rect.Y + rect.Height));
            canvas.DrawLine(pen, new Point(rect.X, rect.Y + rect.Height / 2), new Point(rect.X + rect.Width, rect.Y + rect.Height / 2));
        }
        public static Point Center(this Rectangle rect)
        {
            return new Point(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);
        }
    }
}
