using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SwanseaUniversityComputerScienceApp.Models
{
    /// <summary>
    /// Contains the columns within the database for posts. Also has Data Annotations for the display names of the fields
    /// the user enters as well as stating that a post requires a name and the video link must be in the youtube format
    /// </summary>
    public class Post
    {
        public int Id { get; set; }
        [Required]        
        [Display(Name = "Name")]
        public string PostName { get; set; }
        [Display(Name = "Details")]
        public string PostInformation { get; set; }
        public string TimeAndDate { get; set; }
        public string PostedBy { get; set; }
        [Display(Name = "Video Link (YouTube only)")]
        [RegularExpression(@"(?:https?:\/\/)?(?:www\.)?youtu\.?be(?:\.com)?\/?.*(?:watch|embed)?(?:.*v=|v\/|\/)([\w\-_]+)\&?",
            ErrorMessage = "This must be a valid YouTube linke")]
        public string VideoLink { get; set; }
        [Display(Name = "Select a module: ")]
        public string ModuleCode { get; set; }
    }
}
