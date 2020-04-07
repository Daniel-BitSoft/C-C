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
        public BatchPage()
        {
            InitializeComponent();
        }

        private void BatchIdTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var batchNumberParts = BatchIdTextBox.Text.Split('|');
            if (batchNumberParts.Length == 5)
            {
                var array = App.ArrayProvider.GetArrayByLIMArrayNumber(batchNumberParts[0]);

                if (array != null)
                {
                    BatchIdTextBox.Text = $"{array.ArrayName} (Pts. {batchNumberParts[1]}-{batchNumberParts[2]})";
                    RunDateTextBox.Text = batchNumberParts[3];
                    BlockNumberTextBox.Text = batchNumberParts[4];
                }
                else
                {
                    MessageBox.Show($"Array was not found. Coudn't match array number '{batchNumberParts[0]}' to any of database records");
                    BatchIdTextBox.Text = string.Empty;
                }
            }
            else
            {
                MessageBox.Show($"Batch Id is not in correct format. Batch Id entered is: {BatchIdTextBox.Text}");
                BatchIdTextBox.Text = string.Empty;
            }
        }
    }
}
