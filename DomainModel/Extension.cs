using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainModel
{
    /// <summary>
    /// Represents a borrowing period extension request for a borrowed book.
    /// </summary>
    public class Extension
    {
        /// <summary>
        /// Gets or sets the unique identifier for the extension. 
        /// </summary>
        public int ExtensionId { get; set; }

        /// <summary>
        /// Gets or sets the book identifier for the borrowed book being extended.
        /// </summary>
        [Required(ErrorMessage = "Book ID is required for extension")]
        [Range(1, int.MaxValue, ErrorMessage = "Book ID must be a positive number")]
        public int BookId { get; set; }

        /// <summary>
        /// Gets or sets the reader identifier who requested the extension.
        /// </summary>
        [Required(ErrorMessage = "Reader ID is required for extension")]
        [Range(1, int.MaxValue, ErrorMessage = "Reader ID must be a positive number")]
        public int ReaderId { get; set; }

        /// <summary>
        /// Gets or sets the date when the extension was requested.
        /// </summary>
        [Required(ErrorMessage = "Extension request date is required")]
        public DateTime RequestDate { get; set; }

        /// <summary>
        /// Gets or sets the number of additional days requested for the extension.
        /// </summary>
        [Required(ErrorMessage = "Extension period in days is required")]
        [Range(1, 90, ErrorMessage = "Extension days must be between 1 and 90")]
        public int ExtensionDays { get; set; }

        /// <summary>
        /// Gets or sets the borrowed book record that this extension applies to.
        /// </summary>
        [Required(ErrorMessage = "Borrowed book reference is required")]
        public virtual BorrowedBooks BorrowedBooks { get; set; }
    }
}
