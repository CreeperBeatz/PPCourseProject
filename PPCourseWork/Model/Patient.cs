using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PPCourseWork.Core;


namespace PPCourseWork.Model
{
    public class Patient : ObservableObject
    {
        private int _id;
        private string _name;
        private DateTime _birthDate;
        private bool _isCase;

        [Key]
        public int ID { get { return _id; } set { _id = value; OnPropertyChanged(); } }
        [StringLength(150)]
        public string Name { get { return _name; } set { _name = value; OnPropertyChanged(); } }
        public DateTime BirthDate { get { return _birthDate; } set { _birthDate = value; OnPropertyChanged(); } }
        public bool IsCase { get { return _isCase; } set { _isCase = value; OnPropertyChanged(); } }

        public Patient()
        {
            ID = -1;
            Name = "Default";
            BirthDate = new DateTime(1900, 1, 1);
            IsCase = false;
        }

        public Patient(string name, DateTime birthDate, bool isCase, int id = -1)
        {
            ID = id;
            Name = name;
            BirthDate = birthDate;
            IsCase = isCase;
        }

        public string ToString(char delimiter)
        {
            return ID.ToString() + delimiter + Name + delimiter 
                + BirthDate.ToString() + delimiter + IsCase.ToString();
        }
    }
}
