using SensitivityMatcherXAML.Classes;
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

using SensitivityMatcherXAML.ExtensionMethods;

namespace SensitivityMatcherXAML.UIs
{
    /// <summary>
    /// Interaction logic for HotkeyOverview.xaml
    /// </summary>
    public partial class HotkeyOverview : Window, INotifyPropertyChanged
    {
        public List<Hotkey> Hotkeys { get; set; }
        
        public HotkeyOverview(List<Hotkey> hotkeys)
        {
            InitializeComponent();
            Hotkeys = hotkeys;
            ReadHotkeys();
        }

        private void ReadHotkeys()
        {
            foreach(var hk in Hotkeys)
            {
                if (hk.Target == HotkeyTarget.ClearBounds.ToString())
                {
                    this.TbClearBounds.Text = hk.ToString();
                    this.TbClearBounds.Tag = hk.Target;
                }
                if (hk.Target == HotkeyTarget.TurnLess.ToString())
                {
                    this.TbTurnLess.Text = hk.ToString();
                    this.TbTurnLess.Tag = hk.Target;
                }
                if (hk.Target == HotkeyTarget.TurnOnce.ToString())
                {
                    this.TbTurnOnce.Text = hk.ToString();
                    this.TbTurnOnce.Tag = hk.Target;
                }
                if (hk.Target == HotkeyTarget.TurnMore.ToString())
                {
                    this.TbTurnMore.Text = hk.ToString();
                    this.TbTurnMore.Tag = hk.Target;
                }
                if (hk.Target == HotkeyTarget.TurnMultiple.ToString())
                {
                    this.TbTurnMultiple.Text = hk.ToString();
                    this.TbTurnMultiple.Tag = hk.Target;
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            Hotkeys.SaveHotkeys();
            this.Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ChangeHotkey_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var s = sender as TextBox;

            var hk = s.Text.StringToHotkey(s.Tag.ToString());
            var ch = new ChangeHotkey(hk);
            ch.ShowDialog();
            var replace = Hotkeys.First(x => x.Target == hk.Target);
            var check = Hotkeys.Remove(replace);
            Hotkeys.Add(hk);
            ReadHotkeys();
        }
    }
}
