using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DomainModel
{
   public class Author
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Author" /> class
        /// </summary>
        public Author()
        {
            this.Books = new HashSet<Book>();
        }

        /// <summary>
        /// Gets or sets the Id of the book author.
        /// </summary>
        public int AuthorId { get; set; }

        /// <summary>
        /// Gets or sets the first name of the book author.
        /// </summary>
        [Required(ErrorMessage = "The first name cannot be null")]
        [StringLength(50, ErrorMessage = "First name must have between 5 and 50 characters", MinimumLength = 5)]
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the last name of the book author.
        /// </summary>
        [Required(ErrorMessage = "The last name cannot be null")]
        [StringLength(50, ErrorMessage = "Last name must have between 3 and 30 characters", MinimumLength = 5)]
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets the age of the book author.
        /// </summary>
        [Required(ErrorMessage = "The age cannot be null")]
        [RegularExpression("[1-9]+[0-9]*")]
        [Range(10, 80, ErrorMessage = "Age must be between 10 and 80 years")]
        public int Age { get; set; }

        /// <summary>
        /// Gets or sets the books written by the author.
        /// </summary>
        [Required(ErrorMessage = "You must have written a book to be an author")]
        public virtual ICollection<Book> Books { get; set; }
    }
}
