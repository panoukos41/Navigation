# Navigation

[![Release](https://github.com/panoukos41/navigation/actions/workflows/release.yaml/badge.svg)](https://github.com/panoukos41/couchdb-identity/actions/workflows/release.yaml)
[![NuGet](https://buildstats.info/nuget/P41.Navigation?includePreReleases=true)](https://www.nuget.org/packages/P41.Navigation)
[![MIT License](https://img.shields.io/apm/l/atomic-design-ui.svg?)](https://github.com/panoukos41/navigation/blob/main/LICENSE.md)

A project for key to view navigation using [ReactiveUI](https://github.com/reactiveui/ReactiveUI) while providing ViewModel creation and navigation aware interface.

## Platforms

| Platform          | TFM                      | NuGet |
|-------------------|--------------------------|-------|
| .NET Standard 2.0 | netstandard2.0           |       |
| .NET 6.0 Android  | net6.0-android           |       |
| .NET 5.0 WPF      | net5.0-windows7.0        |       |
| .NET 5.0 WinUI    | net5.0-windows10.0.19041 |       |
| UWP 16299         | uap10.0.16299            |       |

## Installation

> Replace `X` with the version you want!

| Method            | Command                  |
|-------------------|--------------------------|
| Package Manager   | `Install-Package P41.Navigation -Version X`|
| Package Reference | `<PackageReference Include="P41.Navigation" Version="X" />`|
| .NET CLI          | `dotnet add package P41.Navigation --version X`|

## Getting Started

You get started by configuring the `NavigationHost` for a platform:

```csharp
// UWP platform
new NavigationHost()
    .AddPair("page1", static () => typeof(Page1), static () => new Page1ViewModel())
    .AddPair("page2", static () => typeof(Page2), static () => new Page2ViewModel())
    .AddPair("page3", static () => typeof(Page3), static () => new Page3ViewModel());
```

Then from your shared code *(eg: netstandard)* you call navigation methods:

```csharp
host = INavigationHost; // Injected

// Shared ViewModel constructor
Navigate = ReactiveCommand.Create<string>(page =>
{
    var request = new NavigationRequest(page);
    host.Navigate(request);
});

GoBack = ReactiveCommand.Create<Unit>(_ =>
{
    host.GoBack());
}
```

There are also samples in the [samples](./samples) folder to try.

## Build

Install [Visual Studio 2022]() and [.NET 6.0]()

Clone the project and open the solution then you just build the whole solution or project.

## Contribute

Contributions are welcome and appreceated, before you create a pull request please open a [GitHub Issue](https://github.com/panoukos41/navigation/issues/new) to discuss what needs changing and or fixing if the issue doesn't exist!
