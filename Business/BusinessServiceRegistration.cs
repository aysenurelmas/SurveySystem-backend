using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Core.Business;
using Business.Abstracts;
using Business.Concretes;

namespace Business;

public static class BusinessServiceRegistration
{
    public static IServiceCollection AddBusinessServices(this IServiceCollection services)
    {

        services.AddScoped<IParticipationService, ParticipationManager>();
        services.AddScoped<IQuestionService, QuestionManager>();
        services.AddScoped<ISurveyService, SurveyManager>();
        services.AddScoped<IUserService, UserManager>();
        services.AddScoped<IUserOperationClaimService, UserOperationClaimManager>();
        services.AddScoped<IAuthService, AuthManager>();
        services.AddScoped<IAuthOperationService, AuthOperationManager>();


        services.AddSubClassesOfType(Assembly.GetExecutingAssembly(), typeof(BaseBusinessRules));

        //services.AddAutoMapper(Assembly.GetExecutingAssembly());
        services.AddAutoMapper(cfg => { }, Assembly.GetExecutingAssembly());
        return services;
    }

    public static IServiceCollection AddSubClassesOfType(this IServiceCollection services,
     Assembly assembly, Type type, Func<IServiceCollection, Type, IServiceCollection>? addWithLifeCycle = null)
    {
        var types = assembly.GetTypes().Where(t => t.IsSubclassOf(type) && type != t).ToList();
        foreach (var item in types)
            if (addWithLifeCycle == null)
                services.AddScoped(item);

            else
                addWithLifeCycle(services, type);
        return services;
    }

}