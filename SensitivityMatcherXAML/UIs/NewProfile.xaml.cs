using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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
using System.Windows.Shapes;

namespace SensitivityMatcherXAML.UIs
{
    /// <summary>
    /// Interaction logic for NewProfile.xaml
    /// </summary>
    public partial class NewProfile : Window, INotifyPropertyChanged
    {
        public string ProfileName { get; set; }

        public NewProfile()
        {
            try
            {
                InitializeComponent();
            }
            catch(Exception ex)
            {
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            ProfileName = "";
            this.DialogResult = false;
            this.Close();
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            ProfileName = tbNewProfileName.Text;
            this.DialogResult = true;
            this.Close();
        }
    }
}
