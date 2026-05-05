using Xunit;

namespace Servus.Tests.Application;

public class ServusConstantsTests
{
    [Fact]
    public void LogoTests()
    {
        Assert.Contains("servus!", global::Servus.Application.ServusLogo.Logo);
    }

    [Fact]
    public void LogoSmallTests()
    {
        Assert.Contains("servus!", global::Servus.Application.ServusLogo.LogoSmall);
    }

    [Fact]
    public void LogoTinyTests()
    {
        Assert.Contains("servus!", global::Servus.Application.ServusLogo.LogoTiny);
    }
}