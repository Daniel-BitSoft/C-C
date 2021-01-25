using CC.Constants;
using CC.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CC
{
    /// <summary>
    /// Interaction logic for ViewCCPage.xaml
    /// </summary>
    public partial class ViewCCPage : Page
    {
        public ViewCCPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            App.CCProvider.SetArrayAntigens();
            ArrayListbx.ItemsSource = App.ArrayProvider.GetAllArrays(true);
            ArrayListbx.Items.Refresh();
        }

        private void ArrayListbx_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ArrayListbx.SelectedIndex > -1)
            {
                var selectedArray = ArrayListbx.SelectedItem as Array;

                var assignedAntigens = new List<AntigensAssingedToArray>() { new AntigensAssingedToArray { AntigenId = "0", AntigenName = "All Antigens" } };
                assignedAntigens.AddRange(App.CCProvider.ArrayAntigens.Where(a => a.ArrayId == selectedArray.ArrayId).ToList());

                AntigenListbx.ItemsSource = assignedAntigens;
                AntigenListbx.Items.Refresh();

            }
        }

        private void AntigenListbx_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            if (ArrayListbx.SelectedValue != null && AntigenListbx.SelectedValue != null && TypeListbx.SelectedValue != null)
            {
                string ccType = null;
                string antigenId = null;

                if (AntigenListbx.SelectedValue.ToString() != "0")
                {
                    antigenId = AntigenListbx.SelectedValue.ToString();
                }

                switch (TypeListbx.SelectedValue.ToString())
                {
                    case "Negative":
                        ccType = "N";
                        break;
                    case "Positive":
                        ccType = "P";
                        break;
                    case "Calibrator":
                        ccType = "C";
                        break;

                    default:
                        ccType = null;
                        break;
                }


                var lotNumbers = App.CCProvider.GetExistingCC(ArrayListbx.SelectedValue.ToString(), antigenId, ccType);

                LotsGrid.ItemsSource = lotNumbers;
                LotsGrid.Items.Refresh();
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (LotsGrid.SelectedItem != null)
            {
                if (MessageBox.Show("Are you sure about marking as expired?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    var selectedCCId = (LotsGrid.SelectedItem as ActiveCCs).CCId;

                    try
                    {
                        var eventDt = DateTime.Now;

                        var selectedLot = App.dbcontext.CalibControls.FirstOrDefault(a => a.CCId == selectedCCId);
                        selectedLot.ExpirationDate = eventDt;
                        selectedLot.UpdatedDt = eventDt;
                        selectedLot.UpdatedBy = App.LoggedInUser.UserId;

                        string type = string.Empty;

                        switch (selectedLot.Type)
                        {
                            case "C":
                                type = AuditTypes.Control.ToString();
                                break;
                            case "P":
                                type = AuditTypes.PosCalibrator.ToString();
                                break;
                            case "N":
                                type = AuditTypes.NegCalibrator.ToString();
                                break;
                        }

                        var auditRecord = new Audit
                        {
                            RecordId = selectedCCId,
                            Type = type,
                            Description = AuditEvents.CCExpired.ToString(),
                            UpdatedBy = App.LoggedInUser.UserId,
                            UpdatedDt = eventDt
                        };
                        App.dbcontext.Audits.Add(auditRecord);

                        App.dbcontext.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        var logNumber = Logger.Log("MarkCCExpired", new Dictionary<string, object>
                        {
                            { LogConsts.Exception, ex }
                        });

                        MessageBox.Show($"{ Messages.Exception} - log: {logNumber}");
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select from table above before clicking mark as expired");
            }
        }
    }
}
