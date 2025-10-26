using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using RevitWithWPF.Application;
using SteelColumn.CreateElements;
using SteelColumn.GetData;
using SteelColumn.Model;
using SteelColumn;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using RevitWithWPF.View;
using RevitWithWPF.ViewModel;
using Xbim.Ifc;
using Xbim.Ifc4.GeometricConstraintResource;
using Xbim.Ifc4.GeometricModelResource;
using Xbim.Ifc4.GeometryResource;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.ProfileResource;
using Xbim.Ifc4.SharedBldgElements;
using System.Windows.Controls;
using ClassLibrary1;
using ClassLibrary1.Model;
using Xbim.Ifc4.StructuralElementsDomain;
using Autodesk.Revit.Creation;


namespace EventHandelers
{
    public class MainEventHandeler : IExternalEventHandler
    {
       
        public void Execute(UIApplication app)
        {
            try
            {
                MainWindowViewModel m = MainWindow.mv;
                CreateBeam.RevitVersion = Convert.ToInt32(RevitCommand.Doc.Application.VersionName.Split(' ').Last());
                using (Transaction transaction = new Transaction(RevitCommand.Doc, "TEKLA TO REVIT"))
                {
                    transaction.Start();
                    IfcStore xmlModel = IfcStore.Open($"{m.fileDialog.FileName}");

                    //save as XML
                    xmlModel.SaveAs($"{m.fileDialog.FileName}.ifcxml");


                    #region Delete Existing Level

                    //Get all levels in the document
                    FilteredElementCollector collector3 = new FilteredElementCollector(RevitCommand.Doc);
                    ICollection<Element> collection1 = collector3.OfClass(typeof(Level)).ToElements();

                    // Iterate through the levels

                    if (collection1.Count != 0)
                    {
                        foreach (Level e in collection1)
                        {
                            Level level = e as Level;
                            if (level != null)
                            {
                                // Find views associated with the level
                                FilteredElementCollector viewCollector = new FilteredElementCollector(RevitCommand.Doc);
                                ICollection<Element> viewCollection = viewCollector.OfCategory(BuiltInCategory.OST_Views)
                                    .WhereElementIsNotElementType()
                                    .ToElements();

                                foreach (Element viewElement in viewCollection)
                                {
                                    View view = viewElement as View;
                                    if (view != null && view.LevelId == level.Id)
                                    {
                                        // Delete the view


                                        RevitCommand.Doc.Delete(view.Id);

                                        // Delete the level
                                        RevitCommand.Doc.Delete(level.Id);


                                    }
                                }
                            }
                        }
                    }

                    #endregion

                    #region Levels
                    if (m.levels == true)
                    {
                        IEnumerable<IIfcGrid> lvls = (xmlModel.Instances.OfType<IIfcGrid>());
                        foreach (var item in lvls)
                        {

                            Level level1 = Createlvl.CreateLevel(RevitCommand.Doc, GetLevel.GetLevelData(item));
                            //get StructuralPlan ViewFamilyType
                            ViewFamilyType viewFamilyType = new FilteredElementCollector(RevitCommand.Doc)
                                .OfClass(typeof(ViewFamilyType))
                                .Cast<ViewFamilyType>()
                                .FirstOrDefault(vft => vft.ViewFamily == ViewFamily.StructuralPlan);

                            ViewPlan newView = ViewPlan.Create(RevitCommand.Doc, viewFamilyType.Id, level1.Id);
                            string viewName = level1.Name + " Plan ";

                            if (newView != null)
                            {
                                newView.Name = viewName;

                            }
                        }



                    }
                    #endregion

                    #region Beam
                    if (m.Beams == true)
                    {
                        Beam _beam = new Beam();
                        BeamStISEC _beam_ = new BeamStISEC();


                        List<FamilySymbol> BMTypes = new FilteredElementCollector(RevitCommand.Doc).OfCategory(BuiltInCategory.OST_StructuralFraming).
                                WhereElementIsElementType().Cast<FamilySymbol>().ToList();
                        IEnumerable<IfcBeam> beamS = (xmlModel.Instances.OfType<IfcBeam>());


                        foreach (IfcBeam item in beamS)
                        {
                            try
                            {
                                switch (((((((item.Representation.Representations.FirstOrDefault()
                                    as IIfcShapeRepresentation).Items.FirstOrDefault() as IIfcExtrudedAreaSolid)
                                        .SweptArea as IfcArbitraryClosedProfileDef).OuterCurve)
                                        as IfcIndexedPolyCurve).Points as IfcCartesianPointList2D).CoordList.Count)
                                {

                                    case 4:
                                        _beam = Getbeam.GetBeamsData(item);

                                        FamilySymbol familySymbol = CreateBeam.GetBeamFamilySymbolForCon(RevitCommand.Doc, _beam, "M_Concrete-Rectangular Beam", BuiltInCategory.OST_StructuralFraming);

                                        FamilyInstance ele = CreateBeam.CreateBeamInstance(RevitCommand.Doc, _beam, familySymbol);

                                        break;
                                    case 6:

                                        BeamSTLSEC _BeamSTLSEC = CreateBeam.GetBeamsSteelISECData(item);

                                        var familySymbol2 = Util.GetBeamFamilySymbolForLSEC(RevitCommand.Doc, _BeamSTLSEC, "AS_EXT_BEAM_LA", BuiltInCategory.OST_StructuralFraming);

                                        CreateBeam.CreateSTBeamLSECInstance(RevitCommand.Doc, _BeamSTLSEC, familySymbol2);

                                        break;



                                    case 8:

                                        if ((((((item.Representation.Representations.FirstOrDefault() as
                                            IIfcShapeRepresentation).Items.FirstOrDefault() as IIfcExtrudedAreaSolid)
                                            .SweptArea.ProfileName))).ToString()[0] == 67)
                                        {
                                            BeamCsec stCSEC = new BeamCsec();

                                            stCSEC = Getbeam.GetBeamsSteelCsecData(item);

                                            var familySymbol30 = CreateBeam.GetBeamFamilySymbolForCSEC(RevitCommand.Doc, stCSEC, "Param_Beam_Steel_C", BuiltInCategory.OST_StructuralFraming);
                                            CreateBeam.CreateSteelBeamInstanceCsec(RevitCommand.Doc, stCSEC, familySymbol30);
                                        }
                                        else
                                        {

                                            BeamSTLSEC _BeamSTLSEC8 = Getbeam.GetBeamsSteelISECData(item);

                                            var familySymbol8 = CreateBeam.GetBeamFamilySymbolForLSEC(RevitCommand.Doc, _BeamSTLSEC8, "AS_EXT_BEAM_T", BuiltInCategory.OST_StructuralFraming);

                                            CreateBeam.CreateSTBeamLSECInstance(RevitCommand.Doc, _BeamSTLSEC8, familySymbol8);
                                        }
                                        break;
                                    case 12:
                                        BeamStISEC stISE = new BeamStISEC();
                                        stISE = Getbeam.GetBeamsSteelData(item);
                                        var familySymbol6 = CreateBeam.GetBeamFamilySymbolForISEC(RevitCommand.Doc, stISE, "UB-Universal Beams", BuiltInCategory.OST_StructuralFraming);
                                        CreateBeam.CreateSteelBeamInstance(RevitCommand.Doc, stISE, familySymbol6);
                                        break;
                                    case 16:
                                        var asdsad = new FilteredElementCollector(RevitCommand.Doc).OfCategory(BuiltInCategory.OST_StructuralFraming).Cast<FamilySymbol>()
                                            .FirstOrDefault(e => e.Name == "Beam");

                                        Circular stIadSECds = new Circular();

                                        stIadSECds = Getbeam.GetBeamsSteelCircularSECData(item);


                                        var familySymbol5 = CreateBeam.GetBeamFamilySymbolForCircular(RevitCommand.Doc, stIadSECds, "M_HSS-Round Structural Tubing", BuiltInCategory.OST_StructuralFraming);

                                        CreateBeam.CreateSteelBeamCircularnstance(RevitCommand.Doc, stIadSECds, familySymbol5);

                                        break;
                                    default:
                                        BeamStISEC stISECc = new BeamStISEC();
                                        stISECc = Getbeam.GetBeamsSteelData(item);


                                        var familySymbol4 = CreateBeam.GetBeamFamilySymbolForISEC(RevitCommand.Doc, stISECc, "UB-Universal Beams", BuiltInCategory.OST_StructuralFraming);

                                        CreateBeam.CreateSteelBeamInstance(RevitCommand.Doc, stISECc, familySymbol4);
                                        break;


                                }
                            }
                            catch (Exception)
                            {

                                BeamStISEC stISE = new BeamStISEC();
                                stISE = Getbeam.GetBeamsSteelDataex(item);
                                var familySymbol6 = CreateBeam.GetBeamFamilySymbolForISEC(RevitCommand.Doc, stISE, "UB-Universal Beams", BuiltInCategory.OST_StructuralFraming);
                                CreateBeam.CreateSteelBeamInstance(RevitCommand.Doc, stISE, familySymbol6);
                            }

                        }

                    }
                    #endregion

                    List<double> storeysList = new List<double>();

                    #region Columns
                    if (m.Columns == true)
                    {
                        List<IIfcColumn> _columns = xmlModel.Instances.OfType<IIfcColumn>().ToList();
                        Column ColConitem = new Column();
                        SteelColumn.SteelColumn ColSt = new SteelColumn.SteelColumn();

                        List<FamilySymbol> BMsTypes = new FilteredElementCollector(RevitCommand.Doc).OfCategory(BuiltInCategory.OST_StructuralFraming).
                                WhereElementIsElementType().Cast<FamilySymbol>().ToList();

                       
                        // Create a filtered element collector to retrieve all levels
                        FilteredElementCollector collector = new FilteredElementCollector(RevitCommand.Doc)
                            .OfClass(typeof(Level));
                        //FilteredElementCollector collector2 = new FilteredElementCollector(Doc);
                        //ICollection<Element> collection = collector2.OfClass(typeof(Level)).ToElements()
                        // Iterate through the levels and get their elevations
                        if (collector.ToElements().Count != 0)
                        {
                            foreach (Element element in collector)
                            {
                                if (element is Level levell)
                                {
                                    storeysList.Add(levell.Elevation);

                                }
                            }
                        }
                        else
                        {
                            foreach (IIfcColumn ccoolluummnn in _columns)
                            {
                                ColConitem = GetColData.GetColumnsData(ccoolluummnn);
                                var lvlb = CreateStlColumn.MmToFoot(ColConitem.BottomLevel);
                                if (!storeysList.Contains(lvlb))
                                {
                                    storeysList.Add(lvlb);
                                }
                                var lvlt = CreateStlColumn.MmToFoot(ColConitem.TopLevel);
                                if (!storeysList.Contains(lvlt))
                                {
                                    storeysList.Add(lvlt);
                                }
                            }
                        }

                        foreach (IIfcColumn ccoolluummnn in _columns)
                        {
                            try
                            {
                                switch (((((((ccoolluummnn.Representation.Representations.FirstOrDefault()
                                    as IIfcShapeRepresentation).Items.FirstOrDefault() as IIfcExtrudedAreaSolid)
                                        .SweptArea as IfcArbitraryClosedProfileDef).OuterCurve)
                                        as IfcIndexedPolyCurve).Points as IfcCartesianPointList2D).CoordList.Count)
                                {
                                    case 4:


                                        ColConitem = GetColData.GetColumnsData(ccoolluummnn);

                                        FamilySymbol familySymbol = CreateCol.GetColumnFamilySymbol(RevitCommand.Doc, ColConitem, "M_Concrete-Rectangular-Column", BuiltInCategory.OST_StructuralColumns);

                                        var ele = CreateCol.CreateColumnInstance(RevitCommand.Doc, ColConitem, familySymbol, true, storeysList);


                                        break;
                                    case 12:
                                        SteelColumn.SteelColumn ColumnSt = new SteelColumn.SteelColumn();
                                        ColumnSt = GetSteelData.GetSteelColumnData(ccoolluummnn);
                                        var familySymbol3 = CreateStlColumn.GetIShapeColumnFamilySymbol(RevitCommand.Doc, ColumnSt, "UC-Universal Columns-Column", BuiltInCategory.OST_StructuralColumns);
                                        var ele1 = CreateStlColumn.CreateSteelColumnInstance(RevitCommand.Doc, ColumnSt, familySymbol3, true, storeysList);


                                        break;

                                }
                            }
                            catch (Exception)
                            {

                                //ColumnSt stCol = new ColumnSt();
                                //stCol = Createe.GetBeamsSteelData(ccoolluummnn);


                                //var familySymbol4 = Util.GetBeamFamilySymbolForISEC(Doc, stCol, "UB-Universal Beams", BuiltInCategory.OST_StructuralFraming);

                                //util.CreateSteelBeamInstance(Doc, stCol, familySymbol4);
                            }
                        }
                    }
                    #endregion

                    #region Floor
                    if (m.Floors == true)
                    {
                        List<IIfcSlab> _floors = xmlModel.Instances.OfType<IIfcSlab>().ToList();
                        foreach (var floor in _floors)
                        {
                            var floorIfc = GetSlab.GetFloorsData(floor);

                            CreateSlab.CreateFloor(RevitCommand.Doc, floorIfc);
                        }

                    }
                    #endregion

                    #region Slanted Column
                    if (m.SlantedColumns == true)
                    {
                        List<IIfcMember> _inclinedcolumns = xmlModel.Instances.OfType<IIfcMember>().ToList();
                        Member ColConMmbr = new Member();
                        Member BeamConMmbr = new Member();

                        foreach (IIfcMember item in _inclinedcolumns)
                        {
                            try
                            {
                                switch (((((((item.Representation.Representations.FirstOrDefault()
                                    as IIfcShapeRepresentation).Items.FirstOrDefault() as IIfcExtrudedAreaSolid)
                                        .SweptArea as IfcArbitraryClosedProfileDef).OuterCurve)
                                        as IfcIndexedPolyCurve).Points as IfcCartesianPointList2D).CoordList.Count)
                                {

                                    case 4:

                                        try
                                        {
                                            ColConMmbr = GetMember.GetMembersData(item);

                                            FamilySymbol familySymbol = CreateMember.GetMemberFamilySymbol(RevitCommand.Doc, ColConMmbr, "M_Concrete-Rectangular-Column", BuiltInCategory.OST_StructuralColumns);

                                            //List<double> storeysList = new List<double> { -500, 0, 7200 };
                                            var ele = CreateMember.CreateMemberInstance(RevitCommand.Doc, ColConMmbr, familySymbol, true, storeysList);
                                        }
                                        catch (Exception)
                                        {

                                            //BeamConMmbr = Getbeam.GetBeamsData(item);

                                            //FamilySymbol familySymbol = CreateBeam.GetBeamFamilySymbolForCon(Doc, _beam, "M_Concrete-Rectangular Beam", BuiltInCategory.OST_StructuralFraming);

                                            //FamilyInstance ele = CreateBeam.CreateBeamInstance(Doc, _beam, familySymbol); ;
                                        }


                                        break;
                                    case 12:
                                        try
                                        {
                                            SteelMember MemberSt = new SteelMember();
                                            MemberSt = GetSteelMember.GetMembersSteelData(item);
                                            var familySymbol3 = CreateStlMember.GetIShapeMemberFamilySymbol(RevitCommand.Doc, MemberSt, "UC-Universal Columns-Column", BuiltInCategory.OST_StructuralColumns);
                                            var ele1 = CreateStlMember.CreateSteelMemberInstance(RevitCommand.Doc, MemberSt, familySymbol3, true, storeysList);
                                        }
                                        catch (Exception)
                                        {

                                            throw;
                                        }



                                        break;


                                }
                            }
                            catch (Exception)
                            {

                            }
                        }

                    }
                    #endregion

                    #region Footing

                    if (m.Footing == true)
                    {
                        IEnumerable<IfcFooting> FOU = (xmlModel.Instances.OfType<IfcFooting>());
                        foreach (IfcFooting item in FOU)
                        {
                            try
                            {
                                Foundation FF = new Foundation();
                                FF = GetFoundation.foun(item);
                                FamilySymbol familySymbol = Util.GetBeamFamilySymbolForfoundation(RevitCommand.Doc, FF, "M_Footing-Rectangular", BuiltInCategory.OST_StructuralFoundation);
                                var foundation = CreateFoundation.CreateFoundation1(RevitCommand.Doc, familySymbol, FF);
                            }
                            catch (Exception)
                            {


                            }

                        }
                    }

                    #endregion

                    #region Grids

                    if (m.Grids == true)
                    {

                        var axis = (((xmlModel.Instances.OfType<IfcGridAxis>().ToList())));

                        foreach (var item in axis)
                        {
                            try
                            {
                                grid giGrid = Getgrid.GetGridData(item);
                                Creategrid.CreateGrid(RevitCommand.Doc, giGrid);
                            }
                            catch (Exception ex)
                            {
                                ex.ToString();
                            }

                        }
                    }

                    #endregion



                    #region Delete Level

                    // Get all levels in the document
                    FilteredElementCollector collector2 = new FilteredElementCollector(RevitCommand.Doc);
                    ICollection<Element> collection = collector2.OfClass(typeof(Level)).ToElements();

                    // Iterate through the levels


                    foreach (Level e in collection)
                    {
                        Level level = e as Level;
                        if (level != null)
                        {
                            // Check if the level name contains the specific word
                            if (level.Name.Contains("Level"))
                            {
                                // Delete the level
                                RevitCommand.Doc.Delete(level.Id);
                            }
                        }
                    }
                    #endregion

                    transaction.Commit();

                }
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }

        public string GetName()
        {
            return "Revit";
        }
    }
}
