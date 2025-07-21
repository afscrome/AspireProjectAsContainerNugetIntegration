// Imagine the following where in some kind of SDK / Helper package
// These work alongside the msbuild targest in Sdk.targets

using Aspire.Hosting.ApplicationModel;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace Aspire.Hosting;

public static class TestingHelperExtensions
{
    public static IDistributedApplicationTestingBuilder WithTestProjectOverride(this IDistributedApplicationTestingBuilder builder)
    {
        builder.Eventing.Subscribe<BeforeStartEvent>((evt, ct) =>
        {
            var metadata = Assembly.GetEntryAssembly()!.GetCustomAttributes<AssemblyMetadataAttribute>();

            var testTag = metadata.SingleOrDefault(x => x.Key == "localcontainertag")?.Value
                ?? throw new InvalidOperationException("AssemblyMetadataAttribute with key 'localcontainertag' not found.");

            var testContainerProjects = metadata.SingleOrDefault(x => x.Key == "localcontainerprojects")?.Value?.Split(";")
                ?? throw new InvalidOperationException("AssemblyMetadataAttribute with key 'localcontainerprojects' not found.");

            var resourceLoggerService = evt.Services.GetRequiredService<ResourceLoggerService>();

            foreach (var resource in evt.Model.Resources.OfType<ContainerResource>())
            {
                if (!resource.TryGetLastAnnotation<IProjectWithContainerMetadata>(out var projectMetadata))
                    continue;

                //TODO: Should this be case insensitive?
                if (testContainerProjects.Any(x => x.Equals(projectMetadata.ProjectPath)))
                {
                    builder.CreateResourceBuilder(resource)
                        .WithImageTag(testTag)
                        .WithImageRegistry(null);
                        //.WithImagePullPolicy(ImagePullPolicy.Never) // Never policy is missing

                    resourceLoggerService.GetLogger(resource).LogInformation("Overriding to local tag '{Tag}'", testTag);
                }
            }

            return Task.CompletedTask;
        });

        return builder;
    }
}