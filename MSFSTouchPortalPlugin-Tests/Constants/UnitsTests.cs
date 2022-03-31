using MSFSTouchPortalPlugin.Constants;
using Xunit;

namespace MSFSTouchPortalPlugin_Tests.Constants {
  public class UnitsTests {
    [Fact]
    public void ShouldConvertToFloat_ShouldReturnTrue() {
      // arrange
      // act
      var result = Units.IsRealType(Units.degrees);
      // assert
      Assert.True(result);
    }

    [Fact]
    public void ShouldConvertToFloat_ShouldReturnFalse() {
      // arrange
      // act
      var result = Units.IsRealType(Units.Boolean);
      // assert
      Assert.False(result);
    }

  }
}
