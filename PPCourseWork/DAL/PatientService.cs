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
            await semaphoreSlim.WaitAsync();
            Patient result = await _databaseContext.Patients.FindAsync(id);
            semaphoreSlim.Release();
            return result;
        }

        public async Task<bool> PatientIdExistsAsync(int id)
        {
            await semaphoreSlim.WaitAsync();
            bool result = _databaseContext.Patients.Any(o => o.ID == id);
            semaphoreSlim.Release();
            return result;
        }

        public async Task<IEnumerable<Patient>> GetPatientsByNameAsync(string name)
        {
            await semaphoreSlim.WaitAsync();
            var result = await _databaseContext.Patients.Where(x => x.Name == name).ToAsyncEnumerable().ToListAsync();
            semaphoreSlim.Release();
            return result;
        }

        public async Task<IEnumerable<Patient>> GetAllPatientsAsync()
        {
            await semaphoreSlim.WaitAsync();
            var result = await _databaseContext.Patients.ToAsyncEnumerable().ToListAsync();
            semaphoreSlim.Release();
            return result;
        }

        public async Task<int> AddOrUpdatePatientAsync(Patient patient, bool autoGenerateID = true)
        {
            await semaphoreSlim.WaitAsync();
            int result;
            using (var transaction = _databaseContext.Database.BeginTransaction()) {
                if (!autoGenerateID) 
                { 
                    await _databaseContext.Database.ExecuteSqlCommandAsync("SET IDENTITY_INSERT [dbo].[Patients] ON");
                }

                var dbPatient = await _databaseContext.Patients.FindAsync(patient.ID);
                if (dbPatient == null)
                {
                    _databaseContext.Patients.Add(patient);
                }
                else
                {
                    dbPatient.Name = patient.Name;
                    dbPatient.BirthDate = patient.BirthDate;
                    dbPatient.IsCase = patient.IsCase;
                }
                try
                {
                    result = await _databaseContext.SaveChangesAsync();
                } catch (Exception ex)
                {
                    result = -1;
                }
                if (!autoGenerateID) { await _databaseContext.Database.ExecuteSqlCommandAsync("SET IDENTITY_INSERT [dbo].[Patients] OFF"); }
                transaction.Commit();
            }
            semaphoreSlim.Release();
            return result;
        }

        public async Task<int> DeletePatientAsync(int id)
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

            var result = await _databaseContext.SaveChangesAsync();
            semaphoreSlim.Release();
            return result;
        }
        
        public async Task<int> PurgePatientsAsync()
        {
            await semaphoreSlim.WaitAsync();
            _databaseContext.Patients.RemoveRange(_databaseContext.Patients);

            var result = await _databaseContext.SaveChangesAsync();
            return result;
        }
    }
}
