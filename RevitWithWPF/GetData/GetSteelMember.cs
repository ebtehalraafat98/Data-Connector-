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
    public class GetSteelMember
    {
        #region MemberColumnsStMe

        public static List<SteelMember> GetMembersSteelData(List<IIfcMember> membersSt)
        {
            return membersSt.Select(mmbr => GetMembersSteelData(mmbr)).ToList();
        }
        public static SteelMember GetMembersSteelData(IIfcMember SteelMember)
        {
            var CoordList = ((((((SteelMember.Representation.Representations.FirstOrDefault() as IIfcShapeRepresentation).Items.FirstOrDefault() as IIfcExtrudedAreaSolid)
                                        .SweptArea as IfcArbitraryClosedProfileDef).OuterCurve) as IfcIndexedPolyCurve).Points as IfcCartesianPointList2D)
                                        .CoordList;
            Point location = new Point();

            location.X = ((((SteelMember.ObjectPlacement as IfcLocalPlacement).PlacementRelTo as IfcLocalPlacement).RelativePlacement
                   as IfcAxis2Placement3D).Location as IfcCartesianPoint).Coordinates[0];

            location.Y = ((((SteelMember.ObjectPlacement as IfcLocalPlacement).PlacementRelTo as IfcLocalPlacement).RelativePlacement
                   as IfcAxis2Placement3D).Location as IfcCartesianPoint).Coordinates[1];

            location.Z = 0;

            Point refDir = new Point();

            refDir.X = ((((SteelMember.ObjectPlacement as IfcLocalPlacement).PlacementRelTo as IfcLocalPlacement).RelativePlacement
                       as IfcAxis2Placement3D).RefDirection as IfcDirection).DirectionRatios[0];

            refDir.Y = ((((SteelMember.ObjectPlacement as IfcLocalPlacement).PlacementRelTo as IfcLocalPlacement).RelativePlacement
                       as IfcAxis2Placement3D).RefDirection as IfcDirection).DirectionRatios[1];

            refDir.Z = ((((SteelMember.ObjectPlacement as IfcLocalPlacement).PlacementRelTo as IfcLocalPlacement).RelativePlacement
                       as IfcAxis2Placement3D).RefDirection as IfcDirection).DirectionRatios[2];

            Point axis = new Point();

            axis.X = ((((SteelMember.ObjectPlacement as IfcLocalPlacement).PlacementRelTo as IfcLocalPlacement).RelativePlacement
                   as IfcAxis2Placement3D).Axis as IfcDirection).DirectionRatios[0];

            axis.Y = ((((SteelMember.ObjectPlacement as IfcLocalPlacement).PlacementRelTo as IfcLocalPlacement).RelativePlacement
                   as IfcAxis2Placement3D).Axis as IfcDirection).DirectionRatios[1];

            axis.Z = ((((SteelMember.ObjectPlacement as IfcLocalPlacement).PlacementRelTo as IfcLocalPlacement).RelativePlacement
                   as IfcAxis2Placement3D).Axis as IfcDirection).DirectionRatios[2];


            SteelMember mmbr = new SteelMember()
            {

                RefDirection = refDir,

                Axis = axis,

                Location = location,

                Name = SteelMember.Name.Value.ToString(),

                BottomLevel = ((((SteelMember.ObjectPlacement as IfcLocalPlacement).PlacementRelTo as IfcLocalPlacement).RelativePlacement
                   as IfcAxis2Placement3D).Location as IfcCartesianPoint).Coordinates[2],

                TopLevel = ((((SteelMember.ObjectPlacement as IfcLocalPlacement).PlacementRelTo as IfcLocalPlacement).RelativePlacement
                   as IfcAxis2Placement3D).Location as IfcCartesianPoint).Coordinates[2] +
                   ((SteelMember.Representation.Representations.FirstOrDefault() as IIfcShapeRepresentation).Items.FirstOrDefault() as IIfcExtrudedAreaSolid).Depth,

                Width = CoordList[1].FirstOrDefault() - CoordList[0].FirstOrDefault(),

                Depth = CoordList[7].LastOrDefault() - CoordList[0].LastOrDefault(),

                FlangeTh = CoordList[2].LastOrDefault() - CoordList[1].LastOrDefault(),

                WebTh = CoordList[3].FirstOrDefault() - CoordList[10].FirstOrDefault(),

            };
            return mmbr;
        }
        #endregion
    }
}
