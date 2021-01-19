using System;
using System.Collections.Generic;
using System.Text;

namespace Biker.Models
{
    public class AppToken
    {
        public SecurityToken AccessToken { get; set; }
        public SecurityToken RefreshToken { get; set; }
    }
}
