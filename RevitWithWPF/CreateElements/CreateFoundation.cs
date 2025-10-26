using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.DB;
using SteelColumn.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using ClassLibrary1;
using ClassLibrary1.Model;

namespace SteelColumn.CreateElements
{
    
    
    public class CreateFoundation
    {
        public static int RevitVersion;
        public static FamilyInstance CreateFoundation1(Document document, FamilySymbol familySymbol, Foundation foundation)
        {

            StructuralType stBeam = StructuralType.Footing;
            XYZ p1 = new XYZ(Util.MmToFoot(foundation.Location.X), Util.MmToFoot(foundation.Location.Y), Util.MmToFoot(foundation.Location.Z));

            if (!familySymbol.IsActive)
            {
                familySymbol.Activate();
                document.Regenerate();
            }
            FamilyInstance fi = document.Create.NewFamilyInstance(p1, familySymbol, null, stBeam);

            var sdasd = fi.LookupParameter("Reference Level");
            fi.LookupParameter("Height Offset From Level").Set(Util.MmToFoot(foundation.Location.Z) + Util.MmToFoot(foundation.depth));
            fi.LookupParameter("Reference Level").Set(CreateStlColumn.GetSlabLevel(document, Util.MmToFoot(foundation.Location.Z)).Id);
            fi.get_Parameter(BuiltInParameter.SCHEDULE_LEVEL_PARAM).Set(CreateStlColumn.GetSlabLevel(document, Util.MmToFoot(foundation.Location.Z)).Id);
            return fi;
            
        }

    }
}
