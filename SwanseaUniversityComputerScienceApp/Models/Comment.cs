using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SwanseaUniversityComputerScienceApp.Models
{
    /// <summary>
    /// Contains the columns for the comment database. Data Annotations are not used here for various
    /// reasons including not requiring a display name and since the view model handles the input 
    /// from a user 
    /// </summary>
    public class Comment
    {
        public int Id { get; set; }
        public string CommentInformation { get; set; }
        public string DateAndTime { get; set; }
        public string CommentBy { get; set; }

        public virtual Post Posted { get; set; }
    }
}
