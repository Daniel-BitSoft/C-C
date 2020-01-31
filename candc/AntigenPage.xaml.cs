using CC.Constants;
using CC.Models;
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
    /// Interaction logic for AntigenPage.xaml
    /// </summary>
    public partial class AntigenPage : Page
    {
        CrudMode crudMode;

        public AntigenPage()
        {
            InitializeComponent();
            LoadAntigens();
        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            SetEditMode();
            NameText.Text = string.Empty;
            crudMode = CrudMode.Create;
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            SetEditMode();
            crudMode = CrudMode.Update;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            SetViewMode();
            if (crudMode == CrudMode.Create)
            {
                var responseMessage = App.AntigensProvider.CreateAntigen(
                     new Antigen
                     {
                         AntigenName = NameText.Text.Trim()
                     });

                if (string.IsNullOrEmpty(responseMessage.ErrorMessage))
                {
                    AntigensGrid.Items.Add(responseMessage.Antigen);
                    AntigensGrid.Items.Refresh();
                    MessageBox.Show(Messages.SuccessFullyCreated);
                }
                else
                    MessageBox.Show(responseMessage.ErrorMessage);
            }
            else if (crudMode == CrudMode.Update)
            {
                var responseMessage = App.AntigensProvider.UpdateAntigen(AntigensGrid.SelectedItem as Antigen);

                if (string.IsNullOrEmpty(responseMessage))
                {
                    MessageBox.Show(Messages.SuccessFullyUpdated);
                }
                else
                    MessageBox.Show(responseMessage);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            SetViewMode();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadAntigens();
        }

        private void LoadAntigens()
        {
            var response = App.AntigensProvider.GetAntigensNotAssigned();
            if (string.IsNullOrEmpty(response.ErrorMessage))
            {
                AntigensGrid.ItemsSource = response.Antigens;
                AntigensGrid.Items.Refresh();
            }
            else
                MessageBox.Show(response.ErrorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void SetEditMode()
        {
            SaveButton.Visibility = Visibility.Visible;
            CancelButton.Visibility = Visibility.Visible;

            CreateButton.Visibility = Visibility.Hidden;
            EditButton.Visibility = Visibility.Hidden;

            NameText.IsEnabled = true;
        }

        private void SetViewMode()
        {
            SaveButton.Visibility = Visibility.Hidden;
            CancelButton.Visibility = Visibility.Hidden;

            CreateButton.Visibility = Visibility.Visible;
            EditButton.Visibility = Visibility.Visible;
            NameText.IsEnabled = false;

            crudMode = CrudMode.Unknown;
        }

        private void AntigensGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Antigen selectedAntigen;
            if (AntigensGrid.SelectedItem != null)
            {
                selectedAntigen = AntigensGrid.SelectedItem as Antigen;
                NameText.Text = selectedAntigen.AntigenName;
            }
        }
    }
}
