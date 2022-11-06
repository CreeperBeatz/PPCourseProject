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

        public Patient(string csv_line, char delimiter)
        {
            try
            {
                string[] words = csv_line.Split(delimiter);

                //validation
                if (words.Length < 4)
                {
                    throw new FormatException("CSV has less fields than expected!");
                }

                for (int i = 0; i < words.Length; i++)
                {
                    switch (i)
                    {
                        case 0:
                            ID = int.Parse(words[i]);
                            break;
                        case 1:
                            Name = words[i];
                            break;
                        case 2:
                            BirthDate = DateTime.Parse(words[i]);
                            break;
                        case 3:
                            IsCase = bool.Parse(words[i]);
                            break;
                        default:
                            throw new FormatException("CSV File has more fields than expected!");
                    }
                }
            } catch (Exception ex)
            {
                throw new FormatException("CSV File not correctly formatted!" + ex.Message);
            }
        }

        public string ToString(char delimiter)
        {
            return ID.ToString() + delimiter + Name + delimiter 
                + BirthDate.ToString() + delimiter + IsCase.ToString();
        }
    }
}
