using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainModel
{
    /// <summary>
    /// Represents a physical copy of a book edition that can be borrowed or restricted to reading room only. 
    /// </summary>
    public class Copy
    {
        /// <summary>
        /// Gets or sets the unique identifier for the copy. 
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the edition that this copy belongs to.
        /// </summary>
        [Required(ErrorMessage = "Edition reference is required")]
        public virtual Edition Edition { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this copy is restricted to reading room only.
        /// </summary>
        [Required(ErrorMessage = "Reading room restriction status must be specified")]
        public bool IsReadingRoomOnly { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this copy is currently available for borrowing.
        /// </summary>
        [Required(ErrorMessage = "Availability status must be specified")]
        public bool IsAvailable { get; set; }

        /// <summary>
        /// Gets a value indicating whether this copy can be borrowed (not reading room only and available).
        /// </summary>
        public bool CanBeBorrowed => !IsReadingRoomOnly && IsAvailable;
    }
}
