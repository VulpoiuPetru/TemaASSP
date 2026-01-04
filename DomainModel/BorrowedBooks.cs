using DataMapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataMapper
{
    public class BorrowedBooks
    {
        /// <summary>
        /// Gets or sets the book id
        /// </summary>
        [Key, Column(Order = 0)]
        [ForeignKey("Book")]
        public int BookId { get; set; }

        /// <summary>
        /// Gets or sets the reader that borrowed the book
        /// </summary>
        [Key, Column(Order = 1)]
        [ForeignKey("Reader")]
        public int ReaderId { get; set; }

        /// <summary>
        /// Gets or sets the day when the book was borrowed
        /// </summary>
        [Required(ErrorMessage = "Book borrowing start date is neccesary")]
        public DateTime BorrowStartDate { get; set; }

        /// <summary>
        /// Gets or sets the day when the book must be returned to the library
        /// </summary>
        [Required(ErrorMessage = "Book borrowing end date is neccesary")]
        public DateTime BorrowEndDate { get; set; }

        /// <summary>
        /// Gets or sets the day when the book must be returned to the library after an borrow extension has been made
        /// </summary>
        public DateTime BorrowEndDateExtended { get; set; }

        /// <summary>
        /// Gets or sets a virtual Book object for many-to-one relationship
        /// </summary>
        [Required(ErrorMessage = "A book is needed when borrowing")]
        public virtual Book Book { get; set; }

        /// <summary>
        /// Gets or sets a virtual Reader object for many-to-one relationship
        /// </summary>
        [Required(ErrorMessage = "A reader that borrows a book is needed")]
        public virtual Reader Reader { get; set; }
    }
}
