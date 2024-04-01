using Application.Common;
using Application.Common.Mappings;
using Infrastructure;
using MediatR;

namespace ServiceWebPage
{
    public class Startup
    {
        private IConfiguration Configuration;
        private readonly ILogger<Startup> _logger;

        public Startup(IConfiguration configuration, ILogger<Startup> logger)
        {
            Configuration = configuration;
            _logger = logger;
        }
        // IOC
        public void ConfigureServices(IServiceCollection services)
        {

            // services.ConfigureOptions<ConfigureSwagerOptions>();
            // services.AddMediatR(typeof(GetStudentByIdQueryHandler).GetTypeInfo().Assembly);
            // services.AddMediatR(AppDomain.CurrentDomain.GetAssemblies());
            services.AddConfigureHandler();
            services.AddInfrastructure();

            services.AddMongoDb(Configuration);

            services.AddAutoMapper(typeof(MappingProfile));
            services.AddMediatR(AppDomain.CurrentDomain.GetAssemblies());

            services.AddLogging(configure =>
            {
                configure.AddConsole(); // Ajoute la sortie console
            });


            services.AddEndpointsApiExplorer();
            services.AddSingleton(Configuration);
            services.AddControllers();
            
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "dotnet", Version = "V1" });
            });


        }
        // Midlwears
        public void Configure(IApplicationBuilder app, IHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
;
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseCors("AllowOrigin");

            app.UseEndpoints(endpoints => {
                endpoints.MapControllers();
            });

            app.UseSwagger();
            app.UseSwaggerUI(op =>
            {
                op.SwaggerEndpoint("/swagger/v1/swagger.json", "STD API");
                op.RoutePrefix = string.Empty;
                op.DocumentTitle = "My Swagger";
            });

        }

    }
}