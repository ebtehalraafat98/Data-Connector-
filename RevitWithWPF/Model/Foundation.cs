using SteelColumn;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary1.Model
{
	public class Foundation
	{
		public string Name { get; set; }
		public double Length { get; set; }
		public double Width { get; set; }
		public double depth { get; set; }
		public Point Location { get; set; }
		public Point RefDirection { get; set; }
		public Point Axis { get; set; }
	}
}
