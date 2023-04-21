using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdWithLogin_data
{
    public class Ad
    {
        public int Id { get; set; }
        public DateTime DateCreated { get; set; }
        public string Details { get; set; }
        public string Title { get; set; }
    }
}
