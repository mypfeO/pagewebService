using Application.Common.Mappings;
using Application.Common;
using Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;

namespace ServiceWebPage
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        private readonly ILogger<Startup> _logger;

        public Startup(IConfiguration configuration, ILogger<Startup> logger)
        {
            Configuration = configuration;
            _logger = logger;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddConfigureHandler(Configuration);
            services.AddInfrastructure(Configuration);

            services.AddMongoDb(Configuration);

            services.AddAutoMapper(typeof(MappingProfile));
            services.AddMediatR(AppDomain.CurrentDomain.GetAssemblies());

            services.AddLogging(configure => configure.AddConsole());

            services.AddEndpointsApiExplorer();
            services.AddSingleton(Configuration);
            services.AddControllers();

            services.AddSwaggerGen(c => c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "dotnet", Version = "V1" }));
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", builder =>
                    builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader());
            });
 
       

         
        }

        public void Configure(IApplicationBuilder app, IHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseRouting();
           
            app.UseAuthentication();
            app.UseCors("AllowAll"); // Corrected to use "AllowAll" which is defined above

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
