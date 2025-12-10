using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataMapper
{
    public class Copy
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Edition reference is required")]
        public Edition Edition { get; set; }
        public bool IsReadingRoomOnly { get; set; }
        public bool IsAvailable { get; set; }

        // Check if copy can be borrowed
        public bool CanBeBorrowed => !IsReadingRoomOnly && IsAvailable;
    }
}
