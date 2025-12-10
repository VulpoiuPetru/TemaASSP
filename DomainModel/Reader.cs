using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataMapper
{
    public class Reader
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public bool IsStaffMember { get; set; }

        // borrowed books etc.
        public List<Loan> Loans { get; } = new List<Loan>();
    }
}
