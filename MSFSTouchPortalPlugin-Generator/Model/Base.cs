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

  //
  // entry.tp Base Object
  //

  class Configuration
  {
    [Required, RegularExpression(@"^#[A-Fa-f0-9]{6}$")]
    public string ColorDark;
    [Required, RegularExpression(@"^#[A-Fa-f0-9]{6}$")]
    public string ColorLight;
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore), DefaultValue("misc")]
    public string ParentCategory = "misc";
  }

  class Base {
    [Range(1, 6), JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public int? Sdk;
    [Range(1, 10), JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public int? Api;
    [JsonConverter(typeof(VersionNumberJsonConverter))]
    [Required, Range(1, uint.MaxValue)]
    public uint Version;
    [Required, MinLength(5)]
    public string Name;
    [Required, MinLength(5)]
    public string Id;
    [Required, ValidateObject]
    public Configuration Configuration = new();
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public string Plugin_start_cmd = null;
    [ValidateObject]
    public List<TouchPortalCategory> Categories = new();
    public List<TouchPortalSetting> Settings = new();
    // This description text can be used to add information on the top of the plug-in settings page.
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public string SettingsDescription = null;  // api v10
  }

  //
  // Category
  //

  class TouchPortalSubCategory
  {
    [Required, MinLength(5)]
    public string Id ;
    [Required, MinLength(5)]
    public string Name;
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public string Imagepath = null;
  }

  class TouchPortalCategory {
    [Required, MinLength(5)]
    public string Id;
    [Required, MinLength(5)]
    public string Name;
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public string Imagepath = null;
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public List<TouchPortalSubCategory> SubCategories = null;
    public List<TouchPortalAction> Actions = new();
    public List<TouchPortalConnector> Connectors = new();
    public List<TouchPortalEvent> Events = new();
    public List<TouchPortalState> States = new();
  }

  //
  // Actions & Connectors
  //

  // Action text and data fields layout object types (api v7+)

  class TouchPortalLineObject
  {
    // This will be the format of the rendered line in the action.
    public string LineFormat;

    public TouchPortalLineObject(string fmt) { LineFormat = fmt; }
  }

  class TouchPortalLineSuggestion
  {
    // This option will set the width of the first part on a line if it is text [a label].
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public int? FirstLineItemLabelWidth = null;
    // This option will add padding on the left for each line of a multiline format.
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public int? LineIndentation = null;
  }

  class TouchPortalLinesObject {
    // This is the country code of the language this line information contains (or "default").
    [Required, MinLength(2)]
    public string Language = "default";
    // This is the array of line objects representing the lines of the action. This array should have at least 1 entry.
    public List<TouchPortalLineObject> Data;
    // This is a suggestions object where you can specify certain rendering behaviours of the action lines.
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public TouchPortalLineSuggestion Suggestions = new();

    public TouchPortalLinesObject() { }
    public TouchPortalLinesObject(List<TouchPortalLineObject> lines) { Data = lines; }
  }

  class TouchPortalLinesCollection
  {
    // This is the array for lingual action line information. Each entry in this array should represent line information for a specific language.
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public List<TouchPortalLinesObject> Action = null;
    // This is the array for lingual onhold line information.
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public List<TouchPortalLinesObject> Onhold = null;
  }

  // Action data definition
  class TouchPortalActionData
  {
    [Required, MinLength(5)]
    public string Id;
    [Required]
    public string Type;
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public string Label = null;
    [Required, JsonProperty("default")]
    public object DefaultValue;
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public string[] ValueChoices = null;
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore), DefaultValue(double.NaN)]
    public double MinValue = double.NaN;
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore), DefaultValue(double.NaN)]
    public double MaxValue = double.NaN;
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore), DefaultValue(true)]
    public bool AllowDecimals = true;

    // internal use, not for TP
    [JsonIgnore]
    public string FieldLabel = null;
    [JsonIgnore]
    public string FieldSuffix = null;
  }

  // Action and Connector base type
  class TouchPortalActionBase
  {
    [Required, MinLength(5), JsonProperty(Order = -2)]
    public string Id;
    [Required, MinLength(5), JsonProperty(Order = -2)]
    public string Name;
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public string Description = null;  // depr. in api v7
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public string Format = null;  // depr. in api v7
    // This attribute allows you to connect this action to a specified subcategory id.
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public string SubCategoryId = null;  // api v7
    // This is a collection of action data which can be specified by the user.
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public List<TouchPortalActionData> Data = null;
  }

  // Action type specialization
  class TouchPortalAction : TouchPortalActionBase
  {
    [Required, MinLength(2)]
    public string Type;
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public string Prefix = null;  // depr. in api v7
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore), DefaultValue(true)]
    public bool TryInline = true;  // depr. in api v7
    // Can be used in "On Hold" button setup
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public bool HasHoldFunctionality;  // depr. in api v7 but still used due to bug
    // Special formatting when action is used in "On Hold" area
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public string FormatOnHold = null;  // alpha api v7 but left in by mistake
    // This is the object for specifying the action and/or onhold lines.
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public TouchPortalLinesCollection Lines = null;  // api v7
  }

  // Connector type specialization  (currently same as Base)
  class TouchPortalConnector : TouchPortalActionBase { }


  //
  // States
  //

  class TouchPortalState {
    [Required, MinLength(5)]
    public string Id;
    [Required]
    public string Type;
    [Required, JsonProperty("desc")]
    public string Description;
    [Required, JsonProperty("default")]
    public string DefaultValue;
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public string[] ValueChoices = null;
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public string ParentGroup = null;  // api v6
  }

  //
  // Events
  //

  // api v10
  public class TouchPortalLocalState
  {
    // This id of the local state.
    [Required, MinLength(5)]
    public string Id;
    // This name of the local state.
    [Required, MinLength(5)]
    public string Name;
    // The parent category the local state belongs to.
    [Required, MinLength(5)]
    public string ParentCategory;
  }

  public class TouchPortalEvent
  {
    [Required, MinLength(5)]
    public string Id;
    [Required, MinLength(5)]
    public string Name;
    [Required, MinLength(5)]
    public string Format;
    [Required]
    public string Type;
    [Required]
    public string ValueType;
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public string[] ValueChoices = null;
    [Required, MinLength(0)]
    public string ValueStateId;
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public string SubCategoryId = null;  // api v7
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public List<TouchPortalLocalState> Localstates = null;  // api v10
  }

  //
  // Settings
  //

  class TouchPortalTooltip
  {
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public string Title = null;
    [Required, MinLength(10)]
    public string Body;
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public string DocUrl = null;
  }

  class TouchPortalSetting
  {
    [Required, MinLength(5)]
    public string Name;
    [Required]
    public string Type;
    [JsonProperty("default")]
    public string DefaultValue = string.Empty;
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public int? MaxLength;
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore), DefaultValue(double.NaN)]
    public double MinValue = double.NaN;
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore), DefaultValue(double.NaN)]
    public double MaxValue = double.NaN;
    public bool IsPassword = false;
    public bool ReadOnly = false;
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public string[] Choices = null;  // api v10
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public TouchPortalTooltip Tooltip = null;  // api v7
  }


  //
  // JSON validators and formatters
  //

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
