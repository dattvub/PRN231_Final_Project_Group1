using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using PDMS.Application.AutoMapper;
using PDMS.Application.Configurations.Persistence;
using PDMS.Infrastructure.Persistence.Repository;

namespace PDMS.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
        {
            // Đăng kí automapper
            services.AddAutoMapper(typeof(AutoMapperProfile));
            // Đăng kí mediatR
            /*
            services.AddMediatR(typeof(DepartmentAddCommand).GetTypeInfo().Assembly);
            services.AddMediatR(Assembly.GetExecutingAssembly());
            */

            // Đăng kí repository
            
            services.AddScoped(typeof(ITestRepository), typeof(TestRepository));
            /*services.AddScoped(typeof(IAssessmentTypeRepository), typeof(AssessmentTypeRepository));
            services.AddScoped(typeof(IEmployeeRepository), typeof(EmployeeRepository));
            //services.AddScoped(typeof(IGroupRepository), typeof(GroupRepository));
            //services.AddScoped(typeof(IEmployeeGroupRepository), typeof(EmployeeGroupRepository));
            services.AddScoped(typeof(IEvaluationTicketRepository), typeof(EvaluationTicketRepository));
            services.AddScoped(typeof(IWorkTaskRepository), typeof(WorkTaskRepository));*/
            

            //Đăng kí service
            return services;
        }
    }
}
