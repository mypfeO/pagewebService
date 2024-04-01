
using Application.Common.Mappings;
using Domain.Reposotires;
using Infrastructure.Repositories;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Application.Common
{
    public static class MediatRExtensions
    {
        public static IServiceCollection AddConfigureHandler(this IServiceCollection services)
        {
            services.AddMediatR(Assembly.Load("Application"));
            services.AddAutoMapper(typeof(FormulaireMappingProfile));

            return services;
        }
    }
}
