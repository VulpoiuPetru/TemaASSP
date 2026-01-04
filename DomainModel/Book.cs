using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataMapper
{
   public class Book
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Book"/> class
        /// </summary>
        public Book()
        {
            this.Authors = new HashSet<Author>();
            this.Domains = new HashSet<Domain>();
        }

        /// <summary>
        /// Gets or sets the Id of the book.
        /// </summary>
        public int BookId { get; set; }

        /// <summary>
        /// Gets or sets the title of the book.
        /// </summary>
        [Required(ErrorMessage = "The title cannot be null")]
        [StringLength(50, ErrorMessage = "The book title must have between 5 and 50 characters", MinimumLength = 5)]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the domains of the book.
        /// </summary>
        [Required(ErrorMessage = "A book must have at least one domain")]
        public virtual ICollection<Domain> Domains { get; set; }

        /// <summary>
        /// Gets or sets the authors that wrote the book.
        /// </summary>
        [Required(ErrorMessage = "A book must have at least one author")]
        public virtual ICollection<Author> Authors { get; set; }

        /// <summary>
        /// Gets or sets the number of total number of books in the library for the current book
        /// </summary>
        [Range(0, int.MaxValue, ErrorMessage = "Number of total books must be 0 or more")]
        public int NumberOfTotalBooks { get; set; }

        /// <summary>
        /// Gets or sets the number of books only available for the reading room for the current book
        /// </summary>
        [Range(0, int.MaxValue, ErrorMessage = "Number of reading room books must be 0 or more")]
        public int NumberOfReadingRoomBooks { get; set; }

        /// <summary>
        /// Gets or sets the number of available books for borrowing for the current book
        /// </summary>
        [Range(0, int.MaxValue, ErrorMessage = "Number of available books for borrowing must be 0 or more")]
        public int NumberOfAvailableBooks { get; set; }

        /// <summary>
        /// Gets or sets the borrowed books (one-to-many relationship)
        /// </summary>
        public virtual ICollection<BorrowedBooks> BorrowedBooks { get; set; }

        /// <summary>
        /// Gets or sets the current book's edition (one-to-one relation)
        /// </summary>
        [Required(ErrorMessage = "A book must have an edition")]
        public virtual Edition Edition { get; set; }
    }
}
