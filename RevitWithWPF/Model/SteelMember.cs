using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteelColumn
{
    public class SteelMember
    {
        public Point Location { get; set; }
        public Point RefDirection { get; set; }
        public Point Axis { get; set; }
        public string Name { get; set; }
        public double BottomLevel { get; set; }
        public double TopLevel { get; set; }
        public double Width { get; set; }
        public double Depth { get; set; }
        public double FlangeTh { get; set; }
        public double WebTh { get; set; }
    }
}
