using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.DB;
using SteelColumn.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SteelColumn.GetData;
using Xbim.Ifc4.GeometricConstraintResource;
using Xbim.Ifc4.GeometricModelResource;
using Xbim.Ifc4.GeometryResource;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.ProfileResource;

namespace SteelColumn.CreateElements
{
    
    
    public class CreateBeam
    {
        public static int RevitVersion;
        public static FamilyInstance CreateBeamInstance(Autodesk.Revit.DB.Document doc, Beam beam, FamilySymbol fs)
        {
            StructuralType stBeam = StructuralType.Beam;
            XYZ p1 = new XYZ(CreateStlColumn.MmToFoot(beam.Location.X), CreateStlColumn.MmToFoot(beam.Location.Y), CreateStlColumn.MmToFoot(beam.Location.Z) + CreateStlColumn.MmToFoot(beam.H / 2));
            XYZ p2 = p1 + beam.Length * new XYZ(CreateStlColumn.MmToFoot(beam.RefDirection.X), CreateStlColumn.MmToFoot(beam.RefDirection.Y), CreateStlColumn.MmToFoot(beam.RefDirection.Z));
            Line l1 = Line.CreateBound(p1, p2);

            if (!fs.IsActive)
            {
                fs.Activate();
                doc.Regenerate();
            }

            FamilyInstance fi = doc.Create.NewFamilyInstance(l1, fs, null, stBeam);

            fi.LookupParameter("Reference Level").Set(CreateStlColumn.GetLevel(doc, beam.Location.Z).Id);
            return fi;
        }

        public static FamilyInstance CreateSTBeamLSECInstance(Autodesk.Revit.DB.Document doc, BeamSTLSEC beam, FamilySymbol fs)
        {
            StructuralType stBeam = StructuralType.Beam;
            XYZ p1 = new XYZ(CreateStlColumn.MmToFoot(beam.Location.X), CreateStlColumn.MmToFoot(beam.Location.Y), CreateStlColumn.MmToFoot(beam.Location.Z + beam.Depth / 2));
            XYZ p2 = p1 + beam.Length * new XYZ(CreateStlColumn.MmToFoot(beam.RefDirection.X), CreateStlColumn.MmToFoot(beam.RefDirection.Y), CreateStlColumn.MmToFoot(beam.RefDirection.Z));
            Line l1 = Line.CreateBound(p1, p2);

            if (!fs.IsActive)
            {
                fs.Activate();
                doc.Regenerate();
            }
            FamilyInstance fi = doc.Create.NewFamilyInstance(l1, fs, null, stBeam);

            fi.LookupParameter("Reference Level").Set(CreateStlColumn.GetLevel(doc, CreateStlColumn.MmToFoot(beam.Location.Z)).Id);
            return fi;
        }
        public static FamilyInstance CreateSteelBeamInstance(Document doc, BeamStISEC beam, FamilySymbol fs)
        {
            StructuralType stBeam = StructuralType.Beam;
            XYZ p1 = new XYZ(CreateStlColumn.MmToFoot(beam.Location.X), CreateStlColumn.MmToFoot(beam.Location.Y), CreateStlColumn.MmToFoot(beam.Location.Z) + CreateStlColumn.MmToFoot(beam.Depth / 2));
            XYZ p2 = p1 + beam.Length * new XYZ(CreateStlColumn.MmToFoot(beam.RefDirection.X), CreateStlColumn.MmToFoot(beam.RefDirection.Y), CreateStlColumn.MmToFoot(beam.RefDirection.Z));
            Line l1 = Line.CreateBound(p1, p2);
            if (!fs.IsActive)
            {
                fs.Activate();
                doc.Regenerate();
            }
            FamilyInstance fi = doc.Create.NewFamilyInstance(l1, fs, null, stBeam);
            fi.LookupParameter("Reference Level").Set(CreateStlColumn.GetLevel(doc, CreateStlColumn.MmToFoot(beam.Location.Z)).Id);

            return fi;
        }

        public static FamilySymbol GetBeamFamilySymbolForCon(Autodesk.Revit.DB.Document doc, Beam beam, string familyName, BuiltInCategory category)
        {
            FamilySymbol fs = new FilteredElementCollector(doc).OfCategory(category)
                                                        .WhereElementIsElementType()
                                                        .Cast<FamilySymbol>()
                                                        .Where(f => f.FamilyName == familyName)
                                                        .FirstOrDefault(f =>
                                                        {
                                                            if (Math.Abs(f.LookupParameter("h").AsDouble() - CreateStlColumn.MmToFoot(beam.H)) < CreateStlColumn.MmToFoot(0.001) &&
                                                                Math.Abs(f.LookupParameter("b").AsDouble() - CreateStlColumn.MmToFoot(beam.B)) < CreateStlColumn.MmToFoot(0.001))


                                                                return true;

                                                            return false;



                                                        });

            if (fs != null) return fs;

            Family family = default;

            try
            {
                fs = new FilteredElementCollector(doc).OfCategory(category)
                                                        .WhereElementIsElementType()
                                                        .Cast<FamilySymbol>()
                                                        .First(f => f.FamilyName == familyName);

            }
            catch (Exception)
            {
                string directory = $"C:/ProgramData/Autodesk/RVT {RevitVersion}/Libraries/US Imperial/Structural Framing/Concrete/";
                family = CreateStlColumn.OpenFamily(doc, directory, familyName);
                fs = doc.GetElement(family.GetFamilySymbolIds().FirstOrDefault()) as FamilySymbol;

            }

            try
            {

                var columnsTypes = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_StructuralFraming).
                                       WhereElementIsElementType().Cast<FamilySymbol>().ToList();


                if (!columnsTypes.Contains(columnsTypes.FirstOrDefault(c => Math.Abs(c.LookupParameter("h").AsDouble() - CreateStlColumn.MmToFoot(beam.H)) < CreateStlColumn.MmToFoot(0.001) &&
                Math.Abs(c.LookupParameter("b").AsDouble() - CreateStlColumn.MmToFoot(beam.B)) < CreateStlColumn.MmToFoot(0.0001))))
                {



                    ElementType fs1 = fs.Duplicate($"{beam.H}" + "x" + $"{beam.B} " + "X" + $"{beam.Location.X}");
                    fs1.LookupParameter("h").Set(CreateStlColumn.MmToFoot(beam.H));
                    fs1.LookupParameter("b").Set(CreateStlColumn.MmToFoot(beam.B));

                    fs = fs1 as FamilySymbol;

                }
            }
            catch (Exception)
            {


                fs = new FilteredElementCollector(doc).OfCategory(category)
                                                        .WhereElementIsElementType()
                                                        .Cast<FamilySymbol>()
                                                        .First(f => f.FamilyName == familyName);
            }


            return fs;


        }

        public static FamilySymbol GetBeamFamilySymbolForISEC(Autodesk.Revit.DB.Document doc, BeamStISEC beam, string familyName, BuiltInCategory category)
        {
            FamilySymbol fs = new FilteredElementCollector(doc).OfCategory(category)
                                                        .WhereElementIsElementType()
                                                        .Cast<FamilySymbol>()
                                                        .Where(f => f.FamilyName == familyName)
                                                        .FirstOrDefault(f =>
                                                        {
                                                            if (Math.Abs(f.LookupParameter("Web Thickness").AsDouble() * 10 - CreateStlColumn.MmToFoot(beam.WebTh)) < CreateStlColumn.MmToFoot(0.001) &&
                                                                Math.Abs(f.LookupParameter("Flange Thickness").AsDouble() * 10 - CreateStlColumn.MmToFoot(beam.FlangeTh)) < CreateStlColumn.MmToFoot(0.001) &&
                                                                Math.Abs(f.LookupParameter("Width").AsDouble() * 10 - CreateStlColumn.MmToFoot(beam.Width)) < CreateStlColumn.MmToFoot(0.001) &&
                                                                Math.Abs(f.LookupParameter("Height").AsDouble() * 10 - CreateStlColumn.MmToFoot(beam.Depth)) < CreateStlColumn.MmToFoot(0.001))
                                                                return true;

                                                            return false;



                                                        });

            if (fs != null) return fs;
            Family family = default;




            try
            {
                fs = new FilteredElementCollector(doc).OfCategory(category)
                                                        .WhereElementIsElementType()
                                                        .Cast<FamilySymbol>()
                                                        .First(f => f.FamilyName == familyName);

            }
            catch (Exception)
            {
                string directory = $"C:/ProgramData/Autodesk/RVT {RevitVersion}/Libraries/US Imperial/Structural Framing/Concrete/";
                family = CreateStlColumn.OpenFamily(doc, directory, familyName);
                fs = doc.GetElement(family.GetFamilySymbolIds().FirstOrDefault()) as FamilySymbol;

            }


            var columnsTypes = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_StructuralFraming).
                                   WhereElementIsElementType().Cast<FamilySymbol>().ToList();

            try
            {

                if (!columnsTypes.Contains(columnsTypes.FirstOrDefault(beams => beam.Name == $"{beam.Depth}" + $"{beam.FlangeTh}" +
                                                                                             $"{beam.Width}" + $"{beam.WebTh}" + $"{beam.Location.X}")))
                {


                    ElementType fs1 = fs.Duplicate($"{beam.Depth}" + "X" + $"{beam.FlangeTh}" + "X" + $"{beam.Width}" + "X" + $"{beam.WebTh}" + "X" + $"{beam.Location.X}");
                    fs1.LookupParameter("Web Thickness").Set(CreateStlColumn.MmToFoot(beam.WebTh));
                    fs1.LookupParameter("Flange Thickness").Set(CreateStlColumn.MmToFoot(beam.FlangeTh));
                    fs1.LookupParameter("Width").Set(CreateStlColumn.MmToFoot(beam.Width));
                    var sdad = fs1.LookupParameter("Height").Set(CreateStlColumn.MmToFoot(beam.Depth));
                    return fs1 as FamilySymbol;
                }
            }
            catch (Exception)
            {

                fs = new FilteredElementCollector(doc).OfCategory(category)
                                                        .WhereElementIsElementType()
                                                        .Cast<FamilySymbol>()
                                                        .First(f => f.FamilyName == familyName);
            }






            return fs;


        }



        public static FamilySymbol GetBeamFamilySymbolForLSEC(Autodesk.Revit.DB.Document doc, BeamSTLSEC beam, string familyName, BuiltInCategory category)
        {
            FamilySymbol fs = new FilteredElementCollector(doc).OfCategory(category)
                                                        .WhereElementIsElementType()
                                                        .Cast<FamilySymbol>()
                                                        .Where(f => f.FamilyName == familyName)
                                                        .FirstOrDefault(f =>
                                                        {
                                                            if (Math.Abs(f.LookupParameter("Web Thickness").AsDouble() - CreateStlColumn.MmToFoot(beam.WebTh)) < CreateStlColumn.MmToFoot(0.001) &&
                                                                Math.Abs(f.LookupParameter("Flange Thickness").AsDouble() - CreateStlColumn.MmToFoot(beam.FlangeTh)) < CreateStlColumn.MmToFoot(0.001) &&
                                                                Math.Abs(f.LookupParameter("Width").AsDouble() - CreateStlColumn.MmToFoot(beam.Width)) < CreateStlColumn.MmToFoot(0.001) &&
                                                                Math.Abs(f.LookupParameter("Height").AsDouble() - CreateStlColumn.MmToFoot(beam.Depth)) < CreateStlColumn.MmToFoot(0.001))


                                                                return true;

                                                            return false;

                                                        });

            if (fs != null) return fs;
            Family family = default;

            try
            {
                fs = new FilteredElementCollector(doc).OfCategory(category)
                                                        .WhereElementIsElementType()
                                                        .Cast<FamilySymbol>()
                                                        .First(f => f.FamilyName == familyName);

            }
            catch (Exception)
            {
                //string directory = $"C:/ProgramData/Autodesk/RVT {RevitVersion}/Libraries/US Imperial/Structural Framing/Concrete/";
                //family = OpenFamily(doc, directory, familyName);
                //fs = doc.GetElement(family.GetFamilySymbolIds().FirstOrDefault()) as FamilySymbol;


            }

            try
            {

                var columnsTypes = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_StructuralFraming).
                                                   WhereElementIsElementType().Cast<FamilySymbol>().ToList();


                if (!columnsTypes.Contains(columnsTypes.FirstOrDefault(c => c.Name == $"{beam.Depth}" + "X" + $"{beam.FlangeTh}" + "X" + $"{beam.Width}"
                                                                                        + "X" + $"{beam.WebTh}" + "X" + $"{beam.Location.X}" + $"{beam.Location.X}" + $"{beam.Location.Y}")))
                {
                    ElementType fs1 = fs.Duplicate($"{beam.Depth}" + $"{beam.FlangeTh}" + $"{beam.Width}" + $"{beam.WebTh}" + "X" + $"{beam.Location.X}");
                    fs1.LookupParameter("Web Thickness").Set(CreateStlColumn.MmToFoot(beam.WebTh));
                    fs1.LookupParameter("Flange Thickness").Set(CreateStlColumn.MmToFoot(beam.FlangeTh));
                    fs1.LookupParameter("Width").Set(CreateStlColumn.MmToFoot(beam.Width));
                    fs1.LookupParameter("Height").Set(CreateStlColumn.MmToFoot(beam.Depth));
                    return fs1 as FamilySymbol;
                }
            }
            catch (Exception)
            {

                fs = new FilteredElementCollector(doc).OfCategory(category)
                                        .WhereElementIsElementType()
                                        .Cast<FamilySymbol>()
                                        .First(f => f.FamilyName == familyName);
            }

            return fs;
        }
        public static FamilySymbol GetBeamFamilySymbolForCSEC(Autodesk.Revit.DB.Document doc, BeamCsec beam, string familyName, BuiltInCategory category)
        {

            FamilySymbol fs = new FilteredElementCollector(doc).OfCategory(category)
                                                        .WhereElementIsElementType()
                                                        .Cast<FamilySymbol>()
                                                        .Where(f => f.FamilyName == familyName)
                                                        .FirstOrDefault(f =>
                                                        {
                                                            var SADAS = f.LookupParameter("tw").AsDouble() * 10;
                                                            var ASDA = CreateStlColumn.MmToFoot(beam.WebTh);
                                                            if (Math.Abs(f.LookupParameter("tw").AsDouble() * 10 - CreateStlColumn.MmToFoot(beam.WebTh)) < CreateStlColumn.MmToFoot(0.001) &&
                                                                Math.Abs(f.LookupParameter("tf").AsDouble() * 10 - CreateStlColumn.MmToFoot(beam.FlangeTh)) < CreateStlColumn.MmToFoot(0.001) &&
                                                                Math.Abs(f.LookupParameter("b").AsDouble() * 10 - CreateStlColumn.MmToFoot(beam.Width)) < CreateStlColumn.MmToFoot(0.001) &&
                                                                Math.Abs(f.LookupParameter("h").AsDouble() * 10 - CreateStlColumn.MmToFoot(beam.Depth)) < CreateStlColumn.MmToFoot(0.001))
                                                                return true;

                                                            return false;



                                                        });

            if (fs != null) return fs;
            Family family = default;




            try
            {
                fs = new FilteredElementCollector(doc).OfCategory(category)
                                                        .WhereElementIsElementType()
                                                        .Cast<FamilySymbol>()
                                                        .First(f => f.FamilyName == familyName);

            }
            catch (Exception)
            {
                //string directory = $"C:/ProgramData/Autodesk/RVT {RevitVersion}/Libraries/US Imperial/Structural Framing/Concrete/";
                //family = OpenFamily(doc, directory, familyName);
                //fs = doc.GetElement(family.GetFamilySymbolIds().FirstOrDefault()) as FamilySymbol;

            }
            var columnsTypes = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_StructuralFraming).
                                   WhereElementIsElementType().Cast<FamilySymbol>().ToList();

            try
            {

                if (!columnsTypes.Contains(columnsTypes.FirstOrDefault(beams => beam.Name == $"{beam.Depth}" + $"{beam.FlangeTh}" +
                                                                                             $"{beam.Width}" + $"{beam.WebTh}")))
                {
                    ElementType fs1 = fs.Duplicate($"{beam.Depth}" + "X" + $"{beam.FlangeTh}" + "X" + $"{beam.Width}" + "X" + $"{beam.WebTh}");
                    fs1.LookupParameter("tw").Set(CreateStlColumn.MmToFoot(beam.WebTh));
                    fs1.LookupParameter("tf").Set(CreateStlColumn.MmToFoot(beam.FlangeTh));
                    fs1.LookupParameter("b").Set(CreateStlColumn.MmToFoot(beam.Width));
                    var sdad = fs1.LookupParameter("h").Set(CreateStlColumn.MmToFoot(beam.Depth));
                    return fs1 as FamilySymbol;
                }
            }
            catch (Exception)
            {

                fs = new FilteredElementCollector(doc).OfCategory(category)
                                                        .WhereElementIsElementType()
                                                        .Cast<FamilySymbol>()
                                                        .First(f => f.Name == $"{beam.Depth}" + "X" + $"{beam.FlangeTh}" + "X" + $"{beam.Width}" + "X" + $"{beam.WebTh}");
            }
            return fs;
        }

        public static FamilyInstance CreateSteelBeamInstanceCsec(Document doc, BeamCsec beam, FamilySymbol fs)
        {
            StructuralType stBeam = StructuralType.Beam;
            XYZ p1 = new XYZ(CreateStlColumn.MmToFoot(beam.Location.X), CreateStlColumn.MmToFoot(beam.Location.Y), CreateStlColumn.MmToFoot(beam.Location.Z) + CreateStlColumn.MmToFoot(beam.Depth / 2));
            XYZ p2 = p1 + beam.Length * new XYZ(CreateStlColumn.MmToFoot(beam.RefDirection.X), CreateStlColumn.MmToFoot(beam.RefDirection.Y), CreateStlColumn.MmToFoot(beam.RefDirection.Z));
            Line l1 = Line.CreateBound(p1, p2);
            if (!fs.IsActive)
            {
                fs.Activate();
                doc.Regenerate();
            }
            FamilyInstance fi = doc.Create.NewFamilyInstance(l1, fs, null, stBeam);
            fi.LookupParameter("Reference Level").Set(CreateStlColumn.GetLevel(doc, CreateStlColumn.MmToFoot(beam.Location.Z)).Id);
            return fi;
        }

        public static FamilyInstance CreateSteelBeamCircularnstance(Document doc, Circular beam, FamilySymbol fs)
        {
            StructuralType stBeam = StructuralType.Beam;
            XYZ p1 = new XYZ(CreateStlColumn.MmToFoot(beam.Location.X), CreateStlColumn.MmToFoot(beam.Location.Y), CreateStlColumn.MmToFoot(beam.Location.Z) + CreateStlColumn.MmToFoot(beam.Width / 2));
            XYZ p2 = p1 + beam.Length * new XYZ(CreateStlColumn.MmToFoot(beam.RefDirection.X), CreateStlColumn.MmToFoot(beam.RefDirection.Y), CreateStlColumn.MmToFoot(beam.RefDirection.Z));
            Line l1 = Line.CreateBound(p1, p2);
            if (!fs.IsActive)
            {
                fs.Activate();
                doc.Regenerate();
            }
            FamilyInstance fi = doc.Create.NewFamilyInstance(l1, fs, null, stBeam);
            fi.LookupParameter("Reference Level").Set(CreateStlColumn.GetLevel(doc, CreateStlColumn.MmToFoot(beam.Location.Z)).Id);
            return fi;
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
                            .CoordList[1][0] * 2,//

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
                +
                (((((beamSt.Representation.Representations.FirstOrDefault() as
                        IIfcShapeRepresentation).Items.FirstOrDefault() as IIfcExtrudedAreaSolid)
                .SweptArea as IfcArbitraryClosedProfileDef).OuterCurve as
                IfcIndexedPolyCurve).Points as IfcCartesianPointList2D)
                .CoordList[3][1],
            };

            return B;
        }

        public static List<BeamStISEC> GetBeamsSteelISECData(List<IIfcBeam> beamSt)
        {
            List<BeamStISEC> myBeam = new List<BeamStISEC>();
            foreach (IIfcBeam x in beamSt)
            {
                myBeam.Add(Getbeam.GetBeamsSteelData(x));
            }
            return myBeam;
        }


        #endregion

        public static FamilySymbol GetBeamFamilySymbolForCircular(Autodesk.Revit.DB.Document doc, Circular beam, string familyName, BuiltInCategory category)
        {
            FamilySymbol fs = new FilteredElementCollector(doc).OfCategory(category)
                                                        .WhereElementIsElementType()
                                                        .Cast<FamilySymbol>()
                                                        .Where(f => f.FamilyName == familyName)
                                                        .FirstOrDefault(f =>
                                                        {
                                                            if (
                                                                Math.Abs(f.LookupParameter("d").AsDouble() * 10 - CreateStlColumn.MmToFoot(beam.Width)) < CreateStlColumn.MmToFoot(0.001))
                                                                return true;

                                                            return false;



                                                        });

            if (fs != null) return fs;
            Family family = default;


            try
            {
                fs = new FilteredElementCollector(doc).OfCategory(category)
                                                        .WhereElementIsElementType()
                                                        .Cast<FamilySymbol>()
                                                        .First(f => f.FamilyName == familyName);

            }
            catch (Exception)
            {
                //string directory = $"C:/ProgramData/Autodesk/RVT {RevitVersion}/Libraries/US Imperial/Structural Framing/Concrete/";
                //family = OpenFamily(doc, directory, familyName);
                //fs = doc.GetElement(family.GetFamilySymbolIds().FirstOrDefault()) as FamilySymbol;

            }


            var columnsTypes = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_StructuralFraming).
                                   WhereElementIsElementType().Cast<FamilySymbol>().ToList();

            try
            {

                if (!columnsTypes.Contains(columnsTypes.FirstOrDefault(beams => beam.Name == $"{beam.Width}" + $"{beam.Location.X}")))

                {
                    ElementType fs1 = fs.Duplicate($"{beam.Width * 10}" + "X" + $"{beam.Location.X}");

                    fs1.LookupParameter("d").Set(CreateStlColumn.MmToFoot(beam.Width * 10));

                    return fs1 as FamilySymbol;
                }
            }
            catch (Exception)
            {

                fs = new FilteredElementCollector(doc).OfCategory(category)
                                                        .WhereElementIsElementType()
                                                        .Cast<FamilySymbol>()
                                                        .First(f => f.FamilyName == familyName);
            }

            return fs;

        }

    }
}
