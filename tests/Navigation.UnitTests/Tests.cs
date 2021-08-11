using FluentAssertions;
using P41.Navigation;
using P41.Navigation.UnitTests.Util;
using System.Reactive.Linq;
using static Hallstatt.TestController;

Test($"{nameof(NavigationRequest)} should convert to string.", () =>
{
    var navRequest = new NavigationRequest("api")
        .AddPath("user")
        .AddPath("get")
        .AddPath("panos")
        .AddQuery("include", "details")
        .AddQuery("picture", "true");

    var expected = "api/user/get/panos?include=details&picture=true";

    navRequest.ToString().Should().Be(expected);
});

Test($"{nameof(NavigationRequest)} should convert from string.", () =>
{
    var navRequest = "api/user/get/panos?include=details&picture=true";

    var expected = new NavigationRequest("api")
        .AddPath("user")
        .AddPath("get")
        .AddPath("panos")
        .AddQuery("include", "details")
        .AddQuery("picture", "true");

    NavigationRequest.Parse(navRequest).Should().Be(expected);
});

Test($"{nameof(INavigationHost)} scenario, should navigate to/from and count events.", async () =>
{
    // Arrange
    var host = new TestHost();
    var requestHome = new NavigationRequest("home");
    var requestDetails = new NavigationRequest("details").AddPath("panos");
    var requestSettings = new NavigationRequest("settings").AddQuery("sub", false);

    var whenNavigatedCount = 0;
    host.WhenNavigated(request => whenNavigatedCount += 1);

    // Act
    // Navigated to home 1
    await host.Push(requestHome);
    await host.Push(requestHome);

    // Navigated to details 1, Navigating from home 1
    await host.Push(requestDetails);
    await host.Push(requestDetails);

    // Navigated to settings 1, Navigating from details 1
    await host.Push(requestSettings);
    await host.Push(requestSettings);

    // Navigated to details 2, Navigating from settings 1
    await host.Push(requestDetails);

    // Navigated to settings 2, Navigating from details 2
    (await host.Pop()).Should().Be(requestDetails);

    // Navigated to details 3, Navigating from settings 2
    (await host.Pop()).Should().Be(requestSettings);

    // Navigated to home 2, Navigating from details 3
    (await host.Pop()).Should().Be(requestDetails);

    // Returns root multiple times, should not change Count, should return root request.
    (await host.Pop()).Should().Be(requestHome);
    host.Count.Should().Be(1);

    (await host.Pop()).Should().Be(requestHome);
    host.Count.Should().Be(1);

    (await host.Pop()).Should().Be(requestHome);
    host.Count.Should().Be(1);

    // Assert
    var homeVm = host.ViewModels[requestHome.Page];
    var detailsVm = host.ViewModels[requestDetails.Page];
    var settingsVm = host.ViewModels[requestSettings.Page];

    homeVm.NavigatedToCount.Should().Be(2);
    homeVm.NavigatingFromCount.Should().Be(1);

    detailsVm.NavigatedToCount.Should().Be(3);
    detailsVm.NavigatingFromCount.Should().Be(3);

    settingsVm.NavigatedToCount.Should().Be(2);
    settingsVm.NavigatingFromCount.Should().Be(2);

    whenNavigatedCount.Should().Be(7);
});