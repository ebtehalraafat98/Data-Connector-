using Autodesk.Revit.DB;
using SteelColumn.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xbim.Ifc4.GeometricConstraintResource;
using Xbim.Ifc4.GeometryResource;
using Xbim.Ifc4.Interfaces;

namespace SteelColumn.GetData
{
    public class GetLevel
    {

        public static level GetLevelData(IIfcGrid level)
        {
            level lvl = new level();
            lvl.Name = level.Name;
            double elevetion = ((level.ObjectPlacement as IfcLocalPlacement).RelativePlacement as IfcAxis2Placement3D).Location.Z;
            lvl.elevetion = UnitUtils.ConvertToInternalUnits(elevetion, UnitTypeId.Millimeters);

            return lvl;
        }
    }
}
