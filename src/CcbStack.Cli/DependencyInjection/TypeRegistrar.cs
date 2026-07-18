using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;

namespace CcbStack.Cli.DependencyInjection;

/// <summary>
/// Adapts an <see cref="IServiceCollection"/> to Spectre.Console.Cli's
/// <see cref="ITypeRegistrar"/> so <see cref="CommandApp"/> constructs commands (and their
/// dependencies) through the same DI container the rest of ccbstack uses, instead of
/// requiring parameterless constructors.
/// </summary>
public sealed class TypeRegistrar : ITypeRegistrar
{
    private readonly IServiceCollection _services;

    public TypeRegistrar(IServiceCollection services)
    {
        _services = services;
    }

    public ITypeResolver Build()
    {
        return new TypeResolver(_services.BuildServiceProvider());
    }

    public void Register(Type service, Type implementation)
    {
        _services.AddSingleton(service, implementation);
    }

    public void RegisterInstance(Type service, object implementation)
    {
        _services.AddSingleton(service, implementation);
    }

    public void RegisterLazy(Type service, Func<object> factory)
    {
        ArgumentNullException.ThrowIfNull(factory);
        _services.AddSingleton(service, _ => factory());
    }
}
