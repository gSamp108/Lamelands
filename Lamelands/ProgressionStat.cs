using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lamelands
{
    public sealed class ProgressionStat
    {
        public int Level { get; private set; }
        public int Progress { get; private set; }
        public int ProgressCurve { get; private set; }
        public bool RequirementIgnoresLevel { get; private set; }
        public int ProgressRequirement => this.ProgressCurve * (this.RequirementIgnoresLevel ? 1 : (this.Level + 1));

        public ProgressionStat(int progressCurve, bool requirementIgnoresLevel = false)
        {
            this.ProgressCurve = progressCurve;
            this.RequirementIgnoresLevel = requirementIgnoresLevel;
        }

        public void ChangeLevel(int amount)
        {
            this.Level += amount;
            if (this.Level < 0) this.Level = 0;
            this.ChangeProgress(0);
        }

        public void ChangeProgress(int amount)
        {
            this.Progress += amount;
            while (this.Progress >= this.ProgressRequirement)
            {
                this.Progress -= this.ProgressRequirement;
                this.Level += 1;
            }
            while (this.Progress < 0)
            {
                if (this.Level == 0) this.Progress = 0;
                else
                {
                    this.Level -= 1;
                    this.Progress += this.ProgressRequirement;
                }
            }
        }

        public override string ToString()
        {
            return this.Level.ToString() + " (" + this.Progress + "/" + this.ProgressRequirement + ")";
        }
    }
}
