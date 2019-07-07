using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SensitivityMatcherXAML.Helpers
{
    public static class Calculations
    {
        public static double CalculateDegreeMillimeter(int cpi, double yaw)
        {
            return (yaw * cpi) / 25.4;
        }

        public static double CalculateMPI(int cpi, double yaw)
        {
            return (cpi * yaw * 60);
        }

        public static double CalculateCentimeterRev(int cpi, double yaw)
        {
            return 360 / (yaw * cpi) * 2.54;
        }

        public static double CalculateInchRev(int cpi, double yaw)
        {
            return 360 / (yaw * cpi);
        }

        public static double CalculateSensFromNewMPI(double mpi, int cpi, double yaw)
        {

            return (mpi / cpi / 60) / yaw;
        }

        public static double CalculateSensFromNewCPI(double mpi, int cpi, double yaw)
        {

            return (mpi / cpi / 60) / yaw;
        }

        public static double CalculateSensFromNewDegreeMillimeter(double degreePerMillimeter, int cpi, double yaw)
        {

            return (degreePerMillimeter / cpi * 25.4) / yaw;
        }

        public static double CalculateSensFromNewCentimeterRev(double cmRev, int cpi, double yaw)
        {

            return ( 1 / cmRev / cpi * 2.54 * 360) / yaw;
        }

        public static double CalculateSensFromNewInchRev(double inchRev, int cpi, double yaw)
        {

            return (1 / inchRev / cpi * 360) / yaw;
        }
    }
}
