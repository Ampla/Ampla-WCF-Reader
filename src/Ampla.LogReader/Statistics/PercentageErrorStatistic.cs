﻿using System.Collections.Generic;
using Ampla.LogReader.Wcf;

namespace Ampla.LogReader.Statistics
{
    /// <summary>
    ///     Determine the Percentage Error
    /// </summary>
    public class PercentageErrorStatistic : IWcfStatistic
    {
        private readonly string name;
        private int errors;
        private int total ;

        public PercentageErrorStatistic(string name)
        {
            this.name = name;
        }

        /// <summary>
        /// Adds the specified entry.
        /// </summary>
        /// <param name="entry">The entry.</param>
        public void Add(WcfCall entry)
        {
            if (entry.IsFault)
            {
                errors++;
            }
            total++;
        }

        public IEnumerable<Result> Results
        {
            get
            {
                yield return Result.New(name, "Errors", errors);
                yield return Result.New(name, "Total", total);
                yield return Result.New(name, "Percentage", ErrorPercent);
            }
        }

        public double ErrorPercent
        {
            get { return total == 0 ? double.NaN : errors*100.0D/total; }
        }

        public override string ToString()
        {
            return string.Format("{0}\nErrors ({1}) / Total ({2}) = {3:00.0}% errors", name, errors, total, ErrorPercent);
        }

    }
}