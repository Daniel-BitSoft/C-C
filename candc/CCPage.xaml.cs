using CC.Constants;
using CC.Models;
using CC.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace CC
{
    /// <summary>
    /// Interaction logic for CCPage.xaml
    /// </summary>
    public partial class CCPage : Page
    {
        private readonly Regex _regex = new Regex("[^0-9.-]+");

        public List<SerumReference> SerumReferences { get; set; }
        public List<CalibControl> CCList { get; set; }

        public CCPage()
        {
            InitializeComponent();
        }

        private void ArrayListbx_Selected(object sender, RoutedEventArgs e)
        {
            if (ArrayListbx.SelectedIndex > -1)
            {
                var selectedArray = ArrayListbx.SelectedItem as Array;
                var groups = App.CCProvider.ArrayAntigens.Where(a => a.ArrayId == selectedArray.ArrayId).Select(a => a.Group).Distinct().OrderBy(a => a).ToList();

                GroupListbx.ItemsSource = groups;
                GroupListbx.Items.Refresh();
                GroupListbx.SelectedIndex = -1;

                if (groups.Count == 1)
                {
                    AntigenListbx.ItemsSource = App.CCProvider.ArrayAntigens.Where(a => a.ArrayId == selectedArray.ArrayId).ToList();
                    AntigenListbx.Items.Refresh();
                }
                else
                {
                    AntigenListbx.SelectedIndex = -1;
                    AntigenListbx.ItemsSource = null;
                    AntigenListbx.Items.Refresh();
                }

                AntigensGrid.ItemsSource = null;
                AntigensGrid.Items.Refresh();

                // clear rest
            }
        }

        private void GroupListbx_Selected(object sender, RoutedEventArgs e)
        {
            if (GroupListbx.SelectedIndex > -1)
            {
                var selectedArray = ArrayListbx.SelectedItem as Array;
                var selectedGroup = GroupListbx.SelectedItem as string;

                if (App.ccPageType == CCType.N)
                {
                    var arrayAntigens = App.CCProvider.ArrayAntigens.Where(a => a.ArrayId == selectedArray.ArrayId && a.Group == selectedGroup).ToList();

                    AntigensGrid.ItemsSource = App.mapper.Map<List<AntigenRange>>(arrayAntigens);
                    AntigensGrid.Items.Refresh();
                }
                else
                {
                    AntigenListbx.ItemsSource = App.CCProvider.ArrayAntigens.Where(a => a.ArrayId == selectedArray.ArrayId && a.Group == selectedGroup).ToList();
                    AntigenListbx.Items.Refresh();
                }
            }
        }

        private void AntigenListbx_Selected(object sender, RoutedEventArgs e)
        {
            if (AntigenListbx.SelectedIndex > -1)
            {
                AntigensGrid.ItemsSource = new List<AntigenRange> { App.mapper.Map<AntigenRange>(AntigenListbx.SelectedItem as AntigensAssingedToArray) };
                AntigensGrid.Items.Refresh();
                 
            }
        }

        private void DilutionDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DilutionDatePicker.SelectedDate != null)
            {
                ExpirationDateTextBox.Text = DilutionDatePicker.SelectedDate.Value.AddDays(90).ToShortDateString();

                if (ValidatePage(true))
                {
                    SetLotNumber();
                    RemoveErrorBorders();
                }
                else
                {
                    DilutionDatePicker.SelectedDate = null;
                    ExpirationDateTextBox.Text = string.Empty;
                }
            }
        }

        private bool ValidateGetMinMax(List<string> errorMessages)
        {
            if (AntigensGrid.ItemsSource != null)
            {
                bool isValid = true;
                bool completeError = false;
                bool minMaxError = false;

                var antigenRanges = AntigensGrid.ItemsSource as List<AntigenRange>;
                for (int i = 0; i < antigenRanges.Count; i++)
                {
                    // min
                    ContentPresenter minCell = AntigensGrid.Columns[1].GetCellContent(
                        (DataGridRow)AntigensGrid.ItemContainerGenerator.ContainerFromIndex(i)) as ContentPresenter;

                    var minTextBox = (minCell.ContentTemplate.FindName("MinCol", minCell) as TextBox);
                    antigenRanges[i].Min = minTextBox.Text;

                    // max
                    ContentPresenter maxCell = AntigensGrid.Columns[2].GetCellContent(
                      (DataGridRow)AntigensGrid.ItemContainerGenerator.ContainerFromIndex(i)) as ContentPresenter;

                    var maxTextBox = (maxCell.ContentTemplate.FindName("MaxCol", maxCell) as TextBox);
                    antigenRanges[i].Max = maxTextBox.Text;

                    if (string.IsNullOrEmpty(antigenRanges[i].Min) || string.IsNullOrEmpty(antigenRanges[i].Max))
                    {
                        minTextBox.BorderThickness = maxTextBox.BorderThickness = new Thickness(2);
                        minTextBox.BorderBrush = maxTextBox.BorderBrush = Brushes.Red;
                        isValid = false;
                        completeError = true;
                    }
                    else if (Convert.ToDouble(antigenRanges[i].Min) > Convert.ToDouble(antigenRanges[i].Max))
                    {
                        minTextBox.BorderThickness = maxTextBox.BorderThickness = new Thickness(2);
                        minTextBox.BorderBrush = maxTextBox.BorderBrush = Brushes.Red;
                        isValid = false;
                        minMaxError = true;
                    }
                    else
                    {
                        minTextBox.BorderThickness = maxTextBox.BorderThickness = new Thickness(0);
                    }
                }

                if (completeError)
                    errorMessages.Add("Please complete all min/max data");
                if (minMaxError)
                    errorMessages.Add("Min value needs to be smaller than max");

                return isValid;
            }
            else
                return true;
        }

        private void ClearPage()
        {
            SerumReferences = new List<SerumReference>();
            CCList = new List<CalibControl>();

            AntigensGrid.ItemsSource = null;
            AntigensGrid.Items.Refresh();

            SerumRefGrid.ItemsSource = null;
            SerumRefGrid.Items.Refresh();

            ArrayListbx.SelectedIndex = -1;

            AntigenListbx.SelectedIndex = -1;
            AntigenListbx.ItemsSource = null;
            AntigenListbx.Items.Refresh();

            GroupListbx.SelectedIndex = -1;
            GroupListbx.ItemsSource = null;
            GroupListbx.Items.Refresh();

            SerumTextBox.Text =
                DilutionFactorTextBox.Text =
                ExpirationDateTextBox.Text =
                QuantityLabelTextBox.Text =
                LotNumberBlock.Text = string.Empty;

            DilutionDatePicker.SelectedDate = null;
        }

        private void ClearNext()
        {
            SerumReferences = new List<SerumReference>();
            CCList = new List<CalibControl>();

            SerumRefGrid.ItemsSource = null;
            SerumRefGrid.Items.Refresh();

            SerumTextBox.Text =
                DilutionFactorTextBox.Text =
                ExpirationDateTextBox.Text =
                QuantityLabelTextBox.Text =
                LotNumberBlock.Text = string.Empty;

            DilutionDatePicker.SelectedDate = null;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                ClearPage();

                ArrayListbx.TabIndex = 1; GroupListbx.TabIndex = 2;

                if (App.ccPageType == CCType.N)
                {
                    AntigenListbx.Visibility = AntigenLabel.Visibility = Visibility.Hidden;
                    PageNameLabel.Content = "Create Negative Control";

                    //AntigenListbx.TabIndex = -1; AntigensGrid.TabIndex = 3; SerumTextBox.TabIndex = 4; AddSerumBtn.TabIndex = 5; SerumRefGrid.TabIndex = 6;
                    //RemoveSerum.TabIndex = 7; DilutionFactorTextBox.TabIndex = 8; DilutionDatePicker.TabIndex = 9; ExpirationDateTextBox.TabIndex = 10;
                    //QuantityLabelTextBox.TabIndex = 11;
                }
                else
                {
                    //AntigenListbx.TabIndex = 3; AntigensGrid.TabIndex = 4; SerumTextBox.TabIndex = 5; AddSerumBtn.TabIndex = 6; SerumRefGrid.TabIndex = 7;
                    //RemoveSerum.TabIndex = 8; DilutionFactorTextBox.TabIndex = 9; DilutionDatePicker.TabIndex = 10; ExpirationDateTextBox.TabIndex = 11;
                    //QuantityLabelTextBox.TabIndex = 12;

                    AntigenListbx.Visibility = AntigenLabel.Visibility = Visibility.Visible;

                    if (App.ccPageType == CCType.C)
                        PageNameLabel.Content = "Create Calibrator";
                    else
                        PageNameLabel.Content = "Create Positive Control";
                }

                App.CCProvider.SetArrayAntigens();
                ArrayListbx.ItemsSource = App.ArrayProvider.GetAllArrays(true);
                ArrayListbx.Items.Refresh();
            }
            catch (Exception ex)
            {
                var logNumber = Logger.Log(nameof(Page_Loaded), new Dictionary<string, object>
                {
                    { LogConsts.Exception, ex }
                });

                MessageBox.Show($"{ Messages.Exception} - log: {logNumber}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private string SetLotNumber()
        {
            string lotNumber = string.Empty;

            if (ArrayListbx.SelectedItem != null
                && (AntigenListbx.SelectedItem != null || App.ccPageType == CCType.N)
                && !string.IsNullOrEmpty(DilutionFactorTextBox.Text)
                && DilutionDatePicker.SelectedDate.HasValue)
            {
                var selectedArray = ArrayListbx.SelectedItem as Array;

                // Un comment below if sub arrays are allowed in this page
                //string LotNumberArrayname = string.Empty;
                //if (App.ccPageType != CCType.N)
                //{
                //    var selectedAntigen = AntigenListbx.SelectedItem as AntigensAssingedToArray;
                //    var arraySelected = App.CCProvider.ArrayAntigens.Find(a => a.ArrayId == selectedArray.ArrayId);

                //    if (!string.IsNullOrEmpty(arraySelected.ArrayName))
                //    {
                //        var masterArrayHasAntigen = App.CCProvider.ArrayAntigens
                //            .Where(a => a.MasterArrayId == arraySelected.ArrayId)?
                //            .Select(a => a.AntigenName)?.ToList()?.Contains(selectedAntigen.AntigenName);

                //        if (masterArrayHasAntigen.HasValue && masterArrayHasAntigen.Value)
                //        {
                //            LotNumberArrayname = arraySelected.MasterArrayShortName;
                //        }
                //        else
                //        {
                //            LotNumberArrayname = arraySelected.ShortArrayName;
                //        }
                //    }
                //}
                string antigenName = null;

                if (AntigensGrid.Items != null && AntigensGrid.Items.Count > 0)
                {
                    var AntgnRng = AntigensGrid.ItemsSource as List<AntigenRange>;
                    var AntigenRangesSet = AntgnRng.Where(a => !string.IsNullOrWhiteSpace(a.Max) && !string.IsNullOrWhiteSpace(a.Min)).ToList();

                    if (AntigenRangesSet.Count == 1)
                    {
                        antigenName = AntigenRangesSet.First().AntigenName;
                    }

                    if (App.ccPageType == CCType.N && AntigenRangesSet.Count > 1)
                        lotNumber = $"{selectedArray.ShortArrayName}{App.ccPageType}-{DilutionDatePicker.SelectedDate.Value.ToString("MMddyyyy")}";
                    else if (AntigenRangesSet.Count > 0)
                        lotNumber = $"{selectedArray.ShortArrayName}{App.ccPageType}{antigenName}-{DilutionDatePicker.SelectedDate.Value.ToString("MMddyyyy")}";

                    LotNumberBlock.Text = lotNumber;
                }
            }

            return lotNumber;
        }

        private void AddSerumBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(SerumTextBox.Text))
            {
                if (!SerumReferences.Any(a => a.ReferenceNumber == SerumTextBox.Text.Trim()))
                {
                    SerumReferences.Add(new SerumReference { ReferenceNumber = SerumTextBox.Text.Trim() });
                    SerumRefGrid.ItemsSource = SerumReferences;
                    SerumRefGrid.Items.Refresh();
                }
                else
                {
                    MessageBox.Show("This reference number is already added");
                }
                SerumTextBox.Text = string.Empty;
            }
        }

        private void RemoveSerum_Click(object sender, RoutedEventArgs e)
        {
            SerumReferences.Remove(SerumRefGrid.SelectedItem as SerumReference);
            SerumRefGrid.ItemsSource = SerumReferences;
            SerumRefGrid.Items.Refresh();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidatePage())
                return;

            var isSaved = SaveCCs();
            if (isSaved)
            {
                RemoveErrorBorders();
                ClearPage();
            }
        }

        private void SaveAndNextButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidatePage())
                return;

            SaveCCs();
            RemoveErrorBorders();
            if (App.ccPageType != CCType.N)
            {
                if (AntigenListbx.SelectedIndex == AntigenListbx.Items.Count - 1) // last antigen selected
                {
                    if (GroupListbx.SelectedIndex < GroupListbx.Items.Count - 1) // there are still more groups to select from
                    {
                        GroupListbx.SelectedIndex++;

                        ClearNext();
                        AntigensGrid.Focus();

                    }
                    else
                    {
                        MessageBox.Show("That was the last antigen in the Array list");
                        ClearPage();
                    }
                }
                else
                {
                    AntigenListbx.SelectedIndex++;
                    //AntigenListbx.SelectedItem = AntigenListbx.Items[AntigenListbx.SelectedIndex + 1];

                    ClearNext();
                    AntigensGrid.Focus();
                }
            }
            else
            {
                if (GroupListbx.SelectedIndex < GroupListbx.Items.Count - 1) // there are still more groups to select from
                {
                    GroupListbx.SelectedIndex++;

                    ClearNext();
                    AntigensGrid.Focus();

                }
                else
                {
                    MessageBox.Show("That was the last antigen in the Array list");
                    ClearPage();
                }
            }
        }

        private bool SaveCCs()
        {
            try
            {
                var antigenRanges = AntigensGrid.ItemsSource as List<AntigenRange>;

                var activeCCs = App.CCProvider.GetExistingCC(
                    ArrayListbx.SelectedValue.ToString(),
                     null,
                     App.ccPageType.ToString());

                if (activeCCs != null && activeCCs.Any())
                {
                    var antigensIds = activeCCs.Where(a => a.DilutionDate == DilutionDatePicker.SelectedDate.Value.Date).Select(a => a.AntigenId).ToList();
                    var duplicatedAntigens = antigenRanges.Where(a => antigensIds.Contains(a.AntigenId)).ToList();

                    if (duplicatedAntigens != null && duplicatedAntigens.Count > 1)
                    {
                        MessageBox.Show($"Following antigens already have records that are not expired: {string.Join(",", duplicatedAntigens.Select(a => a.AntigenName))}", "Error");
                        return false;
                    }
                    else if (duplicatedAntigens != null && duplicatedAntigens.Count == 1)
                    {
                        MessageBox.Show($"Selected antigen already has record that is not expired: {duplicatedAntigens.First().AntigenName}", "Error");
                        return false;
                    }
                }

                var lotNumber = SetLotNumber();
                var CalibControls = new List<CalibControl>();

                var dilutionFactor = double.Parse(DilutionFactorTextBox.Text);
                var barcodes = new List<Barcode>();

                foreach (var antigenRange in antigenRanges)
                {
                    CalibControls.Add(new CalibControl
                    {
                        AntigenGroup = GroupListbx.Text,
                        ArrayId = ArrayListbx.SelectedValue.ToString(),
                        AntigenId = antigenRange.AntigenId,
                        DilutionDate = DilutionDatePicker.SelectedDate.Value.Date,
                        DilutionFactor = dilutionFactor.ToString(),
                        ExpirationDate = Convert.ToDateTime(ExpirationDateTextBox.Text),
                        LotNumber = lotNumber,
                        Min = Convert.ToDouble(antigenRange.Min),
                        Max = Convert.ToDouble(antigenRange.Max),
                        Serum = string.Join(",", SerumReferences.Select(a => a.ReferenceNumber)),
                        Type = App.ccPageType.ToString()
                    });

                    barcodes.Add(new Barcode
                    {
                        AntigenName = antigenRange.AntigenName,
                        ExpirationDate = ExpirationDateTextBox.Text,
                        LotNumber = lotNumber
                    });
                }


                App.CCProvider.CreateCalibControl(CalibControls);
                MessageBox.Show("Successfully Saved. Click OK to start printing labels");

                PrintDialog printDialog = null;
                foreach (var antigenRange in antigenRanges)
                {
                    var printPage = new CCLabel();
                    if (printDialog != null)
                        printPage.printDlg = printDialog;

                    printPage.Barcode = new Barcode
                    {
                        AntigenName = antigenRange.AntigenName,
                        ExpirationDate = ExpirationDateTextBox.Text,
                        LotNumber = lotNumber
                    };
                    printPage.NumberOfLabels = Convert.ToInt32(QuantityLabelTextBox.Text);
                    printPage.ShowDialog();

                    printDialog = printPage.printDlg;
                }

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ Messages.Exception} - log: {ex.Data["logNumber"]}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        private bool ValidatePage(bool skipPrintCountLbl = false)
        {
            RemoveErrorBorders();

            bool isValid = true;
            List<string> errorMessages = new List<string>();

            if (ArrayListbx.SelectedIndex == -1)
            {
                ArrayLabel.BorderBrush = Brushes.Red;
                ArrayLabel.BorderThickness = new Thickness(2);
                isValid = false;
            }
            if (GroupListbx.SelectedIndex == -1)
            {
                GroupLabel.BorderBrush = Brushes.Red;
                GroupLabel.BorderThickness = new Thickness(2);
                isValid = false;
            }
            if (App.ccPageType != CCType.N && AntigenListbx.SelectedIndex == -1)
            {
                AntigenLabel.BorderBrush = Brushes.Red;
                AntigenLabel.BorderThickness = new Thickness(2);
                isValid = false;
            }
            if (!SerumReferences.Any())
            {
                SerumRefLabel.BorderBrush = Brushes.Red;
                SerumRefLabel.BorderThickness = new Thickness(2);
                isValid = false;
            }
            if (string.IsNullOrWhiteSpace(DilutionFactorTextBox.Text))
            {
                DilutionLabel.BorderBrush = Brushes.Red;
                DilutionLabel.BorderThickness = new Thickness(2);
                isValid = false;
            }
            if (!DilutionDatePicker.SelectedDate.HasValue)
            {
                DateLabel.BorderBrush = Brushes.Red;
                DateLabel.BorderThickness = new Thickness(2);
                isValid = false;
            }
            if (string.IsNullOrWhiteSpace(ExpirationDateTextBox.Text))
            {
                ExpirationLabel.BorderBrush = Brushes.Red;
                ExpirationLabel.BorderThickness = new Thickness(2);
                isValid = false;
            }
            if (!skipPrintCountLbl && string.IsNullOrWhiteSpace(QuantityLabelTextBox.Text))
            {
                QtyLabel.BorderBrush = Brushes.Red;
                QtyLabel.BorderThickness = new Thickness(2);
                isValid = false;
            }

            if (AntigensGrid.ItemsSource == null)
            {
                AntigensGrid.BorderBrush = Brushes.Red;
                AntigensGrid.BorderThickness = new Thickness(2);
                isValid = false;
            }

            if (!isValid)
                errorMessages.Add("Please complete missing fields");

            if (!ValidateGetMinMax(errorMessages))
                isValid = false;

            if (!isValid)
            {
                ErrorMessages.Text = " * " + string.Join("\r\n * ", errorMessages);
            }
            else
            {
                ErrorMessages.Text = string.Empty;
            }

            return isValid;
        }

        private void RemoveErrorBorders()
        {
            ArrayLabel.BorderThickness =
            GroupLabel.BorderThickness =
            AntigenLabel.BorderThickness =
            SerumRefLabel.BorderThickness =
            DilutionLabel.BorderThickness =
            DateLabel.BorderThickness =
            AntigensGrid.BorderThickness =
            ExpirationLabel.BorderThickness =
            QtyLabel.BorderThickness = new Thickness(0);

            ErrorMessages.Text = string.Empty;
        }

        private void QuantityLabelTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var value = QuantityLabelTextBox.Text.Trim();
            if (!string.IsNullOrEmpty(value) && int.TryParse(value, out int qty))
            {
                if (qty < 0)
                    QuantityLabelTextBox.Text = string.Empty;
            }
            else
                QuantityLabelTextBox.Text = string.Empty;
        }


        private void DilutionFactorTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            ValidateNumbers(5, sender, e);
        }

        private void MinCol_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            ValidateNumbers(10, sender, e);
        }

        private void MaxCol_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            ValidateNumbers(10, sender, e);
        }

        private void ValidateNumbers(int maxLength, object sender, TextCompositionEventArgs e)
        {
            var txt = (sender as TextBox).Text;

            if (txt.StartsWith("-"))
            {
                if ((txt.Length == maxLength + 1))
                {
                    e.Handled = true;
                    return;
                }
            }
            else if (txt.Length == maxLength)
            {
                e.Handled = true;
                return;
            }

            if (char.TryParse(e.Text, out char value))
            {
                if ((!char.IsControl(value) && !char.IsDigit(value) &&
                    (value != '.')))
                {
                    e.Handled = true;
                }

                if (value == '-' && txt.Length == 0)
                {
                    e.Handled = false;
                }

                // only allow one decimal point
                if ((value == '.') && (txt.IndexOf('.') > -1))
                {
                    e.Handled = true;
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Content = null;
        }

        private void GroupListbx_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (AntigensGrid.ItemsSource != null)
            {
                AntigensGrid.Focus();
            }
        }

        private void ExistButton_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            ArrayListbx.Focus();
        } 
    }
}
