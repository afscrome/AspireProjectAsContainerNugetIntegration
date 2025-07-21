using Aspire.Hosting.ApplicationModel;


// Put Builder extension methods in Aspire.Hosting so they show up automatically in code completion
#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Aspire.Hosting;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class SdkContainerPackageDistributedBuilderExtensions
{
    // This is a method the package author would put together
    // With `AddProjectContainer<T>` being similar to `AddProject<T>`
    public static IResourceBuilder<ContainerResource> AddApiService(this IDistributedApplicationBuilder builder, string name)
    {
        return builder.AddProjectContainer<Containers.SdkContainerPackage_ApiService>(name)
            .WithEnvironment("SOME__VAR", "some value");
    }

    //TODO: Work out a way of providing a concrete resource type, that way you can add extension methods on that resource type
}