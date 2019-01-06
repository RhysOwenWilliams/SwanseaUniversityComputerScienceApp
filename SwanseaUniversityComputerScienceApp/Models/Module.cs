using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SwanseaUniversityComputerScienceApp.Models
{
    /// <summary>
    /// Contains the columns for the Module database. Data Annotations are not used here 
    /// because this database does not require input from the user, the modules are
    /// defined in the DatabaseInitializer class
    /// </summary>
    public class Module
    {
        public int Id { get; set; }
        public string ModuleName { get; set; }
    }
}
