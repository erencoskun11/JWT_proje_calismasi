using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWT_project.core.Models
{
    public class UserRefreshToken
    {
        public int User { get; set; }
        public string Code { get; set; }
        public DateTime Expiration { get; set; }
    }
}
