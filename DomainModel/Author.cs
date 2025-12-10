using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataMapper
{
    public class Author
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Author name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Author name must be between 2 and 100 characters")]
        public string Name { get; set; }
        public Author(string name) => Name = name;
    }
}
