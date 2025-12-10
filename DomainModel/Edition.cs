using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataMapper
{
    public class Edition
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Book reference is required")]
        public Book Book { get; set; }

        [Required(ErrorMessage = "Publisher is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Publisher name must be between 2 and 100 characters")]
        public string Publisher { get; set; }

        [Range(1500, 2030, ErrorMessage = "Year must be between 1500 and 2030")]
        public int Year { get; set; }

        [Range(1, 10000, ErrorMessage = "Pages must be between 1 and 10000")]
        public int Pages { get; set; }

        [StringLength(50, ErrorMessage = "Type cannot exceed 50 characters")]
        public string Type { get; set; } // Ex: Hardcover, Paperback

        public List<Copy> Copies { get; } = new List<Copy>();

    }
}
