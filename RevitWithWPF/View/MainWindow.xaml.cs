using Microsoft.Extensions.Logging;
using RevitWithWPF.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using Xbim.Common;
using Xbim.Ifc;
using Xbim.IO;
using Xbim.ModelGeometry.Scene;
using Xbim.Presentation.LayerStyling;
using Xbim.Presentation;
using ProgressBar = System.Windows.Controls.ProgressBar;
using Xbim.Geometry.Engine.Interop;
using Microsoft.Extensions.Logging.Abstractions;

namespace RevitWithWPF.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindowViewModel mv ;
        public MainWindow()
        {
            mv =  new MainWindowViewModel();
            this.DataContext = mv;
            // Dont remove the next line, this line enusres this library is loaded
            var duumyInit = new MahApps.Metro.IconPacks.PackIconUnicons();
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


        //#region View IFC

        //private string _openedModelFileName;
        //private BackgroundWorker _loadFileBackgroundWorker;
        //private string _temporaryXbimFileName;
        //private bool _meshModel = true;
        //private bool _multiThreading = true;
        //private double _angularDeflectionOverride = double.NaN;
        //private double _deflectionOverride = double.NaN;
        //public delegate void LoadingCompleteEventHandler(object s, RunWorkerCompletedEventArgs args);
        //public event LoadingCompleteEventHandler LoadingComplete;
        //public static RoutedCommand OpenExportWindowCmd = new RoutedCommand();
        //protected ILogger Logger { get; private set; }
        //public XbimDBAccess FileAccessMode { get; set; } = XbimDBAccess.Read;


        //public IfcStore Model
        //{
        //    get
        //    {
        //        var op = MainFrame.DataContext as ObjectDataProvider;
        //        return op == null ? null : op.ObjectInstance as IfcStore;
        //    }
        //}

        //public string GetOpenedModelFileName()
        //{
        //    return _openedModelFileName;
        //}

        //private void ApplyWorkarounds(IfcStore model)
        //{
        //    model.AddRevitWorkArounds();
        //    model.AddWorkAroundTrimForPolylinesIncorrectlySetToOneForEntireCurve();
        //}

        //private void dlg_OpenAnyFile(object sender, CancelEventArgs e)
        //{
        //    var dlg = sender as OpenFileDialog;
        //    if (dlg != null) LoadAnyModel(dlg.FileName);
        //}

        //public void LoadAnyModel(string modelFileName)
        //{
        //    var fInfo = new FileInfo(modelFileName);
        //    if (!fInfo.Exists) // file does not exist; do nothing
        //        return;
        //    if (fInfo.FullName.ToLower() == GetOpenedModelFileName()) //same file do nothing
        //        return;

        //    //CloseAndDeleteTemporaryFiles();
        //    SetOpenedModelFileName(modelFileName.ToLower());
        //    //ProgressStatusBar.Visibility = Visibility.Visible;
        //    SetWorkerForFileLoad();

        //    var ext = fInfo.Extension.ToLower();
        //    switch (ext)
        //    {
        //        case ".ifc": //it is an Ifc File
        //        case ".ifcxml": //it is an IfcXml File
        //            _loadFileBackgroundWorker.RunWorkerAsync(modelFileName);
        //            break;
        //        default:
        //            Logger.LogWarning("Extension '{extension}' has not been recognised.", ext);
        //            break;
        //    }
        //}

        //private void CloseAndDeleteTemporaryFiles()
        //{
        //    try
        //    {
        //        if (_loadFileBackgroundWorker != null && _loadFileBackgroundWorker.IsBusy)
        //            _loadFileBackgroundWorker.CancelAsync(); //tell it to stop
        //        SetOpenedModelFileName(null);
        //        if (Model != null)
        //        {
        //            Model.Dispose();
        //            ModelProvider.ObjectInstance = null;
        //            ModelProvider.Refresh();
        //        }
        //        if (!(DrawingControl.DefaultLayerStyler is SurfaceLayerStyler))
        //            SetDefaultModeStyler(null, null);
        //    }
        //    finally
        //    {
        //        if (!(_loadFileBackgroundWorker != null && _loadFileBackgroundWorker.IsBusy && _loadFileBackgroundWorker.CancellationPending)) //it is still busy but has been cancelled 
        //        {
        //            if (!string.IsNullOrWhiteSpace(_temporaryXbimFileName) && File.Exists(_temporaryXbimFileName))
        //                File.Delete(_temporaryXbimFileName);
        //            _temporaryXbimFileName = null;
        //        } //else do nothing it will be cleared up in the worker thread
        //    }
        //}

        //private void SetDefaultModeStyler(object sender, RoutedEventArgs e)
        //{
        //    ConnectStylerFeedBack();
        //    DrawingControl.ReloadModel();
        //}

        //private void ConnectStylerFeedBack()
        //{
        //    if (DrawingControl.DefaultLayerStyler is IProgressiveLayerStyler)
        //    {
        //        ((IProgressiveLayerStyler)DrawingControl.DefaultLayerStyler).ProgressChanged += OnProgressChanged;
        //    }
        //}

        //private void SetOpenedModelFileName(string ifcFilename)
        //{
        //    _openedModelFileName = ifcFilename;
        //}

        //private void SetWorkerForFileLoad()
        //{
        //    _loadFileBackgroundWorker = new BackgroundWorker
        //    {
        //        WorkerReportsProgress = true,
        //        WorkerSupportsCancellation = true
        //    };
        //    _loadFileBackgroundWorker.ProgressChanged += OnProgressChanged;
        //    _loadFileBackgroundWorker.DoWork += OpenAcceptableExtension;
        //    _loadFileBackgroundWorker.RunWorkerCompleted += FileLoadCompleted;
        //}

        //private void FileLoadCompleted(object s, RunWorkerCompletedEventArgs args)
        //{
        //    if (args.Result is IfcStore) //all ok
        //    {
        //        //this Triggers the event to load the model into the views 
        //        var context = new Xbim3DModelContext((IModel)args.Result);
        //        context.CreateContext();
        //        ModelProvider.ObjectInstance = args.Result;
        //        ModelProvider.Refresh();
        //        //ProgressBar.Value = 0;
        //        //StatusMsg.Text = "Ready";
        //    }
        //    else //we have a problem
        //    {
        //        var errMsg = args.Result as string;
        //        var exception = args.Result as Exception;
        //        if (exception != null)
        //        {
        //            var sb = new StringBuilder();

        //            var indent = "";
        //            while (exception != null)
        //            {
        //                sb.AppendFormat("{0}{1}\n", indent, exception.Message);
        //                exception = exception.InnerException;
        //                indent += "\t";
        //            }
        //        }
        //        //ProgressBar.Value = 0;
        //        //StatusMsg.Text = "Error/Ready";
        //        SetOpenedModelFileName("");
        //    }
        //    FireLoadingComplete(s, args);
        //}

        //private void OpenAcceptableExtension(object s, DoWorkEventArgs args)
        //{
        //    var worker = s as BackgroundWorker;
        //    var selectedFilename = args.Argument as string;
        //    try
        //    {
        //        if (worker == null)
        //            throw new Exception("Background thread could not be accessed");
        //        _temporaryXbimFileName = System.IO.Path.GetTempFileName();
        //        SetOpenedModelFileName(selectedFilename);
        //        var model = IfcStore.Open(selectedFilename, null, null, worker.ReportProgress, FileAccessMode);
        //        if (_meshModel)
        //        {
        //            ApplyWorkarounds(model);
        //            // mesh direct model
        //            if (model.GeometryStore.IsEmpty)
        //            {
        //                var DoMesh = true;
        //                if (model.ReferencingModel is Xbim.IO.Esent.EsentModel em)
        //                {
        //                    if (!em.CanEdit)
        //                    {
        //                        Logger.LogError(0, null, "Xbim models need to be opened in write mode, if they don't have geometry. Use the View/Settings/General tab to configure.");
        //                        DoMesh = false;
        //                    }
        //                }
        //                if (DoMesh)
        //                {
        //                    try
        //                    {
        //                        var context = new Xbim3DModelContext(model);

        //                        if (!_multiThreading)
        //                            context.MaxThreads = 1;

        //                        SetDeflection(model);
        //                        //upgrade to new geometry representation, uses the default 3D model
        //                        context.CreateContext(worker.ReportProgress, true);
        //                    }
        //                    catch (Exception geomEx)
        //                    {
        //                        var sb = new StringBuilder();
        //                        sb.AppendLine($"Error creating geometry context of '{selectedFilename}' {geomEx.StackTrace}.");
        //                        var newexception = new Exception(sb.ToString(), geomEx);
        //                        Logger.LogError(0, newexception, "Error creating geometry context of {filename}", selectedFilename);
        //                    }
        //                }
        //            }

        //            // mesh references
        //            foreach (var modelReference in model.ReferencedModels)
        //            {
        //                // creates federation geometry contexts if needed
        //                Debug.WriteLine(modelReference.Name);
        //                if (modelReference.Model == null)
        //                    continue;
        //                if (!modelReference.Model.GeometryStore.IsEmpty)
        //                    continue;
        //                var context = new Xbim3DModelContext(modelReference.Model);
        //                if (!_multiThreading)
        //                    context.MaxThreads = 1;

        //                SetDeflection(modelReference.Model);
        //                //upgrade to new geometry representation, uses the default 3D model
        //                context.CreateContext(worker.ReportProgress, true);
        //            }
        //            if (worker.CancellationPending)
        //            //if a cancellation has been requested then don't open the resulting file
        //            {
        //                try
        //                {
        //                    model.Close();
        //                    if (File.Exists(_temporaryXbimFileName))
        //                        File.Delete(_temporaryXbimFileName); //tidy up;
        //                    _temporaryXbimFileName = null;
        //                    SetOpenedModelFileName(null);
        //                }
        //                catch (Exception ex)
        //                {
        //                    Logger.LogError(0, ex, "Failed to cancel open of model {filename}", selectedFilename);
        //                }
        //                return;
        //            }
        //        }
        //        else
        //        {
        //            Logger.LogWarning("Settings prevent mesh creation.");
        //        }
        //        args.Result = model;
        //    }
        //    catch (Exception ex)
        //    {
        //        var sb = new StringBuilder();
        //        sb.AppendLine($"Error opening '{selectedFilename}' {ex.StackTrace}.");
        //        var newexception = new Exception(sb.ToString(), ex);
        //        Logger.LogError(0, ex, "Error opening {filename}", selectedFilename);
        //        args.Result = newexception;
        //    }
        //}

        //private void SetDeflection(IModel model)
        //{
        //    var mf = model.ModelFactors;
        //    if (mf == null)
        //        return;
        //    if (!double.IsNaN(_angularDeflectionOverride))
        //        mf.DeflectionAngle = _angularDeflectionOverride;
        //    if (!double.IsNaN(_deflectionOverride))
        //        mf.DeflectionTolerance = mf.OneMilliMetre * _deflectionOverride;
        //}

        //private void FireLoadingComplete(object s, RunWorkerCompletedEventArgs args)
        //{
        //    if (LoadingComplete != null)
        //    {
        //        LoadingComplete(s, args);
        //    }
        //}

        //private void OnProgressChanged(object s, ProgressChangedEventArgs args)
        //{
        //    if (args.ProgressPercentage < 0 || args.ProgressPercentage > 100)
        //        return;

        //    //Application.Current.Dispatcher.BeginInvoke(
        //    //    DispatcherPriority.Send,
        //    //    new Action(() =>
        //    //    {
        //    //        //ProgressBar.Value = args.ProgressPercentage;
        //    //        //StatusMsg.Text = (string)args.UserState;
        //    //    }));

        //}

        //private void OpenIFC(object sender, RoutedEventArgs e)
        //{
        //    CloseAndDeleteTemporaryFiles();
        //    var corefilters = new[] {

        //    "TAFRA Files|*.ifc;*.ifcxml;",
        //    "IFC File (*.ifc)|*.ifc",
        //    "IFCXML File (*.IfcXml)|*.ifcxml"
        //};

        //    // Filter files by extension 
        //    var dlg = new OpenFileDialog
        //    {
        //        Filter = string.Join("|", corefilters)
        //    };

        //    dlg.FileOk += dlg_OpenAnyFile;
        //    dlg.ShowDialog();

        //    label.Text = dlg.FileName;
        //}

        //private void CommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        //{
        //    if (_loadFileBackgroundWorker != null && _loadFileBackgroundWorker.IsBusy)
        //        e.CanExecute = false;
        //    else
        //    {
        //        if (e.Command == ApplicationCommands.Close || e.Command == ApplicationCommands.SaveAs)
        //        {
        //            e.CanExecute = (Model != null);
        //        }
        //        else if (e.Command == OpenExportWindowCmd)
        //        {
        //            e.CanExecute = (Model != null) && (!string.IsNullOrEmpty(GetOpenedModelFileName()));
        //        }
        //        else
        //            e.CanExecute = true; //for everything else
        //    }
        //}

        //#endregion
    }

}
