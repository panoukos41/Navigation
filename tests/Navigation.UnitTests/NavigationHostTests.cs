using Flurl;
using P41.Navigation.UnitTests.Util;

namespace P41.Navigation.UnitTests;

public class NavigationHostTests
{
    [Fact]
    public async Task Should_navigate_to_from_and_count_events()
    {
        // Arrange
        var host = new TestHost(
            views: new()
            {
                ["home"] = new TestView("home"),
                ["details/{?}"] = new TestView("details"),
                ["settings"] = new TestView("settings"),
            },
            viewModels: new()
            {
                ["home"] = new TestViewModel(),
                ["details/{?}"] = new TestViewModel(),
                ["settings"] = new TestViewModel(),
            });

        Url requestHome = "home";
        Url requestDetails = "details";
        Url requestDetails2 = "details/panos";
        Url requestSettings = "settings?sub=false";

        var whenNavigatedCount = 0;
        host.WhenNavigated(request => whenNavigatedCount += 1);

        // Act
        // Navigated to home 1
        await host.Push(requestHome);
        await host.Push(requestHome);

        // Navigated to details 1, Navigating from home 1
        await host.Push(requestDetails2);
        await host.Push(requestDetails2);

        // Navigated to settings 1, Navigating from details 1
        await host.Push(requestSettings);
        await host.Push(requestSettings);

        // Navigated to details 2, Navigating from settings 1
        await host.Push(requestDetails);
        await host.Push(requestDetails);

        // Navigated to settings 2, Navigating from details 2
        (await host.Pop()).Should().Be(requestDetails);

        // Navigated to details 3, Navigating from settings 2
        (await host.Pop()).Should().Be(requestSettings);

        // Navigated to home 2, Navigating from details 3
        (await host.Pop()).Should().Be(requestDetails2);

        // Returns root multiple times, should not change Count, should return root request.
        (await host.Pop()).Should().Be(requestHome);
        host.Count.Should().Be(1);

        (await host.Pop()).Should().Be(requestHome);
        host.Count.Should().Be(1);

        (await host.Pop()).Should().Be(requestHome);
        host.Count.Should().Be(1);

        // Assert
        var homeVm = host.ViewModels["home"];
        var detailsVm = host.ViewModels["details/{?}"];
        var settingsVm = host.ViewModels["settings"];

        homeVm.NavigatedToCount.Should().Be(2);
        homeVm.NavigatingFromCount.Should().Be(1);

        detailsVm.NavigatedToCount.Should().Be(3);
        detailsVm.NavigatingFromCount.Should().Be(3);

        settingsVm.NavigatedToCount.Should().Be(2);
        settingsVm.NavigatingFromCount.Should().Be(2);

        whenNavigatedCount.Should().Be(7);
    }
}
