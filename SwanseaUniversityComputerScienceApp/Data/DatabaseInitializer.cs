using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SwanseaUniversityComputerScienceApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SwanseaUniversityComputerScienceApp.Data
{
    /// <summary>
    /// This class contains methods for adding data to the various available databases. When the app 
    /// starts up, this class is repeatedly called, but since all methods first check for no data inside the database, 
    /// they are only called when the app is started for the first time.
    /// </summary>
    public class DatabaseInitializer
    {
        /// <summary>
        /// This method is used to add the 6 user accounts to the user database. Each account is given a name which is
        /// Customer1@email.com, Customer2@email.com, Customer3@email.com, Customer4@email.com, 
        /// Customer5@email.com and Member1@email.com. Each of these accounts are then finalised by being given the same. Also adds the remaining data to the application. The data is the modules for CS year 3 and adding some
        /// password - Password123!
        /// </summary>
        /// <param name="context"> the database context </param>
        public static void InitializeUserAccountsAndAppData(ApplicationDbContext context)
        {
            context.Database.EnsureCreated();

            if (!context.Users.Any())
            {
                var users = new ApplicationUser[]
                {
                    new ApplicationUser
                    {
                        UserName = "Member1@email.com",
                        NormalizedUserName = "MEMBER1@EMAIL.COM",
                        Email = "Member1@email.com",
                        NormalizedEmail = "MEMBER1@EMAIL.COM",
                        LockoutEnabled = true,
                        SecurityStamp = Guid.NewGuid().ToString(),
                    },

                    new ApplicationUser
                    {
                        UserName = "Customer1@email.com",
                        NormalizedUserName = "CUSTOMER1@EMAIL.COM",
                        Email = "Customer1@email.com",
                        NormalizedEmail = "CUSTOMER1@EMAIL.COM",
                        LockoutEnabled = true,
                        SecurityStamp = Guid.NewGuid().ToString()
                    },

                    new ApplicationUser
                    {
                        UserName = "Customer2@email.com",
                        NormalizedUserName = "CUSTOMER2@EMAIL.COM",
                        Email = "Customer2@email.com",
                        NormalizedEmail = "CUSTOMER2@EMAIL.COM",
                        LockoutEnabled = true,
                        SecurityStamp = Guid.NewGuid().ToString()
                    },

                    new ApplicationUser
                    {
                        UserName = "Customer3@email.com",
                        NormalizedUserName = "CUSTOMER3@EMAIL.COM",
                        Email = "Customer3@email.com",
                        NormalizedEmail = "CUSTOMER3@EMAIL.COM",
                        LockoutEnabled = true,
                        SecurityStamp = Guid.NewGuid().ToString()
                    },

                    new ApplicationUser
                    {
                        UserName = "Customer4@email.com",
                        NormalizedUserName = "CUSTOMER4@EMAIL.COM",
                        Email = "Customer4@email.com",
                        NormalizedEmail = "CUSTOMER4@EMAIL.COM",
                        LockoutEnabled = true,
                        SecurityStamp = Guid.NewGuid().ToString()
                    },

                    new ApplicationUser
                    {
                        UserName = "Customer5@email.com",
                        NormalizedUserName = "CUSTOMER5@EMAIL.COM",
                        Email = "Customer5@email.com",
                        NormalizedEmail = "CUSTOMER5@EMAIL.COM",
                        LockoutEnabled = true,
                        SecurityStamp = Guid.NewGuid().ToString(),
                    }
                };

                foreach (ApplicationUser u in users)
                {
                    var password = new PasswordHasher<ApplicationUser>();
                    var hashedpassword = password.HashPassword(u, "Password123!");
                    u.PasswordHash = hashedpassword;

                    var userStore = new UserStore<ApplicationUser>(context);
                    var result = userStore.CreateAsync(u);
                }

                context.SaveChangesAsync();
           

            }

            if (!context.Post.Any() && !context.Modules.Any())
            {
                var Posts = new Post[]
                {
                    new Post
                    {
                        PostName = "Mobile Apps Exam",
                        PostInformation = "The exam is on the 10th of January",
                        ModuleCode = "CSC306",
                        PostedBy = "Member1",
                        TimeAndDate = "05/01/19 22:42"
                    },
                    new Post
                    {
                        PostName = "Logic Exam",
                        PostInformation = "The exam is on the 21th of January",
                        ModuleCode = "CSC375",
                        PostedBy = "Member1",
                        TimeAndDate = "05/01/19 22:58"
                    },
                    new Post
                    {
                        PostName = "Slack Page",
                        PostInformation = "Remember to use the slack if you have any questions regarding the coursework since the deadline is approaching",
                        ModuleCode = "CSC348",
                        PostedBy = "Member1",
                        TimeAndDate = "06/01/19 12:16"
                    },
                    new Post
                    {
                        PostName = "Predicate Logic Tips",
                        VideoLink = "kYxYEW2zSlk",
                        PostInformation = "The above video is a quick summary of Predicate Logic for those who are struggling",
                        ModuleCode = "CSC375",
                        PostedBy = "Member1",
                        TimeAndDate = "06/01/19 12:58"
                    },
                    new Post
                    {
                        PostName = "REMINDER: PLEASE COMPLETE THE MODULE FEEDBACK",
                        PostInformation = "The feedback pages are available on Blackboard",
                        ModuleCode = "All Modules",
                        PostedBy = "Member1",
                        TimeAndDate = "07/01/19 09:23"
                    },
                    new Post
                    {
                        PostName = "Marking Scheme Explination",
                        VideoLink = "BQnq3Y-LAD4",
                        PostInformation = "The above video is Sean explaining the marking scheme for those who are confused with anything",
                        ModuleCode = "CSC348",
                        PostedBy = "Member1",
                        TimeAndDate = "07/01/19 10:43"
                    },
                };

                foreach (Post p in Posts)
                {
                    context.Post.Add(p);
                }

                var modules = new Module[]
                {
                    new Module { ModuleName = "All Modules"},
                    new Module { ModuleName = "AR-501" },
                    new Module { ModuleName = "CSC306" },
                    new Module { ModuleName = "CSC309" },
                    new Module { ModuleName = "CSC313" },
                    new Module { ModuleName = "CSC318" },
                    new Module { ModuleName = "CSC327" },
                    new Module { ModuleName = "CSC337" },
                    new Module { ModuleName = "CSC345" },
                    new Module { ModuleName = "CSC348" },
                    new Module { ModuleName = "CSC349" },
                    new Module { ModuleName = "CSC364" },
                    new Module { ModuleName = "CSC368" },
                    new Module { ModuleName = "CSC371" },
                    new Module { ModuleName = "CSC375" },
                    new Module { ModuleName = "CSC385" },
                    new Module { ModuleName = "CSC390" },
                    new Module { ModuleName = "CSP302" },
                    new Module { ModuleName = "CSP344" },
                    new Module { ModuleName = "CSP354" }
                };

                foreach (Module m in modules)
                {
                    context.Modules.Add(m);
                }

                context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// This method is used to add the two roles for the application and add the initialised user accounts to
        /// these roles
        /// </summary>
        /// <param name="context"> the database context for the app </param>
        /// <param name="serviceProvider"> a service object, used for retrieving users and roles</param>
        public static async Task InitialiseUserRolesAsync(ApplicationDbContext context, IServiceProvider serviceProvider)
        {
            var RoleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var UserManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            string[] roles = { "Member", "Customer" };

            IdentityResult result;

            foreach (var role in roles)
            {
                var exists = await RoleManager.RoleExistsAsync(role);

                if (!exists)
                {
                    result = await RoleManager.CreateAsync(new IdentityRole(role));
                }
            }

            ApplicationUser member = await UserManager.FindByEmailAsync("MEMBER1@EMAIL.COM");
            if (!context.UserRoles.Any())
            {
                await UserManager.AddToRoleAsync(member, "Member");

                for (int j = 1; j < 6; j++)
                {
                    ApplicationUser user = await UserManager.FindByEmailAsync("CUSTOMER" + j + "@EMAIL.COM");
                    await UserManager.AddToRoleAsync(user, "Customer");
                }
            }

            if (!context.RoleClaims.Any())
            {
                IdentityRole memberRole = await RoleManager.FindByNameAsync("Member");
                IdentityRole customerRole = await RoleManager.FindByNameAsync("Customer");

                await RoleManager.AddClaimAsync(memberRole, new Claim("AddPost", "Member"));
                await RoleManager.AddClaimAsync(memberRole, new Claim("EditPost", "Member"));
                await RoleManager.AddClaimAsync(memberRole, new Claim("DeletePost", "Member"));
                await RoleManager.AddClaimAsync(memberRole, new Claim("CommentOnPost", "Member"));
                await RoleManager.AddClaimAsync(memberRole, new Claim("ChangeUserRole", "Member"));
                await RoleManager.AddClaimAsync(customerRole, new Claim("CommentOnPost", "Customer"));
            }
        }
    }
}
