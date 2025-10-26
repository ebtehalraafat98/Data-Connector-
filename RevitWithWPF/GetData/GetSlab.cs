using SteelColumn.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using Xbim.Ifc4.GeometricConstraintResource;
using Xbim.Ifc4.GeometricModelResource;
using Xbim.Ifc4.GeometryResource;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.ProfileResource;

namespace SteelColumn.GetData
{
    public class GetSlab
    {
        public static FloorSlab GetFloorsData(IIfcSlab floor)
        {
            Point location = new Point()
            {
                X = ((((floor.ObjectPlacement as IfcLocalPlacement).PlacementRelTo as IfcLocalPlacement).RelativePlacement
                as IfcAxis2Placement3D).Location as IfcCartesianPoint).X,

                Y = ((((floor.ObjectPlacement as IfcLocalPlacement).PlacementRelTo as IfcLocalPlacement).RelativePlacement
                as IfcAxis2Placement3D).Location as IfcCartesianPoint).Y,
                Z = 0

            };

            Point refDir = new Point()
            {
                X = ((((floor.ObjectPlacement as IfcLocalPlacement).PlacementRelTo as IfcLocalPlacement).RelativePlacement
                    as IfcAxis2Placement3D).Location as IfcCartesianPoint).Coordinates[0],

                Y = ((((floor.ObjectPlacement as IfcLocalPlacement).PlacementRelTo as IfcLocalPlacement).RelativePlacement
                    as IfcAxis2Placement3D).Location as IfcCartesianPoint).Coordinates[1],

                Z = 0
            };

            Vector3D refd = new Vector3D(refDir.X, refDir.Y, refDir.Z);
            Vector3D yaxis = Vector3D.CrossProduct(new Vector3D(0, 0, 1), refd);
            yaxis.Normalize();

            Matrix3D transform = new Matrix3D(
                                              refDir.X, refDir.Y, 0, 0,
                                              yaxis.X, yaxis.Y, 0, 0,
                                              0, 0, 1.0, 0,
                                              location.X, location.Y, location.Z, 1.0);

            var x = ((((((floor.Representation.Representations.FirstOrDefault() as IIfcShapeRepresentation).Items.FirstOrDefault() as IIfcExtrudedAreaSolid)
                .SweptArea as IfcArbitraryClosedProfileDef).OuterCurve) as IfcIndexedPolyCurve).Points as IfcCartesianPointList2D)
                .CoordList;


            List<Point> y = new List<Point>();
            Point e;

            for (int i = 0; i < x.Count; i++)
            {
                e = new Point();
                int j = 0;
                e.X = x[i][j];
                e.Y = x[i][j + 1]- 6000;
                e.Z = 0;
                y.Add(e);
            }

            FloorSlab f = new FloorSlab()
            {
                Name = floor.Name.Value.ToString(),

                Location = location,

                RefDirection = refDir,

                Mat = transform,

                //Level = (floor.ContainedInStructure.FirstOrDefault().RelatingStructure as IIfcBuildingStorey).Elevation.Value,



                Profile = y,

                //need to be checked with ifc Viewer (the value of the end point )
                // i think or sure we will add the value of loctaion.x and y to the above value;

                Depth = (floor.Representation.Representations.FirstOrDefault().Items.FirstOrDefault() as IIfcExtrudedAreaSolid).Depth,
                Level = ((((floor.ObjectPlacement as IfcLocalPlacement).PlacementRelTo as IfcLocalPlacement).RelativePlacement
                as IfcAxis2Placement3D).Location as IfcCartesianPoint).Z + (floor.Representation.Representations.FirstOrDefault().Items.FirstOrDefault() as IIfcExtrudedAreaSolid).Depth / 2,

            };


            return f;


        }
    }
}
