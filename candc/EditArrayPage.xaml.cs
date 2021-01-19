using CC.Constants;
using CC.Providers;
using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace CC
{
    /// <summary>
    /// Interaction logic for ArrayPage.xaml
    /// </summary>
    public partial class EditArrayPage : Page
    {
        Array SelectedArray = null;
        Dictionary<string, List<Antigen>> antigensGroups;
        Dictionary<string, List<Antigen>> antigensGroupsAdditions;

        List<ArrayAntigen> selectedArrayAntigens;
        List<ArrayAntigen> antigensToDelete = new List<ArrayAntigen>();
        List<Antigen> antigenList;


        const int antigenGroupCount = 6;

        public EditArrayPage()
        {
            InitializeComponent();

            for (int i = 0; i < antigenGroupCount; i++)
                AntigenGroupDropdown.Items.Add($"Group{i + 1}");
        }


        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (AntigenGroupDropdown.SelectedIndex != -1)
            {
                var selectedAntigen = AntigensGrid.SelectedItem as Antigen;

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

                if (!selectedArrayAntigens.Any(a => a.AntigenId == selectedAntigen.AntigenId && a.Group == selectedGroup))
                {
                    if (antigensGroupsAdditions.ContainsKey(selectedGroup))
                    {
                        // for existing groups with antigens in them
                        antigensGroupsAdditions[selectedGroup].Add(selectedAntigen);
                    }
                    else
                    {
                        // first time a group is being assigned antigens
                        antigensGroupsAdditions.Add(selectedGroup, new List<Antigen> { selectedAntigen });
                    }
                }

                LoadGroupGrids(selectedGroup, antigensGroups[selectedGroup]);

                // remove from deleted antigens
                var antigenToDelete = antigensToDelete.FirstOrDefault(a => a.AntigenId == selectedAntigen.AntigenId && a.Group == selectedGroup);
                if (antigenToDelete != null)
                {
                    antigensToDelete.Remove(antigenToDelete);
                }
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
            ArrayNameText.Text = SelectedArray.ArrayName;
            ArrayCodeTextbx.Text = SelectedArray.ShortArrayName;
            QRArrayCodeTextBx.Text = SelectedArray.LIMArrayNumber;
            SubArrayCheckbox.IsChecked = Convert.ToBoolean(SelectedArray.IsSubArray);

            try
            {
                // get antigens for selected array
                selectedArrayAntigens = App.ArrayProvider.GetArrayAntigens(SelectedArray.ArrayId);

                // split into groups to show in group grids
                Group1.ItemsSource = selectedArrayAntigens.Where(a => a.Group == "Group1").ToList();
                Group1.Items.Refresh();

                Group2.ItemsSource = selectedArrayAntigens.Where(a => a.Group == "Group2").ToList();
                Group2.Items.Refresh();

                Group3.ItemsSource = selectedArrayAntigens.Where(a => a.Group == "Group3").ToList();
                Group3.Items.Refresh();

                Group4.ItemsSource = selectedArrayAntigens.Where(a => a.Group == "Group4").ToList();
                Group4.Items.Refresh();

                Group5.ItemsSource = selectedArrayAntigens.Where(a => a.Group == "Group5").ToList();
                Group5.Items.Refresh();

                Group6.ItemsSource = selectedArrayAntigens.Where(a => a.Group == "Group6").ToList();
                Group6.Items.Refresh();
            }
            catch (Exception ex)
            {
                if (ex.Data.Contains("logNumber"))
                {
                    MessageBox.Show($"{ Messages.Exception} - log: {ex.Data["logNumber"]}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                NavigationService.GoBack();
            }

            if (!string.IsNullOrEmpty(SelectedArray.MasterArrayId))
            {
                MasterArrayDropdown.Text = App.dbcontext.Arrays.First(a => a.ArrayId == SelectedArray.MasterArrayId).ArrayName;

                try
                {
                    var MasterArrayAntigens = App.ArrayProvider.GetArrayAntigens(SelectedArray.MasterArrayId).Select(a => a.AntigenId);
                    var remainingAntigens = MasterArrayAntigens.Except(selectedArrayAntigens.Select(a => a.AntigenId)).ToList();

                    antigenList = App.dbcontext.Antigens.Where(a => remainingAntigens.Contains(a.AntigenId)).ToList();
                    AntigensGrid.ItemsSource = antigenList;
                    AntigensGrid.Items.Refresh();
                }
                catch (Exception ex)
                {
                    if (ex.Data.Contains("logNumber"))
                    {
                        MessageBox.Show($"{ Messages.Exception} - log: {ex.Data["logNumber"]}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }

                    NavigationService.GoBack();
                }
            }
            else
            {
                var unassignedAntigens = App.AntigensProvider.GetAntigensNotAssigned();
                if (string.IsNullOrEmpty(unassignedAntigens.ErrorMessage))
                {
                    antigenList = unassignedAntigens.Antigens;
                    AntigensGrid.ItemsSource = unassignedAntigens.Antigens;
                    AntigensGrid.Items.Refresh();
                }
                else
                {
                    MessageBox.Show(unassignedAntigens.ErrorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    if (NavigationService.CanGoBack)
                        NavigationService.GoBack();
                }
            }
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            Antigen antigen = null;
            string group = string.Empty;

            if (Group1.IsMouseOver)
            {
                antigen = Group1.SelectedItem as Antigen;
                antigensGroups[nameof(Group1)].Remove(antigen);
                group = nameof(Group1);

                Group1.ItemsSource = antigensGroups[nameof(Group1)];
                Group1.Items.Refresh();
            }
            if (Group2.IsMouseOver)
            {
                antigen = Group2.SelectedItem as Antigen;
                antigensGroups[nameof(Group2)].Remove(antigen);
                group = nameof(Group2);

                Group2.ItemsSource = antigensGroups[nameof(Group2)];
                Group2.Items.Refresh();
            }
            if (Group3.IsMouseOver)
            {
                antigen = Group3.SelectedItem as Antigen;
                antigensGroups[nameof(Group3)].Remove(antigen);
                group = nameof(Group3);

                Group3.ItemsSource = antigensGroups[nameof(Group3)];
                Group3.Items.Refresh();
            }
            if (Group4.IsMouseOver)
            {
                antigen = Group4.SelectedItem as Antigen;
                antigensGroups[nameof(Group4)].Remove(antigen);
                group = nameof(Group4);

                Group4.ItemsSource = antigensGroups[nameof(Group4)];
                Group4.Items.Refresh();
            }
            if (Group5.IsMouseOver)
            {
                antigen = Group5.SelectedItem as Antigen;
                antigensGroups[nameof(Group5)].Remove(antigen);
                group = nameof(Group5);

                Group5.ItemsSource = antigensGroups[nameof(Group5)];
                Group5.Items.Refresh();
            }
            if (Group6.IsMouseOver)
            {
                antigen = Group6.SelectedItem as Antigen;
                antigensGroups[nameof(Group6)].Remove(antigen);
                group = nameof(Group6);

                Group6.ItemsSource = antigensGroups[nameof(Group6)];
                Group6.Items.Refresh();
            }

            AddAntigenToDeleteList(new ArrayAntigen { ArrayId = SelectedArray.ArrayId, AntigenId = antigen.AntigenId, Group = group });
        }

        private void AddAntigenToDeleteList(ArrayAntigen aa)
        {
            if (selectedArrayAntigens.Any(a => a.AntigenId == aa.AntigenId && a.Group == aa.Group))
            {
                antigensToDelete.Add(aa);
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
                    var array = App.dbcontext.Arrays.FirstOrDefault(a => a.ArrayId == SelectedArray.ArrayId);
                    if (array != null)
                    {
                        array.ArrayName = ArrayNameText.Text.Trim();
                        array.ShortArrayName = ArrayCodeTextbx.Text.Trim();
                        array.LIMArrayNumber = QRArrayCodeTextBx.Text.Trim();

                        App.dbcontext.SaveChanges();
                    }
                    else
                    {
                        MessageBox.Show("Array not found", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        NavigationService.GoBack();
                    }

                    foreach (var antigen in antigensToDelete)
                    {
                        App.dbcontext.ArrayAntigens.Remove(selectedArrayAntigens.First(a => a.AntigenId == antigen.AntigenId));

                        var auditRecord = new Audit
                        {
                            RecordId = antigen.AntigenId,
                            Type = AuditTypes.ArrayAntigen.ToString(),
                            Description = AuditEvents.ArrayAntigenDeleted.ToString() + $" ArrayId: {SelectedArray.ArrayId}",
                            UpdatedBy = App.LoggedInUser.UserId,
                            UpdatedDt = DateTime.Now
                        };
                        App.dbcontext.Audits.Add(auditRecord);
                    }




                    //if (!string.IsNullOrEmpty(response))
                    //{
                    //    MessageBox.Show(response);
                    //}
                    //else
                    //{
                    //    MessageBox.Show("Successfully saved");

                    //    ResetPage();
                    //    LoadPageData();
                    //}
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
    }
}
