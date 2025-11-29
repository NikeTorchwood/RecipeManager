using Application.Repositories.Abstractions.Base;
using Application.Services.Abstractions.Base;
using Application.Services.Implementations.Base;
using DapperRepositories.NpgSql.Base;

namespace WebApi.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddApplicationLogic(this IServiceCollection services)
    {
        var assemblies = new[] { typeof(RepositoryBase<>).Assembly, typeof(ServiceBase<,,,,>).Assembly };

        var repositoryTypes = assemblies.SelectMany(x => x.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && IsSubclassOfRawGeneric(typeof(RepositoryBase<>), t)))
            .ToArray();

        foreach (var implType in repositoryTypes)
        {
            var interfaceType = implType.GetInterfaces().FirstOrDefault(i =>
                !i.IsGenericType &&
                HasGenericInterface(i, typeof(IRepository<>))
            );

            if (interfaceType != null)
            {
                services.AddScoped(interfaceType, implType);
            }
        }

        var serviceTypes = assemblies.SelectMany(x => x.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && IsSubclassOfRawGeneric(typeof(ServiceBase<,,,,>), t)))
            .ToArray();

        foreach (var implType in serviceTypes)
        {
            var interfaceType = implType.GetInterfaces().FirstOrDefault(i =>
                !i.IsGenericType &&
                HasGenericInterface(i, typeof(IServiceBase<,,,,>))
            );

            if (interfaceType != null)
            {
                services.AddScoped(interfaceType, implType);
            }
        }

        return services;
    }

    private static bool IsSubclassOfRawGeneric(Type genericBase, Type toCheck)
    {
        while (toCheck != null && toCheck != typeof(object))
        {
            var cur = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
            if (genericBase == cur)
            {
                return true;
            }

            toCheck = toCheck.BaseType;
        }

        return false;
    }

    private static bool HasGenericInterface(Type type, Type genericInterface)
        => type.GetInterfaces()
            .Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == genericInterface);
}