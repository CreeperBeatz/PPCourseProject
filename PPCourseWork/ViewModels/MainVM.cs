using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using PPCourseWork.Core;
using PPCourseWork.Model;
using PPCourseWork.ViewModels.Controls;
using PPCourseWork.DAL;
using System.Threading;
using AsyncAwaitBestPractices.MVVM;

namespace PPCourseWork.ViewModels
{
    public class MainVM : ObservableObject
    {
        #region CommandDefinitions
        public ICommand SearchByIdCommand { get; set; }
        public ICommand SearchByNameCommand { get; set; }
        public IAsyncCommand LoadAllPatientsCommand { get; private set; }
        public ICommand MakeAnalisysCommand { get; set; }
        public ICommand LoadCSVCommand { get; set; }
        public ICommand ExportCSVCommand { get; set; }
        public ICommand AddUserCommand { get; set; }
        public ICommand DeleteUserCommand { get; set; }
        #endregion

        #region VariableDefinitions
        //Search screen
        private int _searchID;
        private string _searchName;
        private ObservableCollection<Patient> _patientSearchResults;

        //Add Del User screen
        private string _addName;
        private DateTime _addBirthDate;
        private bool _addIsCase;
        private int _delID;

        //DB
        private PatientService _patientService;
        #endregion

        #region PropertyDefinitions
        //Search screen
        public int SearchID { get { return _searchID; } set { _searchID = value; OnPropertyChanged(); } }
        public string SearchName { get { return _searchName; } set { _searchName = value; OnPropertyChanged(); } }
        public ObservableCollection<Patient> PatientSearchResults { get { return _patientSearchResults; } set { _patientSearchResults = value; OnPropertyChanged(); } }

        //Add Del User Screen
        public string AddName { get { return _addName; } set { _addName = value; OnPropertyChanged(); } }
        public DateTime AddBirthDate { get { return _addBirthDate; } set { _addBirthDate = value; OnPropertyChanged(); } }
        public bool AddIsCase { get { return _addIsCase; } set { _addIsCase = value; OnPropertyChanged(); } }
        public int DelID { get { return _delID; }  set { _delID = value; OnPropertyChanged(); } }

        //Async 

        #endregion
        public MainVM()
        {
            this._patientService = new PatientService(new DatabaseContext());
            this.PatientSearchResults = new ObservableCollection<Patient>();
            this.SearchByIdCommand = new SearchByIDCommand(this);
            this.SearchByNameCommand = new SearchByNameCommand(this);
            this.LoadAllPatientsCommand = new AsyncCommand(LoadAllPatients);
            this.MakeAnalisysCommand = new MakeAnalisysCommand(this);
            this.AddUserCommand = new AddUserCommand(this);
            this.DeleteUserCommand = new DeleteUserCommand(this);
            this.LoadCSVCommand = new LoadCSVCommand(this);
            this.ExportCSVCommand = new ExportCSVCommand(this);
        }

        #region Commands
        public async Task SearchByID()
        {
            //test
            Patient patient1 = new Patient("David", new DateTime(2000, 4, 1), true);
            Patient patient2 = new Patient("Lucy", new DateTime(2001, 5, 23), false);
            PatientSearchResults.Add(patient1);
            PatientSearchResults.Add(patient2);
        }
        public async Task SearchByName()
        {

        }
        public async Task LoadAllPatients()
        {
            try
            {
                PatientSearchResults = new ObservableCollection<Patient>(await _patientService.GetAllPatientsAsync());
            }
            catch
            {

            }
        }
        public async Task AddUser()
        {
            Patient patient = new Patient(AddName, AddBirthDate, AddIsCase);
            await this._patientService.AddPatientAsync(patient);
        }
        public async Task DeleteUser()
        {
            await this._patientService.DeletePatientAsync(DelID);
        }
        public async Task MakeAnalisys()
        {

        }
        public async Task ExportCSV()
        {

        }
        public async Task LoadCSV()
        {

        }
        #endregion
    }
}
