using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MSFSTouchPortalPlugin_Generator.Model {
  class Base {
    [Required, Range(1, int.MaxValue)]
    public int Sdk { get; set; } = 3;
    [Required, Range(1, int.MaxValue)]
    public int Version { get; set; }
    [Required, MinLength(5)]
    public string Name { get; set; } = string.Empty;
    [Required, MinLength(5)]
    public string Id { get; set; } = string.Empty;
    [Required, ValidateObject]
    public Configuration Configuration { get; set; } = new Configuration();
    public string Plugin_start_cmd { get; set; } = string.Empty;
    [ValidateObject]
    public List<TouchPortalCategory> Categories { get; set; } = new List<TouchPortalCategory>();
    public List<object> Settings { get; set; } = new List<object>();
  }

  class Configuration {
    [Required, RegularExpression(@"^#[A-Fa-f0-9]{6}$")]
    public string ColorDark { get; set; } = "#000000";
    [Required, RegularExpression(@"^#[A-Fa-f0-9]{6}$")]
    public string ColorLight { get; set; } = "#23CF5F";
  }

  class TouchPortalCategory {
    [Required, MinLength(5)]
    public string Id { get; set; }
    [Required, MinLength(5)]
    public string Name { get; set; }
    public string Imagepath { get; set; } = string.Empty;
    public List<TouchPortalAction> Actions { get; set; } = new List<TouchPortalAction>();
    public List<object> Events { get; set; } = new List<object>();
    public List<TouchPortalState> States { get; set; } = new List<TouchPortalState>();
  }

  class TouchPortalAction {
    [Required, MinLength(5)]
    public string Id { get; set; }
    [Required, MinLength(5)]
    public string Name { get; set; }
    [Required, MinLength(2)]
    public string Prefix { get; set; }
    [Required, MinLength(2)]
    public string Type { get; set; }
    [Required, MinLength(2)]
    public string Description { get; set; }
    public bool TryInline { get; set; } = true;
    [Required, MinLength(2)]
    public string Format { get; set; }
    public List<TouchPortalActionData> Data { get; set; } = new List<TouchPortalActionData>();
  }

  class TouchPortalActionData {
    [Required, MinLength(5)]
    public string Id { get; set; }
    [Required]
    public string Type { get; set; }
    [Required]
    public string Label { get; set; } = "Action";
    [Required, JsonProperty("default")]
    public string DefaultValue { get; set; }
    [Required]
    public string[] ValueChoices { get; set; }
  }

  class TouchPortalState {
    [Required, MinLength(5)]
    public string Id { get; set; }
    [Required]
    public string Type { get; set; }
    [Required, JsonProperty("desc")]
    public string Description { get; set; }
    [Required, JsonProperty("default")]
    public string DefaultValue { get; set; }
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
