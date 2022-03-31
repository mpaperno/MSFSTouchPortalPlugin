using System.Collections.Generic;

namespace MSFSTouchPortalPlugin_Generator.Model {
  public class DocBase {
    public string Title { get; set; }
    public string Overview { get; set; }
    public List<DocSetting> Settings { get; set; } = new();
    public List<DocCategory> Categories { get; set; } = new();
  }

  public class DocSetting {
    public string Name { get; set; }
    public string Description { get; set; }
    public string Type { get; set; }
    public string DefaultValue { get; set; }
    public int MaxLength { get; set; }
    public double MinValue { get; set; }
    public double MaxValue { get; set; }
    public bool IsPassword { get; set; }
    public bool ReadOnly { get; set; }
  }

  public class DocCategory {
    public string Name { get; set; }
    public List<DocAction> Actions { get; set; } = new();
    public List<DocState> States { get; set; } = new();
    public List<object> Events { get; set; } = new();
  }

  public class DocAction {
    public string Name { get; set; }
    public string Description { get; set; }
    public string Type { get; set; }
    public string Format { get; set; }
    public bool HasHoldFunctionality { get; set; } = false;
    public List<DocActionData> Data { get; set; } = new();
    public List<DocActionMapping> Mappings { get; set; } = new();
  }

  public class DocActionData {
    public string Type { get; set; }
    public string Values { get; set; }
    public string DefaultValue { get; set; }
    public double MinValue { get; set; }
    public double MaxValue { get; set; }
    public bool AllowDecimals { get; set; }
  }

  public class DocActionMapping
  {
    public string ActionId { get; set; }
    public string[] Values { get; set; }
  }

  public class DocState {
    public string Id { get; set; }
    public string Type { get; set; }
    public string Description { get; set; }
    public string DefaultValue { get; set; }
    public string SimVarName { get; set; }
    public string Unit { get; set; }
  }
}
