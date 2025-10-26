using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace SteelColumn.Model
{
    public class FloorSlab
    {
        public string Name { get; set; }
        public double Level { get; set; }
        public List<Point> Profile { get; set; }
        public Matrix3D Mat { get; set; }
        public Point RefDirection { get; set; }
        public Point Location { get; set; }
        public double Depth { get; set; }
    }
}
