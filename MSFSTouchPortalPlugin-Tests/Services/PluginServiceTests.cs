using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using MSFSTouchPortalPlugin.Configuration;
using MSFSTouchPortalPlugin.Interfaces;
using MSFSTouchPortalPlugin.Services;
using System;
using System.Threading;
using System.Threading.Tasks;
using TouchPortalSDK;
using TouchPortalSDK.Interfaces;
using Xunit;

namespace MSFSTouchPortalPlugin_Tests.Services {
  public class PluginServiceTests {
    [Fact]
    public void Constructor_ShouldCreateAndDispose() {
      // arrange
      var mockHostApplicationLifetime = new Mock<IHostApplicationLifetime>();
      var mockILoggerPluginService = new NullLogger<PluginService>();
      var mockITouchPortalClientFactory = new Mock<ITouchPortalClientFactory>();
      var mockITouchPortalClient = new Mock<ITouchPortalClient>();
      mockITouchPortalClientFactory.Setup(c => c.Create(It.IsAny<ITouchPortalEventHandler>())).Returns(mockITouchPortalClient.Object);
      var mockISimConnectService = new Mock<ISimConnectService>();
      var mockIReflectionService = new Mock<IReflectionService>();
      var mockPluginConfig = new Mock<PluginConfig>(new object[] { new NullLogger<PluginConfig>() });

      // act
      var service = new PluginService(mockHostApplicationLifetime.Object, mockILoggerPluginService, mockITouchPortalClientFactory.Object, mockISimConnectService.Object, mockIReflectionService.Object, mockPluginConfig.Object);

      // assert
      Assert.NotNull(service);
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNull() {
      // arrange
      var mockHostApplicationLifetime = new Mock<IHostApplicationLifetime>();
      var mockILoggerPluginService = new NullLogger<PluginService>();
      var mockITouchPortalClientFactory = new Mock<ITouchPortalClientFactory>();
      var mockISimConnectService = new Mock<ISimConnectService>();
      var mockIReflectionService = new Mock<IReflectionService>();
      var mockPluginConfig = new Mock<PluginConfig>(new object[] { new NullLogger<PluginConfig>() });

      // act/assert
      Assert.Throws<ArgumentNullException>(() => new PluginService(null, mockILoggerPluginService, mockITouchPortalClientFactory.Object, mockISimConnectService.Object, mockIReflectionService.Object, mockPluginConfig.Object));
      Assert.Throws<ArgumentNullException>(() => new PluginService(mockHostApplicationLifetime.Object, null, mockITouchPortalClientFactory.Object, mockISimConnectService.Object, mockIReflectionService.Object, mockPluginConfig.Object));
      Assert.Throws<ArgumentNullException>(() => new PluginService(mockHostApplicationLifetime.Object, mockILoggerPluginService, null, mockISimConnectService.Object, mockIReflectionService.Object, mockPluginConfig.Object));
      Assert.Throws<ArgumentNullException>(() => new PluginService(mockHostApplicationLifetime.Object, mockILoggerPluginService, mockITouchPortalClientFactory.Object, null, mockIReflectionService.Object, mockPluginConfig.Object));
      Assert.Throws<ArgumentNullException>(() => new PluginService(mockHostApplicationLifetime.Object, mockILoggerPluginService, mockITouchPortalClientFactory.Object, mockISimConnectService.Object, null, mockPluginConfig.Object));
      Assert.Throws<ArgumentNullException>(() => new PluginService(mockHostApplicationLifetime.Object, mockILoggerPluginService, mockITouchPortalClientFactory.Object, mockISimConnectService.Object, mockIReflectionService.Object, null));
    }

    [Fact]
    public void PluginId_ShouldBeExpected() {
      // arrange
      var mockHostApplicationLifetime = new Mock<IHostApplicationLifetime>();
      var mockILoggerPluginService = new NullLogger<PluginService>();
      var mockITouchPortalClientFactory = new Mock<ITouchPortalClientFactory>();
      var mockITouchPortalClient = new Mock<ITouchPortalClient>();
      mockITouchPortalClientFactory.Setup(c => c.Create(It.IsAny<ITouchPortalEventHandler>())).Returns(mockITouchPortalClient.Object);
      var mockISimConnectService = new Mock<ISimConnectService>();
      var mockIReflectionService = new Mock<IReflectionService>();
      var mockPluginConfig = new Mock<PluginConfig>(new object[] { new NullLogger<PluginConfig>() });

      // act
      var service = new PluginService(mockHostApplicationLifetime.Object, mockILoggerPluginService, mockITouchPortalClientFactory.Object, mockISimConnectService.Object, mockIReflectionService.Object, mockPluginConfig.Object);

      // assert
      Assert.Equal("MSFSTouchPortalPlugin", service.PluginId);
    }

    [Fact]
    public async Task StopAsync_ShouldComplete() {
      // arrange
      var mockHostApplicationLifetime = new Mock<IHostApplicationLifetime>();
      var mockILoggerPluginService = new NullLogger<PluginService>();
      var mockITouchPortalClientFactory = new Mock<ITouchPortalClientFactory>();
      var mockITouchPortalClient = new Mock<ITouchPortalClient>();
      mockITouchPortalClientFactory.Setup(c => c.Create(It.IsAny<ITouchPortalEventHandler>())).Returns(mockITouchPortalClient.Object);
      var mockISimConnectService = new Mock<ISimConnectService>();
      var mockIReflectionService = new Mock<IReflectionService>();
      var mockPluginConfig = new Mock<PluginConfig>(new object[] { new NullLogger<PluginConfig>() });

      // act
      var service = new PluginService(mockHostApplicationLifetime.Object, mockILoggerPluginService, mockITouchPortalClientFactory.Object, mockISimConnectService.Object, mockIReflectionService.Object, mockPluginConfig.Object);

      Assert.NotNull(service);
      await service.StopAsync(new CancellationToken());
    }
  }
}
