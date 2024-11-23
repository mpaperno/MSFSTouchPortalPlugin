using Microsoft.Extensions.Hosting;
using MSFSTouchPortalPlugin_Generator.Configuration;
using MSFSTouchPortalPlugin_Generator.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MSFSTouchPortalPlugin_Generator {
  internal class RunService : IHostedService {
    private readonly IHostApplicationLifetime _hostApplicationLifetime;
    private readonly GeneratorOptions _options;
    private readonly IGenerateEntry _generateEntry;
    private readonly IGenerateDoc _generateDoc;

    public RunService(IHostApplicationLifetime hostApplicationLifetime, GeneratorOptions options, IGenerateEntry generateEntry, IGenerateDoc generateDoc) {
      _hostApplicationLifetime = hostApplicationLifetime ?? throw new ArgumentNullException(nameof(hostApplicationLifetime));
      _options = options ?? throw new ArgumentNullException(nameof(options));
      _generateEntry = generateEntry ?? throw new ArgumentNullException(nameof(generateEntry));
      _generateDoc = generateDoc ?? throw new ArgumentNullException(nameof(generateDoc));
    }

    public Task StartAsync(CancellationToken cancellationToken) {
      var g = new List<string>(_options.Generate);
      if (g.Contains("entry"))
        _generateEntry.Generate();
      if (g.Contains("doc"))
        _generateDoc.Generate();
      _hostApplicationLifetime.StopApplication();
      return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) {
      return Task.CompletedTask;
    }
  }
}
