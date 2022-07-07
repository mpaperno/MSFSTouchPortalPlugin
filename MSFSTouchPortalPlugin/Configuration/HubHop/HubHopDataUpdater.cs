using MSFSTouchPortalPlugin.Configuration.HubHop;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace MSFSTouchPortalPlugin.Configuration
{
  internal static class HubHopDataUpdater
  {
    const string ApiBaseURL = "https://hubhop-api-mgtm.azure-api.net/api/v1/";
    const string ApiPresetsCmd = "presets?type=json";
    const string ApiMostRecentCmd = "statistics/last";

    public static async Task<bool> CheckForUpdates(DateTime lastUpdate, int timeoutSec = 45) {
      try {
        string latest;
        using (var client = new HttpClient()) {
          client.Timeout = new TimeSpan(0, 0, timeoutSec);
          HttpResponseMessage resp = await client.GetAsync(ApiBaseURL + ApiMostRecentCmd).ConfigureAwait(false);
          if (!resp.IsSuccessStatusCode) {
            Common.Logger?.LogError($"HTTP response error in CheckForUpdates(): {resp.StatusCode} - {resp.ReasonPhrase}; for request: {resp.RequestMessage}");
            return false;  // assume any update request would also fail
          }
          latest = await resp.Content.ReadAsStringAsync().ConfigureAwait(false);

          if (string.IsNullOrWhiteSpace(latest)) {
            Common.Logger?.LogWarning($"CheckForUpdates({lastUpdate}) downloaded data from {ApiBaseURL}{ApiMostRecentCmd} was empty.");
            return true;  // assume we need an update, I guess
          }
        }

        var presets = JsonConvert.DeserializeObject<List<HubHopPreset>>(latest, Common.SerializerSettings);

        if (presets.Count == 0) {
          Common.Logger?.LogWarning($"CheckForUpdates({lastUpdate}) found no deserialized presets in data: [{latest}]");
          return true;  // assume we need an update, I guess
        }

        //Common.Logger?.LogDebug($"Found latest listing:\n      RAW:\n      " + latest + "\n      SERIALIZED:\n      " +
        //  JsonConvert.SerializeObject(presets, Common.SerializerSettings));

        if (DateTime.TryParse(presets[^1].CreatedDate, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AssumeUniversal, out var lastDt)) {
          Common.Logger?.LogDebug($"CheckForUpdates({lastUpdate}) found version with {lastDt.ToUniversalTime()}, updated: {lastDt > lastUpdate}");
          return lastDt > lastUpdate;
        }
        Common.Logger?.LogDebug($"CheckForUpdates({lastUpdate}) could not parse date in {presets[^1].CreatedDate}");
      }
      catch (HttpRequestException e) {
        Common.Logger?.LogError(e, "HTTP exception in CheckForUpdates(): " + e.Message);
        return false;   // assume any update request would also fail
      }
      catch (Exception e) {
        Common.Logger?.LogError(e, "Exception in CheckForUpdates(): " + e.Message);
      }
      return true;  // default to needing an update
    }

    public static async Task<bool> DownloadPresets(string destination = null) {
      if (destination == null)
        destination = Common.PresetsFile;
      try {
        await DownloadSingleFile(new Uri(ApiBaseURL + ApiPresetsCmd), destination).ConfigureAwait(false);
        Common.Logger?.LogDebug($"DownloadPresets({destination}) was successful.");
        return true;
      }
      catch (Exception e) {
        Common.Logger?.LogError(e, "Exception in DownloadPresets(): " + e.Message);
      }
      return false;
    }

    static async Task DownloadSingleFile(Uri uri, string filename) {
      //SecurityProtocolType oldType = ServicePointManager.SecurityProtocol;
      //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
      string tmpFile = filename + ".tmp";
      using var client = new HttpClient();
      using (var fs = new FileStream(tmpFile, FileMode.OpenOrCreate)) {
        client.Timeout = new TimeSpan(0, 0, 60);
        using var s = await client.GetStreamAsync(uri).ConfigureAwait(false);
        await s.CopyToAsync(fs);
      }
      File.Delete(filename);
      File.Move(tmpFile, filename);
      //ServicePointManager.SecurityProtocol = oldType;
    }

  }
}
