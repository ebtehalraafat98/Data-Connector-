using Autodesk.Revit.DB;
using SteelColumn.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace SteelColumn.CreateElements
{
    public class CreateSlab
    {
        public static Floor CreateFloor(Document doc, FloorSlab floor)
        {

            //////
            List<XYZ> pts1 = floor.Profile.Select(pt =>
            {
                Point3D pt1 = new Point3D(pt.X, pt.Y, pt.Z);
                return new XYZ(CreateStlColumn.MmToFoot(pt1.X), CreateStlColumn.MmToFoot(pt1.Y), CreateStlColumn.MmToFoot(pt1.Z));

            }).ToList();
            IList<Curve> curves = new List<Curve>();

            ////(pts1[i], pts1[i + 1])
            //curArr.Append(CurveLoop.Create(curves.Create(pts1[i], pts1[i + 1]));
            for (int i = 0; i < pts1.Count - 1; i++)
            {
                curves.Add(Line.CreateBound(pts1[i], pts1[i + 1]));
            }
            curves.Add(Line.CreateBound(pts1[pts1.Count - 1], pts1[0]));
            IList<CurveLoop> curArr = new List<CurveLoop> { CurveLoop.Create(curves) };
            //curArr.Append(CurveLoop.Create(curves));
            var levelid = CreateStlColumn.GetSlabLevel(doc, CreateStlColumn.MmToFoot(floor.Level)).Id;
            Floor f = Floor.Create(doc, curArr, GetFloorType(doc, floor).Id, levelid);
            //doc.Create.NewFloor(curArr, GetFloorType(doc, floor), CreateStlColumn.GetLevel(doc, CreateStlColumn.MmToFoot(floor.Level)), false);
            f.LookupParameter("Height Offset From Level").Set(0);

            return f;
        }

        public List<Floor> CreateFloorList(Document doc, List<FloorSlab> floors)
        {
            return floors.Select(floor => CreateFloor(doc, floor)).ToList();
        }

        public static FloorType GetFloorType(Document doc, FloorSlab floor)
        {

            FloorType f = new FilteredElementCollector(doc).OfClass(typeof(FloorType))
                                                            .Where(ft => ft is FloorType)
                                                            .FirstOrDefault(e =>
                                                            {
                                                                CompoundStructure comp = (e as FloorType).GetCompoundStructure();
                                                                if (comp.GetLayerWidth(0) == CreateStlColumn.MmToFoot(floor.Depth))
                                                                {
                                                                    return true;
                                                                }
                                                                return false;

                                                            }) as FloorType;
            if (f != null) return f;

            f = new FilteredElementCollector(doc).OfClass(typeof(FloorType)).FirstOrDefault() as FloorType;
            f = f.Duplicate(String.Format("FloorNs {0} CM", floor.Depth / 10)) as FloorType;
            CompoundStructure compound = f.GetCompoundStructure();
            compound.SetLayerWidth(0, CreateStlColumn.MmToFoot(floor.Depth));
            f.SetCompoundStructure(compound);

            return f;


        }
    }
}
