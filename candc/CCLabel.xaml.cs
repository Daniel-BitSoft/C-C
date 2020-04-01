using CC.Constants;
using CC.Models;
using CC.Providers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing.Printing;
using System.Linq;
using System.Printing;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CC
{
    /// <summary>
    /// Interaction logic for CCLabel.xaml
    /// </summary>
    public partial class CCLabel : Window
    {
        private int NumberOfLabels;
        private List<Barcode> Barcodes;
        //private TechnoRiver.SmartCodeWeb.SmartCodeWebControl barcodeGenerator;

        public CCLabel(List<Barcode> barcodes, int numberOfLabels)
        {
            InitializeComponent();
            NumberOfLabels = numberOfLabels;
            Barcodes = barcodes;


            /// TODO: PLEASE INSTALL TECHNORIVER OR ANY OTHER BARCODE GENERATOR DLL TO GENERATE BARCODE AND ASSIGN TO BarcodeImage Control. CONFIGURE BELOW:
            /// (CHANGE THIS IF OTHER BARCODE GENERATOR USED)

            //  *******************************
            //TechnoRiver.SmartCodeWeb.SmartCodeWebControl barcodeGenerator = new TechnoRiver.SmartCodeWeb.SmartCodeWebControl();
            //barcodeGenerator.RegistrationKey = "QWLB-TRWC-0017-0115";
            //barcodeGenerator.AutoSizing = TechnoRiver.SmartCodeWeb.SmartCodeWebControl.BarcodeAutoSizing.Yes;
            //barcodeGenerator.Symbology = TechnoRiver.SmartCodeWeb.SmartCodeWebControl.BarcodeSymbology.CODE128;
            //barcodeGenerator.DisplayText = TechnoRiver.SmartCodeWeb.SmartCodeWebControl.BarcodeDisplayText.Yes; 
            //barcodeGenerator.DisplayText = TechnoRiver.SmartCodeWeb.SmartCodeWebControl.BarcodeDisplayText.No;

            //  *******************************
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {

                LocalPrintServer localPrinter = new LocalPrintServer();
                var printers = localPrinter.GetPrintQueues();

                string machineName = Environment.MachineName;
                string DefaultLBLPrinter = Convert.ToString(ConfigurationManager.AppSettings[machineName]);

                if (printers.Any(a => a.FullName == DefaultLBLPrinter))
                {

                    PrintQueue pq = localPrinter.GetPrintQueue(DefaultLBLPrinter);
                    //if (!pq.IsNotAvailable) // In this if, somehow we need to find out if printer is available. It doen't work right now
                    //{
                    Print(localPrinter, pq);
                    //}
                    //else
                    //    Print(localPrinter, null);
                }
                else
                {
                    Print(localPrinter, null);
                }
            }
            catch (Exception ex)
            {
                var logNumber = Logger.Log(nameof(Window_Loaded), new Dictionary<string, object>
                {
                    { LogConsts.Exception, ex }
                });

                MessageBox.Show($"{ Messages.Exception} - log: {logNumber}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                this.Close();
            }
        }

        private void Print(LocalPrintServer localPrinter, PrintQueue pq)
        {
            PrintQueueCollection localPrinterCollection = localPrinter.GetPrintQueues();
            System.Collections.IEnumerator localPrinterEnumerator = localPrinterCollection.GetEnumerator();

            var printDlg = new PrintDialog();
            if (pq != null)
                printDlg.PrintQueue = pq;

            if (printDlg.ShowDialog() == true)
            {
                foreach (var barcode in Barcodes)
                {
                    BarcodeText.Text = $"Lot: {barcode.LotNumber} | Exp: {barcode.ExpirationDate} | {barcode.AntigenName}";

                    /// TODO: SET BARCODE GENERATOR FOR EACH BARCODE AND ASSING BarcodeImage SOURCE. CODE BELOW IS FOR TechnoRiver
                    /// (CHANGE THIS IF OTHER BARCODE GENERATOR USED)

                    //  ******************************* 
                    //barcodeGenerator.BarcodeData = barcode.LotNumber; 

                    //var barcodeImg = control.GetBitMap();
                    //var ScreenCapture = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                    //                     barcodeImg.GetHbitmap(),
                    //                     IntPtr.Zero,
                    //                     System.Windows.Int32Rect.Empty,
                    //                     BitmapSizeOptions.FromWidthAndHeight(600, 150));

                    //BarcodeImage.Source = ScreenCapture;

                    //  *******************************


                    //get selected printer capabilities
                    var capabilities = printDlg.PrintQueue.GetPrintCapabilities(printDlg.PrintTicket);

                    //get the size of the printer page
                    Size sz = new Size(capabilities.PageImageableArea.ExtentWidth, capabilities.PageImageableArea.ExtentHeight);

                    //update the layout of the visual to the printer page size.
                    PrintGrid.Measure(sz);
                    PrintGrid.Arrange(new Rect(new Point(3, -5), sz));

                    //now print the visual to printer to fit on the one page.
                    for (int i = 0; i < NumberOfLabels; i++)
                        printDlg.PrintVisual(PrintGrid, null);
                }
            }
        }
    }
}
