/*
This file is part of the MSFS Touch Portal Plugin project.
https://github.com/mpaperno/MSFSTouchPortalPlugin

COPYRIGHT:
(c) 2020 Tim Lewis;
(c) Maxim Paperno; All Rights Reserved.

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
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MSFSTouchPortalPlugin_Generator.Model {
  class Base {
    [Required, Range(1, int.MaxValue)]
    public int Sdk { get; set; } = 3;
    [JsonConverter(typeof(VersionNumberJsonConverter))]
    [Required, Range(1, uint.MaxValue)]
    public uint Version { get; set; }
    [Required, MinLength(5)]
    public string Name { get; set; } = string.Empty;
    [Required, MinLength(5)]
    public string Id { get; set; } = string.Empty;
    [Required, ValidateObject]
    public Configuration Configuration { get; set; } = new Configuration();
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    [DefaultValue(null)]
    public string Plugin_start_cmd { get; set; } = null;
    [ValidateObject]
    public List<TouchPortalCategory> Categories { get; set; } = new();
    public List<TouchPortalSetting> Settings { get; set; } = new();
  }

  class Configuration {
    [Required, RegularExpression(@"^#[A-Fa-f0-9]{6}$")]
    public string ColorDark { get; set; }
    [Required, RegularExpression(@"^#[A-Fa-f0-9]{6}$")]
    public string ColorLight { get; set; }
  }

  class TouchPortalCategory {
    [Required, MinLength(5)]
    public string Id { get; set; }
    [Required, MinLength(5)]
    public string Name { get; set; }
    public string Imagepath { get; set; } = string.Empty;
    public List<TouchPortalAction> Actions { get; set; } = new();
    public List<TouchPortalConnector> Connectors { get; set; } = new();
    public List<TouchPortalEvent> Events { get; set; } = new();
    public List<TouchPortalState> States { get; set; } = new();
  }

  class TouchPortalActionBase
  {
    [Required, MinLength(5)]
    [JsonProperty(Order = -2)]
    public string Id { get; set; }
    [Required, MinLength(5)]
    [JsonProperty(Order = -2)]
    public string Name { get; set; }
    [Required, MinLength(2)]
    public string Format { get; set; }
    public List<TouchPortalActionData> Data { get; set; } = new List<TouchPortalActionData>();
  }

  class TouchPortalAction : TouchPortalActionBase
  {
    [Required, MinLength(2)]
    public string Prefix { get; set; }
    [Required, MinLength(2)]
    public string Type { get; set; }
    [Required, MinLength(2)]
    public string Description { get; set; }
    public bool TryInline { get; set; } = true;
    public bool HasHoldFunctionality { get; set; } = false;
  }

  class TouchPortalConnector : TouchPortalActionBase
  { }

  class TouchPortalActionData {
    [Required, MinLength(5)]
    public string Id { get; set; }
    [Required]
    public string Type { get; set; }
    [Required]
    public string Label { get; set; } = "Action";
    [Required, JsonProperty("default")]
    public object DefaultValue { get; set; } = null;
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public string[] ValueChoices { get; set; }
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    [DefaultValue(double.NaN)]
    public double MinValue { get; set; } = double.NaN;
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    [DefaultValue(double.NaN)]
    public double MaxValue { get; set; } = double.NaN;
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    [DefaultValue(true)]
    public bool AllowDecimals { get; set; } = true;
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

  public class TouchPortalEvent
  {
    [Required, MinLength(5)]
    public string Id { get; set; }
    [Required, MinLength(5)]
    public string Name { get; set; }
    [Required, MinLength(5)]
    public string Format { get; set; }
    [Required]
    public string Type { get; set; }
    [Required]
    public string ValueType { get; set; }
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public virtual string[] ValueChoices { get; set; }
    [Required, MinLength(5)]
    public string ValueStateId { get; set; }
  }

  class TouchPortalSetting
  {
    [Required, MinLength(5)]
    public string Name { get; set; }
    [Required]
    public string Type { get; set; }
    [JsonProperty("default")]
    public string DefaultValue { get; set; } = string.Empty;
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public int MaxLength { get; set; } = 0;
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    [DefaultValue(double.NaN)]
    public double MinValue { get; set; } = double.NaN;
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    [DefaultValue(double.NaN)]
    public double MaxValue { get; set; } = double.NaN;
    public bool IsPassword { get; set; } = false;
    public bool ReadOnly { get; set; } = false;
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

  public sealed class VersionNumberJsonConverter : JsonConverter
  {
    public override bool CanConvert(System.Type objectType) {
      return typeof(uint).Equals(objectType);
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
      writer.WriteValue(uint.Parse($"{value:X}"));
    }

    public override object ReadJson(JsonReader reader, System.Type objectType, object existingValue, JsonSerializer serializer) {
      throw new System.NotImplementedException();
    }
  }
}
