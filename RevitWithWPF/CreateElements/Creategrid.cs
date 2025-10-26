using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteelColumn.CreateElements
{
    public class Creategrid
    {
        public static Autodesk.Revit.DB.Grid CreateGrid(Autodesk.Revit.DB.Document doc, Model.grid grid)
        {
            Grid gg = null;
            bool check = GridNameExists(doc, grid.Name);
            if (check == false)
            {
                XYZ staXyz = new XYZ(grid.StartPoint.X, grid.StartPoint.Y, grid.StartPoint.Z);
                XYZ endXyz = new XYZ(grid.EndPoint.X, grid.EndPoint.Y, grid.EndPoint.Z);
                Line Gridline = Line.CreateBound(staXyz, endXyz);
                gg = Grid.Create(doc, Gridline);
                gg.Name = grid.Name;

            }
            return gg;
        }
        public static bool GridNameExists(Document doc, string gridName)
        {
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            collector.OfClass(typeof(Grid));

            foreach (Grid grid in collector)
            {
                if (grid.Name == gridName)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
