﻿using Domain.Reposotires;
using Domaine.Reposotires;
using Infrastructure.GoogleService;
using Infrastructure.Repositories;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        
        {
            services.AddScoped<IRepositoryPageWeb, MongoRepositoryPageWeb>();
            services.AddScoped<IRepositoryFormulaire, MongoRepositoryFormulaire>();

            services.AddMediatR(Assembly.GetExecutingAssembly());
            services.AddSingleton<IGoogleSheetsService>(provider =>
             new GoogleSheetsService(configuration["GoogleSheets:CredentialsFilePath"]));


        }
    }
}
