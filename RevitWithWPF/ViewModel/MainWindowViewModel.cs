using Autodesk.Revit.Creation;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Microsoft.Win32;
using RevitWithWPF.Application;
using RevitWithWPF.Command;
using RevitWithWPF.Model;
using RevitWithWPF.View;
using SteelColumn.Model;
using SteelColumn;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Forms;
using Xbim.Ifc;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.SharedBldgElements;
using Grid = Autodesk.Revit.DB.Grid;
using SteelColumn.GetData;
using SteelColumn.CreateElements;
using Xbim.Ifc4.GeometricConstraintResource;
using Xbim.Ifc4.GeometricModelResource;
using Xbim.Ifc4.GeometryResource;
using Xbim.Ifc4.ProfileResource;
using Xbim.Common;
using Microsoft.Extensions.Logging.Abstractions;
using Xbim.ModelGeometry.Scene;
using Xbim.Geometry.Engine.Interop;

namespace RevitWithWPF.ViewModel
{
    public class MainWindowViewModel : VMBase
    {
        #region Proberties

        private System.Windows.Forms.OpenFileDialog _fileDialog;

        public System.Windows.Forms.OpenFileDialog fileDialog
        {
            get { return _fileDialog; }
            set { _fileDialog = value; }
        }

        private bool _levels;

        public bool levels
        {
            get { return _levels; }
            set { _levels = value; }
        }


        private bool _Beams;

        public bool Beams
        {
            get { return _Beams; }
            set { _Beams = value; }
        }


        private bool _Columns;

        public bool Columns
        {
            get { return _Columns; }
            set { _Columns = value; }
        }


        private bool _Floors;

        public bool Floors
        {
            get { return _Floors; }
            set { _Floors = value; }
        }

        private bool _Footing;

        public bool Footing
        {
            get { return _Footing; }
            set { _Footing = value; }
        }

        private bool _Grids;

        public bool Grids
        {
            get { return _Grids; }
            set { _Grids = value; }
        }
        private bool _SlantedColumns;

        public bool SlantedColumns
        {
            get { return _SlantedColumns; }
            set { _SlantedColumns = value; }
        }

        public MyCommand BrowseCommand { get; set; }

        private string _FilePath;
        public string FilePath
        {
            get { return _FilePath; }
            set
            {
                _FilePath = value;
                OnPropertyChanged();
            }
        }

        private IModel _model;

        public IModel Model
        {
            get { return _model; }
            set
            {
                _model = value;
                OnPropertyChanged(nameof(Model));
            }
        }

       


        public MyCommand Create { get; set; }

        #endregion

        #region Constructor
        public MainWindowViewModel()
        { 
            BrowseCommand = new MyCommand(ExecuteBrowseCommand, CanBrowse);
            Create = new MyCommand(Createe, CanBrowse);
        }



        #endregion

        #region Methods
         

        private void ApplyWorkarounds(IfcStore model)
        {
            model.AddRevitWorkArounds();
            model.AddWorkAroundTrimForPolylinesIncorrectlySetToOneForEntireCurve();
        }

        private void ExecuteBrowseCommand(object obj)
        {
            try
            {
                fileDialog = new System.Windows.Forms.OpenFileDialog();
                fileDialog.Filter = "IFC Files|*.ifc";
                fileDialog.Title = "Select an IFC File";
                fileDialog.InitialDirectory = @"C:\";
                if (fileDialog.ShowDialog() == DialogResult.OK)
                {
                    FilePath = fileDialog.FileName;
                    var model = IfcStore.Open(System.IO.Path.GetFullPath(fileDialog.FileName));
                    if (model.GeometryStore.IsEmpty)
                    {
                        var context = new Xbim3DModelContext(model);
                        context.CreateContext(adjustWcs: false);
                    }
                    Model = model;
                }
            }
            catch (Exception ex)
            {
                 MessageBox.Show(ex.ToString());
            }

        }

        public void Createe(object parameter)
        {

            try
            {
                RevitCommand.exEvent.Raise();
            }
            catch (Exception ex)
            {
                ex.ToString();
            }

        }

        public bool CanBrowse(object parameter)
        {
            return true;
        }



        #endregion

    }
}
