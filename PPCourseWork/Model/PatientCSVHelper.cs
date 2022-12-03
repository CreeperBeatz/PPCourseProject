using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPCourseWork.Model
{
    public class PatientCSVHelper
    {
        public static char DELIMITER = ',';

        public static async Task ExportCSV(string filePath, IEnumerable<Patient> patients)
        {
            // Check if directory exists
            if (!Directory.Exists(filePath))
            {
                throw new ArgumentException("Filepath doesn't exist!");
            }

            // Format the filepath to accomodate for fileName
            if (filePath[filePath.Length - 1] != '\\')
            {
                filePath += '\\';
            }

            // Generate a safe filename
            string fileName = "PatientExport" + DateTime.Now.Year + "_" + DateTime.Now.Month
                + "_" + DateTime.Now.Day + "_" + DateTime.Now.Hour + "-" + DateTime.Now.Minute +
                "-" + DateTime.Now.Second + ".csv";

            // Generate UTF-8 CSV text
            string csv = String.Join("\n", await patients.Select(x => x.ToString(DELIMITER)).ToAsyncEnumerable().ToListAsync());
            var encodedText = Encoding.UTF8.GetBytes(csv);

            // Write the encoded text
            using (FileStream sourceStream = new FileStream(filePath + fileName,
                FileMode.Append, FileAccess.Write, FileShare.None,
                bufferSize: 4096, useAsync: true))
            {
                await sourceStream.WriteAsync(encodedText, 0, encodedText.Length);
            };
        }

        public static async Task<IEnumerable<Patient>> LoadCSV(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new ArgumentException("File doesn't exist!");
            }

            using (StreamReader reader = File.OpenText(filePath))
            {
                string line;
                List<Patient> patients = new List<Patient>();

                while ((line = await reader.ReadLineAsync()) != null)
                {
                    patients.Add(new Patient(line, DELIMITER));
                }

                return patients;
            }
        }

    }
}
