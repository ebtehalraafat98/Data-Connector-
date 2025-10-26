using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xbim.Ifc4.GeometricConstraintResource;
using Xbim.Ifc4.GeometricModelResource;
using Xbim.Ifc4.GeometryResource;

namespace SteelColumn.GetData
{
    public class Getgrid
    {
        public static Model.grid GetGridData(IfcGridAxis Grids)
        {
            Point startPoint = new Point();
            startPoint.X =
            UnitUtils.ConvertToInternalUnits((((Grids.AxisCurve as IfcIndexedPolyCurve).Points as IfcCartesianPointList2D)
                .CoordList[0][0]), UnitTypeId.Millimeters);

            startPoint.Y =
            UnitUtils.ConvertToInternalUnits((((Grids.AxisCurve as IfcIndexedPolyCurve).Points as IfcCartesianPointList2D)
                .CoordList[0][1]), UnitTypeId.Millimeters);
            startPoint.Z = 0;

            Point endpPoint = new Point();
            endpPoint.X =
            UnitUtils.ConvertToInternalUnits((((Grids.AxisCurve as IfcIndexedPolyCurve).Points as IfcCartesianPointList2D)
                .CoordList[1][0]), UnitTypeId.Millimeters);
            endpPoint.Y =
            UnitUtils.ConvertToInternalUnits((((Grids.AxisCurve as IfcIndexedPolyCurve).Points as IfcCartesianPointList2D)
                .CoordList[1][1]), UnitTypeId.Millimeters);
            endpPoint.Z = 0;


            Model.grid grid = new Model.grid()
            {
                Name = Grids.AxisTag,
                StartPoint = startPoint,
                EndPoint = endpPoint
            };


            return grid;

        }
    }
}
