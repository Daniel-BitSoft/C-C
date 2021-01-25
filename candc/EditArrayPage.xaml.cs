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
        public Array SelectedArray = null;
        Dictionary<string, List<Antigen>> antigensGroupsAdditions = new Dictionary<string, List<Antigen>>();

        List<AntigensAssingedToArray> selectedArrayAntigens;
        List<ArrayAntigen> antigensToDelete = new List<ArrayAntigen>();
        List<Antigen> antigenList = new List<Antigen>();


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
                var antigenToDelete = antigensToDelete.FirstOrDefault(a => a.AntigenId == selectedAntigen.AntigenId);
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

            ArrayNameText.Foreground = ArrayCodeTextbx.Foreground = QRArrayCodeTextBx.Foreground = Brushes.Black;

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
                antigensGroups = new Dictionary<string, List<Antigen>>();

                // get antigens for selected array
                selectedArrayAntigens = App.dbcontext.GetAntigensAssingedToArray(SelectedArray.ArrayId).ToList();

                // split into groups to show in group grids
                var g1 = selectedArrayAntigens.Where(a => a.Group == "Group1").ToList();
                Group1.ItemsSource = g1;
                Group1.Items.Refresh();
                antigensGroups.Add("Group1", App.mapper.Map<List<Antigen>>(g1));

                var g2 = selectedArrayAntigens.Where(a => a.Group == "Group2").ToList();
                Group2.ItemsSource = g2;
                Group2.Items.Refresh();
                antigensGroups.Add("Group2", App.mapper.Map<List<Antigen>>(g2));

                var g3 = selectedArrayAntigens.Where(a => a.Group == "Group3").ToList();
                Group3.ItemsSource = g3;
                Group3.Items.Refresh();
                antigensGroups.Add("Group3", App.mapper.Map<List<Antigen>>(g3));

                var g4 = selectedArrayAntigens.Where(a => a.Group == "Group4").ToList();
                Group4.ItemsSource = g4;
                Group4.Items.Refresh();
                antigensGroups.Add("Group4", App.mapper.Map<List<Antigen>>(g4));

                var g5 = selectedArrayAntigens.Where(a => a.Group == "Group5").ToList();
                Group5.ItemsSource = g5;
                Group5.Items.Refresh();
                antigensGroups.Add("Group5", App.mapper.Map<List<Antigen>>(g5));

                var g6 = selectedArrayAntigens.Where(a => a.Group == "Group6").ToList();
                Group6.ItemsSource = g6;
                Group6.Items.Refresh();
                antigensGroups.Add("Group6", App.mapper.Map<List<Antigen>>(g6));
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

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            Antigen antigen = null;
            string group = string.Empty;

            if (Group1.IsMouseOver)
            {
                group = nameof(Group1);
                antigen = App.mapper.Map<Antigen>(Group1.SelectedItem as AntigensAssingedToArray);
                antigensGroups[group].Remove(antigensGroups[group].First(a=>a.AntigenId == antigen.AntigenId));
                
                Group1.ItemsSource = antigensGroups[group];
                Group1.Items.Refresh();

                if (antigensGroupsAdditions.ContainsKey(group))
                {
                    var removeAddition = antigensGroupsAdditions[group].FirstOrDefault(a => a.AntigenId == antigen.AntigenId);
                    if (removeAddition != null)
                    {
                        antigensGroupsAdditions[group].Remove(removeAddition);
                    }
                }
            }
            if (Group2.IsMouseOver)
            {
                group = nameof(Group2);

                antigen = App.mapper.Map<Antigen>(Group2.SelectedItem as AntigensAssingedToArray);
                antigensGroups[group].Remove(antigensGroups[group].First(a => a.AntigenId == antigen.AntigenId));

                Group2.ItemsSource = antigensGroups[group];
                Group2.Items.Refresh();

                if (antigensGroupsAdditions.ContainsKey(group))
                {
                    var removeAddition = antigensGroupsAdditions[group].FirstOrDefault(a => a.AntigenId == antigen.AntigenId);
                    if (removeAddition != null)
                    {
                        antigensGroupsAdditions[group].Remove(removeAddition);
                    }
                }
            }
            if (Group3.IsMouseOver)
            {
                group = nameof(Group3);

                antigen = App.mapper.Map<Antigen>(Group3.SelectedItem as AntigensAssingedToArray);
                antigensGroups[group].Remove(antigensGroups[group].First(a => a.AntigenId == antigen.AntigenId));
                
                Group3.ItemsSource = antigensGroups[group];
                Group3.Items.Refresh();

                if (antigensGroupsAdditions.ContainsKey(group))
                {
                    var removeAddition = antigensGroupsAdditions[group].FirstOrDefault(a => a.AntigenId == antigen.AntigenId);
                    if (removeAddition != null)
                    {
                        antigensGroupsAdditions[group].Remove(removeAddition);
                    }
                }
            }
            if (Group4.IsMouseOver)
            {
                group = nameof(Group4);

                antigen = App.mapper.Map<Antigen>(Group4.SelectedItem as AntigensAssingedToArray);
                antigensGroups[group].Remove(antigensGroups[group].First(a => a.AntigenId == antigen.AntigenId));

                Group4.ItemsSource = antigensGroups[group];
                Group4.Items.Refresh();

                if (antigensGroupsAdditions.ContainsKey(group))
                {
                    var removeAddition = antigensGroupsAdditions[group].FirstOrDefault(a => a.AntigenId == antigen.AntigenId);
                    if (removeAddition != null)
                    {
                        antigensGroupsAdditions[group].Remove(removeAddition);
                    }
                }
            }
            if (Group5.IsMouseOver)
            {
                group = nameof(Group5);

                antigen = App.mapper.Map<Antigen>(Group5.SelectedItem as AntigensAssingedToArray);
                antigensGroups[group].Remove(antigensGroups[group].First(a => a.AntigenId == antigen.AntigenId));

                Group5.ItemsSource = antigensGroups[group];
                Group5.Items.Refresh();

                if (antigensGroupsAdditions.ContainsKey(group))
                {
                    var removeAddition = antigensGroupsAdditions[group].FirstOrDefault(a => a.AntigenId == antigen.AntigenId);
                    if (removeAddition != null)
                    {
                        antigensGroupsAdditions[group].Remove(removeAddition);
                    }
                }
            }
            if (Group6.IsMouseOver)
            {
                group = nameof(Group6);

                antigen = App.mapper.Map<Antigen>(Group6.SelectedItem as AntigensAssingedToArray);
                antigensGroups[group].Remove(antigensGroups[group].First(a => a.AntigenId == antigen.AntigenId));

                Group6.ItemsSource = antigensGroups[group];
                Group6.Items.Refresh();

                if (antigensGroupsAdditions.ContainsKey(group))
                {
                    var removeAddition = antigensGroupsAdditions[group].FirstOrDefault(a => a.AntigenId == antigen.AntigenId);
                    if (removeAddition != null)
                    {
                        antigensGroupsAdditions[group].Remove(removeAddition);
                    }
                }
            }

            antigenList.Add(antigen);
            AntigensGrid.ItemsSource = antigenList;
            AntigensGrid.Items.Refresh();

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
                    }
                    else
                    {
                        MessageBox.Show("Array not found", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        NavigationService.GoBack();
                    }

                    foreach (var antigen in antigensToDelete)
                    {
                        var aa = App.dbcontext.ArrayAntigens.First(a => a.AntigenId == antigen.AntigenId && a.ArrayId == antigen.ArrayId);
                        App.dbcontext.ArrayAntigens.Remove(aa);

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

                    foreach (var group in antigensGroupsAdditions)
                    {
                        foreach (var antigen in group.Value)
                        {
                            var aa = App.dbcontext.ArrayAntigens.First(a => a.AntigenId == antigen.AntigenId && a.ArrayId == SelectedArray.ArrayId);
                            aa.Group = group.Key;
                        }
                    }

                    App.dbcontext.SaveChanges();

                    MessageBox.Show("Successfully updated");
                    NavigationService.GoBack();
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
