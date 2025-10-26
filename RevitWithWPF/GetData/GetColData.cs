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
    public class GetColData
    {
        public static Column GetColumnsData(IIfcColumn column)
        {
            var CoordList = ((((((column.Representation.Representations.FirstOrDefault() as IIfcShapeRepresentation).Items.FirstOrDefault() as IIfcExtrudedAreaSolid)
                                        .SweptArea as IfcArbitraryClosedProfileDef).OuterCurve) as IfcIndexedPolyCurve).Points as IfcCartesianPointList2D)
                                        .CoordList;

            Point location = new Point()
            {
                X = ((((column.ObjectPlacement as IfcLocalPlacement).PlacementRelTo as IfcLocalPlacement).RelativePlacement
                   as IfcAxis2Placement3D).Location as IfcCartesianPoint).Coordinates[0],

                Y = ((((column.ObjectPlacement as IfcLocalPlacement).PlacementRelTo as IfcLocalPlacement).RelativePlacement
                   as IfcAxis2Placement3D).Location as IfcCartesianPoint).Coordinates[1],
                Z = 0

            };


            Point refDir = new Point()
            {
                X = ((((column.ObjectPlacement as IfcLocalPlacement).PlacementRelTo as
                    IfcLocalPlacement).RelativePlacement as IfcAxis2Placement3D).Axis as IfcDirection).X,

                Y = ((((column.ObjectPlacement as IfcLocalPlacement).PlacementRelTo as
                    IfcLocalPlacement).RelativePlacement as IfcAxis2Placement3D).Axis as IfcDirection).Y,

                Z = ((((column.ObjectPlacement as IfcLocalPlacement).PlacementRelTo as
                    IfcLocalPlacement).RelativePlacement as IfcAxis2Placement3D).Axis as IfcDirection).Z,
            };
            Column col = new Column()
            {


                Location = location,

                Name = column.Name.Value.ToString(),

                BottomLevel = ((((column.ObjectPlacement as IfcLocalPlacement).PlacementRelTo as IfcLocalPlacement).RelativePlacement
                   as IfcAxis2Placement3D).Location as IfcCartesianPoint).Coordinates[2],

                TopLevel = ((((column.ObjectPlacement as IfcLocalPlacement).PlacementRelTo as IfcLocalPlacement).RelativePlacement
                   as IfcAxis2Placement3D).Location as IfcCartesianPoint).Coordinates[2] +
                   ((column.Representation.Representations.FirstOrDefault() as IIfcShapeRepresentation).Items.FirstOrDefault() as IIfcExtrudedAreaSolid).Depth,

                Width = CoordList[1].FirstOrDefault() - CoordList[0].FirstOrDefault(),

                Depth = CoordList[2].LastOrDefault() - CoordList[1].LastOrDefault(),

                RefDirection = refDir


            };
            return col;
        }
    }
}
