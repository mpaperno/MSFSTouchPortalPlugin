﻿using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace MSFSTouchPortalPlugin.Interfaces {
  /// <summary>
  /// Handles communication with the Touch Portal
  /// </summary>
  internal interface IPluginService : IHostedService {
    new Task StartAsync(CancellationToken cancellationToken);
    new Task StopAsync(CancellationToken cancellationToken);
  }
}
