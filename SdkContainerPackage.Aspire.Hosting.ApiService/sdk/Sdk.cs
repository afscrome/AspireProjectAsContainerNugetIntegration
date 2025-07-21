// Imagine the following where in some kind of SDK / Helper package
// These work alongside the msbuild targest in Sdk.targets

using Aspire.Hosting.ApplicationModel;

namespace Aspire.Hosting;

//TODO: If upstreamed to core Aspire, this could be combined with IProjectMetadata
public interface IProjectWithContainerMetadata : IProjectMetadata
{
    string ContainerRegistry { get; }
    string ContainerRepository { get; }
    string ContainerImageTag { get; }
}


public static class ProjectWithContainerMetadataExtensions
{
    public static IResourceBuilder<ContainerResource> AddProjectContainer<T>(this IDistributedApplicationBuilder builder, string name)
        where T : IProjectWithContainerMetadata, new()
    {
        var metadata = new T();

        var project = builder.AddContainer(name, metadata.ContainerRepository)
            .WithAnnotation(metadata)
            .WithImageTag(metadata.ContainerImageTag ?? "latest");

        if (!string.IsNullOrEmpty(metadata.ContainerRegistry))
        {
            project.WithImageRegistry(metadata.ContainerRegistry);
        }

        // TODO: Infer endpoints from `ContainerPort`
        // https://learn.microsoft.com/en-us/dotnet/core/containers/publish-configuration#containerport 
        project.WithHttpEndpoint(targetPort: 8080);

        // AddProejct implicitly adds WithOtlpExporter, so add that here for consistency
        return project.WithOtlpExporter();
    }
}