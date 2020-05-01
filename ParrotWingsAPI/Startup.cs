namespace ParrotWings
{
    using System.Threading.Tasks;
    using EFCore.DbContextFactory.Extensions;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.SignalR;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Microsoft.IdentityModel.Tokens;
    using ParrotWingsAPI;
    using ParrotWingsData;

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            string connection = Configuration.GetConnectionString("DefaultConnection");

            // only for log and ensurecreated
            services.AddDbContext<ParrotWingsDBContext>(options => options.UseLoggerFactory(LoggerFactory.Create(builder => builder.AddConsole())).UseSqlServer(connection));
            
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.WithOrigins("http://localhost:4200")
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
            });

            services.AddControllers();
            services.AddSignalR();

            var appSettingsSection = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);

            var appSettings = appSettingsSection.Get<AppSettings>();
            var key = System.Text.Encoding.ASCII.GetBytes(appSettings.Secret);
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(x =>
                {
                    x.RequireHttpsMetadata = false;
                    x.SaveToken = true;
                    x.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                    x.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var accessToken = context.Request.Query["access_token"];

                            var path = context.HttpContext.Request.Path;
                            if (!string.IsNullOrEmpty(accessToken) && (path.StartsWithSegments("/hubs/balance") || path.StartsWithSegments("/hubs/transactions")))
                            {
                                context.Token = accessToken;
                            }
                            return Task.CompletedTask;
                        }
                    };
                }
            );

            services.AddSqlServerDbContextFactory<ParrotWingsDBContext>(connection);

            services.AddScoped<IHashFunction, HashFunction>();
            services.AddScoped<IUsersService, UsersService>();
            services.AddScoped<IAuthenticateService, AuthenticateService>();
            services.AddScoped<IUserBalancesService, UserBalancesService>();
            services.AddScoped<ITransactionsService, TransactionsService>();
            services.AddSingleton<IUserIdProvider, UserGuidProvider>();
            services.AddSingleton<BalanceSubscription, BalanceSubscription>();
            services.AddSingleton<TransactionSubscription, TransactionSubscription>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ParrotWingsDBContext context)
        {
            if (env.IsDevelopment())
            {
                context.Database.EnsureCreated();
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors("CorsPolicy");

            app.UseAuthentication();
            app.UseAuthorization();


            app.UseEndpoints(endpoints => {
                endpoints.MapControllers();
                endpoints.MapHub<BalanceHub>("/hubs/balance");
                endpoints.MapHub<TransactionHub>("/hubs/transactions");
            });

            app.UseSqlTableDependency<BalanceSubscription>(Configuration.GetConnectionString("DefaultConnection"));
            app.UseSqlTableDependency<TransactionSubscription>(Configuration.GetConnectionString("DefaultConnection"));
        }
    }
}
