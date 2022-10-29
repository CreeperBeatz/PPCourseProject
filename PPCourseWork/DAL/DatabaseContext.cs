using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PPCourseWork.Model;
using PPCourseWork.Migrations;

namespace PPCourseWork.DAL
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext() : base("data source=.\\SQLEXPRESS; initial catalog=PatientSystem; integrated security=SSPI")
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<DatabaseContext, Configuration>());
        }

        public DbSet<Patient> Patients { get; set; }
    }
}
