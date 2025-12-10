using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataMapper
{
    public class Copy
    {
        public int Id { get; set; }
        public Edition Edition { get; set; }
        public bool IsReadingRoomOnly { get; set; }
        public bool IsAvailable { get; set; }
    }
}
