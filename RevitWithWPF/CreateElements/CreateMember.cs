using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using SteelColumn.CreateElements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace SteelColumn
{
    public class CreateMember
    {
        public static List<FamilyInstance> CreateMemberInstance(Document doc, List<Member> cols, string familyName, bool isStructural, List<double> storeys)
        {
            FamilySymbol colType;
            return cols.Select(mmbr =>
            {
                colType = GetMemberFamilySymbol(doc, mmbr, familyName, BuiltInCategory.OST_StructuralColumns);
                return CreateMemberInstance(doc, mmbr, colType, isStructural, storeys);

            }).ToList();
        }

        public static FamilyInstance CreateMemberInstance(Document doc, Member member, FamilySymbol colType, bool isStructural, List<double> storeys)
        {

            XYZ location = new XYZ(CreateStlColumn.MmToFoot(member.Location.X), CreateStlColumn.MmToFoot(member.Location.Y), CreateStlColumn.MmToFoot(member.Location.Z));
            Level colBottomLevel = CreateStlColumn.GetLevel(doc, CreateStlColumn.MapToStorey(CreateStlColumn.MmToFoot(member.BottomLevel), storeys));
            Level colTopLevel = CreateStlColumn.GetLevel(doc, CreateStlColumn.MapToStorey(CreateStlColumn.MmToFoot(member.TopLevel), storeys));
            //Level colBottomLevel = Level.Create(doc, 0);
            //Level colTopLevel = Level.Create(doc, Util.MmToFoot(5000));

            double colRotation = member.RefDirection.Y >= 0.0 ?
                Vector3D.AngleBetween(new Vector3D(member.Axis.X, member.Axis.Y, member.Axis.Z), new Vector3D(member.Axis.X, member.Axis.Y, member.Axis.Z)) :
                90 + Vector3D.AngleBetween(new Vector3D(member.Axis.X, member.Axis.Y, member.Axis.Z), new Vector3D(member.Axis.X, member.Axis.Y, member.Axis.Z));

            colRotation *= Math.PI / 180.0;

            return CreateMemberInstance(doc, location, colBottomLevel, colTopLevel, colType, colRotation, isStructural, storeys, member.RefDirection);
        }
        public static FamilyInstance CreateMemberInstance(Document doc, XYZ location, Level bottomLvl, Level topLvl, FamilySymbol colType, double rotationAngleRad, bool isStructural, List<double> storeys,Point RefDirection)
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
            XYZ endPoint = location + (topLvl.Elevation- bottomLvl.Elevation) * new XYZ(RefDirection.X, RefDirection.Y, RefDirection.Z);

            Parameter param = mmbr.get_Parameter(BuiltInParameter.SLANTED_COLUMN_TYPE_PARAM);
            param.Set((double)SlantedOrVerticalColumnType.CT_EndPoint);

            // After setting to a slanted member,
            // the location should be a curve.
            LocationCurve strElemCurve = mmbr.Location as LocationCurve;

            // Set the start and end point of a curve.
            if (strElemCurve != null)
            {
                Line line = Line.CreateBound(location,  endPoint);
                strElemCurve.Curve = line;
            }
                        
            #endregion

            return mmbr;
        }
        public static FamilySymbol GetMemberFamilySymbol(Document doc, Member mmbr, string familyName, BuiltInCategory category)
        {
            string b = category == BuiltInCategory.OST_StructuralColumns ? "b" : "Width";
            string h = category == BuiltInCategory.OST_StructuralColumns ? "h" : "Depth";

            FamilySymbol fs = new FilteredElementCollector(doc).OfCategory(category)
                                                        .WhereElementIsElementType()
                                                        .Cast<FamilySymbol>()
                                                        .Where(f => f.FamilyName == familyName)
                                                        .FirstOrDefault(f =>
                                                        {
                                                            if (f.LookupParameter(h).AsDouble() == CreateStlColumn.MmToFoot(mmbr.Depth) &&
                                                                    f.LookupParameter(b).AsDouble() == CreateStlColumn.MmToFoot(mmbr.Width))

                                                            
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

                ElementType fs1 = fs.Duplicate(String.Format("Member {0:0}x{1:0}", mmbr.Width / 10, mmbr.Depth / 10));
                fs1.LookupParameter(b).Set(CreateStlColumn.MmToFoot(mmbr.Width));
                fs1.LookupParameter(h).Set(CreateStlColumn.MmToFoot(mmbr.Depth));
                return fs1 as FamilySymbol;
            }

        }
    }
}
