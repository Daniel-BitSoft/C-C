using CC.Constants;
using CC.Providers;
using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using CC.Models;

namespace CC
{
    /// <summary>
    /// Interaction logic for ArrayPage.xaml
    /// </summary>
    public partial class ArrayPage : Page
    {
        Dictionary<string, List<Antigen>> antigensGroups;
        List<Antigen> antigenList;
        const int antigenGroupCount = 6;

        public ArrayPage()
        {
            InitializeComponent();

            for (int i = 0; i < antigenGroupCount; i++)
                AntigenGroupDropdown.Items.Add($"Group{i + 1}");
        }

        private void SubArrayCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            MasterArrayDropdown.IsEnabled = true;
        }

        private void SubArrayCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            MasterArrayDropdown.IsEnabled = false;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (AntigenGroupDropdown.SelectedIndex != -1)
            {
                var selectedAntigen = AntigensGrid.SelectedItem as Antigen;

                if (antigensGroups.Any(a => a.Value.Any(b => b.AntigenId == selectedAntigen.AntigenId)))
                {
                    MessageBox.Show("Antigen is already added to a group");
                    return;
                }

                var selectedGroup = AntigenGroupDropdown.SelectedItem.ToString();
                if (antigensGroups.ContainsKey(selectedGroup))
                {
                    // for existing groups with antigens in them
                    antigensGroups[selectedGroup].Add(selectedAntigen);
                }
                else
                {
                    // first time a group is being assigned antigens
                    antigensGroups.Add(selectedGroup, new List<Antigen> { selectedAntigen });
                }

                LoadGroupGrids(selectedGroup, antigensGroups[selectedGroup]);
            }
            else
            {
                MessageBox.Show("Please select group first");
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            ResetPage();
            LoadPageData();
        }

        private void ResetPage()
        {
            antigensGroups = new Dictionary<string, List<Antigen>>();

            ArrayNameText.Text = "i.e. Array 3X";
            ArrayCodeTextbx.Text = "i.e. A3X";
            QRArrayCodeTextBx.Text = "i.e. 3";

            ArrayNameText.Foreground = ArrayCodeTextbx.Foreground = QRArrayCodeTextBx.Foreground = Brushes.LightGray;
            ArrayNameText.FontStyle = ArrayCodeTextbx.FontStyle = QRArrayCodeTextBx.FontStyle = FontStyles.Italic;

            SubArrayCheckbox.IsChecked = false;
            MasterArrayDropdown.IsEnabled = false;

            MasterArrayDropdown.SelectedIndex = -1;
            AntigenGroupDropdown.SelectedIndex = -1;

            Group1.ItemsSource = null;
            Group1.Items.Refresh();
            Group2.ItemsSource = null;
            Group2.Items.Refresh();
            Group3.ItemsSource = null;
            Group3.Items.Refresh();
            Group4.ItemsSource = null;
            Group4.Items.Refresh();
            Group5.ItemsSource = null;
            Group5.Items.Refresh();
            Group6.ItemsSource = null;
            Group6.Items.Refresh();
        }

        private void LoadPageData()
        {
            var arrays = App.ArrayProvider.GetAllArrays(true);
            MasterArrayDropdown.ItemsSource = arrays;
            MasterArrayDropdown.Items.Refresh();

            var antigens = App.AntigensProvider.GetAntigensNotAssigned();
            if (string.IsNullOrEmpty(antigens.ErrorMessage))
            {
                antigenList = antigens.Antigens;
                AntigensGrid.ItemsSource = antigens.Antigens;
                AntigensGrid.Items.Refresh();
            }
            else if (antigens.Antigens == null || !antigens.Antigens.Any())
            {
                MessageBox.Show(Messages.UnassignedAntigensNotFound, "Note", MessageBoxButton.OK, MessageBoxImage.Error);
                if (NavigationService.CanGoBack)
                    NavigationService.GoBack();
            }
            else
            {
                MessageBox.Show(antigens.ErrorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                if (NavigationService.CanGoBack)
                    NavigationService.GoBack();
            }
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            if (Group1.IsMouseOver)
            {
                antigensGroups[nameof(Group1)].Remove(Group1.SelectedItem as Antigen);
                Group1.ItemsSource = antigensGroups[nameof(Group1)];
                Group1.Items.Refresh();
            }
            if (Group2.IsMouseOver)
            {
                antigensGroups[nameof(Group2)].Remove(Group2.SelectedItem as Antigen);
                Group2.ItemsSource = antigensGroups[nameof(Group2)];
                Group2.Items.Refresh();
            }
            if (Group3.IsMouseOver)
            {
                antigensGroups[nameof(Group3)].Remove(Group3.SelectedItem as Antigen);
                Group3.ItemsSource = antigensGroups[nameof(Group3)];
                Group3.Items.Refresh();
            }
            if (Group4.IsMouseOver)
            {
                antigensGroups[nameof(Group4)].Remove(Group4.SelectedItem as Antigen);
                Group4.ItemsSource = antigensGroups[nameof(Group4)];
                Group4.Items.Refresh();
            }
            if (Group5.IsMouseOver)
            {
                antigensGroups[nameof(Group5)].Remove(Group5.SelectedItem as Antigen);
                Group5.ItemsSource = antigensGroups[nameof(Group5)];
                Group5.Items.Refresh();
            }
            if (Group6.IsMouseOver)
            {
                antigensGroups[nameof(Group6)].Remove(Group6.SelectedItem as Antigen);
                Group6.ItemsSource = antigensGroups[nameof(Group6)];
                Group6.Items.Refresh();
            }
        }

        private void LoadGroupGrids(string selectedGroup, List<Antigen> antigens)
        {
            switch (selectedGroup)
            {
                case "Group1":
                    Group1.ItemsSource = antigens;
                    Group1.Items.Refresh();
                    break;
                case "Group2":
                    Group2.ItemsSource = antigens;
                    Group2.Items.Refresh();
                    break;
                case "Group3":
                    Group3.ItemsSource = antigens;
                    Group3.Items.Refresh();
                    break;
                case "Group4":
                    Group4.ItemsSource = antigens;
                    Group4.Items.Refresh();
                    break;
                case "Group5":
                    Group5.ItemsSource = antigens;
                    Group5.Items.Refresh();
                    break;
                case "Group6":
                    Group6.ItemsSource = antigens;
                    Group6.Items.Refresh();
                    break;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var errorMessage = ValidatePage();
                if (string.IsNullOrEmpty(errorMessage))
                {
                    string masterArrayId = null;
                    var isSubArray = SubArrayCheckbox.IsChecked.HasValue && SubArrayCheckbox.IsChecked.Value;

                    if (isSubArray && MasterArrayDropdown.SelectedItem != null)
                        masterArrayId = ((Array)MasterArrayDropdown.SelectedItem).ArrayId;

                    var response = App.ArrayProvider.CreateArray(new Array
                    {
                        ArrayName = ArrayNameText.Text.Trim(),
                        ShortArrayName = ArrayCodeTextbx.Text.Trim(),
                        LIMArrayNumber = QRArrayCodeTextBx.Text.Trim(),
                        IsSubArray = isSubArray,
                        MasterArrayId = masterArrayId
                    },
                    antigensGroups);

                    if (!string.IsNullOrEmpty(response))
                    {
                        MessageBox.Show(response);
                    }
                    else
                    {
                        MessageBox.Show("Successfully saved");

                        ResetPage();
                        LoadPageData();
                    }
                }
                else
                {
                    MessageBox.Show(errorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                var logNumber = Logger.Log(nameof(SaveButton_Click), new Dictionary<string, object>
                {
                    { LogConsts.Exception, ex }
                });

                MessageBox.Show($"{ Messages.Exception} - log: {logNumber}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private string ValidatePage()
        {
            var isValid = true;

            if (string.IsNullOrWhiteSpace(ArrayNameText.Text))
            {
                ArrayNameLbl.BorderBrush = Brushes.Red;
                ArrayNameLbl.BorderThickness = new Thickness(2);
                isValid = false;
            }
            else
            {
                ArrayNameLbl.BorderThickness = new Thickness(0);
            }

            if (string.IsNullOrWhiteSpace(ArrayCodeTextbx.Text))
            {
                ArrayCodeLbl.BorderBrush = Brushes.Red;
                ArrayCodeLbl.BorderThickness = new Thickness(2);
                isValid = false;
            }
            else
            {
                ArrayCodeLbl.BorderThickness = new Thickness(0);
            }

            if (string.IsNullOrWhiteSpace(QRArrayCodeTextBx.Text))
            {
                QRArrCodeLbl.BorderBrush = Brushes.Red;
                QRArrCodeLbl.BorderThickness = new Thickness(2);
                isValid = false;
            }
            else
            {
                QRArrCodeLbl.BorderThickness = new Thickness(0);
            }

            if (Convert.ToBoolean(SubArrayCheckbox.IsChecked) && MasterArrayDropdown.SelectedIndex < 0)
            {
                MasterArrayLbl.BorderBrush = Brushes.Red;
                MasterArrayLbl.BorderThickness = new Thickness(2);
                isValid = false;
            }
            else
            {
                MasterArrayLbl.BorderThickness = new Thickness(0);
            }

            var existingArrays = App.ArrayProvider.GetAllArrays(false);
            if (existingArrays.Any(a => a.ArrayName == ArrayNameText.Text.Trim()))
            {
                return "Array name must be unique";
            }
            else if (existingArrays.Any(a => a.ShortArrayName == ArrayCodeTextbx.Text.Trim()))
            {
                return "Array Code must be unique";
            }
            else if (existingArrays.Any(a => a.LIMArrayNumber == QRArrayCodeTextBx.Text.Trim()))
            {
                return "QR Array Code must be unique";
            }
            else if (!antigensGroups.Any())
            {
                return "Please add at least one antigen in one antigen group before saving";
            }
            else if (!isValid)
            {
                return "Please complete required fields marked with red before saving";
            }

            return string.Empty;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Content = null;
        }

        private void ArrayNameText_GotFocus(object sender, RoutedEventArgs e)
        {
            if (ArrayNameText.Foreground != Brushes.Black)
            {
                ArrayNameText.Text = string.Empty;
                ArrayNameText.Foreground = Brushes.Black;
                ArrayNameText.FontStyle = FontStyles.Normal;
            }
        }

        private void ArrayCodeTextbx_GotFocus(object sender, RoutedEventArgs e)
        {
            if (ArrayCodeTextbx.Foreground != Brushes.Black)
            {
                ArrayCodeTextbx.Text = string.Empty;
                ArrayCodeTextbx.Foreground = Brushes.Black;
                ArrayCodeTextbx.FontStyle = FontStyles.Normal;
            }
        }

        private void AntigenSearchText_TextChanged(object sender, TextChangedEventArgs e)
        {
            AntigensGrid.ItemsSource = antigenList.Where(a => a.AntigenName.ToLower().Contains(AntigenSearchText.Text.ToLower()));
            AntigensGrid.Items.Refresh();
        }

        private void QRArrayCodeTextBx_GotFocus(object sender, RoutedEventArgs e)
        {
            if (QRArrayCodeTextBx.Foreground != Brushes.Black)
            {
                QRArrayCodeTextBx.Text = string.Empty;
                QRArrayCodeTextBx.Foreground = Brushes.Black;
                QRArrayCodeTextBx.FontStyle = FontStyles.Normal;
            }
        }

        private void MasterArrayDropdown_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MasterArrayDropdown.SelectedItem != null && MasterArrayDropdown.SelectedIndex >= 0)
            {
                var masterArray = MasterArrayDropdown.SelectedItem as Array;
                var result = App.AntigensProvider.GetArrayAntigens(masterArray.ArrayId);

                if(string.IsNullOrEmpty(result.ErrorMessage))
                {
                    antigenList = result.Antigens;
                    AntigensGrid.ItemsSource = antigenList;
                    AntigensGrid.Items.Refresh();
                }
                else
                {
                    MessageBox.Show(result.ErrorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
             }
        }
    }
}
