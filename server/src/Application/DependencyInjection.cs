using System.Reflection;
using Application.Common.Behaviours;
using Application.Common.Messaging;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Application;


public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        services.AddValidatorsFromAssembly(assembly);

        foreach(var type in assembly.GetTypes())
        {
            foreach(var interfaceType in type.GetInterfaces())
            {
                if(interfaceType.IsGenericType && 
                        interfaceType.GetGenericTypeDefinition() == typeof(IRequestHandler<,>))
                {
                    services.AddTransient(interfaceType, type);
                }
            }
        }

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
        services.AddScoped<ISender, Dispatcher>();

        return services;
    }
}