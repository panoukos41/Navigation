using FluentAssertions;
using P41.Navigation;
using P41.Navigation.Host;
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

Test($"{nameof(NavigationRequest)} identical requests should be equal", () =>
{
    var requestLeft = new NavigationRequest("home").AddPath("10").AddQuery("lang", "en");
    var requestRight = new NavigationRequest("home").AddPath("10").AddQuery("lang", "en");

    requestLeft.Equals(requestRight).Should().Be(true);
});

Test($"{nameof(NavigationRequest)} different requests should not be equal", () =>
{
    var requestLeft = new NavigationRequest("home").AddPath("10").AddQuery("lang", "gr");
    var requestRight = new NavigationRequest("home").AddPath("10").AddQuery("lang", "en");

    requestLeft.Equals(requestRight).Should().Be(false);
});

Test($"{nameof(NavigationStack)} should convert to json.", () =>
{
    var stack = new NavigationStack();
    stack.Push(new NavigationRequest("home"));
    stack.Push(new NavigationRequest("details").AddQuery("num", "10"));

    var expected = @"[""details?num=10"",""home""]";
    var actual = stack.ToJson();

    actual.Should().Be(expected);
});

Test($"{nameof(NavigationStack)} should convert from json.", () =>
{
    var json = @"[""details?num=10"",""home""]";

    var expected = new NavigationStack(2);
    expected.Push(new NavigationRequest("home"));
    expected.Push(new NavigationRequest("details").AddQuery("num", "10"));

    var actual = NavigationStack.Parse(json);

    actual.Equals(expected).Should().Be(true);
});

Test($"{nameof(NavigationStack)} identical stacks should be equal", () =>
{
    var stackLeft = new NavigationStack();
    stackLeft.Push(new NavigationRequest("home"));
    stackLeft.Push(new NavigationRequest("details").AddQuery("num", "10"));

    var stackRight = new NavigationStack();
    stackRight.Push(new NavigationRequest("home"));
    stackRight.Push(new NavigationRequest("details").AddQuery("num", "10"));

    stackLeft.Equals(stackRight).Should().Be(true);
});

Test($"{nameof(NavigationStack)} different stacks should not be equal", () =>
{
    var stackLeft = new NavigationStack();
    stackLeft.Push(new NavigationRequest("home"));
    stackLeft.Push(new NavigationRequest("details").AddQuery("num", "10"));

    var stackRight = new NavigationStack();
    stackRight.Push(new NavigationRequest("home"));
    stackRight.Push(new NavigationRequest("details").AddQuery("num", "1"));

    stackLeft.Equals(stackRight).Should().Be(false);
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