using Microsoft.Extensions.Hosting;
using MSFSTouchPortalPlugin_Generator.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MSFSTouchPortalPlugin_Generator {
  internal class RunService : IHostedService {
    private readonly IHostApplicationLifetime _hostApplicationLifetime;
    private readonly IGenerateEntry _generateEntry;
    private readonly IGenerateDoc _generateDoc;

    public RunService(IHostApplicationLifetime hostApplicationLifetime, IGenerateEntry generateEntry, IGenerateDoc generateDoc) {
      _hostApplicationLifetime = hostApplicationLifetime ?? throw new ArgumentNullException(nameof(hostApplicationLifetime));
      _generateEntry = generateEntry ?? throw new ArgumentNullException(nameof(generateEntry));
      _generateDoc = generateDoc ?? throw new ArgumentNullException(nameof(generateDoc));
    }

    public Task StartAsync(CancellationToken cancellationToken) {
      _generateEntry.Generate();
      _generateDoc.Generate();
      _hostApplicationLifetime.StopApplication();
      return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) {
      return Task.CompletedTask;
    }
  }
}
