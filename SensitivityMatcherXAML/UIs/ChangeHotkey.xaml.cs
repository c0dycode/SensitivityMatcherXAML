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
using SensitivityMatcherXAML.Classes;

namespace SensitivityMatcherXAML.UIs
{
    /// <summary>
    /// Interaction logic for ChangeHotkey.xaml
    /// </summary>
    public partial class ChangeHotkey : Window, INotifyPropertyChanged
    {
        private Hotkey Hotkey;

        public ChangeHotkey(Hotkey hotKey)
        {
            InitializeComponent();
            this.DataContext = this;
            Hotkey = hotKey;

            this.TbKey.Text = KeyInterop.KeyFromVirtualKey((int)hotKey.KeyCode).ToString();
            this.CbCTRLModifier.IsChecked = hotKey.CTRLModifier;
            this.CbALTModifier.IsChecked = hotKey.ALTModifier;
            this.CbSHIFTModifier.IsChecked = hotKey.SHIFTModifier;

            this.TbKey.Focus();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            this.Hotkey.CTRLModifier = (bool)CbCTRLModifier.IsChecked;
            this.Hotkey.ALTModifier = (bool)CbALTModifier.IsChecked;
            this.Hotkey.SHIFTModifier = (bool)CbSHIFTModifier.IsChecked;
            this.Hotkey.UpdateModifier();
            this.Close();
        }

        private void TbKey_KeyDown(object sender, KeyEventArgs e)
        {
            Hotkey.KeyCode = (uint)KeyInterop.VirtualKeyFromKey(e.Key);
            var kc = new KeyConverter();
            this.TbKey.Text = "";
            this.TbKey.Text = e.Key.ToString();
            e.Handled = true;
        }
    }
}
