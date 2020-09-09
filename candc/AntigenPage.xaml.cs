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
        List<Antigen> antigenList;

        public AntigenPage()
        {
            InitializeComponent();
        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            SetEditMode();
            NameText.Text = string.Empty;
            crudMode = CrudMode.Create;
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (AntigensGrid.SelectedItem != null)
            {
                NameText.Text = (AntigensGrid.SelectedItem as Antigen).AntigenName;

                SetEditMode();
                crudMode = CrudMode.Update;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (crudMode == CrudMode.Create)
            {
                if (!string.IsNullOrWhiteSpace(NameText.Text))
                {
                    var existingAntigen = App.dbcontext.Antigens.FirstOrDefault(a => a.AntigenName.ToLower() == NameText.Text.Trim().ToLower());
                    if (existingAntigen == null)
                    {
                        var responseMessage = App.AntigensProvider.CreateAntigen(
                             new Antigen
                             {
                                 AntigenName = NameText.Text.Trim()
                             });

                        if (string.IsNullOrEmpty(responseMessage.ErrorMessage))
                        {
                            antigenList.Add(responseMessage.Antigen);

                            MessageBox.Show(Messages.SuccessFullyCreated);
                            SetViewMode();
                            LoadAntigens();
                        }
                        else
                            MessageBox.Show(responseMessage.ErrorMessage);
                    }
                    else
                    {
                        MessageBox.Show("Antigen already exists");
                    }
                }
            }
            else if (crudMode == CrudMode.Update)
            {
                if (!string.IsNullOrWhiteSpace(NameText.Text))
                {
                    var antigen = AntigensGrid.SelectedItem as Antigen;
                    antigen.AntigenName = NameText.Text.Trim();

                    var responseMessage = App.AntigensProvider.UpdateAntigen(antigen);

                    if (string.IsNullOrEmpty(responseMessage))
                    {
                        antigenList.First(a => a.AntigenId == antigen.AntigenId).AntigenName = antigen.AntigenName;

                        MessageBox.Show(Messages.SuccessFullyUpdated);
                        SetViewMode();
                        LoadAntigens();
                    }
                    else
                    {
                        MessageBox.Show(responseMessage);
                    }
                }
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            SetViewMode();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadAntigens();
            SaveButton.Visibility = CancelButton.Visibility = Visibility.Hidden;
        }

        private void LoadAntigens(List<Antigen> antigens = null)
        {
            if (antigens == null || !antigens.Any())
            {
                var response = App.AntigensProvider.GetAntigensNotAssigned();
                if (string.IsNullOrEmpty(response.ErrorMessage))
                { 
                    antigenList = response.Antigens;
                    AntigensGrid.ItemsSource = response.Antigens;
                    AntigensGrid.Items.Refresh();

                    AntigensGridAssigned.ItemsSource = App.AntigensProvider.GetAntigensAssignedToArray().Antigens;
                    AntigensGridAssigned.Items.Refresh();
                }
                else
                    MessageBox.Show(response.ErrorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                AntigensGrid.ItemsSource = antigens;
                AntigensGrid.Items.Refresh();
            }
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
            NameText.Text = string.Empty;
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
