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
    /// Interaction logic for CCPage.xaml
    /// </summary>
    public partial class CCPage : Page
    {
        public CCType CCType { get; set; }

        public CCPage()
        {
            InitializeComponent();
        }

        private void ArrayListbx_Selected(object sender, RoutedEventArgs e)
        {
            var selectedArray = ArrayListbx.SelectedItem as Array;
            var groups = App.CCProvider.ArrayAntigens.Select(a => a.Group).Distinct().ToList();

            GroupListbx.ItemsSource = groups;
            GroupListbx.Items.Refresh();

            if (groups.Count == 1)
            {
                AntigenListbx.ItemsSource = App.CCProvider.ArrayAntigens.Where(a => a.ArrayId == selectedArray.ArrayId).ToList();
                AntigenListbx.Items.Refresh();
            }
            // clear rest

        }

        private void GroupListbx_Selected(object sender, RoutedEventArgs e)
        {
            var selectedArray = ArrayListbx.SelectedItem as Array;
            var selectedGroup = GroupListbx.SelectedItem as string;

            AntigenListbx.ItemsSource = App.CCProvider.ArrayAntigens.Where(a => a.ArrayId == selectedArray.ArrayId && a.Group == selectedGroup).ToList();
            AntigenListbx.Items.Refresh();

        }

        private void AntigenListbx_Selected(object sender, RoutedEventArgs e)
        {
            // clear rest
        }

        private void DilutionFactorTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            SetLotNumber();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                App.CCProvider.SetArrayAntigens();
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

        private void SetLotNumber()
        {
            if (ArrayListbx.SelectedItem != null && AntigenListbx.SelectedItem != null && !string.IsNullOrEmpty(DilutionFactorTextBox.Text))
            {
                var selectedArray = ArrayListbx.SelectedItem as Array;
                var selectedAntigen = AntigenListbx.SelectedItem as Antigen;

                string lotNumber = string.Empty;
                string LotNumberArrayname = string.Empty;

                var arraySelected = App.CCProvider.ArrayAntigens.Find(a => a.ArrayName == selectedArray.ArrayId);

                if (!string.IsNullOrEmpty(arraySelected.ArrayName))
                {
                    var masterArrayHasAntigen = App.CCProvider.ArrayAntigens
                        .Where(a => a.MasterArrayId == arraySelected.ArrayId)?
                        .Select(a => a.AntigenName)?.ToList()?.Contains(selectedAntigen.AntigenName);

                    if (masterArrayHasAntigen.HasValue && masterArrayHasAntigen.Value)
                    {
                        LotNumberArrayname = arraySelected.MasterArrayName;
                    }
                    else
                    {
                        LotNumberArrayname = arraySelected.ArrayName;
                    }
                }

                lotNumber = $"A{LotNumberArrayname}{CCType}{selectedAntigen.AntigenName}-{DilutionDatePicker.SelectedDate.Value.ToString("MMddyyyy")}";
                LotNumberBlock.Text = lotNumber;
            }
        }
    }
}
