using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using SteelColumn.CreateElements;

namespace SteelColumn
{
    public class CreateCol
    {
        public static List<FamilyInstance> CreateColumnInstance(Document doc, List<Column> cols, string familyName, bool isStructural, List<double> storeys)
        {
            FamilySymbol colType;
            return cols.Select(col =>
            {
                colType = GetColumnFamilySymbol(doc, col, familyName, BuiltInCategory.OST_StructuralColumns);
                return CreateColumnInstance(doc, col, colType, isStructural, storeys);

            }).ToList();
        }

        public static FamilyInstance CreateColumnInstance(Document doc, Column column, FamilySymbol colType, bool isStructural, List<double> storeys)
        {

            XYZ location = new XYZ(CreateStlColumn.MmToFoot(column.Location.X), CreateStlColumn.MmToFoot(column.Location.Y), 0);
            Level colBottomLevel = CreateStlColumn.GetLevel(doc, CreateStlColumn.MapToStorey(CreateStlColumn.MmToFoot(column.BottomLevel), storeys));
            Level colTopLevel = CreateStlColumn.GetLevel(doc, CreateStlColumn.MapToStorey(CreateStlColumn.MmToFoot(column.TopLevel), storeys));
            //Level colBottomLevel = Level.Create(doc, 0);
            //Level colTopLevel = Level.Create(doc, Util.MmToFoot(5000));

            double colRotation = column.RefDirection.X >= 0.0 ?
                90 + Vector3D.AngleBetween(new Vector3D(column.RefDirection.X, column.RefDirection.Y, column.RefDirection.Z), new Vector3D(0, -1, 0)) :
                90 + Vector3D.AngleBetween(new Vector3D(column.RefDirection.X, column.RefDirection.Y, column.RefDirection.Z), new Vector3D(0, -1, 0));

            colRotation *= Math.PI / 180.0;

            return CreateColumnInstance(doc, location, colBottomLevel, colTopLevel, colType, colRotation, isStructural, storeys);
        }
        public static FamilyInstance CreateColumnInstance(Document doc, XYZ location, Level bottomLvl, Level topLvl, FamilySymbol colType, double rotationAngleRad, bool isStructural, List<double> storeys)
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
        public static FamilySymbol GetColumnFamilySymbol(Document doc, Column col, string familyName, BuiltInCategory category)
        {
            string b = category == BuiltInCategory.OST_StructuralColumns ? "b" : "Width";
            string h = category == BuiltInCategory.OST_StructuralColumns ? "h" : "Depth";

            FamilySymbol fs = new FilteredElementCollector(doc).OfCategory(category)
                                                        .WhereElementIsElementType()
                                                        .Cast<FamilySymbol>()
                                                        .Where(f => f.FamilyName == familyName)
                                                        .FirstOrDefault(f =>
                                                        {
                                                            if (f.LookupParameter(h).AsDouble() == CreateStlColumn.MmToFoot(col.Depth) &&
                                                                    f.LookupParameter(b).AsDouble() == CreateStlColumn.MmToFoot(col.Width))

                                                            //if (Math.Abs(f.LookupParameter("Depth").AsDouble() - Util.MmToFoot(col.Depth)) < Util.MmToFoot(0.001) &&
                                                            //Math.Abs(f.LookupParameter("Width").AsDouble() - Util.MmToFoot(col.Width)) < Util.MmToFoot(0.001))
                                                            {
                                                                return true;
                                                            }
                                                            else
                                                            {
                                                                return false;

                                                            }
                                                        });

            if (fs != null) return fs;
            else
            {
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
                    string directory = $"C:/ProgramData/Autodesk/{CreateBeam.RevitVersion}/Libraries/US Imperial/Structural Columns/Concrete/";
                    family = CreateStlColumn.OpenFamily(doc, directory, familyName);
                    fs = doc.GetElement(family.GetFamilySymbolIds().FirstOrDefault()) as FamilySymbol;
                }

                ElementType fs1 = fs.Duplicate(String.Format("Column {0:0}x{1:0}", col.Width / 10, col.Depth / 10));
                fs1.LookupParameter(b).Set(CreateStlColumn.MmToFoot(col.Width));
                fs1.LookupParameter(h).Set(CreateStlColumn.MmToFoot(col.Depth));
                return fs1 as FamilySymbol;
            }

        }
    }
}
