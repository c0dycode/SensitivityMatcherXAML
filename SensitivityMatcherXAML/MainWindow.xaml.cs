using SensitivityMatcherCS.Classes;
using SensitivityMatcherXAML.ExtensionMethods;
using SensitivityMatcherXAML.UIs;
using SimpleJsonWrapper;
using Squirrel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace SensitivityMatcherXAML
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public static CultureInfo ci { get; set; }

        #region Public Properties
        public List<string> Presets { get; set; }               = new List<string>();
        public List<ConfigSetting> ConfigsList { get; set; }    = new List<ConfigSetting>();

        public double Yaw { get; set; }                         = 0.022;
        public double Sens { get; set; }                        = 1.0;

        public int gPartition { get; set; }                     = 959;
        public int iDefaultTurnPeriod { get; set; }             = 1000;

        public static int Freq { get; set; }                    = 60;
        public int gDelay { get; set; }                         = (int)Math.Round(Math.Ceiling(1000.0 / Freq));
        public int gCycle { get; set; }                         = 20;
        public double gResidual                                 = 0.0;
        public double[] gBounds                                 = { 0.0, 0.0 };

        public double Increment { get { return Sens * Yaw; } }

        public bool bEnableHotkeys { get; set; }
        #endregion

        #region Private Members
        private decimal _precisePI = 3.1415926535897932384626433832795028841971693993751058209749445923078164062862089986280348253421170679821480865132823066470938446095505822317253594081284811174502841027019385211055596446229489549303819644288109756659334461284756482337867831652712019091456485669234603486104543266482133936072602491412737245870066063155881748815209209628292540917153643678925903600113305305488204665213841469519415116M;
        private int _iMode;
        private IntPtr wndHandle = IntPtr.Zero;
        private const int HOTKEY_TEST = 9539;
        private const int HOTKEY_TURNLESS = 9538;
        private const int HOTKEY_TURNMORE = 9540;
        private const int HOTKEY_CLEARBOUNDS = 9541;
        private const int HOTKEY_TURNALOT = 9542;
        #endregion

        // No need to implement this, as we're using PropertyChanged.Fody
        public event PropertyChangedEventHandler PropertyChanged;

        public MainWindow()
        {
            // Set SP to TLS 1.2, otherwise we'd need to switch to .NET 4.6.1 and that increases .exe size to >2MB instead of 800kb
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
#if DEBUG
            CheckForUpdates();
#endif

            InitializeComponent();

            // Set the current thread to use the invariant culture so that we
            // use decimal points as per the game when converting doubles.
            System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            var currentCulture = System.Threading.Thread.CurrentThread.CurrentCulture.Name;
            ci = new CultureInfo(currentCulture)
            {
                NumberFormat = { NumberDecimalSeparator = "." }
            };

            System.Threading.Thread.CurrentThread.CurrentCulture = ci;
            System.Threading.Thread.CurrentThread.CurrentUICulture = ci;
        }

        private async Task CheckForUpdates()
        {
            ReleaseEntry release = null;
#if DEBUG
            using (var updateManager = new UpdateManager(@"J:\Programmierungen\MouseEventTest\SensitivityMatcherUpdates"))
#else
            using (var updateManager = UpdateManager.GitHubUpdateManager(""))
#endif
            {

                var updateInfo = await updateManager.CheckForUpdate();
                if (updateInfo.ReleasesToApply.Count > 0)
                {
                    System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
                    FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);

                    string msg = "New version available!"   +
                                 "\n\nCurrent version: "    + updateInfo.CurrentlyInstalledVersion.Version +
                                 "\nNew version: "          + updateInfo.FutureReleaseEntry.Version +
                                 "\n\nUpdate application now?";
                    MessageBoxResult dialogResult = MessageBox.Show(msg, fvi.ProductName, MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (dialogResult == MessageBoxResult.Yes)
                    {
                        release = await updateManager.UpdateApp();
                    }
                }
                if (release != null)
                    UpdateManager.RestartApp();
            }
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            HwndSource source = PresentationSource.FromVisual(this) as HwndSource;
            source.AddHook(WndProc);
            wndHandle = new WindowInteropHelper(this).Handle;
            this.DataContext = this;
            ReadPresets();
        }

        /// <summary>
        /// Read the stored Presets and Settings from the respective files
        /// </summary>
        private void ReadPresets()
        {
            Presets.AddRange(JsonWrapper.JsonFileToList<string>("Presets.json"));
            ConfigsList.AddRange(JsonWrapper.JsonFileToList<ConfigSetting>("Settings.json"));
            this.cbPresets.SelectedIndex = Presets.IndexOf("Quake/Source");
            _iMode = 1;
        }

        private void CbPresets_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var lastSens = Sens;
            double tempsens;
            double tempyaw;
            switch (e.AddedItems[0]?.ToString())
            {
                case "Measure any game":
                    Sens = 0.022;
                    Yaw = 1;
                    ClearBounds();
                    bEnableHotkeys = true;
                    break;
                case "Add new game...":
                    var wnd = new NewProfile();
                    wnd.ShowDialog();

                    if (wnd.DialogResult == true)
                    {
                        var settings = new ConfigSetting { Name = wnd.ProfileName, Sens = this.Sens, Yaw = this.Yaw }.SaveConfigSetting(ConfigsList);
                        var profile = wnd.ProfileName;
                        Presets.Insert(Presets.Count - 1, profile);

                        this.cbPresets.Items.Refresh();
                        this.cbPresets.SelectedIndex = Presets.IndexOf(profile);
                    }
                    break;
                case "< Swap Yaw & Sens >":
                    string previous = string.Empty;
                    if(e.RemovedItems.Count != 0)
                        previous = e.RemovedItems[0].ToString();

                    tempsens = Sens;
                    tempyaw = Yaw;
                    Sens = tempyaw;
                    Yaw = tempsens;
                    this.cbPresets.SelectionChanged -= CbPresets_SelectionChanged;
                    if (previous != string.Empty)
                        this.cbPresets.SelectedIndex = Presets.IndexOf(previous);
                    else
                        this.cbPresets.SelectedIndex = 0;
                    this.cbPresets.SelectionChanged += CbPresets_SelectionChanged;
                    break;
                case "Rainbow6/Reflex":
                    var conf = ConfigsList.Where(x => x.Name == cbPresets.SelectedItem.ToString()).FirstOrDefault();
                    if (conf != null)
                    {
                        this.Sens *= Yaw / (double)((decimal)conf.Yaw / Math.Round(_precisePI, 28));
                        this.Yaw = (double)((decimal)conf.Yaw / Math.Round(_precisePI, 28));
                    }
                    bEnableHotkeys = false;
                    break;
                default:
                    conf = ConfigsList.Where(x => x.Name == cbPresets.SelectedItem.ToString()).FirstOrDefault();
                    if (conf != null)
                    {
                        this.Sens *= this.Yaw / conf.Yaw;
                        this.Yaw = conf.Yaw;
                    }
                    gResidual = 0;
                    bEnableHotkeys = false;
                    break;
            }
        }

        private bool _bHotkeysRegistered = false;
        /// <summary>
        /// Register the Hotkeys
        /// </summary>
        private void RegisterHotkeys()
        {
            const uint VK_F10 = 0x79;
            const uint VK_NUMPAD2 = 0x62;
            const uint VK_NUMPAD4 = 0x64;
            const uint VK_NUMPAD5 = 0x65;
            const uint VK_NUMPAD6 = 0x66;
            const uint VK_NUMPAD8 = 0x68;
            const uint MOD_NONE = 0x0000;
            const uint MOD_CTRL = 0x0002;

#region Basic Hotkeys
            if (!_bHotkeysRegistered)
            {
                if (!Native.RegisterHotKey(wndHandle, HOTKEY_TURNLESS, MOD_NONE, VK_NUMPAD4))
                {
                    MessageBox.Show("Failed to register Hotkey for TurnLess!");
                    return;
                }
                if (!Native.RegisterHotKey(wndHandle, HOTKEY_TEST, MOD_NONE, VK_NUMPAD5))
                {
                    MessageBox.Show("Failed to register Hotkey for TurnMouse!");
                    return;
                }
                if (!Native.RegisterHotKey(wndHandle, HOTKEY_TURNMORE, MOD_NONE, VK_NUMPAD6))
                {
                    MessageBox.Show("Failed to register Hotkey for TurnMore!");
                    return;
                }
                if (!Native.RegisterHotKey(wndHandle, HOTKEY_CLEARBOUNDS, MOD_NONE, VK_NUMPAD8))
                {
                    MessageBox.Show("Failed to register Hotkey for ClearBounds!");
                    return;
                }
                if (!Native.RegisterHotKey(wndHandle, HOTKEY_TURNALOT, MOD_NONE, VK_NUMPAD2))
                {
                    MessageBox.Show("Failed to register Hotkey for TurnMore!");
                    return;
                }
                _bHotkeysRegistered = true;
            }
#endregion
#region CTRL-Hotkeys
            //Application.AddMessageFilter(WndProc);
            //if (!Native.RegisterHotKey(this.Handle, HOTKEY_TURNLESS, MOD_CTRL, VK_NUMPAD4))
            //{
            //    MessageBox.Show("Failed to register Hotkey for TurnLess!");
            //    return;
            //}
            //if (!Native.RegisterHotKey(this.Handle, HOTKEY_TEST, MOD_CTRL, VK_NUMPAD5))
            //{
            //    MessageBox.Show("Failed to register Hotkey for TurnMouse!");
            //    return;
            //}
            //if (!Native.RegisterHotKey(this.Handle, HOTKEY_TURNMORE, MOD_CTRL, VK_NUMPAD6))
            //{
            //    MessageBox.Show("Failed to register Hotkey for TurnMore!");
            //    return;
            //}
            //if (!Native.RegisterHotKey(this.Handle, HOTKEY_CLEARBOUNDS, MOD_CTRL, VK_NUMPAD8))
            //{
            //    MessageBox.Show("Failed to register Hotkey for ClearBounds!");
            //    return;
            //}
            //if (!Native.RegisterHotKey(this.Handle, HOTKEY_TURNALOT, MOD_CTRL, VK_NUMPAD2))
            //{
            //    MessageBox.Show("Failed to register Hotkey for TurnMore!");
            //    return;
            //} 
#endregion

        }

        /// <summary>
        /// Handle the Buttonpresses and if one of our registered Hotkeys was pressed, handle it accordingly
        /// </summary>
        /// <param name="hwnd"></param>
        /// <param name="msg"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <param name="handled"></param>
        /// <returns></returns>
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            const int WM_HOTKEY = 0x0312;
            switch (msg)
            {
                case WM_HOTKEY:
                    switch (wParam.ToInt32())
                    {
                        case HOTKEY_TEST:
                            TestMouse(1);
                            break;
                        case HOTKEY_TURNLESS:
                            DecreasePolygon();
                            break;
                        case HOTKEY_TURNMORE:
                            IncreasePolygon();
                            break;
                        case HOTKEY_CLEARBOUNDS:
                            ClearBounds();
                            break;
                        case HOTKEY_TURNALOT:
                            TestMouse(gCycle);
                            break;
                    }
                    break;
            }
            return IntPtr.Zero;
        }

        private void TestMouse(int cycle)
        {
            if (_iMode == 1)
            {
                _iMode = 0;
                dynamic partition = gPartition;
                dynamic delay = gDelay;

                dynamic turn = 0.0;
                dynamic totalcount = 1;
                dynamic grandTotal = ((cycle * 360) + gResidual) / Sens;

                while (cycle > 0)
                {
                    cycle--;
                    turn = 360.0;
                    totalcount = (turn + gResidual) / (Sens);
                    totalcount = Math.Round(totalcount);
                    gResidual = (turn + gResidual) - (Sens * totalcount);

                    while (totalcount > partition)
                    {
                        if (_iMode < 0)
                            break;
                        Native.mouse_event(Native.MOUSEEVENTF_MOVE, (int)gPartition, 0, 0, (System.UIntPtr)0);
                        totalcount -= partition;
                        System.Threading.Thread.Sleep((int)gDelay);
                    }
                    if (_iMode < 0)
                        break;

                    Native.mouse_event(Native.MOUSEEVENTF_MOVE, (int)totalcount, 0, 0, (System.UIntPtr)0);
                    System.Threading.Thread.Sleep((int)gDelay);
                }
                if (_iMode == 0)
                {
                    _iMode = 1;
                    gResidual = Sens * Math.Round(grandTotal - Math.Round(grandTotal));
                }
            }
        }

        private void DecreasePolygon()
        {
            if (_iMode > 0)
                _iMode = 0;

            gResidual = 0.0;
            gBounds[0] = Sens;
            if (gBounds[1] < gBounds[0])
            {
                gBounds[1] = 0.0;
                Sens = gBounds[0] * 2;
            }
            else
                Sens = (gBounds[0] + gBounds[1]) / 2;
            _iMode = 1;
        }

        private void IncreasePolygon()
        {
            if (_iMode > 0)
                _iMode = 0;

            gResidual = 0.0;
            gBounds[1] = Sens;
            if (gBounds[1] < gBounds[0])
            {
                gBounds[0] = 0.0;
                Sens = gBounds[1] / 2;
            }
            else
                Sens = (gBounds[0] + gBounds[1]) / 2;
            _iMode = 1;
        }

        private void ClearBounds()
        {
            gResidual = 0.0;
            gBounds[0] = 0.0;
            gBounds[1] = 0.0;
            gPartition = NormalizedPartition((int)iDefaultTurnPeriod);
        }

        private int NormalizedPartition(int turntime)
        {
            var inc = Sens;
            var total = Math.Round(360 / inc);
            var slice = Math.Ceiling(total * gDelay / turntime);
            if (slice > total)
                slice = total;
            return (int)slice;
        }

        private dynamic UpdatePartition(dynamic limit, double[] bound)
        {
            dynamic error = 1;
            if (Math.Abs(bound[1]) > double.Epsilon && (bound[1] > bound[0]))
                error = GlobalUncertainty("%") / 100;
            var parti = NormalizedPartition(iDefaultTurnPeriod * error);
            if (gPartition > limit)
                gPartition = limit;
            return parti;
        }

        private dynamic GlobalUncertainty(string mode = ".")
        {
            var output = (gBounds[1] - gBounds[0]) / 2;
            if (mode == "%")
                output = (gBounds[1] - gBounds[0]) * 50 / Sens;
            else if (mode == "rev")
                output = Math.Ceiling(Sens * Sens / (gBounds[1] - gBounds[0]) / 360);
            if (output < 0 || (Math.Abs(gBounds[1]) < double.Epsilon))
                return double.PositiveInfinity;
            return output;
        }

        private void UnRegisterHotkeys()
        {
            if (_bHotkeysRegistered)
            {
                Native.UnregisterHotKey(wndHandle, HOTKEY_TURNLESS);
                Native.UnregisterHotKey(wndHandle, HOTKEY_TEST);
                Native.UnregisterHotKey(wndHandle, HOTKEY_TURNMORE);
                Native.UnregisterHotKey(wndHandle, HOTKEY_CLEARBOUNDS);
                Native.UnregisterHotKey(wndHandle, HOTKEY_TURNALOT);
                _bHotkeysRegistered = false;
            }
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            UnRegisterHotkeys();
        }

        private void BtnSaveCurrent_Click(object sender, RoutedEventArgs e)
        {
            var presets = JsonWrapper.WriteToJsonFile("Presets.json", Presets);
            bool settings = false;

            if (cbPresets.SelectedItem != null
                && 
                cbPresets.SelectedItem.ToString() != string.Empty
                &&
                cbPresets.SelectedItem.ToString() != "Measure any game"
                &&
                cbPresets.SelectedItem.ToString() != "< Swap Yaw & Sens >")
            {
                settings = new ConfigSetting { Name = cbPresets.SelectedItem.ToString(), Sens = Sens, Yaw = Yaw }.SaveConfigSetting(ConfigsList);
            }

            if (presets && settings)
                MessageBox.Show("Successfully saved Presets and Settings!");
        }

        private void BtnChangeHotkeys_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void CheckBoxHotkeyEnabled_Checked(object sender, RoutedEventArgs e)
        {
            RegisterHotkeys();
        }

        private void CheckBoxHotkeyEnabled_Unchecked(object sender, RoutedEventArgs e)
        {
            UnRegisterHotkeys();
        }
    }
}
