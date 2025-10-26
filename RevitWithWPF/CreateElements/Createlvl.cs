using Autodesk.Revit.DB;
using SteelColumn.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteelColumn.CreateElements
{
    public class Createlvl
    {
        public static Level CreateLevel(Document doc, level level)
        {
            Level level1 = Level.Create(doc, level.elevetion);
            level1.Name = level.Name;
            return level1;

        }
    }
}
