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
    /// Interaction logic for ViewCCPage.xaml
    /// </summary>
    public partial class ViewCCPage : Page
    {
        public ViewCCPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            App.CCProvider.SetArrayAntigens();
            ArrayListbx.ItemsSource = App.ArrayProvider.GetAllArrays(true);
            ArrayListbx.Items.Refresh();
        }

        private void ArrayListbx_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ArrayListbx.SelectedIndex > -1)
            {
                var selectedArray = ArrayListbx.SelectedItem as Array;

                AntigenListbx.ItemsSource = App.CCProvider.ArrayAntigens.Where(a => a.ArrayId == selectedArray.ArrayId).ToList();
                AntigenListbx.Items.Refresh();

            }
        }

        private void AntigenListbx_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }


    }
}
