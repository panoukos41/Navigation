# Navigation

[![Release](https://github.com/panoukos41/navigation/actions/workflows/release.yaml/badge.svg)](https://github.com/panoukos41/navigation/actions/workflows/release.yaml)
[![NuGet](https://buildstats.info/nuget/P41.Navigation?includePreReleases=true)](https://www.nuget.org/packages/P41.Navigation)
[![MIT License](https://img.shields.io/apm/l/atomic-design-ui.svg?)](https://github.com/panoukos41/navigation/blob/main/LICENSE.md)

A project for key to view navigation using [ReactiveUI](https://github.com/reactiveui/ReactiveUI) while providing ViewModel creation and navigation aware interface.

## Platforms

| Platform          | TFM                      |
|-------------------|--------------------------|
| .NET Standard 2.0 | netstandard2.0           |
| .NET 6.0 Android  | net6.0-android           |
| .NET 5.0 WPF      | net5.0-windows7.0        |
| .NET 5.0 WinUI    | net5.0-windows10.0.19041 |
| UWP 16299         | uap10.0.16299            |

## Installation

> Replace `X` with the version you want!

### .NET CLI
```csharp
dotnet add package P41.Navigation --version X
```

### Package Reference
```csharp
<PackageReference Include="P41.Navigation" Version="X" />
```

## Getting Started

You get started by configuring the `NavigationHost` for a platform:

### UWP
```csharp
// Application (App.xaml.cs in sample)
services.AddSingleton(sp =>
    new NavigationHost()
    .Map("page1", static () => Services.Resolve<Page1ViewModel>(), static () => typeof(Page1))
    .Map("page2", static () => Services.Resolve<Page2ViewModel>(), static () => typeof(Page2))
    .Map("page3", static () => Services.Resolve<Page3ViewModel>(), static () => typeof(Page3)));

// At the end of Application OnLaunched
Window.Current.Activate();
Services.Resolve<INavigationHost>().Navigate("page1/100");
```

### Android
```csharp
// Application (AndroidApp in sample)
services.AddSingleton(sp =>
    new NavigationHost()
    .Map("page1", static () => Services.Resolve<Page1ViewModel>(), static () => new Page1())
    .Map("page2", static () => Services.Resolve<Page2ViewModel>(), static () => new Page2())
    .Map("page3", static () => Services.Resolve<Page3ViewModel>(), static () => new Page3())
);

// Main Activity (Shell in sample) at the end of OnCreate
host.SetFragmentManager(SupportFragmentManager)
    .SetFragmentContainerId(R.Id.MainContainer)
    .Navigate("page1");
```

### .NET Standard 2.0
Then from your shared code *(eg: netstandard)* you call navigation methods:

```csharp
host = INavigationHost; // Injected

// Shared ViewModel constructor
Navigate = ReactiveCommand.Create<string>(page =>
{
    host.Navigate(page);
});

GoBack = ReactiveCommand.Create<Unit>(_ =>
{
    host.GoBack());
}
```

There are also samples in the [samples](./samples) folder that you should see and try!

## Build

Install [Visual Studio 2022 Preview](https://visualstudio.microsoft.com/vs/preview) and [.NET 6.0](https://dotnet.microsoft.com/download/dotnet/6.0)

Clone the project and open the solution then you just build the whole solution or specific project you want to build.

## Contribute

Contributions are welcome and appreciated, before you create a pull request please open a [GitHub Issue](https://github.com/panoukos41/navigation/issues/new) to discuss what needs changing and or fixing if the issue doesn't exist!
