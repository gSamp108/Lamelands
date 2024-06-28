using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lamelands
{
    public sealed class TickLog
    {
        public enum TickLogEntryTypes { Build, Move }
        public sealed class TickLogEntry
        {
            public TickLogEntryTypes Type;
            public Tile FromTile;
            public Tile ToTile;
        }

        public List<TickLogEntry> Entires = new List<TickLogEntry>();
    }
}
