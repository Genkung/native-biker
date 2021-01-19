using System;
using System.Collections.Generic;
using System.Text;

namespace Biker.Models
{
    public class SecurityToken
    {
        public string Token { get; set; }
        public DateTime ExpiredDate { get; set; }
    }
}
