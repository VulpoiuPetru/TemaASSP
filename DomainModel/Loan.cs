using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataMapper
{
    public class Loan
    {
        public int Id { get; set; }
        public Reader Reader { get; set; }
        public Copy Copy { get; set; }
        public DateTime BorrowDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public int ExtensionCount { get; set; }
        public int LoanPeriodDays { get; set; }
    }
}
