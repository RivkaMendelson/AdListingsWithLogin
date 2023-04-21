using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdWithLogin_data
{
    public class AdWithPersonInfo
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime DateCreated { get; set; }
        public string Details { get; set; }
        public int PersonId { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
    }
}
