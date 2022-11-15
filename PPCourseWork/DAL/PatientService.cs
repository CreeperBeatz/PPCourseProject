using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PPCourseWork.Model;

namespace PPCourseWork.DAL
{
    public class PatientService
    {
        private readonly DatabaseContext _databaseContext;
        private static System.Threading.SemaphoreSlim semaphoreSlim = new System.Threading.SemaphoreSlim(1, 1);
        public static bool IsBisy { get { return semaphoreSlim.CurrentCount == 0; } }
        public PatientService(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;

            //Make a dummy request to initialize DB Connection
            _databaseContext.Patients.Any();
        }

        public async Task<int> AddPatient(Patient patient)
        {
            await semaphoreSlim.WaitAsync();
            _databaseContext.Patients.Add(patient);
            var result = await _databaseContext.SaveChangesAsync();
            semaphoreSlim.Release();
            return result;
        }

        /*
        public async Task<int> UpdatePatients(IEnumerable<Patient> patients)
        {
            // Currently Unoptimized, consider a better approach
            _databaseContext.Patients.RemoveRange(_databaseContext.Patients);
            _databaseContext.Patients.AddRange(patients);
            return await _databaseContext.SaveChangesAsync();
        }*/

        public async Task<Patient> GetPatientByIDAsync(int id)
        {
            try
            {
                await semaphoreSlim.WaitAsync();
                return await _databaseContext.Patients.FindAsync(id);
            }
            finally { semaphoreSlim.Release(); }
        }

        public async Task<bool> PatientIdExistsAsync(int id)
        {
            try
            {
                await semaphoreSlim.WaitAsync();
                return _databaseContext.Patients.Any(o => o.ID == id);
            }
            finally { semaphoreSlim.Release(); }
        }

        public async Task<IEnumerable<Patient>> GetPatientsByNameAsync(string name)
        {
            try 
            {
                await semaphoreSlim.WaitAsync();
                return await _databaseContext.Patients.Where(x => x.Name == name).ToAsyncEnumerable().ToListAsync();
            }
            finally { semaphoreSlim.Release(); }
        }

        public async Task<IEnumerable<Patient>> GetAllPatientsAsync()
        {
            try
            {
                await semaphoreSlim.WaitAsync();
                return await _databaseContext.Patients.ToAsyncEnumerable().ToListAsync();
            }
            finally { semaphoreSlim.Release(); }
        }

        public async Task<int> AddOrUpdatePatientAsync(Patient patient, bool generateId = true)
        {
            try
            {
                await semaphoreSlim.WaitAsync();
                var dbPatient = await _databaseContext.Patients.FindAsync(patient.ID);
                if (dbPatient == null)
                {
                    if (!generateId)
                    {
                        //Hack the EF since dbset doesn't support identity insert
                        using (var transaction = _databaseContext.Database.BeginTransaction())
                        {
                            //Get current ident for restorement later
                            int ident = Convert.ToInt32(_databaseContext.Database.SqlQuery<decimal>("SELECT IDENT_CURRENT('[dbo].[Patients]')").FirstOrDefault());
                            //reseed ident to patient we want to insert
                            await _databaseContext.Database.ExecuteSqlCommandAsync($"DBCC CHECKIDENT('[dbo].[Patients]', RESEED, {patient.ID - 1})");
                            _databaseContext.Patients.Add(patient);
                            //Save changes with current indent
                            await _databaseContext.SaveChangesAsync();
                            //reseed ident to previous state
                            await _databaseContext.Database.ExecuteSqlCommandAsync($"DBCC CHECKIDENT('[dbo].[Patients]', RESEED, {ident})");
                            transaction.Commit();
                        }
                    }
                    else
                    {
                        _databaseContext.Patients.Add(patient);
                    }
                }
                else
                {
                    dbPatient.Name = patient.Name;
                    dbPatient.BirthDate = patient.BirthDate;
                    dbPatient.IsCase = patient.IsCase;
                }
                return await _databaseContext.SaveChangesAsync();
            }
            finally { semaphoreSlim.Release(); }
        }

        //When adding a patient, assign a custom ID, not DB Generated one
        private async Task<int> IdentityInsertPatientAsync(Patient patient)
        {
            if (patient.ID == null)
            {
                throw new ArgumentNullException(nameof(patient) + " ID can not be null on IdentityInsert");
            }

            try
            {
                await semaphoreSlim.WaitAsync();
                Patient dbPatient = await _databaseContext.Patients.FindAsync(patient.ID);
                if (dbPatient != null)
                {
                    throw new InvalidOperationException("Patient already exists!");
                }

                int result;
                using (var transaction = _databaseContext.Database.BeginTransaction())
                {

                    //Hack the EF since dbset doesn't support identity insert
                    //Get current ident for restorement later
                    int ident = Convert.ToInt32(_databaseContext.Database.SqlQuery<decimal>("SELECT IDENT_CURRENT('[dbo].[Patients]')").FirstOrDefault());
                    //reseed ident to patient we want to insert
                    await _databaseContext.Database.ExecuteSqlCommandAsync($"DBCC CHECKIDENT('[dbo].[Patients]', RESEED, {patient.ID - 1})");
                    _databaseContext.Patients.Add(patient);
                    //reseed ident to previous state
                    result = await _databaseContext.SaveChangesAsync();
                    await _databaseContext.Database.ExecuteSqlCommandAsync($"DBCC CHECKIDENT('[dbo].[Patients]', RESEED, {ident})");
                    transaction.Commit();
                }
                return result;
            }
            finally { semaphoreSlim.Release(); }
        }

        public async Task<int> DeletePatientAsync(int id)
        {
            try
            {
                await semaphoreSlim.WaitAsync();
                Patient patient = await _databaseContext.Patients.FindAsync(id);
                if (patient != null)
                {
                    _databaseContext.Patients.Remove(patient);
                }
                else
                {
                    throw new KeyNotFoundException($"Patient with id: {id} doesn't exist!");
                }

                return await _databaseContext.SaveChangesAsync();
            }
            finally { semaphoreSlim.Release(); }            
        }

        public async Task<int> PurgePatientsAsync()
        {
            try 
            {
                await semaphoreSlim.WaitAsync();
                _databaseContext.Patients.RemoveRange(_databaseContext.Patients);
                return await _databaseContext.SaveChangesAsync();
            }
            finally { semaphoreSlim.Release(); }
        }
    }
}
