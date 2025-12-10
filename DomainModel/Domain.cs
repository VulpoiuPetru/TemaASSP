using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataMapper
{
    public class Domain
    {

        public int Id { get; set; }

        public string Name { get; set; }

        public Domain Parent { get; set; }

        public List<Domain> Children { get; } = new List<Domain>();

        public Domain(string name)
        {
            Name = name;
        }
    }
}
