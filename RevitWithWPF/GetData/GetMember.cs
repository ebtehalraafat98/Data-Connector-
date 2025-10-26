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

namespace SteelColumn
{
    public class GetMember
    {

        public static List<Member> GetMembersData(List<IIfcMember> members)
        {

            return members.Select(mmbr => GetMembersData(mmbr)).ToList();
        }

        
        #region MemberColumnsMe

        public static Member GetMembersData(IIfcMember member)
        {
            var CoordList = ((((((member.Representation.Representations.FirstOrDefault() as IIfcShapeRepresentation).Items.FirstOrDefault() as IIfcExtrudedAreaSolid)
                                        .SweptArea as IfcArbitraryClosedProfileDef).OuterCurve) as IfcIndexedPolyCurve).Points as IfcCartesianPointList2D)
                                        .CoordList;

            Point location = new Point()
            {
                X = ((((member.ObjectPlacement as IfcLocalPlacement).PlacementRelTo as IfcLocalPlacement).RelativePlacement
                   as IfcAxis2Placement3D).Location as IfcCartesianPoint).Coordinates[0],

                Y = ((((member.ObjectPlacement as IfcLocalPlacement).PlacementRelTo as IfcLocalPlacement).RelativePlacement
                   as IfcAxis2Placement3D).Location as IfcCartesianPoint).Coordinates[1],
                Z = ((((member.ObjectPlacement as IfcLocalPlacement).PlacementRelTo as IfcLocalPlacement).RelativePlacement
                   as IfcAxis2Placement3D).Location as IfcCartesianPoint).Coordinates[2],

            };


            Point refDir = new Point()
            {
                //X = ((((member.ObjectPlacement as IfcLocalPlacement).PlacementRelTo as IfcLocalPlacement).RelativePlacement
                //as IfcAxis2Placement3D).RefDirection as IfcDirection).DirectionRatios[0],

                X = ((((member.ObjectPlacement as IfcLocalPlacement).PlacementRelTo as
                    IfcLocalPlacement).RelativePlacement as IfcAxis2Placement3D).RefDirection as IfcDirection).X,

                Y = ((((member.ObjectPlacement as IfcLocalPlacement).PlacementRelTo as
                    IfcLocalPlacement).RelativePlacement as IfcAxis2Placement3D).RefDirection as IfcDirection).Y,

                Z = ((((member.ObjectPlacement as IfcLocalPlacement).PlacementRelTo as
                    IfcLocalPlacement).RelativePlacement as IfcAxis2Placement3D).RefDirection as IfcDirection).Z,
            };

            Point axis = new Point()
            {
                //X = ((((member.ObjectPlacement as IfcLocalPlacement).PlacementRelTo as IfcLocalPlacement).RelativePlacement
                //as IfcAxis2Placement3D).RefDirection as IfcDirection).DirectionRatios[0],

                X = ((((member.ObjectPlacement as IfcLocalPlacement).PlacementRelTo as
                   IfcLocalPlacement).RelativePlacement as IfcAxis2Placement3D).Axis as IfcDirection).X,

                Y = ((((member.ObjectPlacement as IfcLocalPlacement).PlacementRelTo as
                   IfcLocalPlacement).RelativePlacement as IfcAxis2Placement3D).Axis as IfcDirection).Y,

                Z = ((((member.ObjectPlacement as IfcLocalPlacement).PlacementRelTo as
                   IfcLocalPlacement).RelativePlacement as IfcAxis2Placement3D).Axis as IfcDirection).Z,
            };
            Member mmbr = new Member()
            {


                Location = location,

                Name = member.Name.Value.ToString(),

                BottomLevel = ((((member.ObjectPlacement as IfcLocalPlacement).PlacementRelTo as IfcLocalPlacement).RelativePlacement
                   as IfcAxis2Placement3D).Location as IfcCartesianPoint).Coordinates[2],

                TopLevel = ((((member.ObjectPlacement as IfcLocalPlacement).PlacementRelTo as IfcLocalPlacement).RelativePlacement
                   as IfcAxis2Placement3D).Location as IfcCartesianPoint).Coordinates[2] +
                   ((member.Representation.Representations.FirstOrDefault() as IIfcShapeRepresentation).Items.FirstOrDefault() as IIfcExtrudedAreaSolid).Depth,

                Width = CoordList[1].FirstOrDefault() - CoordList[0].FirstOrDefault(),

                Depth = CoordList[2].LastOrDefault() - CoordList[1].LastOrDefault(),

                RefDirection = refDir,

                Axis = axis


            };
            return mmbr;
        }
        #endregion

        
    }
}
