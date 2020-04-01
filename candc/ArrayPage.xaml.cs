using CC.Constants;
using CC.Models;
using CC.Providers;
using System;
using System.Collections.Generic;
using System.Configuration;
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

        private void GroupDropdown_Selected(object sender, RoutedEventArgs e)
        {

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
                var selectedGroup = AntigenGroupDropdown.SelectedItem.ToString();
                if (antigensGroups.ContainsKey(selectedGroup))
                {
                    // for existing groups with antigens in them
                    antigensGroups[selectedGroup].Add(AntigensGrid.SelectedItem as Antigen);
                }
                else
                {
                    // first time a group is being assigned antigens
                    antigensGroups.Add(selectedGroup, new List<Antigen> { AntigensGrid.SelectedItem as Antigen });
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
            ArrayNameText.Foreground = ArrayCodeTextbx.Foreground = Brushes.LightGray;
            ArrayNameText.FontStyle = ArrayCodeTextbx.FontStyle = FontStyles.Italic;

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
                string masterArrayId = null;
                var isSubArray = SubArrayCheckbox.IsChecked.HasValue && SubArrayCheckbox.IsChecked.Value;

                if (isSubArray && MasterArrayDropdown.SelectedItem != null)
                    masterArrayId = ((Array)MasterArrayDropdown.SelectedItem).ArrayId;

                var response = App.ArrayProvider.CreateArray(new Array
                {
                    ArrayName = ArrayNameText.Text.Trim(),
                    ShortArrayName = ArrayCodeTextbx.Text.Trim(),
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
                }

                ResetPage();
                LoadPageData();
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

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)
                NavigationService.GoBack();
            else
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

        private void RemoveAntigenGroup1_Click(object sender, RoutedEventArgs e)
        {
            
        }
        private void RemoveAntigenGroup2_Click(object sender, RoutedEventArgs e)
        {

        }
        private void RemoveAntigenGroup3_Click(object sender, RoutedEventArgs e)
        {

        }
        private void RemoveAntigenGroup4_Click(object sender, RoutedEventArgs e)
        {

        }
        private void RemoveAntigenGroup5_Click(object sender, RoutedEventArgs e)
        {

        }
        private void RemoveAntigenGroup6_Click(object sender, RoutedEventArgs e)
        {

        }

        private void AntigenSearchText_TextChanged(object sender, TextChangedEventArgs e)
        {
            AntigensGrid.ItemsSource = antigenList.Where(a => a.AntigenName.ToLower().Contains(AntigenSearchText.Text.ToLower()));
            AntigensGrid.Items.Refresh();
        } 
    }
}
