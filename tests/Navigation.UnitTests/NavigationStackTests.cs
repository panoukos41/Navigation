using FluentAssertions;
using Flurl;
using P41.Navigation.Host;
using Xunit;

namespace P41.Navigation.UnitTests;

public class NavigationStackTests
{
    [Fact]
    public void Should_convert_to_json()
    {
        var stack = new NavigationStack();
        stack.Push("home");
        stack.Push("details".SetQueryParam("num", "10"));

        var expected = @"[""details?num=10"",""home""]";
        var actual = stack.ToJson();

        actual.Should().Be(expected);
    }

    [Fact]
    public void Should_convert_from_json()
    {
        var json = @"[""details?num=10"",""home""]";

        var expected = new NavigationStack(2);
        expected.Push("home");
        expected.Push("details".SetQueryParam("num", "10"));

        var actual = NavigationStack.Parse(json);

        actual.Equals(expected).Should().Be(true);
    }

    [Fact]
    public void Should_be_equal()
    {
        var stackLeft = new NavigationStack();
        stackLeft.Push("home");
        stackLeft.Push("details".SetQueryParam("num", "10"));

        var stackRight = new NavigationStack();
        stackRight.Push("home");
        stackRight.Push("details".SetQueryParam("num", "10"));

        stackLeft.Equals(stackRight).Should().Be(true);
    }

    [Fact]
    public void Should_not_be_equal()
    {
        var stackLeft = new NavigationStack();
        stackLeft.Push("home");
        stackLeft.Push("details".SetQueryParam("num", "10"));

        var stackRight = new NavigationStack();
        stackRight.Push("home");
        stackRight.Push("details".SetQueryParam("num", "1"));

        stackLeft.Equals(stackRight).Should().Be(false);
    }
}
