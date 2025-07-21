# Aspire Container Project Resources

This project is a play around with making it easier to publish a project as a containerised Aspire Component to allow other teams to use your app.
This spike assumes that your apps publish containers  via the .Net SDK.

- `SdkContainerPackage.ApiService`, `SdkContainerPackage.ServiceDefaults` are pretty much what you'd expect from the sample app

- `SdkContainerPackage.Aspire.Hosting.ApiService` - a project that can be published as a nuget package for others to hae their own `builder.AddMyApp()` method.
  The `sdk` folder includes some helpers & targets that use a pattern similar to the `IProjectMetadata` in `Aspire.Hosting` to take the registry, repository and tags from the referenced project and build them up into a container.
  For now these live in the project themselves, although longer term they could be generalised into a generic helper package / unstreamed to the core aspire sdk.

- `SdkContainerPackage.Tests` - a test project for running E2E tests on the nuget package with the app running as a container.
  Includes some magic to try and make sure when you run locally you test against a locally built container.
  This is helpful for:
    - Local dev - make sure your testing with code changes to your local projects.
    - Build server - you can run tests w/out needing to push the container to a remote registry, or only push the image once you've validated all the tests pass.

# Known Issues

## Nuget package

- Extracting the container data relies on a custom `OutputComputeContainerConfig` target in referenced applications (defined in `Directory.Build.targets`).
  Need to work out a way to get this information from the base dotnet sdk.
- I'd like to come up with a way to provide a more concrete resource type than `ContainerResource` to allow a package author to define extension methods for just their resource.  (e.g. `WithLogging(level)`, `WithMockSmtpServer()`, `WithRealSmtpServer()`)
  - It would also be really nice to be able to quickly swap between a project & container resource, but that likely requires deeper work in the core aspire libraries 
    https://github.com/dotnet/aspire/issues/8984#issuecomment-2844523369

## Tests
- Test project only works via `dotnet test` - the `VSTest` target doesn't seem to be hit with Visual Studio test runner.  Haven't tested Rider or VS Code.
- Test project results in container being double built - once in `dotnet test` and once in `dotnet publish`
  - Hopefully fixed in .Net 10 - https://github.com/dotnet/sdk/pull/49556
  - If incremental build isn't clever about avoiding rebuild if only tags change, that may still be problematic 
