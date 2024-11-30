/*
This file is part of the MSFS Touch Portal Plugin project.
https://github.com/mpaperno/MSFSTouchPortalPlugin

COPYRIGHT: (c) Maxim Paperno; All Rights Reserved.

This file may be used under the terms of the GNU General Public License (GPL)
as published by the Free Software Foundation, either version 3 of the Licenses,
or (at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

A copy of the GNU GPL is included with this project
and is also available at <http://www.gnu.org/licenses/>.
*/

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace MSFSTouchPortalPlugin.Helpers
{

  internal struct VersionCheckResult
  {
    public Version ReleaseVersion;
    public DateTime ReleaseDate;
    public string ReleaseName;
    public string ReleaseNotes;
    public string ReleaseUrl;
    public string ErrorMessage;
    public bool ReleaseIsNewer;
    public bool PreRelease;

    public override string ToString()
    {
      if (!string.IsNullOrEmpty(ErrorMessage))
        return $"VersionCheckResult(Error: {ErrorMessage})";
      if (ReleaseVersion == null)
        return "VersionCheckResult(Release information is unknown)";
      return $"VersionCheckResult(Release Version: {ReleaseVersion}; Date: {ReleaseDate}; Name: {ReleaseName}; URL: {ReleaseUrl}; Newer? {ReleaseIsNewer}; Pre-release? {PreRelease})";
    }
  }

  internal delegate void VersionCheckResultHandler(VersionCheckResult result);

  internal static class GitHubVersionCheck
  {
    const string ApiBaseURL = "https://api.github.com/repos/";
    const string ApiReleasesEndpoint = "releases";

    public static event VersionCheckResultHandler OnVersionCheckResult;

    public static async Task<VersionCheckResult> CheckForUpdates(string ownerRepo, Version currentVersion, int timeoutSec = 45)
    {
      VersionCheckResult result = new() {
        ReleaseIsNewer = false
      };

      try {
        string releasesJson;
        string apiUrl = $"{ApiBaseURL}{ownerRepo}/{ApiReleasesEndpoint}";
        using (var client = new HttpClient()) {
          client.Timeout = new TimeSpan(0, 0, timeoutSec);
          client.DefaultRequestHeaders.Add("Accept", "application/vnd.github+json");
          client.DefaultRequestHeaders.Add("X-GitHub-Api-Version", "2022-11-28");
          client.DefaultRequestHeaders.Add("User-Agent", ownerRepo);
          HttpResponseMessage resp = await client.GetAsync(apiUrl).ConfigureAwait(false);
          if (!resp.IsSuccessStatusCode) {
            result.ErrorMessage = string.Format("HTTP response error in CheckForUpdates(): {0} - {1}; for request: {2}", resp.StatusCode, resp.ReasonPhrase, resp.RequestMessage);
            return result;
          }
          releasesJson = await resp.Content.ReadAsStringAsync().ConfigureAwait(false);

          if (string.IsNullOrWhiteSpace(releasesJson)) {
            result.ErrorMessage = string.Format("CheckForUpdates() downloaded data from '{0}' was empty.", apiUrl);
            return result;
          }
        }

        JArray releases = (JArray)JsonConvert.DeserializeObject(releasesJson, typeof(JArray));  //Object<List<HubHopPreset>>(releasesJson, Common.SerializerSettings);

        if (releases.Type != JTokenType.Array || releases.Count == 0) {
          result.ErrorMessage = string.Format("CheckForUpdates() found no releases at URL '{0}'", apiUrl);
          return result;
        }

        JObject latest = (JObject)releases.First;
        if (latest.Type != JTokenType.Object || !latest.ContainsKey("tag_name")) {
          result.ErrorMessage = string.Format("CheckForUpdates() found invalid latest release data at URL '{0}'", apiUrl);
          return result;
        }

        if (!Version.TryParse(System.Text.RegularExpressions.Regex.Replace(latest.Property("tag_name").Value.ToString(), @"[^\d\.]", string.Empty), out result.ReleaseVersion)) {
          result.ErrorMessage = string.Format("Error parsing latest release version from result '{0}'", latest.Property("tag_name").ToString());
          return result;
        }

        result.ReleaseName = latest.Property("name").Value.ToString();
        result.ReleaseNotes = latest.Property("body").Value.ToString();
        result.ReleaseUrl = latest.Property("html_url").Value.ToString();
        result.ReleaseIsNewer = result.ReleaseVersion.CompareTo(currentVersion) > 0;
        result.PreRelease = (bool)latest.Property("prerelease").Value;
        DateTime.TryParse(latest.Property("published_at").Value.ToString(), out result.ReleaseDate);

        OnVersionCheckResult?.Invoke(result);
        return result;

      }
      catch (Exception e) {
        result.ErrorMessage = string.Format("Exception in CheckForUpdates(): {0}", e.Message);
      }
      return result;
    }

  }
}
