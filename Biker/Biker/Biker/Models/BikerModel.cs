using System;
using System.Collections.Generic;
using System.Text;

namespace Biker.Models
{
    public class BikerModel
    {
        public string _id { get; set; }
        public bool OnWorkStatus { get; set; }
        public string Name { get; set; }
        public string profileImage { get; set; }
        public bool Suspended { get; set; }
        public double Rating { get; set; }
    }
}
