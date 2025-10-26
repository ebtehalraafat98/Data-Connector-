using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xbim.Ifc4.GeometricConstraintResource;
using Xbim.Ifc4.GeometricModelResource;
using Xbim.Ifc4.GeometryResource;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.ProfileResource;

namespace SteelColumn
{
    public class GetSteelData
    {
        public static SteelColumn GetSteelColumnData(IIfcColumn columnSt)
        {
            var CoordList = ((((((columnSt.Representation.Representations.FirstOrDefault() as IIfcShapeRepresentation).Items.FirstOrDefault() as IIfcExtrudedAreaSolid)
                                        .SweptArea as IfcArbitraryClosedProfileDef).OuterCurve) as IfcIndexedPolyCurve).Points as IfcCartesianPointList2D)
                                        .CoordList;
            Point location = new Point();

            location.X = ((((columnSt.ObjectPlacement as IfcLocalPlacement).PlacementRelTo as IfcLocalPlacement).RelativePlacement
                   as IfcAxis2Placement3D).Location as IfcCartesianPoint).Coordinates[0];

            location.Y = ((((columnSt.ObjectPlacement as IfcLocalPlacement).PlacementRelTo as IfcLocalPlacement).RelativePlacement
                   as IfcAxis2Placement3D).Location as IfcCartesianPoint).Coordinates[1];
            location.Z = 0;

            Point refDir = new Point();

            refDir.X = ((((columnSt.ObjectPlacement as IfcLocalPlacement).PlacementRelTo as IfcLocalPlacement).RelativePlacement
                       as IfcAxis2Placement3D).Axis as IfcDirection).DirectionRatios[0];

            refDir.Y = ((((columnSt.ObjectPlacement as IfcLocalPlacement).PlacementRelTo as IfcLocalPlacement).RelativePlacement
                   as IfcAxis2Placement3D).Axis as IfcDirection).DirectionRatios[1];

            refDir.Z = ((((columnSt.ObjectPlacement as IfcLocalPlacement).PlacementRelTo as IfcLocalPlacement).RelativePlacement
                   as IfcAxis2Placement3D).Axis as IfcDirection).DirectionRatios[2];

            SteelColumn col = new SteelColumn()
            {

                RefDirection = refDir,

                Location = location,

                Name = columnSt.Name.Value.ToString(),

                BottomLevel = ((((columnSt.ObjectPlacement as IfcLocalPlacement).PlacementRelTo as IfcLocalPlacement).RelativePlacement
                   as IfcAxis2Placement3D).Location as IfcCartesianPoint).Coordinates[2],

                TopLevel = ((((columnSt.ObjectPlacement as IfcLocalPlacement).PlacementRelTo as IfcLocalPlacement).RelativePlacement
                   as IfcAxis2Placement3D).Location as IfcCartesianPoint).Coordinates[2] +
                   ((columnSt.Representation.Representations.FirstOrDefault() as IIfcShapeRepresentation).Items.FirstOrDefault() as IIfcExtrudedAreaSolid).Depth,

                Width = CoordList[1].FirstOrDefault() - CoordList[0].FirstOrDefault(),

                Depth = CoordList[7].LastOrDefault() - CoordList[0].LastOrDefault(),

                FlangeTh = CoordList[2].LastOrDefault() - CoordList[1].LastOrDefault(),

                WebTh = CoordList[3].FirstOrDefault() - CoordList[10].FirstOrDefault(),

            };
            return col;
        }
    }
}
