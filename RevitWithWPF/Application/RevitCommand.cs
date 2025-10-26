using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using RevitWithWPF.View;
using System;
using System.Windows;
using System.Windows.Interop;
using EventHandelers;
using Document = Autodesk.Revit.DB.Document;

namespace RevitWithWPF.Application
{
    [Transaction(TransactionMode.Manual)]
    public class RevitCommand : IExternalCommand
    {
        public static Document Doc { get; set; }
        public static UIDocument UiDoc { get; set; }
        public static MainWindow MW { get; set; }
        public static ExternalEvent exEvent;

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            IExternalEventHandler Handeler_Event = new MainEventHandeler();
            exEvent = ExternalEvent.Create(Handeler_Event);
            UiDoc = commandData.Application.ActiveUIDocument;
            Doc = UiDoc.Document;
            try
            {
                MW ??= new MainWindow();

                SetWindowOwner(commandData.Application, MW);

                MW.Closed += (e, s) =>
                {
                    MW = null;
                };

                MW.Show();
                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                message = ex.Message;

                return Result.Failed;
            }
        }

        private static void SetWindowOwner(UIApplication application, Window window)
        {
            HwndSource hwndSource = HwndSource.FromHwnd(application.MainWindowHandle);
            Window currentWindow = hwndSource.RootVisual as Window;
            window.Owner = currentWindow;
        }
    }
}