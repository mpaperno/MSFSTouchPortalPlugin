using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MSFSTouchPortalPlugin_Generator.Model {
  class Base {
    [Required, Range(1, int.MaxValue)]
    public int sdk { get; set; } = 2;
    [Required, Range(1, int.MaxValue)]
    public int version { get; set; }
    [Required, MinLength(5)]
    public string name { get; set; } = string.Empty;
    [Required, MinLength(5)]
    public string id { get; set; } = string.Empty;
    [Required, ValidateObject]
    public Configuration configuration { get; set; } = new Configuration();
    public string plugin_start_cmd { get; set; } = string.Empty;
    [ValidateObject]
    public List<TouchPortalCategory> categories { get; set; } = new List<TouchPortalCategory>();
  }

  class Configuration {
    [Required, RegularExpression(@"^#[A-Fa-f0-9]{6}$")]
    public string colorDark { get; set; } = "#000000";
    [Required, RegularExpression(@"^#[A-Fa-f0-9]{6}$")]
    public string colorLight { get; set; } = "#23CF5F";
  }

  class TouchPortalCategory {
    [Required, MinLength(5)]
    public string id { get; set; }
    [Required, MinLength(5)]
    public string name { get; set; }
    public string imagepath { get; set; } = string.Empty;
    public List<TouchPortalAction> actions { get; set; } = new List<TouchPortalAction>();
    public List<object> events { get; set; } = new List<object>();
    public List<TouchPortalState> states { get; set; } = new List<TouchPortalState>();
  }

  class TouchPortalAction {
    [Required, MinLength(5)]
    public string id { get; set; }
    [Required, MinLength(5)]
    public string name { get; set; }
    [Required, MinLength(2)]
    public string prefix { get; set; }
    [Required, MinLength(2)]
    public string type { get; set; }
    [Required, MinLength(2)]
    public string description { get; set; }
    public bool tryInline { get; set; } = true;
    [Required, MinLength(2)]
    public string format { get; set; }
    public List<TouchPortalActionData> data { get; set; } = new List<TouchPortalActionData>();
  }

  class TouchPortalActionData {
    [Required, MinLength(5)]
    public string id { get; set; }
    [Required]
    public string type { get; set; }
    [Required]
    public string label { get; set; } = "Action";
    [Required, JsonProperty("default")]
    public string defaultValue { get; set; }
    [Required]
    public string[] valueChoices { get; set; }
  }

  class TouchPortalState {
    [Required, MinLength(5)]
    public string id { get; set; }
    [Required]
    public string type { get; set; }
    [Required, JsonProperty("desc")]
    public string description { get; set; }
    [Required, JsonProperty("default")]
    public string defaultValue { get; set; }
  }

  public class ValidateObjectAttribute : ValidationAttribute {
    protected override ValidationResult IsValid(object value, ValidationContext validationContext) {
      var results = new List<ValidationResult>();
      var context = new ValidationContext(value, null, null);

      Validator.TryValidateObject(value, context, results, true);

      if (results.Count != 0) {
        var compositeResults = new CompositeValidationResult($"Validation for {validationContext.DisplayName} failed!");
        results.ForEach(compositeResults.AddResult);

        return compositeResults;
      }

      return ValidationResult.Success;
    }
  }

  public class CompositeValidationResult : ValidationResult {
    private readonly List<ValidationResult> _results = new List<ValidationResult>();

    public IEnumerable<ValidationResult> Results {
      get {
        return _results;
      }
    }

    public CompositeValidationResult(string errorMessage) : base(errorMessage) { }
    public CompositeValidationResult(string errorMessage, IEnumerable<string> memberNames) : base(errorMessage, memberNames) { }
    protected CompositeValidationResult(ValidationResult validationResult) : base(validationResult) { }

    public void AddResult(ValidationResult validationResult) {
      _results.Add(validationResult);
    }
  }
}
