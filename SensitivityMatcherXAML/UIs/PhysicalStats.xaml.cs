using SensitivityMatcherXAML.Classes;
using SensitivityMatcherXAML.Helpers;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace SensitivityMatcherXAML.UIs
{
    /// <summary>
    /// Interaction logic for PhysicalStats.xaml
    /// </summary>
    public partial class PhysicalStats : Window, INotifyPropertyChanged
    {
        // Fody PropertyChanged handles this for us during compilation
        public event PropertyChangedEventHandler PropertyChanged;

        public int CPI { get; set; } = 800;

        public double Increment { get { return MainWindow.BaseSettings.Increment; } }

        public double Sens { get { return MainWindow.BaseSettings.Sens; } set { MainWindow.BaseSettings.Sens = value; } }

        public MainWindow MainWindow { get; set; }

        private bool isLocked { get; set; } = false;

        [Obsolete("Ignore the default constructor")]
        private PhysicalStats()
        {
            InitializeComponent();
        }

        public PhysicalStats(MainWindow wind)
        {
            InitializeComponent();
            this.DataContext = this;
            this.MainWindow = wind;

            UpdateCalculations(this);
        }

        private void CbLockPhysSens_Click(object sender, RoutedEventArgs e)
        {
            var cb = sender as CheckBox;
            if(cb.IsChecked.HasValue)
            {
                this.TbCmRev.IsEnabled ^= true;
                this.TbDegMM.IsEnabled ^= true;
                this.TbInRev.IsEnabled ^= true;
                this.TbMPI.IsEnabled ^= true;
                this.isLocked ^= true;
            }
        }

        private void RemoveEvents()
        {
            this.TbVirtualFactor.TextChanged -= PhysicalStats_TextChanged;
            this.TbDegMM.TextChanged -= PhysicalStats_TextChanged;
            this.TbMPI.TextChanged -= PhysicalStats_TextChanged;
            this.TbCmRev.TextChanged -= PhysicalStats_TextChanged;
            this.TbInRev.TextChanged -= PhysicalStats_TextChanged;
        }

        private void AddEvents()
        {
            this.TbVirtualFactor.TextChanged += PhysicalStats_TextChanged;
            this.TbDegMM.TextChanged += PhysicalStats_TextChanged;
            this.TbMPI.TextChanged += PhysicalStats_TextChanged;
            this.TbCmRev.TextChanged += PhysicalStats_TextChanged;
            this.TbInRev.TextChanged += PhysicalStats_TextChanged;
        }

        /// <summary>
        /// Update the UI-Text with the new Calculations
        /// </summary>
        private void UpdateCalculations(object sender)
        {
            RemoveEvents();

            if((sender as TextBox) != this.TbVirtualFactor)
                this.TbVirtualFactor.Text = Increment.ToString();

            if ((sender as TextBox) != this.TbDegMM)
                this.TbDegMM.Text = Calculations.CalculateDegreeMillimeter(CPI, Increment).ToString();

            if ((sender as TextBox) != this.TbMPI)
                this.TbMPI.Text = Calculations.CalculateMPI(CPI, Increment).ToString();

            if ((sender as TextBox) != this.TbCmRev)
                this.TbCmRev.Text = Calculations.CalculateCentimeterRev(CPI, Increment).ToString();

            if ((sender as TextBox) != this.TbInRev)
                this.TbInRev.Text = Calculations.CalculateInchRev(CPI, Increment).ToString();

            AddEvents();
        }

        private void PhysicalStats_TextChanged(object sender, TextChangedEventArgs e)
        {
            var s = sender as TextBox;
            if (string.IsNullOrEmpty(s.Text))
                return;

            if(s == this.TbPhysicalFactor)
            {
                int cpi;
                bool check = int.TryParse(s.Text, out cpi);
                if (check)
                    this.CPI = cpi;

                if (isLocked)
                {
                    double current;
                    bool check2 = double.TryParse(this.TbMPI.Text, out current);
                    if (check2)
                        MainWindow.BaseSettings.Sens = current / CPI / 60 / MainWindow.BaseSettings.Yaw;
                }
            }

            if (s == this.TbDegMM)
            {
                double current;
                bool check = double.TryParse(this.TbDegMM.Text, out current);
                if (check)
                    MainWindow.BaseSettings.Sens = Calculations.CalculateSensFromNewDegreeMillimeter(current, CPI, MainWindow.BaseSettings.Yaw);
            }

            else if (s == this.TbMPI)
            {
                double current;
                bool check = double.TryParse(this.TbMPI.Text, out current);
                if (check)
                    MainWindow.BaseSettings.Sens = Calculations.CalculateSensFromNewMPI(current, CPI, MainWindow.BaseSettings.Yaw);
            }

            else if (s == this.TbCmRev)
            {
                double current;
                bool check = double.TryParse(this.TbCmRev.Text, out current);
                if (check)
                    MainWindow.BaseSettings.Sens = Calculations.CalculateSensFromNewCentimeterRev(current, CPI, MainWindow.BaseSettings.Yaw);
            }

            else if (s == this.TbInRev)
            {
                double current;
                bool check = double.TryParse(this.TbInRev.Text, out current);
                if (check)
                    MainWindow.BaseSettings.Sens = Calculations.CalculateSensFromNewInchRev(current, CPI, MainWindow.BaseSettings.Yaw);
            }
            
            if (!isLocked)
            {
                UpdateCalculations(s);
            }
        }
    }
}
