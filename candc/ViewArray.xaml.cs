using CC.Constants;
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
    /// Interaction logic for ViewArray.xaml
    /// </summary>
    public partial class ViewArray : Page
    {
        public ViewArray()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var response = App.ArrayProvider.GetAllArrays(false);
                ArrayGrid.ItemsSource = response;
                ArrayGrid.Items.Refresh();
            }
            catch (Exception ex)
            {
                if (ex.Data.Contains("logNumber"))
                {
                    MessageBox.Show($"{ Messages.Exception} - log: {ex.Data["logNumber"]}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            
        }

        private void EditArrayBtn_Click(object sender, RoutedEventArgs e)
        {
            if(ArrayGrid.SelectedItem != null)
            {
                var selectedArray = ArrayGrid.SelectedItem as Array;
                var page = new EditArrayPage { SelectedArray = selectedArray };
                NavigationService.Navigate(page);
            }
        }
    }
}
