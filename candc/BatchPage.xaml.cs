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
    /// Interaction logic for BatchPage.xaml
    /// </summary>
    public partial class BatchPage : Page
    {
        public Array array { get; set; }

        public BatchPage()
        {
            InitializeComponent();
        }

        private void BatchIdTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var batchNumberParts = BatchIdTextBox.Text.Split('|');
            if (batchNumberParts.Length == 5)
            {
                array = App.ArrayProvider.GetArrayByLIMArrayNumber(batchNumberParts[0]);

                if (array != null)
                {
                    BatchIdTextBox.Text = $"{array.ArrayName} (Pts. {batchNumberParts[1]}-{batchNumberParts[2]})";
                    RunDateTextBox.Text = batchNumberParts[3];
                    BlockNumberTextBox.Text = batchNumberParts[4];

                    var groups = App.CCProvider.ArrayAntigens.Where(a => a.ArrayId == array.ArrayId).Select(a => a.Group).Distinct().ToList();

                    AntigenGroupCombo.ItemsSource = groups;
                    AntigenGroupCombo.Items.Refresh();
                    AntigenGroupCombo.SelectedIndex = 0;
                }
                else
                {
                    MessageBox.Show($"Array was not found. Coudn't match array number '{batchNumberParts[0]}' to any of database records");
                    BatchIdTextBox.Text = string.Empty;
                }
            }
        }


        private void AntigenGroupCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AntigenGroupCombo.SelectedIndex > -1)
            {
                var selectedGroup = AntigenGroupCombo.SelectedItem as string;
                var arrayAntigens = App.CCProvider.ArrayAntigens.Where(a => a.ArrayId == array.ArrayId && a.Group == selectedGroup).ToList();

                CalibGid.ItemsSource = App.mapper.Map<List<BatchAntigen>>(arrayAntigens);
                NegGrid.ItemsSource = App.mapper.Map<List<BatchAntigen>>(arrayAntigens);
                PosGrid.ItemsSource = App.mapper.Map<List<BatchAntigen>>(arrayAntigens);

                CalibGid.Items.Refresh();
                NegGrid.Items.Refresh();
                PosGrid.Items.Refresh();
            }
        }

        private void ApplyAllBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(GenericNegLotTextBox.Text))
            {
                var negBatch = NegGrid.ItemsSource as List<BatchAntigen>;
                negBatch.ForEach(a => a.LotNumber = GenericNegLotTextBox.Text.Trim());

                NegGrid.Items.Refresh();
            }
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateForm())
            {

            }
            else
            {
                MessageBox.Show("Please complete the form before saving", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        } 

        private void SaveAndNextBtn_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateForm())
            {

            }
            else
            {
                // message
            }
        }

        private bool ValidateForm()
        {
            bool isValid = true;

            if (string.IsNullOrWhiteSpace(BatchIdTextBox.Text))
            {
                BatchIdLabel.BorderBrush = Brushes.Red;
                BatchIdLabel.BorderThickness = new Thickness(2);
                isValid = false;
            }
            else
                BatchIdLabel.BorderThickness = new Thickness(0);

            if (string.IsNullOrWhiteSpace(BlockNumberTextBox.Text))
            {
                BlockNumberLabel.BorderBrush = Brushes.Red;
                BlockNumberLabel.BorderThickness = new Thickness(2);
                isValid = false;
            }
            else
                BlockNumberLabel.BorderThickness = new Thickness(0);


            if (string.IsNullOrWhiteSpace(RunDateTextBox.Text))
            {
                RunDateLabel.BorderBrush = Brushes.Red;
                RunDateLabel.BorderThickness = new Thickness(2);
                isValid = false;
            }
            else
                RunDateLabel.BorderThickness = new Thickness(0);

            var negBatch = NegGrid.ItemsSource as List<BatchAntigen>;
            var posBatch = PosGrid.ItemsSource as List<BatchAntigen>;
            var CalibBatch = CalibGid.ItemsSource as List<BatchAntigen>;

            if (negBatch != null && negBatch.Any())
            {
                if (!AllowPartialCheckbox.IsChecked.HasValue || !AllowPartialCheckbox.IsChecked.Value)
                {
                    isValid = negBatch.All(a => !string.IsNullOrWhiteSpace(a.LotNumber)) &&
                              posBatch.All(a => !string.IsNullOrWhiteSpace(a.LotNumber)) &&
                              CalibBatch.All(a => !string.IsNullOrWhiteSpace(a.LotNumber));
                    if (!isValid)
                    {
                        for (int i = 0; i < NegGrid.Items.Count; i++)
                        {
                            if (string.IsNullOrWhiteSpace(negBatch[i].LotNumber))
                            {
                                var row = NegGrid.ItemContainerGenerator.ContainerFromIndex(i) as DataGridRow;
                                row.BorderBrush = Brushes.Red;
                                row.BorderThickness = new Thickness(2);
                            }
                            else
                            {
                                var row = NegGrid.ItemContainerGenerator.ContainerFromIndex(i) as DataGridRow;
                                row.BorderBrush = Brushes.LightGray;
                                row.BorderThickness = new Thickness(1);
                            }

                            if (string.IsNullOrWhiteSpace(posBatch[i].LotNumber))
                            {
                                var row = PosGrid.ItemContainerGenerator.ContainerFromIndex(i) as DataGridRow;
                                row.BorderBrush = Brushes.Red;
                                row.BorderThickness = new Thickness(2);
                            }
                            else
                            {
                                var row = PosGrid.ItemContainerGenerator.ContainerFromIndex(i) as DataGridRow;
                                row.BorderBrush = Brushes.LightGray;
                                row.BorderThickness = new Thickness(1);
                            }

                            if (string.IsNullOrWhiteSpace(CalibBatch[i].LotNumber))
                            {
                                var row = CalibGid.ItemContainerGenerator.ContainerFromIndex(i) as DataGridRow;
                                row.BorderBrush = Brushes.Red;
                                row.BorderThickness = new Thickness(2);
                            }
                            else
                            {
                                var row = CalibGid.ItemContainerGenerator.ContainerFromIndex(i) as DataGridRow;
                                row.BorderBrush = Brushes.LightGray;
                                row.BorderThickness = new Thickness(1);
                            }
                        }

                        MessageBox.Show("Some antigens do not have lot number. Please complete form before saving", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return isValid;
                    }
                    else
                    {
                        for (int i = 0; i < NegGrid.Items.Count; i++)
                        {
                            var negRow = NegGrid.ItemContainerGenerator.ContainerFromIndex(i) as DataGridRow;
                            negRow.BorderBrush = Brushes.LightGray;
                            negRow.BorderThickness = new Thickness(1);

                            var posRow = PosGrid.ItemContainerGenerator.ContainerFromIndex(i) as DataGridRow;
                            posRow.BorderBrush = Brushes.LightGray;
                            posRow.BorderThickness = new Thickness(1);

                            var calRow = CalibGid.ItemContainerGenerator.ContainerFromIndex(i) as DataGridRow;
                            calRow.BorderBrush = Brushes.LightGray;
                            calRow.BorderThickness = new Thickness(1);
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < NegGrid.Items.Count; i++)
                    {
                        if (string.IsNullOrWhiteSpace(negBatch[i].LotNumber) ||
                            string.IsNullOrWhiteSpace(posBatch[i].LotNumber) ||
                            string.IsNullOrWhiteSpace(CalibBatch[i].LotNumber))
                        {
                            if (!(string.IsNullOrWhiteSpace(negBatch[i].LotNumber) &&
                                  string.IsNullOrWhiteSpace(posBatch[i].LotNumber) &&
                                  string.IsNullOrWhiteSpace(CalibBatch[i].LotNumber)))
                            {
                                if (string.IsNullOrWhiteSpace(negBatch[i].LotNumber))
                                {
                                    var row = NegGrid.ItemContainerGenerator.ContainerFromIndex(i) as DataGridRow;
                                    row.BorderBrush = Brushes.Red;
                                    row.BorderThickness = new Thickness(2);
                                }
                                else
                                {
                                    var negRow = NegGrid.ItemContainerGenerator.ContainerFromIndex(i) as DataGridRow;
                                    negRow.BorderBrush = Brushes.LightGray;
                                    negRow.BorderThickness = new Thickness(1);
                                }
                                if (string.IsNullOrWhiteSpace(posBatch[i].LotNumber))
                                {
                                    var row = PosGrid.ItemContainerGenerator.ContainerFromIndex(i) as DataGridRow;
                                    row.BorderBrush = Brushes.Red;
                                    row.BorderThickness = new Thickness(2);
                                }
                                else
                                {
                                    var posRow = PosGrid.ItemContainerGenerator.ContainerFromIndex(i) as DataGridRow;
                                    posRow.BorderBrush = Brushes.LightGray;
                                    posRow.BorderThickness = new Thickness(1);
                                }
                                if (string.IsNullOrWhiteSpace(CalibBatch[i].LotNumber))
                                {
                                    var row = CalibGid.ItemContainerGenerator.ContainerFromIndex(i) as DataGridRow;
                                    row.BorderBrush = Brushes.Red;
                                    row.BorderThickness = new Thickness(2);
                                }
                                else
                                {
                                    var calRow = CalibGid.ItemContainerGenerator.ContainerFromIndex(i) as DataGridRow;
                                    calRow.BorderBrush = Brushes.LightGray;
                                    calRow.BorderThickness = new Thickness(1);
                                }

                                isValid = false;
                            }
                        }
                    }
                }
            } 

            if (isValid)
            {
                NegGrid.Items.Refresh();
                PosGrid.Items.Refresh();
                CalibGid.Items.Refresh();
            }

            return isValid;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            App.CCProvider.SetArrayAntigens();

            BatchIdTextBox.Text = RunDateTextBox.Text = BlockNumberTextBox.Text = GenericNegLotTextBox.Text = string.Empty;

            AntigenGroupCombo.ItemsSource = null;
            AntigenGroupCombo.Items.Refresh();

            AllowPartialCheckbox.IsChecked = false;

            CalibGid.ItemsSource = NegGrid.ItemsSource = PosGrid.ItemsSource = null;
            CalibGid.Items.Refresh();
            NegGrid.Items.Refresh();
            PosGrid.Items.Refresh();

            BatchIdLabel.BorderThickness = RunDateLabel.BorderThickness = BlockNumberLabel.BorderThickness = new Thickness(0);
        }

    }
}
