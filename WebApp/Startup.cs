using Configuration;
using DbStructure;
using Entities;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MySqlIdentityDal;
using MySqlIdentityModel;
using System.Net;
using System.Threading.Tasks;
using WebApp.Services;

namespace WebApp
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddDataProtection()
				.SetApplicationName("WebApp")
				.PersistKeysToFileSystem(new System.IO.DirectoryInfo(System.Environment.CurrentDirectory));

			services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
			{
				options.SignIn.RequireConfirmedAccount = false;
				options.SignIn.RequireConfirmedEmail = false;
				options.SignIn.RequireConfirmedPhoneNumber = false;
				options.Password.RequireDigit = false;
				options.Password.RequireLowercase = false;
				options.Password.RequiredLength = 3;
				options.Password.RequiredUniqueChars = 0;
				options.Password.RequireNonAlphanumeric = false;
				options.Password.RequireUppercase = false;
			})
				.AddDefaultTokenProviders();

			services.ConfigureApplicationCookie(config =>
			{
				config.Events = new CookieAuthenticationEvents
				{
					OnRedirectToLogin = ctx => {
						if (ctx.Request.Path.StartsWithSegments("/api"))
						{
							ctx.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
						}
						else
						{
							ctx.Response.Redirect(ctx.RedirectUri);
						}

						return Task.FromResult(0);
					}
				};
			});

			var _ConfigurationManager = new ConfigurationManager(Configuration);
			services.AddSingleton<IConfigurationManager>(_ConfigurationManager);
			services.AddSingleton<IUserStore<ApplicationUser>, UserStore>();
			services.AddSingleton<IRoleStore<ApplicationRole>, RoleStore>();
			services.AddSingleton<IArticleStore, ArticleStore>();
			services.AddSingleton<IInitializeDatabase, InitializeDatabase>();

			// Add application services.
			services.AddTransient<IEmailSender, EmailSender>();

			services.ConfigureApplicationCookie(options =>
			{
				options.LoginPath = "/Login";
				options.AccessDeniedPath = "/AccessDenied";
			});

			// create database
			IInitializeDatabase database = new InitializeDatabase(_ConfigurationManager);
			services.AddSingleton<IInitializeDatabase>(database);
#if (DEBUG)
			// This code create database and structure only in debug
			database.CreateUpdateDb();
#endif
			services.AddRazorPages();
			services.AddControllers();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseExceptionHandler("/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}

			//app.UseHttpsRedirection();
			app.UseStaticFiles();

			app.UseAuthentication();
			app.UseRouting();
			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapRazorPages();
				endpoints.MapControllers();
			});
		}
	}
}
