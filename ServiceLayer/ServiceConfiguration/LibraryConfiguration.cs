using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.ServiceConfiguration
{
    /// <summary>
    /// Library configuration settings loaded from external configuration. 
    /// All threshold values are configurable without recompilation.
    /// </summary>
    public class LibraryConfiguration
    {
        /// <summary>
        /// Maximum domains per book. 
        /// </summary>
        public int DOMENII { get; set; }

        /// <summary>
        /// Maximum books a reader can borrow in PER period.
        /// </summary>
        public int NMC { get; set; }

        /// <summary>
        /// Period in days for NMC limit.
        /// </summary>
        public int PER { get; set; }

        /// <summary>
        /// Maximum books per borrowing session.
        /// </summary>
        public int C { get; set; }

        /// <summary>
        /// Maximum books from same domain in L months.
        /// </summary>
        public int D { get; set; }

        /// <summary>
        /// Period in months for D limit.
        /// </summary>
        public int L { get; set; }

        /// <summary>
        /// Maximum extension days in last 3 months.
        /// </summary>
        public int LIM { get; set; }

        /// <summary>
        /// Minimum days between borrowing same book.
        /// </summary>
        public int DELTA { get; set; }

        /// <summary>
        /// Maximum books per day for regular readers.
        /// </summary>
        public int NCZ { get; set; }

        /// <summary>
        /// Maximum books staff can lend per day.
        /// </summary>
        public int PERSIMP { get; set; }
    }
}
