using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataMapper
{
    public class Edition
    {
        public int Id { get; set; }
        public Book Book { get; set; }
        public string Publisher { get; set; }
        public int Year { get; set; }
        public int Pages { get; set; }
        public string Type { get; set; } // Ex: Hardcover, Paperback

    }
}
