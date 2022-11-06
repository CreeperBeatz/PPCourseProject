using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Forms;
using PPCourseWork.Core;
using PPCourseWork.Model;
using PPCourseWork.DAL;
using System.Threading;
using AsyncAwaitBestPractices.MVVM;
using System.Windows;
using System.IO;

namespace PPCourseWork.ViewModels
{
    public class MainVM : ObservableObject
    {
        #region CommandDefinitions
        public IAsyncCommand SearchByIdCommand { get; set; }
        public IAsyncCommand SearchByNameCommand { get; set; }
        public IAsyncCommand LoadAllPatientsCommand { get; private set; }
        public IAsyncCommand MakeAnalisysCommand { get; set; }
        public IAsyncCommand LoadCSVCommand { get; set; }
        public IAsyncCommand ExportCSVCommand { get; set; }
        public IAsyncCommand ChooseExportFolderCommand { get; set; }
        public IAsyncCommand ChooseLoadFileCommand { get; set; }
        public IAsyncCommand AddUserCommand { get; set; }
        public IAsyncCommand DeleteUserCommand { get; set; }
        public IAsyncCommand PurgeDBCommand { get; set; }
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

        //Analisys screen
        private ObservableCollection<AnalisysItem> _analisysItems;

        //Import Export Screen
        private string _loadPath;
        private string _exportPath;

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

        //Analisys screen
        public ObservableCollection<AnalisysItem> AnalisysItems { get { return _analisysItems; } set { _analisysItems = value; OnPropertyChanged(); } }

        //CSV Import Export screen
        public string LoadPath { get { return _loadPath; } set { _loadPath = value; OnPropertyChanged(); } }
        public string ExportPath { get { return _exportPath; } set { _exportPath = value; OnPropertyChanged(); } }
        #endregion
        public MainVM()
        {
            this._patientService = new PatientService(new DatabaseContext());
            this.PatientSearchResults = new ObservableCollection<Patient>();

            this.SearchByIdCommand = new AsyncCommand(SearchByID);
            this.SearchByNameCommand = new AsyncCommand(SearchByName);
            this.LoadAllPatientsCommand = new AsyncCommand(LoadAllPatients);
            this.MakeAnalisysCommand = new AsyncCommand(MakeAnalisys);
            this.AddUserCommand = new AsyncCommand(AddUser);
            this.DeleteUserCommand = new AsyncCommand(DeleteUser);
            this.LoadCSVCommand = new AsyncCommand(LoadCSV);
            this.ExportCSVCommand = new AsyncCommand(ExportCSV);
            this.PurgeDBCommand = new AsyncCommand(PurgeDB);
            this.ChooseExportFolderCommand = new AsyncCommand(ChooseExportFolder);
            this.ChooseLoadFileCommand = new AsyncCommand(ChooseLoadFile);

            // Default birthdate to 01.01.2000, so it's not 01.01.0001
            this.AddBirthDate = new DateTime(2000, 1, 1);
        }

        #region Commands
        public async Task SearchByID()
        {
            try
            {
                var patient_task = _patientService.GetPatientByIDAsync(SearchID);
                PatientSearchResults = new ObservableCollection<Patient>();
                Patient patient = await patient_task;
                if (patient != null)
                {
                    PatientSearchResults.Add(patient);
                }
            } catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }
        public async Task SearchByName()
        {
            try
            {
                var patient_task = _patientService.GetPatientsByNameAsync(SearchName);
                PatientSearchResults = new ObservableCollection<Patient>(await patient_task);
            }
            catch
            {

            }
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
            this.AnalisysItems = new ObservableCollection<AnalisysItem>();

            //TODO Separate into Get 100 records DB and pass task to AddCase task
            foreach (var patient in await this._patientService.GetAllPatientsAsync())
            {
                bool year_exists = false;
                for(int index = 0; index < AnalisysItems.Count; index++)
                {
                    if (AnalisysItems[index].Year == patient.BirthDate.Year)
                    {
                        AnalisysItems[index].Cases++;
                        year_exists = true;
                        break;
                    }
                }
                if (!year_exists) 
                {
                    //TODO when into separate task, add to local array
                    AnalisysItems.Add(new AnalisysItem(patient.BirthDate.Year));
                }
            }

            //TODO Concat all Tasks' results into AnalisysItems
        }
        public async Task ExportCSV()
        {
            if(Directory.Exists(ExportPath))
            {
                if (ExportPath[ExportPath.Length-1] != '\\')
                {
                    ExportPath += '\\';
                }
                string fileName = "PatientExport" + DateTime.Now.Year + "_" + DateTime.Now.Month
                    + "_" + DateTime.Now.Day + "_" + DateTime.Now.Hour + "-" + DateTime.Now.Minute +
                    "-" + DateTime.Now.Second + ".csv";
                
                var patients = await this._patientService.GetAllPatientsAsync();

                string csv = String.Join("\n", await patients.Select(x => x.ToString(',')).ToAsyncEnumerable().ToListAsync());
                var encodedText = Encoding.UTF8.GetBytes(csv);

                using (FileStream sourceStream = new FileStream(ExportPath + fileName,
                    FileMode.Append, FileAccess.Write, FileShare.None,
                    bufferSize: 4096, useAsync: true))
                {
                    await sourceStream.WriteAsync(encodedText, 0, encodedText.Length);
                    System.Windows.MessageBox.Show("Database exported");
                };
            } else
            {
                System.Windows.MessageBox.Show("Path doesn't exist!");
            }
        }
        public async Task LoadCSV()
        {
            if (File.Exists(LoadPath))
            {
                // Load patiets
            } else
            {
                System.Windows.MessageBox.Show("File Doesn't Exist!");
            }
        }
        public async Task PurgeDB()
        {
            if(System.Windows.MessageBox.Show("Are you sure you want to delete ALL contents of the database?", "Irreversable action!", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                await this._patientService.PurgePatientsAsync();
            }
        }
        public async Task ChooseLoadFile()
        {
            using (var dialog = new System.Windows.Forms.OpenFileDialog())
            {
                dialog.Filter = "CSV files (*.csv)|*.csv";
                var result = dialog.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    LoadPath = dialog.FileName;
                }
            }
        }

        public async Task ChooseExportFolder()
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                var result = dialog.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    ExportPath = dialog.SelectedPath;
                }
            }
        }
        #endregion

    }
}
