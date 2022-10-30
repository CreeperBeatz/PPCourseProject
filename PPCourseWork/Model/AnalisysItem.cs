using PPCourseWork.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPCourseWork.Model
{
    public class AnalisysItem : ObservableObject
    {
        private int _year;
        private int _cases;

        public int Year { get { return _year; } set { _year = value; OnPropertyChanged(); } }
        public int Cases { get { return _cases; } set { _cases = value; OnPropertyChanged(); } }

        public AnalisysItem(int year, int cases = 1)
        {
            Year = year;
            Cases = cases;
        }
    }
}
