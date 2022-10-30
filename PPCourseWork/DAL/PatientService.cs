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
        public PatientService(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;

            //Make a dummy request to initialize DB Connection
            _databaseContext.Patients.Any();
        }

        public async Task<int> AddPatient(Patient patient)
        {
            _databaseContext.Patients.Add(patient);
            return await _databaseContext.SaveChangesAsync();
        }

        public async Task<int> UpdatePatients(IEnumerable<Patient> patients)
        {
            // Currently Unoptimized, consider a better approach
            _databaseContext.Patients.RemoveRange(_databaseContext.Patients);
            _databaseContext.Patients.AddRange(patients);
            return await _databaseContext.SaveChangesAsync();
        }

        public async Task<Patient> GetPatientByIDAsync(int id)
        {
            return await _databaseContext.Patients.FindAsync(id);
        }

        public async Task<IEnumerable<Patient>> GetPatientsByNameAsync(string name)
        {
            return await _databaseContext.Patients.Where(x => x.Name == name).ToAsyncEnumerable().ToListAsync();
        }

        public async Task<IEnumerable<Patient>> GetAllPatientsAsync()
        {
            return await _databaseContext.Patients.ToAsyncEnumerable().ToListAsync();
        }

        public async Task<int> AddPatientAsync(Patient patient)
        {
            _databaseContext.Patients.Add(patient);
            return await _databaseContext.SaveChangesAsync();
        }

        public async Task<int> DeletePatientAsync(int id)
        {
            Patient patient = await _databaseContext.Patients.FindAsync(id);
            if (patient != null)
            {
                _databaseContext.Patients.Remove(patient);
                return await _databaseContext.SaveChangesAsync();
            }
            else
            {
                throw new KeyNotFoundException($"Patient with id: {id} doesn't exist!");
            }
        }
    }
}
