
using Application.Common.Mappings;
using Domain.Reposotires;
using Infrastructure.Repositories;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Graph.ExternalConnectors;
using System.Reflection;

namespace Application.Common
{
    public static class MediatRExtensions
    {
        public static IServiceCollection AddConfigureHandler(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMediatR(Assembly.Load("Application"));
            services.AddAutoMapper(typeof(FormulaireMappingProfile));
            services.Configure<GoogleApiSettings>(configuration.GetSection("GoogleApiSettings"));
            services.AddMediatR(typeof(SubmitFormCommandHandler).Assembly);
            return services;
        }
    }
}
