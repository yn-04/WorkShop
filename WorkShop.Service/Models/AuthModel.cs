using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkShop.Service.Models
{
    public  class LoginRequest
    {
        public string Email {  get; set; }
        public string Password { get; set; }
    }
}
