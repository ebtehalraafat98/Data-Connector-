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

namespace SteelColumn.GetData
{
    public class Getbeam
    {
        #region Beams

        public static Beam GetBeamsData(IIfcBeam beam)
        {

            Point location = new Point();


            location.X = ((((beam.ObjectPlacement as IfcLocalPlacement).PlacementRelTo as IfcLocalPlacement).RelativePlacement
                    as IfcAxis2Placement3D).Location as IfcCartesianPoint).Coordinates[0];

            location.Y = ((((beam.ObjectPlacement as IfcLocalPlacement).PlacementRelTo as IfcLocalPlacement).RelativePlacement
                     as IfcAxis2Placement3D).Location as IfcCartesianPoint).Coordinates[1];



            location.Z = ((((beam.ObjectPlacement as IfcLocalPlacement).PlacementRelTo as IfcLocalPlacement).RelativePlacement
                     as IfcAxis2Placement3D).Location as IfcCartesianPoint).Coordinates[2];





            Point refDirection = new Point();




            refDirection.X = ((((beam.ObjectPlacement as IfcLocalPlacement).PlacementRelTo as
                    IfcLocalPlacement).RelativePlacement as IfcAxis2Placement3D).RefDirection as IfcDirection).X;

            refDirection.Y = ((((beam.ObjectPlacement as IfcLocalPlacement).PlacementRelTo as
                    IfcLocalPlacement).RelativePlacement as IfcAxis2Placement3D).RefDirection as IfcDirection).Y;

            refDirection.Z = ((((beam.ObjectPlacement as IfcLocalPlacement).PlacementRelTo as
                    IfcLocalPlacement).RelativePlacement as IfcAxis2Placement3D).RefDirection as IfcDirection).Z;

            Point axis = new Point();

            axis.X = ((((beam.ObjectPlacement as IfcLocalPlacement).PlacementRelTo as
                    IfcLocalPlacement).RelativePlacement as IfcAxis2Placement3D).Axis as IfcDirection).X;

            axis.Y = ((((beam.ObjectPlacement as IfcLocalPlacement).PlacementRelTo as
                    IfcLocalPlacement).RelativePlacement as IfcAxis2Placement3D).Axis as IfcDirection).Y;

            axis.Z = ((((beam.ObjectPlacement as IfcLocalPlacement).PlacementRelTo as
                    IfcLocalPlacement).RelativePlacement as IfcAxis2Placement3D).Axis as IfcDirection).Z;




            var p1 = ((((((beam.Representation.Representations.FirstOrDefault() as IIfcShapeRepresentation).Items.FirstOrDefault() as IIfcExtrudedAreaSolid)
                .SweptArea as IfcArbitraryClosedProfileDef).OuterCurve) as IfcIndexedPolyCurve).Points as IfcCartesianPointList2D)
                .CoordList[0];


            var p2 = ((((((beam.Representation.Representations.FirstOrDefault() as IIfcShapeRepresentation).Items.FirstOrDefault() as IIfcExtrudedAreaSolid)
                    .SweptArea as IfcArbitraryClosedProfileDef).OuterCurve) as IfcIndexedPolyCurve).Points as IfcCartesianPointList2D)
                    .CoordList[1];

            var p3 = ((((((beam.Representation.Representations.FirstOrDefault() as IIfcShapeRepresentation).Items.FirstOrDefault() as IIfcExtrudedAreaSolid)
                    .SweptArea as IfcArbitraryClosedProfileDef).OuterCurve) as IfcIndexedPolyCurve).Points as IfcCartesianPointList2D)
                    .CoordList[2];




            Beam B = new Beam()
            {
                Name = beam.Name,
                RefDirection = refDirection,
                Axis = axis,
                Location = location,
                H = p3[1] * 2,
                B = p3[0] * 2,
                Length = ((beam.Representation.Representations.FirstOrDefault() as IIfcShapeRepresentation).Items.FirstOrDefault() as IIfcExtrudedAreaSolid).Depth
            };

            return B;
        }
        public static List<Beam> GetBeamsData(List<IIfcBeam> beam)
        {
            List<Beam> myBeam = new List<Beam>();
            foreach (IIfcBeam x in beam)
            {
                myBeam.Add(GetBeamsData(x));
            }
            return myBeam;
        }


        #endregion
        #region Steel Beams I sec
        public static BeamStISEC GetBeamsSteelDataex(IIfcBeam beamSt)
        {

            Point location = new Point();

            location.X = ((((beamSt.ObjectPlacement as IfcLocalPlacement).PlacementRelTo as IfcLocalPlacement).RelativePlacement
                    as IfcAxis2Placement3D).Location as IfcCartesianPoint).Coordinates[0];

            location.Y = ((((beamSt.ObjectPlacement as IfcLocalPlacement).PlacementRelTo as IfcLocalPlacement).RelativePlacement
                     as IfcAxis2Placement3D).Location as IfcCartesianPoint).Coordinates[1];



            location.Z = ((((beamSt.ObjectPlacement as IfcLocalPlacement).PlacementRelTo as IfcLocalPlacement).RelativePlacement
                     as IfcAxis2Placement3D).Location as IfcCartesianPoint).Coordinates[2];


            Point refDirection = new Point();

            refDirection.X = ((((beamSt.ObjectPlacement as IfcLocalPlacement).PlacementRelTo as
                    IfcLocalPlacement).RelativePlacement as IfcAxis2Placement3D).RefDirection as IfcDirection).X;

            refDirection.Y = ((((beamSt.ObjectPlacement as IfcLocalPlacement).PlacementRelTo as
                    IfcLocalPlacement).RelativePlacement as IfcAxis2Placement3D).RefDirection as IfcDirection).Y;

            refDirection.Z = ((((beamSt.ObjectPlacement as IfcLocalPlacement).PlacementRelTo as
                    IfcLocalPlacement).RelativePlacement as IfcAxis2Placement3D).RefDirection as IfcDirection).Z;

            Point axis = new Point();

            axis.X = ((((beamSt.ObjectPlacement as IfcLocalPlacement).PlacementRelTo as
                    IfcLocalPlacement).RelativePlacement as IfcAxis2Placement3D).Axis as IfcDirection).X;

            axis.Y = ((((beamSt.ObjectPlacement as IfcLocalPlacement).PlacementRelTo as
                    IfcLocalPlacement).RelativePlacement as IfcAxis2Placement3D).Axis as IfcDirection).Y;

            axis.Z = ((((beamSt.ObjectPlacement as IfcLocalPlacement).PlacementRelTo as
                    IfcLocalPlacement).RelativePlacement as IfcAxis2Placement3D).Axis as IfcDirection).Z;


            BeamStISEC B = new BeamStISEC()
            {
                Name = beamSt.Name,
                RefDirection = refDirection,
                Axis = axis,
                Location = location,
                Length = 6600/*((beamSt.Representation.Representations.FirstOrDefault() as IIfcShapeRepresentation)*/
                //			.Items.FirstOrDefault() as IIfcExtrudedAreaSolid).Depth,
                ,



                Width = 100 /*((((beamSt.Representation.Representations.FirstOrDefault().Items.FirstOrDefault() as IfcPolygonalFaceSet).Coordinates.CoordList[6][0] * 2)))*/,


                Depth = 100/*((((beamSt.Representation.Representations.FirstOrDefault().Items.FirstOrDefault() as IfcPolygonalFaceSet).Coordinates.CoordList[7][1] * 2)))*/,

                FlangeTh = 15 /*((((beamSt.Representation.Representations.FirstOrDefault().Items.FirstOrDefault() as IfcPolygonalFaceSet).Coordinates.CoordList[3][0]*/
                //- (beamSt.Representation.Representations.FirstOrDefault().Items.FirstOrDefault() as IfcPolygonalFaceSet).Coordinates.CoordList[3][0])))
                ,


                WebTh = 10 /*((((beamSt.Representation.Representations.FirstOrDefault().Items.FirstOrDefault() as IfcPolygonalFaceSet).Coordinates.CoordList[8][1])))*/


            };

            return B;
        }




        #endregion
        public static BeamStISEC GetBeamsSteelData(IIfcBeam beamSt)
        {

            Point location = new Point();

            location.X = ((((beamSt.ObjectPlacement as IfcLocalPlacement).PlacementRelTo as IfcLocalPlacement).RelativePlacement
                    as IfcAxis2Placement3D).Location as IfcCartesianPoint).Coordinates[0];

            location.Y = ((((beamSt.ObjectPlacement as IfcLocalPlacement).PlacementRelTo as IfcLocalPlacement).RelativePlacement
                     as IfcAxis2Placement3D).Location as IfcCartesianPoint).Coordinates[1];



            location.Z = ((((beamSt.ObjectPlacement as IfcLocalPlacement).PlacementRelTo as IfcLocalPlacement).RelativePlacement
                     as IfcAxis2Placement3D).Location as IfcCartesianPoint).Coordinates[2];


            Point refDirection = new Point();

            refDirection.X = ((((beamSt.ObjectPlacement as IfcLocalPlacement).PlacementRelTo as
                    IfcLocalPlacement).RelativePlacement as IfcAxis2Placement3D).RefDirection as IfcDirection).X;

            refDirection.Y = ((((beamSt.ObjectPlacement as IfcLocalPlacement).PlacementRelTo as
                    IfcLocalPlacement).RelativePlacement as IfcAxis2Placement3D).RefDirection as IfcDirection).Y;

            refDirection.Z = ((((beamSt.ObjectPlacement as IfcLocalPlacement).PlacementRelTo as
                    IfcLocalPlacement).RelativePlacement as IfcAxis2Placement3D).RefDirection as IfcDirection).Z;

            Point axis = new Point();

            axis.X = ((((beamSt.ObjectPlacement as IfcLocalPlacement).PlacementRelTo as
        IfcLocalPlacement).RelativePlacement as IfcAxis2Placement3D).Axis as IfcDirection).X;

            axis.Y = ((((beamSt.ObjectPlacement as IfcLocalPlacement).PlacementRelTo as
                    IfcLocalPlacement).RelativePlacement as IfcAxis2Placement3D).Axis as IfcDirection).Y;

            axis.Z = ((((beamSt.ObjectPlacement as IfcLocalPlacement).PlacementRelTo as
                    IfcLocalPlacement).RelativePlacement as IfcAxis2Placement3D).Axis as IfcDirection).Z;


            BeamStISEC B = new BeamStISEC()
            {
                Name = beamSt.Name,
                RefDirection = refDirection,
                Axis = axis,
                Location = location,
                Length = ((beamSt.Representation.Representations.FirstOrDefault() as IIfcShapeRepresentation)
                            .Items.FirstOrDefault() as IIfcExtrudedAreaSolid).Depth,



                Width = ((((((beamSt.Representation.Representations.FirstOrDefault() as
                    IIfcShapeRepresentation).Items.FirstOrDefault() as IIfcExtrudedAreaSolid)
                    .SweptArea as IfcArbitraryClosedProfileDef).OuterCurve) as
                    IfcIndexedPolyCurve).Points as IfcCartesianPointList2D)
                    .CoordList[6][0] * 2,

                Depth = ((((((beamSt.Representation.Representations.FirstOrDefault() as
                IIfcShapeRepresentation).Items.FirstOrDefault() as IIfcExtrudedAreaSolid)
                .SweptArea as IfcArbitraryClosedProfileDef).OuterCurve) as
                IfcIndexedPolyCurve).Points as IfcCartesianPointList2D)
                .CoordList[7][1] * 2,


                FlangeTh = (((((beamSt.Representation.Representations.FirstOrDefault() as
                IIfcShapeRepresentation).Items.FirstOrDefault() as IIfcExtrudedAreaSolid)
                .SweptArea as IfcArbitraryClosedProfileDef).OuterCurve as
                IfcIndexedPolyCurve).Points as IfcCartesianPointList2D)
                .CoordList[3][0] * 2,

                WebTh = ((((((beamSt.Representation.Representations.FirstOrDefault() as
                IIfcShapeRepresentation).Items.FirstOrDefault() as IIfcExtrudedAreaSolid)
                .SweptArea as IfcArbitraryClosedProfileDef).OuterCurve) as
                IfcIndexedPolyCurve).Points as IfcCartesianPointList2D)
                .CoordList[7][1] - ((((((beamSt.Representation.Representations.FirstOrDefault() as
                IIfcShapeRepresentation).Items.FirstOrDefault() as IIfcExtrudedAreaSolid)
                .SweptArea as IfcArbitraryClosedProfileDef).OuterCurve) as
                IfcIndexedPolyCurve).Points as IfcCartesianPointList2D)
                .CoordList[8][1]

            };

            return B;
        }

        public static List<BeamStISEC> GetBeamsSteelData(List<IIfcBeam> beamSt)
        {
            List<BeamStISEC> myBeam = new List<BeamStISEC>();
            foreach (IIfcBeam x in beamSt)
            {
                myBeam.Add(GetBeamsSteelData(x));
            }
            return myBeam;
        }


      
        #region Steel Beams L sec

        public static BeamSTLSEC GetBeamsSteelISECData(IIfcBeam beamSt)
        {

            Point location = new Point();

            location.X = ((((beamSt.ObjectPlacement as IfcLocalPlacement).PlacementRelTo as IfcLocalPlacement).RelativePlacement
                    as IfcAxis2Placement3D).Location as IfcCartesianPoint).Coordinates[0];

            location.Y = ((((beamSt.ObjectPlacement as IfcLocalPlacement).PlacementRelTo as IfcLocalPlacement).RelativePlacement
                     as IfcAxis2Placement3D).Location as IfcCartesianPoint).Coordinates[1];



            location.Z = ((((beamSt.ObjectPlacement as IfcLocalPlacement).PlacementRelTo as IfcLocalPlacement).RelativePlacement
                     as IfcAxis2Placement3D).Location as IfcCartesianPoint).Coordinates[2];


            Point refDirection = new Point();

            refDirection.X = ((((beamSt.ObjectPlacement as IfcLocalPlacement).PlacementRelTo as
                    IfcLocalPlacement).RelativePlacement as IfcAxis2Placement3D).RefDirection as IfcDirection).X;

            refDirection.Y = ((((beamSt.ObjectPlacement as IfcLocalPlacement).PlacementRelTo as
                    IfcLocalPlacement).RelativePlacement as IfcAxis2Placement3D).RefDirection as IfcDirection).Y;

            refDirection.Z = ((((beamSt.ObjectPlacement as IfcLocalPlacement).PlacementRelTo as
                    IfcLocalPlacement).RelativePlacement as IfcAxis2Placement3D).RefDirection as IfcDirection).Z;

            Point axis = new Point();

            axis.X = ((((beamSt.ObjectPlacement as IfcLocalPlacement).PlacementRelTo as
                    IfcLocalPlacement).RelativePlacement as IfcAxis2Placement3D).Axis as IfcDirection).X;

            axis.Y = ((((beamSt.ObjectPlacement as IfcLocalPlacement).PlacementRelTo as
                    IfcLocalPlacement).RelativePlacement as IfcAxis2Placement3D).Axis as IfcDirection).Y;

            axis.Z = ((((beamSt.ObjectPlacement as IfcLocalPlacement).PlacementRelTo as
                    IfcLocalPlacement).RelativePlacement as IfcAxis2Placement3D).Axis as IfcDirection).Z;

            var Width = ((((((beamSt.Representation.Representations.FirstOrDefault() as
                            IIfcShapeRepresentation).Items.FirstOrDefault() as IIfcExtrudedAreaSolid)
                            .SweptArea as IfcArbitraryClosedProfileDef).OuterCurve) as
                            IfcIndexedPolyCurve).Points as IfcCartesianPointList2D)
                            .CoordList[4][1];

            var Depth = ((((((beamSt.Representation.Representations.FirstOrDefault() as
                            IIfcShapeRepresentation).Items.FirstOrDefault() as IIfcExtrudedAreaSolid)
                            .SweptArea as IfcArbitraryClosedProfileDef).OuterCurve) as
                            IfcIndexedPolyCurve).Points as IfcCartesianPointList2D)
                            .CoordList[4][1] * 2;

            var FlangeTh = (((((beamSt.Representation.Representations.FirstOrDefault() as
                        IIfcShapeRepresentation).Items.FirstOrDefault() as IIfcExtrudedAreaSolid)
                .SweptArea as IfcArbitraryClosedProfileDef).OuterCurve as
                IfcIndexedPolyCurve).Points as IfcCartesianPointList2D)
                .CoordList[4][1]
                -
                (((((beamSt.Representation.Representations.FirstOrDefault() as
                        IIfcShapeRepresentation).Items.FirstOrDefault() as IIfcExtrudedAreaSolid)
                .SweptArea as IfcArbitraryClosedProfileDef).OuterCurve as
                IfcIndexedPolyCurve).Points as IfcCartesianPointList2D)
                .CoordList[3][1];



            var WebTh = ((((((beamSt.Representation.Representations.FirstOrDefault() as
                        IIfcShapeRepresentation).Items.FirstOrDefault() as IIfcExtrudedAreaSolid)
                        .SweptArea as IfcArbitraryClosedProfileDef).OuterCurve) as
                        IfcIndexedPolyCurve).Points as IfcCartesianPointList2D)
                        .CoordList[1][0] * 2;


            BeamSTLSEC B = new BeamSTLSEC()
            {
                Name = beamSt.Name,
                RefDirection = refDirection,
                Axis = axis,
                Location = location,
                Length = ((beamSt.Representation.Representations.FirstOrDefault() as IIfcShapeRepresentation)
                    .Items.FirstOrDefault() as IIfcExtrudedAreaSolid).Depth,



                Width = ((((((beamSt.Representation.Representations.FirstOrDefault() as
                            IIfcShapeRepresentation).Items.FirstOrDefault() as IIfcExtrudedAreaSolid)
                            .SweptArea as IfcArbitraryClosedProfileDef).OuterCurve) as
                            IfcIndexedPolyCurve).Points as IfcCartesianPointList2D)
                            .CoordList[4][1],//

                Depth = ((((((beamSt.Representation.Representations.FirstOrDefault() as
                            IIfcShapeRepresentation).Items.FirstOrDefault() as IIfcExtrudedAreaSolid)
                            .SweptArea as IfcArbitraryClosedProfileDef).OuterCurve) as
                            IfcIndexedPolyCurve).Points as IfcCartesianPointList2D)
                            .CoordList[4][1] * 2,//



                FlangeTh = (((((beamSt.Representation.Representations.FirstOrDefault() as
                        IIfcShapeRepresentation).Items.FirstOrDefault() as IIfcExtrudedAreaSolid)
                .SweptArea as IfcArbitraryClosedProfileDef).OuterCurve as
                IfcIndexedPolyCurve).Points as IfcCartesianPointList2D)
                .CoordList[4][1]
                -
                (((((beamSt.Representation.Representations.FirstOrDefault() as
                        IIfcShapeRepresentation).Items.FirstOrDefault() as IIfcExtrudedAreaSolid)
                .SweptArea as IfcArbitraryClosedProfileDef).OuterCurve as
                IfcIndexedPolyCurve).Points as IfcCartesianPointList2D)
                .CoordList[3][1],

                WebTh = ((((((beamSt.Representation.Representations.FirstOrDefault() as
                        IIfcShapeRepresentation).Items.FirstOrDefault() as IIfcExtrudedAreaSolid)
                        .SweptArea as IfcArbitraryClosedProfileDef).OuterCurve) as
                        IfcIndexedPolyCurve).Points as IfcCartesianPointList2D)
                        .CoordList[1][0] * 2

            };

            return B;
        }

        public static List<BeamStISEC> GetBeamsSteelISECData(List<IIfcBeam> beamSt)
        {
            List<BeamStISEC> myBeam = new List<BeamStISEC>();
            foreach (IIfcBeam x in beamSt)
            {
                myBeam.Add(GetBeamsSteelData(x));
            }
            return myBeam;
        }



        #endregion
        #region Circle Setion
        public static Circular GetBeamsSteelCircularSECData(IIfcBeam beamSt)
        {

            Point location = new Point();

            location.X = ((((beamSt.ObjectPlacement as IfcLocalPlacement).PlacementRelTo as IfcLocalPlacement).RelativePlacement
                    as IfcAxis2Placement3D).Location as IfcCartesianPoint).Coordinates[0];

            location.Y = ((((beamSt.ObjectPlacement as IfcLocalPlacement).PlacementRelTo as IfcLocalPlacement).RelativePlacement
                     as IfcAxis2Placement3D).Location as IfcCartesianPoint).Coordinates[1];



            location.Z = ((((beamSt.ObjectPlacement as IfcLocalPlacement).PlacementRelTo as IfcLocalPlacement).RelativePlacement
                     as IfcAxis2Placement3D).Location as IfcCartesianPoint).Coordinates[2];


            Point refDirection = new Point();

            refDirection.X = ((((beamSt.ObjectPlacement as IfcLocalPlacement).PlacementRelTo as
                    IfcLocalPlacement).RelativePlacement as IfcAxis2Placement3D).RefDirection as IfcDirection).X;

            refDirection.Y = ((((beamSt.ObjectPlacement as IfcLocalPlacement).PlacementRelTo as
                    IfcLocalPlacement).RelativePlacement as IfcAxis2Placement3D).RefDirection as IfcDirection).Y;

            refDirection.Z = ((((beamSt.ObjectPlacement as IfcLocalPlacement).PlacementRelTo as
                    IfcLocalPlacement).RelativePlacement as IfcAxis2Placement3D).RefDirection as IfcDirection).Z;

            Point axis = new Point();

            axis.X = ((((beamSt.ObjectPlacement as IfcLocalPlacement).PlacementRelTo as
                    IfcLocalPlacement).RelativePlacement as IfcAxis2Placement3D).Axis as IfcDirection).X;

            axis.Y = ((((beamSt.ObjectPlacement as IfcLocalPlacement).PlacementRelTo as
                    IfcLocalPlacement).RelativePlacement as IfcAxis2Placement3D).Axis as IfcDirection).Y;

            axis.Z = ((((beamSt.ObjectPlacement as IfcLocalPlacement).PlacementRelTo as
                    IfcLocalPlacement).RelativePlacement as IfcAxis2Placement3D).Axis as IfcDirection).Z;

            var Width = ((((((beamSt.Representation.Representations.FirstOrDefault() as
                            IIfcShapeRepresentation).Items.FirstOrDefault() as IIfcExtrudedAreaSolid)
                            .SweptArea as IfcArbitraryClosedProfileDef).OuterCurve) as
                            IfcIndexedPolyCurve).Points as IfcCartesianPointList2D)
                            .CoordList[4][1];

            var Depth = ((((((beamSt.Representation.Representations.FirstOrDefault() as
                            IIfcShapeRepresentation).Items.FirstOrDefault() as IIfcExtrudedAreaSolid)
                            .SweptArea as IfcArbitraryClosedProfileDef).OuterCurve) as
                            IfcIndexedPolyCurve).Points as IfcCartesianPointList2D)
                            .CoordList[8][0] * 2;




            Circular B = new Circular()
            {
                Name = beamSt.Name,
                RefDirection = refDirection,
                Axis = axis,
                Location = location,
                Length = ((beamSt.Representation.Representations.FirstOrDefault() as IIfcShapeRepresentation)
                    .Items.FirstOrDefault() as IIfcExtrudedAreaSolid).Depth,



                Width = Math.Abs(((((((beamSt.Representation.Representations.FirstOrDefault() as
                            IIfcShapeRepresentation).Items.FirstOrDefault() as IIfcExtrudedAreaSolid)
                            .SweptArea as IfcArbitraryClosedProfileDef).OuterCurve) as
                            IfcIndexedPolyCurve).Points as IfcCartesianPointList2D)
                            .CoordList[8][0])//



            };

            return B;
        }
        #endregion
        #region C Section
        public static BeamCsec GetBeamsSteelCsecData(IIfcBeam beamSt)
        {

            Point location = new Point();

            location.X = ((((beamSt.ObjectPlacement as IfcLocalPlacement).PlacementRelTo as IfcLocalPlacement).RelativePlacement
                    as IfcAxis2Placement3D).Location as IfcCartesianPoint).Coordinates[0];

            location.Y = ((((beamSt.ObjectPlacement as IfcLocalPlacement).PlacementRelTo as IfcLocalPlacement).RelativePlacement
                     as IfcAxis2Placement3D).Location as IfcCartesianPoint).Coordinates[1];



            location.Z = ((((beamSt.ObjectPlacement as IfcLocalPlacement).PlacementRelTo as IfcLocalPlacement).RelativePlacement
                     as IfcAxis2Placement3D).Location as IfcCartesianPoint).Coordinates[2];


            Point refDirection = new Point();

            refDirection.X = ((((beamSt.ObjectPlacement as IfcLocalPlacement).PlacementRelTo as
                    IfcLocalPlacement).RelativePlacement as IfcAxis2Placement3D).RefDirection as IfcDirection).X;

            refDirection.Y = ((((beamSt.ObjectPlacement as IfcLocalPlacement).PlacementRelTo as
                    IfcLocalPlacement).RelativePlacement as IfcAxis2Placement3D).RefDirection as IfcDirection).Y;

            refDirection.Z = ((((beamSt.ObjectPlacement as IfcLocalPlacement).PlacementRelTo as
                    IfcLocalPlacement).RelativePlacement as IfcAxis2Placement3D).RefDirection as IfcDirection).Z;

            Point axis = new Point();

            axis.X = ((((beamSt.ObjectPlacement as IfcLocalPlacement).PlacementRelTo as
                    IfcLocalPlacement).RelativePlacement as IfcAxis2Placement3D).Axis as IfcDirection).X;

            axis.Y = ((((beamSt.ObjectPlacement as IfcLocalPlacement).PlacementRelTo as
                    IfcLocalPlacement).RelativePlacement as IfcAxis2Placement3D).Axis as IfcDirection).Y;

            axis.Z = ((((beamSt.ObjectPlacement as IfcLocalPlacement).PlacementRelTo as
                    IfcLocalPlacement).RelativePlacement as IfcAxis2Placement3D).Axis as IfcDirection).Z;


            BeamCsec B = new BeamCsec()
            {
                Name = beamSt.Name,
                RefDirection = refDirection,
                Axis = axis,
                Location = location,
                Length = ((beamSt.Representation.Representations.FirstOrDefault() as IIfcShapeRepresentation)
                            .Items.FirstOrDefault() as IIfcExtrudedAreaSolid).Depth,



                Width = ((((((beamSt.Representation.Representations.FirstOrDefault() as
                        IIfcShapeRepresentation).Items.FirstOrDefault() as IIfcExtrudedAreaSolid)
                        .SweptArea as IfcArbitraryClosedProfileDef).OuterCurve) as
                        IfcIndexedPolyCurve).Points as IfcCartesianPointList2D)
                        .CoordList[2][0] * 2,

                Depth = ((((((beamSt.Representation.Representations.FirstOrDefault() as
                        IIfcShapeRepresentation).Items.FirstOrDefault() as IIfcExtrudedAreaSolid)
                        .SweptArea as IfcArbitraryClosedProfileDef).OuterCurve) as
                        IfcIndexedPolyCurve).Points as IfcCartesianPointList2D)
                        .CoordList[6][1] * 2,


                FlangeTh = ((((((beamSt.Representation.Representations.FirstOrDefault() as
                        IIfcShapeRepresentation).Items.FirstOrDefault() as IIfcExtrudedAreaSolid)
                        .SweptArea as IfcArbitraryClosedProfileDef).OuterCurve) as
                        IfcIndexedPolyCurve).Points as IfcCartesianPointList2D)
                        .CoordList[6][1] - ((((((beamSt.Representation.Representations.FirstOrDefault() as
                        IIfcShapeRepresentation).Items.FirstOrDefault() as IIfcExtrudedAreaSolid)
                        .SweptArea as IfcArbitraryClosedProfileDef).OuterCurve) as
                        IfcIndexedPolyCurve).Points as IfcCartesianPointList2D)
                        .CoordList[5][1],

                WebTh = ((((((beamSt.Representation.Representations.FirstOrDefault() as
                        IIfcShapeRepresentation).Items.FirstOrDefault() as IIfcExtrudedAreaSolid)
                        .SweptArea as IfcArbitraryClosedProfileDef).OuterCurve) as
                        IfcIndexedPolyCurve).Points as IfcCartesianPointList2D)
                        .CoordList[6][1] - ((((((beamSt.Representation.Representations.FirstOrDefault() as
                        IIfcShapeRepresentation).Items.FirstOrDefault() as IIfcExtrudedAreaSolid)
                        .SweptArea as IfcArbitraryClosedProfileDef).OuterCurve) as
                        IfcIndexedPolyCurve).Points as IfcCartesianPointList2D)
                        .CoordList[5][1]

            };

            return B;
        }
        #endregion
    }
}
