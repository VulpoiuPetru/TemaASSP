using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainModel
{
    public class Domain
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="Domain"/> class
        /// </summary>
        public Domain()
        {
            this.Books = new HashSet<Book>();
            this.Subdomains = new HashSet<Domain>();
        }

        /// <summary>
        /// Gets or sets the Id of the domain
        /// </summary>
        public int DomainId { get; set; }

        /// <summary>
        /// Gets or sets the name of the book domain
        /// </summary>
        [Required(ErrorMessage = "The name cannot be null")]
        [StringLength(50, ErrorMessage = "The domain name must be between 5 and 50 characters", MinimumLength = 5)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the parent domain of the current domain.
        /// </summary>
        public Domain Parent { get; set; }

        /// <summary>
        /// Gets or sets the subdomains of the current domain.
        /// </summary>
        public virtual ICollection<Domain> Subdomains { get; set; }

        /// <summary>
        /// Gets or sets the books from the domain.
        /// </summary>
        public virtual ICollection<Book> Books { get; set; }

        /// <summary>
        /// Gets all ancestor domains (parent, grandparent, etc.) of the current domain
        /// </summary>
        /// <returns>A list of all ancestor domains</returns>
        public List<Domain> GetAllAncestors()
        {
            var ancestors = new List<Domain>();
            var current = this.Parent;

            while (current != null)
            {
                ancestors.Add(current);
                current = current.Parent;
            }

            return ancestors;
        }
    }
}
