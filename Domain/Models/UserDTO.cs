using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models
{
    public class UserDTO
    {
        public Guid uuid { get; set; }
        public string name { get; set; }
        public int age { get; set; }
    }
}
