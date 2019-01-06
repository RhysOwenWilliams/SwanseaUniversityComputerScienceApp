using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SwanseaUniversityComputerScienceApp.Models
{
    /// <summary>
    /// ViewModel for the Post details. This allows for comments to be added to the post. Retrieves
    /// the comment content, current signed in user and time from the Comment.cs model
    /// </summary>
    public class PostCommentsViewModel
    {
        public Post Post { get; set; }
        public int PostID { get; set; }
        [Required]
        public string CommentName { get; set; }
        public List<Comment> Comments { get; set; }
    }
}
