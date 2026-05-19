using Ambev.DeveloperEvaluation.IoC.Extensions;
using Microsoft.AspNetCore.Builder;
using System.Reflection;

namespace Ambev.DeveloperEvaluation.IoC.ModuleInitializers;

/// <summary>
/// Module initializer responsible for registering AutoMapper and scanning
/// all profiles from the Application assembly plus any additional assemblies
/// provided by the caller (e.g., WebAPI layer profiles).
/// </summary>
public class AutoMapperModuleInitializer : IModuleInitializer
{
    private readonly Assembly[] _additionalAssemblies;

    /// <summary>
    /// Initializes a new instance of <see cref="AutoMapperModuleInitializer"/>.
    /// </summary>
    /// <param name="additionalAssemblies">
    /// Extra assemblies to scan for AutoMapper profiles.
    /// Pass <c>typeof(Program).Assembly</c> from the WebAPI entry point.
    /// </param>
    public AutoMapperModuleInitializer(params Assembly[] additionalAssemblies)
    {
        _additionalAssemblies = additionalAssemblies;
    }

    /// <inheritdoc />
    public void Initialize(WebApplicationBuilder builder)
    {
        builder.Services.AddAutoMapperConfiguration(
            builder.Configuration,
            _additionalAssemblies);
    }
}