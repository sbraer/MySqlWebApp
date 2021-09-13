using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySqlIdentityModel;

namespace WebApp.Pages
{
	[AllowAnonymous]
	public class RegisterModel : PageModel
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly RoleManager<ApplicationRole> _roleManager;

		public RegisterModel(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
		{
			_userManager = userManager;
			_roleManager = roleManager;
		}

		public void OnGet(string returnUrl = null)
		{
			ShowForm = true;
			ReturnUrl = returnUrl;
		}

		public async Task<IActionResult> OnPostAsync()
		{
			if (ModelState.IsValid)
			{
				try
				{
					var user = new ApplicationUser
					{
						Email = Username,
						Id = Guid.NewGuid().ToString(),
						UserName = Username,
						PasswordHash = Password
					};

					var result = await _userManager.CreateAsync(user, Password);
					if (result.Succeeded)
					{
						await CheckRole(user);
						await CheckClaims(user);
						Message = "User created";
						ShowForm = false;
					}
					else
					{
						Message = result.Errors.Select(t => t.Description).Aggregate((i, j) => i + ", " + j);
					}
				}
				catch (Exception ex)
				{
					Message = ex.Message;
				}
			}
			else
			{
				await Task.Yield();
			}

			return Page();
		}

		private async Task CheckClaims(ApplicationUser user)
		{
			if (user.UserName == "a3@a3.it")
			{
				var test1 = new List<Claim>()
				{
					new Claim(ClaimTypes.Name, "a3"),
					new Claim(ClaimTypes.Email, "a3@a3.it"),
					new Claim(ClaimTypes.DateOfBirth, "01/01/2000"),
				};

				await _userManager.AddClaimsAsync(user, test1);
			}

			await Task.CompletedTask;
		}

        private async Task CheckRole(ApplicationUser user)
		{
			if (user.UserName == "a1@a1.it")
			{
				// Check if role admin exist
				ApplicationRole role = await _roleManager.FindByNameAsync("ADMIN");
				if (role == null)
				{
					var result = await _roleManager.CreateAsync(new ApplicationRole
					{
						Id = Guid.NewGuid().ToString(),
						Name = "Admin",
						NormalizedName = "ADMIN"
					});

					if (!result.Succeeded)
					{
						throw new Exception("Error creating new Role Admin");
					}
				}

				await _userManager.AddToRoleAsync(user, "ADMIN");
			}

			{
				// Check if role admin exist
				ApplicationRole role = await _roleManager.FindByNameAsync("USERS");
				if (role == null)
				{
					var result = await _roleManager.CreateAsync(new ApplicationRole
					{
						Id = Guid.NewGuid().ToString(),
						Name = "Users",
						NormalizedName = "USERS"
					});

					if (!result.Succeeded)
					{
						throw new Exception("Error creating new Role Users");
					}
				}

				await _userManager.AddToRoleAsync(user, "USERS");
			}
		}

		[BindProperty]
		[Required(ErrorMessage = "Username is required"), MaxLength(50, ErrorMessage = "50 max length")]
		public string Username { get; set; } = null!;

		[BindProperty]
		[Required(ErrorMessage = "Password is required"), MaxLength(50, ErrorMessage = "50 max length")]
		public string Password { get; set; } = null!;

		public bool ShowForm { get; protected set; }
		public string Message { private set; get; }

		public string ReturnUrl { get; set; }
		public string ConfirmPassword { get; set; }
	}
}
