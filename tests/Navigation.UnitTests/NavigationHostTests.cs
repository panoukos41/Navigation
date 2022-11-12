using Flurl;
using P41.Navigation.UnitTests.Util;
using System.Runtime.InteropServices;

namespace P41.Navigation.UnitTests;

public class NavigationHostTests
{
    [Fact]
    public void Should_Navigate_Correctly()
    {
        // Arrange
        var host = new TestHost()
            .Map("home", () => new("home"))
            .Map("details/{?}", () => new("details"))
            .Map("settings", () => new("settings"));

        Url requestHome = "home";
        Url requestDetails = "details";
        Url requestDetails2 = "details/panos";
        Url requestSettings = "settings?sub=false";

        var whenNavigatedCount = 0;
        host.WhenNavigated(request => whenNavigatedCount += 1);

        // Act
        // Navigated to home 1
        host.Navigate(requestHome);
        host.Navigate(requestHome);

        //ShouldBe(requestHome, host.Views[0]);

        // Navigated to details 1, Navigating from home 1
        host.Navigate(requestDetails2);
        host.Navigate(requestDetails2);

        // Navigated to settings 1, Navigating from details 1
        host.Navigate(requestSettings);
        host.Navigate(requestSettings);

        // Navigated to details 2, Navigating from settings 1
        host.Navigate(requestDetails);
        host.Navigate(requestDetails);

        // Navigated to settings 2, Navigating from details 2
        host.GoBack();
        host.CurrentRequest.Should().Be(requestSettings);

        // Navigated to details 3, Navigating from settings 2
        host.GoBack();
        host.CurrentRequest.Should().Be(requestDetails2);

        // Navigated to home 2, Navigating from details 3
        host.GoBack();
        host.CurrentRequest.Should().Be(requestHome);

        // Returns root multiple times, should not change Count, should return root request.
        host.GoBack();
        host.Count.Should().Be(1);

        host.GoBack();
        host.Count.Should().Be(1);

        host.GoBack();
        host.Count.Should().Be(1);

        whenNavigatedCount.Should().Be(7);

        void ShouldBe(Url request, TestView view)
        {
            host.CurrentRequest.Should().Be(request);
            host.CurrentView.Should().Be(view);
        }
    }
}
