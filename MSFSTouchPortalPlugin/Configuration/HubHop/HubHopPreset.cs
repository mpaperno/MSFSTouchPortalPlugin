using SQLite;

namespace MSFSTouchPortalPlugin.Configuration
{
  internal class HubHopPreset
  {
    [PrimaryKey]
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
