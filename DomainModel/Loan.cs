using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataMapper
{
    public class Loan
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Reader reference is required")]
        public Reader Reader { get; set; }

        [Required(ErrorMessage = "Copy reference is required")]
        public Copy Copy { get; set; }

        [Required(ErrorMessage = "Borrow date is required")]
        public DateTime BorrowDate { get; set; }
        public DateTime? ReturnDate { get; set; }

        [Range(0, 50, ErrorMessage = "Extension count must be between 0 and 50")]
        public int ExtensionCount { get; set; }

        [Range(1, 365, ErrorMessage = "Loan period must be between 1 and 365 days")]
        public int LoanPeriodDays { get; set; }

        // Calculate due date based on borrow date and loan period
        public DateTime DueDate => BorrowDate.AddDays(LoanPeriodDays);

        // Check if loan is currently active
        public bool IsActive => ReturnDate == null;
    }
}
