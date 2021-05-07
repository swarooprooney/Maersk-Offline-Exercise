using Maersk.Sorting.Api.Configuration;
using Maersk.Sorting.Api.DataLayer;
using Maersk.Sorting.Api.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Maersk.Sorting.Api.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<DBConfiguration>(options => { options.ConnectionString = configuration.GetConnectionString("JobConnectionString"); });
            services.AddSingleton<ISortJobProcessor, SortJobProcessor>();
            services.AddScoped<IRepoService, RepoService>();
            services.AddScoped<IDatabaseWrapper, MongoWrapper>();
            return services;
        }
    }
}
