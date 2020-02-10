using CC.Constants;
using CC.Models;
using CC.Providers;
using System;
using System.Collections.Generic;
using System.Configuration;
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
        const int antigenGroupCount = 6;

        public ArrayPage()
        {
            InitializeComponent();

            for (int i = 0; i <= antigenGroupCount; i++)
                AntigenGroupDropdown.Items.Add($"Group{i}");
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

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            antigensGroups = new Dictionary<string, List<Antigen>>();
            Group1.Visibility = Group1Label.Visibility =
            Group2.Visibility = Group2Label.Visibility =
            Group3.Visibility = Group3Label.Visibility =
            Group4.Visibility = Group4Label.Visibility =
            Group5.Visibility = Group5Label.Visibility =
            Group6.Visibility = Group6Label.Visibility = Visibility.Hidden;

            var arrays = App.ArrayProvider.GetAllArrays(true);
            MasterArrayDropdown.ItemsSource = arrays;
            MasterArrayDropdown.Items.Refresh();

            var antigens = App.AntigensProvider.GetAntigensNotAssigned();
            if (string.IsNullOrEmpty(antigens.ErrorMessage))
            {
                AntigensGrid.ItemsSource = antigens.Antigens;
                AntigensGrid.Items.Refresh();
            }
            else if (antigens.Antigens == null || !antigens.Antigens.Any())
            {
                MessageBox.Show(Messages.UnassignedAntigensNotFound, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (Group1.IsFocused)
            {
                antigensGroups[nameof(Group1)].Remove(Group1.SelectedItem as Antigen);
                Group1.Items.Remove(Group1.SelectedItem);
            }
            if (Group2.IsFocused)
            {
                antigensGroups[nameof(Group2)].Remove(Group2.SelectedItem as Antigen);
                Group2.Items.Remove(Group2.SelectedItem);
            }
            if (Group3.IsFocused)
            {
                antigensGroups[nameof(Group3)].Remove(Group3.SelectedItem as Antigen);
                Group3.Items.Remove(Group3.SelectedItem);
            }
            if (Group4.IsFocused)
            {
                antigensGroups[nameof(Group4)].Remove(Group4.SelectedItem as Antigen);
                Group4.Items.Remove(Group4.SelectedItem);
            }
            if (Group5.IsFocused)
            {
                antigensGroups[nameof(Group5)].Remove(Group5.SelectedItem as Antigen);
                Group5.Items.Remove(Group5.SelectedItem);
            }
            if (Group6.IsFocused)
            {
                antigensGroups[nameof(Group6)].Remove(Group6.SelectedItem as Antigen);
                Group6.Items.Remove(Group6.SelectedItem);
            }
        }

        private void LoadGroupGrids(string selectedGroup, List<Antigen> antigens)
        {
            switch (selectedGroup)
            {
                case "Group1":
                    Group1.ItemsSource = antigens;
                    Group1.Items.Refresh();
                    Group1.Visibility = Visibility.Visible;
                    Group1Label.Visibility = Visibility.Visible;
                    break;
                case "Group2":
                    Group2.ItemsSource = antigens;
                    Group2.Items.Refresh();
                    Group2.Visibility = Visibility.Visible;
                    Group2Label.Visibility = Visibility.Visible;
                    break;
                case "Group3":
                    Group3.ItemsSource = antigens;
                    Group3.Items.Refresh();
                    Group3.Visibility = Visibility.Visible;
                    Group3Label.Visibility = Visibility.Visible;
                    break;
                case "Group4":
                    Group4.ItemsSource = antigens;
                    Group4.Items.Refresh();
                    Group4.Visibility = Visibility.Visible;
                    Group4Label.Visibility = Visibility.Visible;
                    break;
                case "Group5":
                    Group5.ItemsSource = antigens;
                    Group5.Items.Refresh();
                    Group5.Visibility = Visibility.Visible;
                    Group5Label.Visibility = Visibility.Visible;
                    break;
                case "Group6":
                    Group6.ItemsSource = antigens;
                    Group6.Items.Refresh();
                    Group6.Visibility = Visibility.Visible;
                    Group6Label.Visibility = Visibility.Visible;
                    break;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var response = App.ArrayProvider.CreateArray(new Array
                {
                    ArrayName = ArrayNameText.Text.Trim(),
                    IsSubArray = SubArrayCheckbox.IsChecked.HasValue && SubArrayCheckbox.IsChecked.Value,
                    MasterArrayId = ((Array)MasterArrayDropdown.SelectedItem).ArrayId
                },
                antigensGroups);

                if (!string.IsNullOrEmpty(response))
                {
                    MessageBox.Show(response);
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

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)
                NavigationService.GoBack();
            else
                NavigationService.Content = null;
        }
    }
}
