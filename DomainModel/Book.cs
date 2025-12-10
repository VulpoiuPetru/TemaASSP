using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataMapper
{
   public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public List<Author> Authors { get; } = new List<Author>();
        public List<Edition> Editions { get; } = new List<Edition>();

        public List<Domain> Domains { get; } = new List<Domain>();


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
