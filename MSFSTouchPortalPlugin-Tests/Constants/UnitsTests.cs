using MSFSTouchPortalPlugin.Constants;
using Xunit;

namespace MSFSTouchPortalPlugin_Tests.Constants {
  public class UnitsTests {
    [Fact]
    public void IsRealType_ShouldReturnTrue() {
      // arrange
      // act
      var result = Units.IsRealType("degrees");
      // assert
      Assert.True(result);
    }

    [Fact]
    public void IsRealType_ShouldReturnFalse() {
      // arrange
      // act
      var result = Units.IsRealType("Boolean");
      // assert
      Assert.False(result);
    }

    [Fact]
    public void IsStringType_ShouldReturnTrue() {
      var result = Units.IsStringType("String");
      Assert.True(result);
    }

    [Fact]
    public void IsIntegralType_ShouldReturnTrue() {
      var result = Units.IsIntegralType("position 16k");
      Assert.True(result);
    }

    [Fact]
    public void IsIntegralType_ShouldReturnFalse() {
      var result = Units.IsIntegralType("Bool");
      Assert.False(result);
    }

    [Fact]
    public void IsBooleanType_ShouldReturnTrue() {
      var result = Units.IsBooleanType("Boolean");
      Assert.True(result);
    }

  }
}
