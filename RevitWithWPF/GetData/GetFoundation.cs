using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.DB;
using SteelColumn.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xbim.Ifc4.GeometricConstraintResource;
using Xbim.Ifc4.GeometricModelResource;
using Xbim.Ifc4.GeometryResource;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.ProfileResource;
using ClassLibrary1.Model;
using Xbim.Ifc4.StructuralElementsDomain;
using Xbim.Ifc4.RepresentationResource;

namespace SteelColumn.GetData
{
    public class GetFoundation
    {
        #region Footing

        public static Foundation foun(IfcFooting ifcFooting)
        {
            var name = ifcFooting.Description;
            var Depth = (((ifcFooting.Representation.Representations.FirstOrDefault() as IfcShapeRepresentation).Items.FirstOrDefault() as IfcExtrudedAreaSolid).Depth);


            var width = (((((ifcFooting.Representation.Representations.FirstOrDefault() as Xbim.Ifc4.Interfaces.IIfcShapeRepresentation).Items.FirstOrDefault()
                         as Xbim.Ifc4.Interfaces.IIfcExtrudedAreaSolid).SweptArea as Xbim.Ifc4.ProfileResource.IfcArbitraryClosedProfileDef).OuterCurve as IfcIndexedPolyCurve
                         ).Points as IfcCartesianPointList2D).CoordList[2][0] * 2;

            var length = (((((ifcFooting.Representation.Representations.FirstOrDefault() as Xbim.Ifc4.Interfaces.IIfcShapeRepresentation).Items.FirstOrDefault()
                        as Xbim.Ifc4.Interfaces.IIfcExtrudedAreaSolid).SweptArea as Xbim.Ifc4.ProfileResource.IfcArbitraryClosedProfileDef).OuterCurve as IfcIndexedPolyCurve
                         ).Points as IfcCartesianPointList2D).CoordList[2][0] * 2;


            Point _Location = new Point();

            var ifcLength = ((((ifcFooting.ObjectPlacement as IfcLocalPlacement).PlacementRelTo as IfcLocalPlacement).RelativePlacement
                             as IfcAxis2Placement3D).Axis).X;


            _Location.X = ((((ifcFooting.ObjectPlacement as IfcLocalPlacement).PlacementRelTo as IfcLocalPlacement).RelativePlacement
                 as IfcAxis2Placement3D).Location as IfcCartesianPoint).Coordinates[0];

            _Location.Y = ((((ifcFooting.ObjectPlacement as IfcLocalPlacement).PlacementRelTo as IfcLocalPlacement).RelativePlacement
                        as IfcAxis2Placement3D).Location as IfcCartesianPoint).Coordinates[1];

            _Location.Z = ((((ifcFooting.ObjectPlacement as IfcLocalPlacement).PlacementRelTo as IfcLocalPlacement).RelativePlacement
                        as IfcAxis2Placement3D).Location as IfcCartesianPoint).Coordinates[2];

            Point _Axis = new Point();

            _Axis.X = ((((ifcFooting.ObjectPlacement as IfcLocalPlacement).PlacementRelTo as IfcLocalPlacement).RelativePlacement
                            as IfcAxis2Placement3D).Axis).X;

            _Axis.Y = ((((ifcFooting.ObjectPlacement as IfcLocalPlacement).PlacementRelTo as IfcLocalPlacement).RelativePlacement
                             as IfcAxis2Placement3D).Axis).Y;

            _Axis.Z = ((((ifcFooting.ObjectPlacement as IfcLocalPlacement).PlacementRelTo as IfcLocalPlacement).RelativePlacement
                             as IfcAxis2Placement3D).Axis).Z;

            Point _RefDirection = new Point();


            _RefDirection.X = ((((ifcFooting.ObjectPlacement as IfcLocalPlacement).PlacementRelTo as IfcLocalPlacement).RelativePlacement
                             as IfcAxis2Placement3D).RefDirection).X;


            _RefDirection.Y = ((((ifcFooting.ObjectPlacement as IfcLocalPlacement).PlacementRelTo as IfcLocalPlacement).RelativePlacement
                             as IfcAxis2Placement3D).RefDirection).Y;


            _RefDirection.Z = ((((ifcFooting.ObjectPlacement as IfcLocalPlacement).PlacementRelTo as IfcLocalPlacement).RelativePlacement
                             as IfcAxis2Placement3D).RefDirection).Z;


            Foundation c = new Foundation()
            {
                Name = ifcFooting.Description,

                Length = (((((ifcFooting.Representation.Representations.FirstOrDefault() as Xbim.Ifc4.Interfaces.IIfcShapeRepresentation).Items.FirstOrDefault()
                        as Xbim.Ifc4.Interfaces.IIfcExtrudedAreaSolid).SweptArea as Xbim.Ifc4.ProfileResource.IfcArbitraryClosedProfileDef).OuterCurve as IfcIndexedPolyCurve
                         ).Points as IfcCartesianPointList2D).CoordList[2][0] * 2,

                Width = (((((ifcFooting.Representation.Representations.FirstOrDefault() as Xbim.Ifc4.Interfaces.IIfcShapeRepresentation).Items.FirstOrDefault()
                         as Xbim.Ifc4.Interfaces.IIfcExtrudedAreaSolid).SweptArea as Xbim.Ifc4.ProfileResource.IfcArbitraryClosedProfileDef).OuterCurve as IfcIndexedPolyCurve
                         ).Points as IfcCartesianPointList2D).CoordList[2][0] * 2,

                depth = (((ifcFooting.Representation.Representations.FirstOrDefault() as IfcShapeRepresentation).Items.FirstOrDefault() as IfcExtrudedAreaSolid).Depth),

                Location = _Location,

                Axis = _Axis,

                RefDirection = _RefDirection


            };
            return c;
            
        }

        #endregion
    }
}
