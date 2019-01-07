using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SwanseaUniversityComputerScienceApp.Data;
using SwanseaUniversityComputerScienceApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SwanseaUniversityComputerScienceApp.Controllers
{
    /// <summary>
    /// This controller class contains all CRUD operations for the application along with some additional methods.
    /// There is functionality that allows for comments to be added to posts, display who posted a posts and who commented
    /// a comment with the time stamp for both. Posters can also embed a video into their posts.
    /// </summary>
    [Authorize]
    public class PostsController : Controller
    {
        private readonly String youtubeHyperlink = "www.youtube.com/watch?v=";

        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        /// <summary>
        /// Constructor to insantiate the connection to our database and user database
        /// </summary>
        /// <param name="context"> the connection to the general database for the application </param>
        /// <param name="userManager"> the connection to the user database </param>
        public PostsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // GET: Posts
        /// <summary>
        /// Initial startup page, displays all the posts available to the user. Also contains the
        /// logic for the search bar and dropdown list present in the navbar. The logic is to
        /// search the database for posts containing a specific string and also search the database
        /// for posts containing the given module
        /// </summary>
        /// <returns> a display for the main page </returns>
        [Authorize]
        public async Task<IActionResult> Index(string search, string module)
        {
            ViewBag.Module = SetModuleNavbarList();

            ViewData["SearchedFor"] = search;
            ViewData["ModuleFilter"] = module;

            if (module == "All Modules")
            {
                module = "";
            }

            var searchForPostContaining = from data in _context.Post
                                          where data.ModuleCode == module || module == "" || module == null
                                          select data;

            if (!String.IsNullOrEmpty(search))
            {
                searchForPostContaining = searchForPostContaining.Where(post => post.PostName.Contains(search));
            }           

            return View(await searchForPostContaining.ToListAsync());
        }

        /// <summary>
        ///  USed to display the users and roles in two separate dropdown lists, then list the 
        ///  users and their role in a dictionary data structure that will be used to display
        ///  both the users and their role in a table
        /// </summary>
        /// <returns> the html view page for UserRoles </returns>
        [Authorize(Policy = "CanChangeRole")]
        public async Task<IActionResult> UserRoles()
        {
            ViewBag.Module = SetModuleNavbarList();
            String signedInUser = _userManager.GetUserName(HttpContext.User);

            List<ApplicationUser> allUsers = _userManager.Users
                .OrderBy(name => name.UserName)
                .ToList();

            List<ApplicationUser> users = _userManager.Users
                .OrderBy(name => name.Email)
                .Where(name => name.Email != signedInUser)
                .Where(name => name.Email != "Member1@email.com")
                .ToList();

            List<IdentityRole> roles = _context.Roles
                .ToList();
            ViewBag.Users = users;
            ViewBag.Roles = roles;

            Dictionary<ApplicationUser, string> userRoles = new Dictionary<ApplicationUser, string>();

            foreach (ApplicationUser u in allUsers)
            {
                foreach (var r in roles)
                {
                    if (await _userManager.IsInRoleAsync(u, r.ToString()))
                    {
                        userRoles.Add(u, r.ToString());
                    }
                }
                
            }

            ViewBag.UserRoles = userRoles;

            return View();
        }

        /// <summary>
        /// Used when the user (who is a member) submits the form within the UserRoles view page.
        /// It retrieves the chosen user and role and switches their role. For example, if the user
        /// was previously a Customer, then it adds them to the Member role and removes their
        /// Customer role since it is unnecessary for them to be in two roles at one, and if they
        /// want to remove their increased rights as a member (such as being able to add a post)
        /// then the Member role would need to be removed from the account
        /// </summary>
        /// <param name="user"> the chosen user account from the drodown list </param>
        /// <param name="role"> the chosen role from the </param>
        /// <returns> redirects the user back to the UserRoles page upon completion </returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "CanChangeRole")]
        public async Task<IActionResult> UserRoles(string user, string role)
        {
            if (ModelState.IsValid)
            {
                if (user != null)
                {
                    var selectedUser = await _userManager.FindByEmailAsync(user);

                    if (role == "Member")
                    {
                        await _userManager.AddToRoleAsync(selectedUser, "Member");
                        await _userManager.RemoveFromRoleAsync(selectedUser, "Customer");
                    }
                    else
                    {
                        await _userManager.AddToRoleAsync(selectedUser, "Customer");
                        await _userManager.RemoveFromRoleAsync(selectedUser, "Member");
                    }
                }

                return (RedirectToAction(nameof(UserRoles)));
            }          

            // Only arrives here upon failure to change the user role
            return View();
        }

        // GET: Posts/Details/5
        /// <summary>
        /// Accesses a specific post, depending on what the user clicks. Also calls for comments that have the matching
        /// post ID.
        /// </summary>
        /// <param name="id"> the specific ID for the selected post </param>
        /// <returns> the details view </returns>
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            ViewBag.Module = SetModuleNavbarList();

            if (id == null)
            {
                return NotFound();
            }

            Post post = await _context.Post
                .SingleOrDefaultAsync(m => m.Id == id);

            if (post == null)
            {
                return NotFound();
            }

            PostCommentsViewModel viewModel = await GetPostCommentDetailsViewModelFromPost(post);

            viewModel.Post = post;

            List<Comment> comments = await _context.Comment
                .Where(x => x.Posted == post).ToListAsync();

            viewModel.Comments = comments;

            return View(viewModel);
        }

        /// <summary>
        /// Allows users to comment of specific posts and links the comment to said post such that all posts do not 
        /// have the same comments. Also saves the current signed in user, i.e. who posted the comment, and the time
        /// and date that they commented.
        /// </summary>
        /// <param name="viewModel"> our viewmodel which is for the post details, joins the comments to this view </param>
        /// <returns> a view containing the post specification and comments </returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "CanComment")]
        public async Task<IActionResult> Details([Bind("PostID,CommentContent")] PostCommentsViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                Comment comment = new Comment();

                comment.DateAndTime = DateTime.Now.ToString("dd/MM/yy HH:mm");

                comment.CommentContent = viewModel.CommentContent;

                comment.CommentBy = GetUser();

                Post post = await _context.Post
                    .SingleOrDefaultAsync(m => m.Id == viewModel.PostID);

                if (post == null)
                {
                    return NotFound();
                }

                comment.Posted = post;
                _context.Comment.Add(comment);
                await _context.SaveChangesAsync();

                viewModel = await GetPostCommentDetailsViewModelFromPost(post);
            }
            return View(viewModel);
        }

        /// <summary>
        /// Joins posts and comments together, depending on if the comment was commented on a post
        /// </summary>
        /// <param name="post"> the post that the user has commented on </param>
        /// <returns> a view model with the linked post - comments </returns>
        private async Task<PostCommentsViewModel> GetPostCommentDetailsViewModelFromPost(Post post)
        {
            ViewBag.Module = SetModuleNavbarList();

            PostCommentsViewModel viewModel = new PostCommentsViewModel();

            viewModel.Post = post;

            List<Comment> comments = await _context.Comment
                .Where(x => x.Posted == post).ToListAsync();

            viewModel.Comments = comments;
            return viewModel;
        }

        // GET: Posts/Create
        /// <summary>
        /// Used to display the create view which allows users to create a new post
        /// </summary>
        /// <returns> a view to create a post </returns>
        [Authorize(Policy = "CanAdd")]
        public IActionResult Create(PostCommentsViewModel viewModel)
        {
            ViewBag.SelectedModule = ListOfModules();
            ViewBag.Module = SetModuleNavbarList();
            return View();
        }

        // POST: Posts/Create
        /// <summary>
        /// When a user submits a post, this checks it and validates it. It adds the created post to the context 
        /// database. Includes data such as a video link if provided, the user who posted the post and the date
        /// and time of the post
        /// </summary>
        /// <param name="post"> a new post created by the user </param>
        /// <returns> a post to be rendered into the view </returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "CanAdd")]
        public async Task<IActionResult> Create([Bind("Id,PostName,PostInformation,SelectedModule,VideoLink")] Post post)
        {
            if (ModelState.IsValid)
            {
                if (post.VideoLink != null)
                {
                    EmbedYoutubeVideo(post);
                }

                post.TimeAndDate = DateTime.Now.ToString("dd/MM/yy HH:mm");

                post.PostedBy = GetUser();

                post.ModuleCode = Request.Form["SelectedModule"].ToString();

                _context.Add(post);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(post);
        }

        // GET: Posts/Edit/5
        /// <summary>
        /// Displays the view which allows a user to edit a post and change what is displayed
        /// </summary>
        /// <param name="id"> the id for the post in question </param>
        /// <returns> the edit page containing text areas to change a post </returns>
        [Authorize(Policy = "CanEdit")]
        public async Task<IActionResult> Edit(int? id)
        {
            ViewBag.SelectedModule = ListOfModules();
            ViewBag.Module = SetModuleNavbarList();

            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Post.SingleOrDefaultAsync(m => m.Id == id);

            if (post.VideoLink != null)
            {
                post.VideoLink = youtubeHyperlink + post.VideoLink;
            }

            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // POST: Posts/Edit/5
        /// <summary>
        /// Validates when a user edits a post. Works similarly to the create method except it takes new data
        /// if present and replaces the old data.
        /// </summary>
        /// <param name="id"> the ID for the selected post </param>
        /// <param name="post"> the selected post itself </param>
        /// <returns> the updated post that gets sent to the view for rendering </returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "CanEdit")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,PostName,PostInformation,SelectedModule,VideoLink")] Post post)
        {
            if (id != post.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (post.VideoLink != null)
                    {
                        EmbedYoutubeVideo(post);
                    }

                    post.TimeAndDate = DateTime.Now.ToString("dd/MM/yy HH:mm");

                    post.PostedBy = GetUser();

                    post.ModuleCode = Request.Form["SelectedModule"].ToString();

                    _context.Update(post);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(post.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(post);
        }

        // GET: Posts/Delete/5
        /// <summary>
        /// Displays the delete page for the selected post.
        /// </summary>
        /// <param name="id"> the ID for the selected post by the user </param>
        /// <returns> the delete page, allowing users to delete the selected post </returns>
        [Authorize(Policy = "CanDelete")]
        public async Task<IActionResult> Delete(int? id)
        {
            ViewBag.Module = SetModuleNavbarList();

            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Post
                .SingleOrDefaultAsync(m => m.Id == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // POST: Posts/Delete/5
        /// <summary>
        /// Removes a post that was chosen to be deleted. Also has a few lines of code which iterate over
        /// each comment on the post and deletes them from the database also.
        /// </summary>
        /// <param name="id"> the ID for the post to be deleted </param>
        /// <returns> redirects the user back to the index view once a post has been removed </returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "CanDelete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var post = await _context.Post.SingleOrDefaultAsync(m => m.Id == id);
            _context.Post.Remove(post);

            var commentQuery = from c in _context.Comment
                               where c.Posted == post
                               select c;

            foreach (var comment in commentQuery)
            {
                _context.Comment.Remove(comment);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Returns true or false if there exists a post matching the matching ID
        /// </summary>
        /// <param name="id"> the ID of a chosen post </param>
        /// <returns> any post which has the same ID as what is given </returns>
        private bool PostExists(int id)
        {
            return _context.Post.Any(e => e.Id == id);
        }

        /// <summary>
        /// Gets the email address for the user that is currently signed in but also cuts the email
        /// off at the '@' symbol and creates a hypothetical username based off the users email address
        /// </summary>
        /// <returns> a 'username' for the user signed in </returns>
        private string GetUser()
        {
            var user = _userManager.GetUserName(HttpContext.User);
            var username = Regex.Split(user, "@").First();
            return username;
        }

        /// <summary>
        /// Used to split a youtube video up after the 'v='. It is cut off here because the set of characters
        /// that follows the split is what is necessary to embed a video into our application
        /// </summary>
        /// <param name="post"> the current post where the video link was pasted into </param>
        private void EmbedYoutubeVideo(Post post)
        {
            if (post.VideoLink.Contains(youtubeHyperlink))
            {
                // Get the database input, cut at the 'v=' and get what remains after the split, then store
                string video = post.VideoLink;
                string embed = Regex.Split(video, "v=").Last();
                post.VideoLink = embed;
            }
            else
            {
                // If the input doesn't contain 'www.youtube.com/watch?v=', store null instead
                post.VideoLink = null;
            }
        }

        /// <summary>
        /// Accessess the module database and returns a list which has both the name of the module as
        /// its text and value. The text is what displays on the dropdown list and the value is what
        /// is accessed when a given text is chosen. Both are the same since we require the module name
        /// for the navbar filtering
        /// </summary>
        /// <returns></returns>
        private List<SelectListItem> ListOfModules()
        {
            List<SelectListItem> modules = _context.Modules.OrderBy(module => module.ModuleName)
                .Select(select => new SelectListItem
                {
                    Text = select.ModuleName,
                    Value = select.ModuleName
                }).ToList();

            return modules;
        }

        /// <summary>
        /// Accessess the module database to access all modules
        /// </summary>
        /// <returns> a list of all modules for the navbar dropdown list </returns>
        public IQueryable<String> SetModuleNavbarList()
        {
            return from t in _context.Modules
                   orderby t.ModuleName
                   select t.ModuleName;
        }
    }
}
