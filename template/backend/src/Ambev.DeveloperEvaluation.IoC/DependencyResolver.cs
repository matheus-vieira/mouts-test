using Ambev.DeveloperEvaluation.IoC.ModuleInitializers;
using Microsoft.AspNetCore.Builder;
using System.Reflection;

namespace Ambev.DeveloperEvaluation.IoC;

/// <summary>
/// Central dependency resolver responsible for orchestrating the initialization 
/// of all system modules.
/// </summary>
public static class DependencyResolver
{
    /// <summary>
    /// Registers all dependencies across system layers.
    /// </summary>
    /// <param name="builder">The web application builder.</param>
    /// <param name="additionalAssemblies">
    /// Optional extra assemblies to scan for AutoMapper profiles (usually 'typeof(Program).Assembly').
    /// </param>
    public static void RegisterDependencies(this WebApplicationBuilder builder, params Assembly[] additionalAssemblies)
    {
        new ApplicationModuleInitializer().Initialize(builder);
        new AutoMapperModuleInitializer(additionalAssemblies).Initialize(builder);
        new MediatRModuleInitializer().Initialize(builder);
        new ValidationModuleInitializer().Initialize(builder);
        new InfrastructureModuleInitializer().Initialize(builder);
        new WebApiModuleInitializer().Initialize(builder);
    }
}