using Flurl;

namespace P41.Navigation.UnitTests;

public class UrlTests
{
    [Fact]
    public void Should_convert_to_string()
    {
        var navRequest = "api"
            .AppendPathSegment("user")
            .AppendPathSegment("get")
            .AppendPathSegment("panos")
            .SetQueryParam("include", "details")
            .SetQueryParam("picture", "true");

        var expected = "api/user/get/panos?include=details&picture=true";

        navRequest.ToString().Should().Be(expected);
    }

    [Fact]
    public void Should_convert_from_string()
    {
        var navRequest = "api/user/get/panos?include=details&picture=true";

        var expected = "api"
            .AppendPathSegment("user")
            .AppendPathSegment("get")
            .AppendPathSegment("panos")
            .SetQueryParam("include", "details")
            .SetQueryParam("picture", "true");

        Url.Parse(navRequest).Should().Be(expected);
    }

    [Fact]
    public void Should_be_equal()
    {
        var requestLeft = "home".AppendPathSegment("10").SetQueryParam("lang", "en");
        var requestRight = "home".AppendPathSegment("10").SetQueryParam("lang", "en");

        requestLeft.Equals(requestRight).Should().Be(true);
    }

    [Fact]
    public void Should_not_be_equal()
    {
        var requestLeft = "home".AppendPathSegment("10").SetQueryParam("lang", "gr");
        var requestRight = "home".AppendPathSegment("10").SetQueryParam("lang", "en");

        requestLeft.Equals(requestRight).Should().Be(false);
    }
}
