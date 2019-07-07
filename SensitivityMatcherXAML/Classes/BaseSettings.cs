using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SensitivityMatcherXAML.Classes
{
    public class BaseSettings : INotifyPropertyChanged
    {
        public double Yaw { get; set; } = 0.022;

        public double Sens { get; set; } = 1.0;

        public double Increment { get { return Yaw * Sens; } }

        public int GPartition { get; set; } = 959;

        public int IDefaultTurnPeriod { get; set; } = 1000;

        public static int Freq { get; set; } = 60;

        public int GDelay { get; set; } = (int)Math.Round(Math.Ceiling(1000.0 / Freq));

        public int GCycle { get; set; } = 20;

        public double GResidual = 0.0;

        public double[] GBounds = { 0.0, 0.0 };

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
