using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;


namespace DomainModel
{
    public class Reader
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Reader"/> class
        /// </summary>
        public Reader()
        {
            this.Extensions = new HashSet<Extension>();
        }

        /// <summary>
        /// Gets or sets the id of the user
        /// </summary>
        public int ReaderId { get; set; }

        /// <summary>
        /// Gets or sets the first name of the reader
        /// </summary>
        [Required(ErrorMessage = "The first name cannot be null")]
        [StringLength(50, ErrorMessage = "First name must have between 3 and 30 characters", MinimumLength = 5)]
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the last name of the reader
        /// </summary>
        [Required(ErrorMessage = "The last name cannot be null")]
        [StringLength(50, ErrorMessage = "Last name must have between 3 and 30 characters", MinimumLength = 5)]
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets the age of the reader
        /// </summary>
        [Required(ErrorMessage = "The age cannot be null")]
        [RegularExpression("[1-9]+[0-9]*")]
        [Range(10, 80, ErrorMessage = "Age must be between 10 and 80 years")]
        public int Age { get; set; }

        /// <summary>
        /// Gets or sets the phone number of the reader
        /// </summary>
        [Phone(ErrorMessage = "Invalid phone number")]
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Gets or sets the email of the reader
        /// </summary>
        [EmailAddress(ErrorMessage = "Invalid e-mail address")]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the total number of extensions requested by the current reader
        /// </summary>
        [Range(0, int.MaxValue, ErrorMessage = "The value must be positive")]
        public int NumberOfExtensions { get; set; }

        /// <summary>
        /// Gets or sets the extensions of the current reader
        /// </summary>
        public ICollection<Extension> Extensions { get; set; }

        /// <summary>
        /// Gets or sets the borrowed books (one-to-many relationship)
        /// </summary>
        public virtual ICollection<BorrowedBooks> BorrowedBooks { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether a reader is also an employee for the library
        /// </summary>
        [Required(ErrorMessage = "The isEmployee bool cannot be null")]
        public bool IsEmployee { get; set; }

        /// <summary>
        /// Custom validation:  at least one contact method required
        /// </summary>
        public bool HasValidContact()
        {
            return !string.IsNullOrWhiteSpace(Email) || !string.IsNullOrWhiteSpace(PhoneNumber);
        }
    }
}
