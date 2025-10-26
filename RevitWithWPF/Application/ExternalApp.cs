using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Drawing;
using System.Drawing.Imaging;
using Autodesk.Revit.DB;
using System.Runtime.Remoting.Contexts;


namespace RevitWithWPF.Application
{

    [Transaction(TransactionMode.ReadOnly)]
    [Regeneration(RegenerationOption.Manual)]
    class ExternalApp : IExternalApplication
    {
        
        


        public Result OnStartup(UIControlledApplication application)
        {
            const string Ribbon_Tab = "Data Connector";
            const string Ribbon_Panel1 = "Import IFC";
            //const string Ribbon_Panel2 = "3D Viewer";

            #region Rubbin Tab

            ////////////////////////// Create Ribbon Tab ///////////////////////////////////////////
            //application.CreateRibbonTab(Ribbon_Tab);
            RibbonPanel panel1 = null;
            RibbonPanel panel2 = null;
            // RibbonPanel panel3 = null;

            List<RibbonPanel> panels = application.GetRibbonPanels(Ribbon_Tab);
            panel1 = panels[0];
            //panel1 = application.CreateRibbonPanel(Ribbon_Tab, Ribbon_Panel1);
            //panel2 = application.CreateRibbonPanel(Ribbon_Tab, Ribbon_Panel2);
            //panel3 = application.CreateRibbonPanel(Ribbon_Tab, Ribbon_Panel3);

            ///////////////////////////////////////////////////////

            #endregion

            #region Images

            ////////////////////////////////Rename Grids Pic //////////////////////////////////////////

            Image img = RevitWithWPF.Properties.Resources._1;
            ImageSource imgSrc = GetImageSource(img);

            //Image img2 = RevitWithWPF.Properties.Resources.etabs;
            //ImageSource imgSrc2 = GetImageSource(img2);

            //Image img3 = RevitWithWPF.Properties.Resources._3d_viewer_logo_blue;
            //ImageSource imgSrc3 = GetImageSource(img3);

            #endregion

            #region Create buttons data

            // create button data(what the button does)

            PushButtonData btnData = new PushButtonData("AddinBtn1", "From Tekla",
                Assembly.GetExecutingAssembly().Location, "RevitWithWPF.Application.RevitCommand")
            {
                ToolTip = "import IFC From Tekla",
                Image = imgSrc,
                LargeImage = imgSrc,
            };
            //PushButtonData btnData2 = new PushButtonData("AddinBtn2", "From Etabs",
            //    Assembly.GetExecutingAssembly().Location, "Etabs.Context._Context")
            //{
            //    ToolTip = "From Etabs",
            //    Image = imgSrc2,
            //    LargeImage = imgSrc2,
            //};
            //PushButtonData btnData3 = new PushButtonData("AddinBtn3", "3D Viewer",
            //    Assembly.GetExecutingAssembly().Location, "RevitWithWPF.Application.RevitCommand")
            //{
            //    ToolTip = "From Etabs",
            //    Image = imgSrc3,
            //    LargeImage = imgSrc3,
            //};


            #endregion

            #region Create button

            //add the button to the ribbon


            PushButton button1 = panel1.AddItem(btnData) as PushButton;
            button1.Enabled = true;

            //PushButton button2 = panel1.AddItem(btnData2) as PushButton;
            //button2.Enabled = true;
            //PushButton button3 = panel2.AddItem(btnData3) as PushButton;
            //button3.Enabled = true;
            return Result.Succeeded;

            #endregion

        }

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;

        }

        private BitmapSource GetImageSource(Image img)
        {
            BitmapImage bmp = new BitmapImage();
            using (MemoryStream ms = new MemoryStream())
            {
                img.Save(ms, ImageFormat.Png);
                ms.Position = 0;
                bmp.BeginInit();
                bmp.CacheOption = BitmapCacheOption.OnLoad;
                bmp.UriSource = null;
                bmp.StreamSource = ms;
                bmp.EndInit();
            }

            return bmp;
        }


    }
}