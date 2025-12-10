using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataMapper
{
    public class Domain
    {

        public int Id { get; set; }

        [Required(ErrorMessage = "Domain name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Domain name must be between 2 and 100 characters")]
        public string Name { get; set; }

        public Domain Parent { get; set; }

        public List<Domain> Children { get; } = new List<Domain>();

        public Domain(string name)
        {
            Name = name;
        }

        // Get all ancestor domains (parents, grandparents, etc.)
        public List<Domain> GetAncestors()
        {
            var ancestors = new List<Domain>();
            var current = Parent;
            while (current != null)
            {
                ancestors.Add(current);
                current = current.Parent;
            }
            return ancestors;
        }

        // Check if this domain is leaf (has no children)
        public bool IsLeaf => Children.Count == 0;
    }
}
