using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainModel
{
    public class Edition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Edition"/> class
        /// </summary>
        public Edition()
        {
            this.Copies = new HashSet<Copy>();
        }

        /// <summary>
        /// Gets or sets the id
        /// </summary>
        [ForeignKey("Book")]
        public int EditionId { get; set; }

        /// <summary>
        /// Gets or sets the name of the publisher for the current book
        /// </summary>
        [Required(ErrorMessage = "The publisher name cannot be null")]
        [StringLength(50, ErrorMessage = "The publisher name must be between 5 and 50 characters", MinimumLength = 5)]
        public string Publisher { get; set; }

        /// <summary>
        /// Gets or sets the number of pages for the current book
        /// </summary>
        [Required(ErrorMessage = "The book's number of pages cannot be null")]
        [Range(3, int.MaxValue, ErrorMessage = "The book must contain more than 2 pages")]
        public int NumberOfPages { get; set; }

        /// <summary>
        /// Gets or sets the year when the current book edition was published
        /// </summary>
        [Required(ErrorMessage = "The book's year of publishing cannot be null")]
        [Range(1400, 2100, ErrorMessage = "Year of publishing must be between 1400 and 2100")]
        public int YearOfPublishing { get; set; }

        /// <summary>
        /// Gets or sets the type of the book (paperback or hardcover)
        /// </summary>
        [Required(ErrorMessage = "The book's type cannot be null")]
        [StringLength(50, ErrorMessage = "The book's type must contain between 5 and 50 characters", MinimumLength = 5)]
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the book (for one-to-one relation)
        /// </summary>
        [Required(ErrorMessage = "The edition's coresponding book cannot be null")]
        public virtual Book Book { get; set; }

        /// <summary>
        /// Gets or sets the copies of this edition
        /// </summary>
        public virtual ICollection<Copy> Copies { get; set; }

    }
}
