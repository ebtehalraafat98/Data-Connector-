using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using SteelColumn.CreateElements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace SteelColumn
{
    public class CreateStlColumn
    {
        static int lvlNumber = 1;
        public static Level GetLevel(Autodesk.Revit.DB.Document doc, double elevation)
        {
            Level rLevel = null;
            ;
            try
            {

                rLevel = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Levels)
                                                        .WhereElementIsNotElementType()
                                                        .Cast<Level>()
                                                        .First(lvl => lvl.Elevation == elevation);
                //var col = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Levels)
                //                                        .WhereElementIsNotElementType()
                //                                        .Cast<Level>().ToList();
                //foreach (var item in col)
                //{
                //    if (item.Elevation == elevation)
                //    {
                //        rLevel = item;
                //    }
                //}
            }
            catch (Exception)
            {
                rLevel = Level.Create(doc,elevation);
                rLevel.LookupParameter("Name").Set(String.Format("Level {0}", lvlNumber++));
                //rLevel.LookupParameter("Name").Set($"{lvlNumber++}");

            }

            return rLevel;
        }
        public static Level GetSlabLevel(Autodesk.Revit.DB.Document doc, double elevation)
        {
            Level rLevel = null;
            ;
            try
            {

                rLevel = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Levels)
                                                        .WhereElementIsNotElementType()
                                                        .Cast<Level>()
                                                        .First(lvl => lvl.Elevation == elevation);
                //var col = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Levels)
                //                                        .WhereElementIsNotElementType()
                //                                        .Cast<Level>().ToList();
                //foreach (var item in col)
                //{
                //    if (item.Elevation == elevation)
                //    {
                //        rLevel = item;
                //    }
                //}
            }
            catch (Exception)
            {
                rLevel = Level.Create(doc, elevation);
                rLevel.LookupParameter("Name").Set(String.Format("SlabLevel {0}", lvlNumber++));
                //rLevel.LookupParameter("Name").Set($"{lvlNumber++}");

            }

            return rLevel;
        }

        public static double MapToStorey(double level, List<double> storeys)
        {
            Dictionary<int, double> def = new Dictionary<int, double>();
            int i = 0;
            foreach (double st in storeys)
            {
                def.Add(i, Math.Abs(level - st));
                i++;
            }
            return storeys[def.OrderBy(kvp => kvp.Value).First().Key];
        }
        public static Family OpenFamily(Document doc, string directory, string familyName)
        {
            string path = directory + familyName + ".rfa";

            Func<Element, bool> nameEquals = e => e.Name.Equals(familyName);

            FilteredElementCollector collector = new FilteredElementCollector(doc).OfClass(typeof(Family));

            Family f = collector.Any<Element>(nameEquals) ? collector.First<Element>(nameEquals) as Family : null;

            if (f == null) doc.LoadFamily(path, out f);

            return f;
        }
        public static FamilySymbol GetIShapeColumnFamilySymbol(Document doc, SteelColumn col, string familyName, BuiltInCategory category)
        {
            FamilySymbol fs = new FilteredElementCollector(doc).OfCategory(category)
                                                        .WhereElementIsElementType()
                                                        .Cast<FamilySymbol>()
                                                        .Where(f => f.FamilyName == familyName)
                                                        .FirstOrDefault(f =>
                                                        {
                                                            if (f.LookupParameter("Width").AsDouble() == MmToFoot(col.Width) &&
                                                                f.LookupParameter("Height").AsDouble() == MmToFoot(col.Depth) &&
                                                                f.LookupParameter("Flange Thickness").AsDouble() == MmToFoot(col.FlangeTh) &&
                                                                f.LookupParameter("Web Thickness").AsDouble() == MmToFoot(col.WebTh)
                                                                )
                                                            {
                                                                return true;
                                                            }
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
                string directory = $"C:/ProgramData/Autodesk/RVT {CreateBeam.RevitVersion}/Libraries/US Imperial/Structural Columns/Steel/AISC 14.1/";
                family = OpenFamily(doc, directory, familyName);
                fs = doc.GetElement(family.GetFamilySymbolIds().FirstOrDefault()) as FamilySymbol;
            }

            ElementType fs1 = fs.Duplicate(String.Format("I-Column Custom {0:0}x{1:0}", col.Width / 10, col.Depth / 10));
            fs1.LookupParameter("Width").Set(MmToFoot(col.Width));
            fs1.LookupParameter("Height").Set(MmToFoot(col.Depth));
            fs1.LookupParameter("Flange Thickness").Set(MmToFoot(col.FlangeTh));
            fs1.LookupParameter("Web Thickness").Set(MmToFoot(col.WebTh));
            fs1.LookupParameter("Web Fillet").Set(MmToFoot(0.85 * (col.WebTh)));

            return fs1 as FamilySymbol;
        }

        public static FamilyInstance CreateSteelColumnInstance(Document doc, XYZ location, Level bottomLvl, Level topLvl, FamilySymbol colType, double rotationAngleRad, bool isStructural)
        {

            FamilyInstance col;
            StructuralType structuralType = isStructural ? StructuralType.Column : StructuralType.NonStructural;
            if (!colType.IsActive) colType.Activate();
            col = doc.Create.NewFamilyInstance(location, colType, bottomLvl, structuralType);
            
            col.LookupParameter("Top Level").Set(topLvl.Id);
            col.LookupParameter("Top Offset").Set(0);
            col.LookupParameter("Base Offset").Set(0);
            ElementTransformUtils.RotateElement(doc, col.Id, Line.CreateBound(location, location + XYZ.BasisZ), rotationAngleRad);


            return col;
        }

        public static FamilyInstance CreateSteelColumnInstance(Document doc, SteelColumn column, FamilySymbol colType, bool isStructural, List<double> storeys)
        {

            XYZ location = new XYZ(MmToFoot(column.Location.X), MmToFoot(column.Location.Y), 0);
            Level colBottomLevel = GetLevel(doc, MapToStorey(MmToFoot(column.BottomLevel), storeys));
            Level colTopLevel = GetLevel(doc, MapToStorey(MmToFoot(column.TopLevel), storeys));

            double colRotation = column.RefDirection.X >= 0.0 ?
                90 + Vector3D.AngleBetween(new Vector3D(column.RefDirection.X, column.RefDirection.Y, column.RefDirection.Z), new Vector3D(0, -1, 0)) :
                90 + Vector3D.AngleBetween(new Vector3D(column.RefDirection.X, column.RefDirection.Y, column.RefDirection.Z), new Vector3D(0, -1, 0));

            colRotation *= Math.PI / 180.0;

            return CreateSteelColumnInstance(doc, location, colBottomLevel, colTopLevel, colType, colRotation, isStructural);

        }

        public static List<FamilyInstance> CreateSteelColumnInstance(Document doc, List<SteelColumn> cols, string familyName, bool isStructural, List<double> storeys)
        {
            FamilySymbol colType;
            return cols.Select(col =>
            {
                colType = GetIShapeColumnFamilySymbol(doc, col, familyName, BuiltInCategory.OST_StructuralColumns);
                return CreateSteelColumnInstance(doc, col, colType, isStructural, storeys);

            }).ToList();
        }




        const double _inchToMm = 25.4;
        const double _footToMm = 12 * _inchToMm;
        const double _footToMeter = _footToMm * 0.001;
        const double _sqfToSqm = _footToMeter * _footToMeter;
        const double _cubicFootToCubicMeter = _footToMeter * _sqfToSqm;

        /// <summary>
        /// Convert a given length in feet to millimetres.
        /// </summary>
        public static double FootToMm(double length)
        {
            return length * _footToMm;
        }

        /// <summary>
        /// Convert a given length in feet to millimetres,
        /// rounded to the closest millimetre.
        /// </summary>
        public static int FootToMmInt(double length)
        {
            //return (int) ( _feet_to_mm * d + 0.5 );
            return (int)Math.Round(_footToMm * length,
              MidpointRounding.AwayFromZero);
        }

        /// <summary>
        /// Convert a given length in feet to metres.
        /// </summary>
        public static double FootToMetre(double length)
        {
            return length * _footToMeter;
        }

        /// <summary>
        /// Convert a given length in millimetres to feet.
        /// </summary>
        public static double MmToFoot(double length)
        {
            return UnitUtils.ConvertToInternalUnits(length, UnitTypeId.Millimeters);
        }

        /// <summary>
        /// Convert a given point or vector from millimetres to feet.
        /// </summary>
        public static XYZ MmToFoot(XYZ v)
        {
            return v.Divide(_footToMm);
        }

        /// <summary>
        /// Convert a given volume in feet to cubic meters.
        /// </summary>
        public static double CubicFootToCubicMeter(double volume)
        {
            return volume * _cubicFootToCubicMeter;
        }
    }
}
