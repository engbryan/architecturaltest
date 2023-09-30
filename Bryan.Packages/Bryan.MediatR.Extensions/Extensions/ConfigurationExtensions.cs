using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Bryan.MediatR.Extensions;

[ExcludeFromCodeCoverage]
public static class ConfigurationExtensions
{
    public static void ConfigureFluentValidation(this IServiceCollection services, params Assembly[] assemblies)
    {
        var abstractValidatorType = typeof(AbstractValidator<>);
        var array =
            (
                from type in assemblies.SelectMany((a) => a.DefinedTypes)
                where
                    (type.BaseType?.IsGenericType ?? false) &&
                    type.BaseType!.GetGenericTypeDefinition() == abstractValidatorType
                select type
            )
            //.Select(Activator.CreateInstance)
            .ToArray();

        foreach (var obj in array)
        {
            //services.AddSingleton(obj.GetType().BaseType, obj);
            services.AddSingleton(obj.BaseType!, obj);
        }
    }
}