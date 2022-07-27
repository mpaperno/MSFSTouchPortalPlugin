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

namespace MSFSTouchPortalPlugin.Configuration
{
  internal class HubHopPreset
  {
    [SQLite.PrimaryKey]
    public string Id { get; set; }
    public HubHopType PresetType { get; set; }
    public string Vendor { get; set; }
    public string Aircraft { get; set; }
    public string System { get; set; }
    public string CreatedDate { get; set; }  // actually updated date
    public string Path { get; set; }
    public string Code { get; set; }
    public string Label { get; set; }
    public int Version { get; set; }
    public string Status { get; set; }
    public string Description { get; set; }
    public string Author { get; set; }
    public string UpdatedBy { get; set; }
    public int Reported { get; set; }
    public int Score { get; set; }

    // database tracking fields
    public long CreatedDateTS { get; set; }
    public long LastDbUpdate { get; set; }

    // the statistics/last API response for some reason uses "_id" property name
    [Newtonsoft.Json.JsonProperty()]
    private string _id { set => Id = value; }

    public override string ToString() {
      return $"{GetType().Name}: {{{Id}, {PresetType}, {Path}, {CreatedDate}, {Status}, v{Version}}}";
    }


    //[OnError]
    //internal void OnError(StreamingContext context, ErrorContext errorContext) {
    //  //You can check if exception is for a specific member then ignore it
    //  if (errorContext.Member.ToString().CompareTo("SomeObject") == 0) {
    //    errorContext.Handled = true;
    //  }
    //}
  }
}
