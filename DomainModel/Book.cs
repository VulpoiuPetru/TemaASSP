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
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [StringLength(200, MinimumLength = 1, ErrorMessage = "Title must be between 1 and 200 characters")]
        public string Title { get; set; }

        [Required(ErrorMessage = "At least one author is required")]
        [MinLength(1, ErrorMessage = "At least one author must be specified")]
        public List<Author> Authors { get; } = new List<Author>();
        public List<Edition> Editions { get; } = new List<Edition>();

        [Required(ErrorMessage = "At least one domain is required")]
        [MinLength(1, ErrorMessage = "At least one domain must be specified")]
        public List<Domain> Domains { get; } = new List<Domain>();

        // Assign domains with validation rules
        public void SetDomains(IEnumerable<Domain> domains, int maxDomains)
        {
            var domainList = domains.ToList();
            // Validate count
            if (domainList.Count > maxDomains)
                throw new InvalidDomainAssignmentException($"Maximum allowed domains per book is {maxDomains}");

            // Validate: no ancestor-descendant pairs
            for (int i = 0; i < domainList.Count; i++)
                for (int j = 0; j < domainList.Count; j++)
                    if (i != j && IsAncestor(domainList[i], domainList[j]))
                        throw new InvalidDomainAssignmentException("Cannot assign both ancestor and descendant domains directly to book.");

            Domains.Clear();
            Domains.AddRange(domainList);
        }

        // Gets all domains including parent domains (automatic inclusion)
        public List<Domain> GetAllDomains()
        {
            var allDomains = new HashSet<Domain>();

            foreach (var domain in Domains)
            {
                allDomains.Add(domain);
                // Add all parent domains
                var current = domain.Parent;
                while (current != null)
                {
                    allDomains.Add(current);
                    current = current.Parent;
                }
            }

            return allDomains.ToList();
        }

        private bool IsAncestor(Domain ancestor, Domain descendant)
        {
            var current = descendant.Parent;
            while (current != null)
            {
                if (current == ancestor) return true;
                current = current.Parent;
            }
            return false;
        }
    }
    public class InvalidDomainAssignmentException : System.Exception
    {
        public InvalidDomainAssignmentException(string message) : base(message) { }
    }
}
