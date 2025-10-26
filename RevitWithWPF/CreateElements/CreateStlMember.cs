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
    public class CreateStlMember
    {
      public static FamilySymbol GetIShapeMemberFamilySymbol(Document doc, SteelMember mmbr, string familyName, BuiltInCategory category)
        {
            FamilySymbol fs = new FilteredElementCollector(doc).OfCategory(category)
                                                        .WhereElementIsElementType()
                                                        .Cast<FamilySymbol>()
                                                        .Where(f => f.FamilyName == familyName)
                                                        .FirstOrDefault(f =>
                                                        {
                                                            if (f.LookupParameter("Width").AsDouble() == CreateStlColumn.MmToFoot(mmbr.Width) &&
                                                                f.LookupParameter("Height").AsDouble() == CreateStlColumn.MmToFoot(mmbr.Depth) &&
                                                                f.LookupParameter("Flange Thickness").AsDouble() == CreateStlColumn.MmToFoot(mmbr.FlangeTh) &&
                                                                f.LookupParameter("Web Thickness").AsDouble() == CreateStlColumn.MmToFoot(mmbr.WebTh)
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
                family = CreateStlColumn.OpenFamily(doc, directory, familyName);
                fs = doc.GetElement(family.GetFamilySymbolIds().FirstOrDefault()) as FamilySymbol;
            }

            ElementType fs1 = fs.Duplicate(String.Format("I-Member Custom {0:0}x{1:0}", mmbr.Width / 10, mmbr.Depth / 10));
            fs1.LookupParameter("Width").Set(CreateStlColumn.MmToFoot(mmbr.Width));
            fs1.LookupParameter("Height").Set(CreateStlColumn.MmToFoot(mmbr.Depth));
            fs1.LookupParameter("Flange Thickness").Set(CreateStlColumn.MmToFoot(mmbr.FlangeTh));
            fs1.LookupParameter("Web Thickness").Set(CreateStlColumn.MmToFoot(mmbr.WebTh));
            fs1.LookupParameter("Web Fillet").Set(CreateStlColumn.MmToFoot(0.85 * (mmbr.WebTh)));

            return fs1 as FamilySymbol;
        }

        public static FamilyInstance CreateSteelMemberInstance(Document doc, XYZ location, Level bottomLvl, Level topLvl, FamilySymbol colType, double rotationAngleRad, bool isStructural, List<double> storeys, Point RefDirection)
        {

            FamilyInstance mmbr;
            StructuralType structuralType = isStructural ? StructuralType.Column : StructuralType.NonStructural;
            if (!colType.IsActive) colType.Activate();
            mmbr = doc.Create.NewFamilyInstance(location, colType, bottomLvl, structuralType);

            mmbr.LookupParameter("Top Level").Set(topLvl.Id);
            mmbr.LookupParameter("Top Offset").Set(0);
            mmbr.LookupParameter("Base Offset").Set(0);
            ElementTransformUtils.RotateElement(doc, mmbr.Id, Line.CreateBound(location, location + XYZ.BasisZ), rotationAngleRad);

             #region tryRotation

            XYZ startPoint = new XYZ(0.0, 0.0, 0.0);
            XYZ endPoint = location + (topLvl.Elevation - bottomLvl.Elevation) * new XYZ(RefDirection.X, RefDirection.Y, RefDirection.Z);

            Parameter param = mmbr.get_Parameter(BuiltInParameter.SLANTED_COLUMN_TYPE_PARAM);
            param.Set((double)SlantedOrVerticalColumnType.CT_EndPoint);

            // After setting to a slanted member,
            // the location should be a curve.
            LocationCurve strElemCurve = mmbr.Location as LocationCurve;

            // Set the start and end point of a curve.
            if (strElemCurve != null)
            {
                Line line = Line.CreateBound(location, endPoint);
                strElemCurve.Curve = line;
            }

            #endregion


            return mmbr;
        }

        public static FamilyInstance CreateSteelMemberInstance(Document doc, SteelMember member, FamilySymbol colType, bool isStructural, List<double> storeys)
        {

            XYZ location = new XYZ(CreateStlColumn.MmToFoot(member.Location.X), CreateStlColumn.MmToFoot(member.Location.Y), 0);
            Level colBottomLevel = CreateStlColumn.GetLevel(doc, CreateStlColumn.MapToStorey(CreateStlColumn.MmToFoot(member.BottomLevel), storeys));
            Level colTopLevel = CreateStlColumn.GetLevel(doc, CreateStlColumn.MapToStorey(CreateStlColumn.MmToFoot(member.TopLevel), storeys));

            double colRotation = member.RefDirection.X >= 0.0 ?
                90 + Vector3D.AngleBetween(new Vector3D(member.Axis.X, member.Axis.Y, member.Axis.Z), new Vector3D(member.Axis.X, member.Axis.Y, member.Axis.Z)) :
                90 + Vector3D.AngleBetween(new Vector3D(member.Axis.X, member.Axis.Y, member.Axis.Z), new Vector3D(member.Axis.X, member.Axis.Y, member.Axis.Z));


            colRotation *= Math.PI / 180.0;

            return CreateSteelMemberInstance(doc, location, colBottomLevel, colTopLevel, colType, colRotation, isStructural, storeys, member.RefDirection);

        }

        public static List<FamilyInstance> CreateSteelMemberInstance(Document doc, List<SteelMember> cols, string familyName, bool isStructural, List<double> storeys)
        {
            FamilySymbol colType;
            return cols.Select(mmbr =>
            {
                colType = GetIShapeMemberFamilySymbol(doc, mmbr, familyName, BuiltInCategory.OST_StructuralColumns);
                return CreateSteelMemberInstance(doc, mmbr, colType, isStructural, storeys);

            }).ToList();
        } 
    }
}
