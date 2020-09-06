using MSFSTouchPortalPlugin_Generator.Model;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Reflection;
using TouchPortalExtension.Attributes;

namespace MSFSTouchPortalPlugin_Generator {
  class Program {
    private const string _PLUGIN_NAME = "MSFSTouchPortalPlugin";
    static void Main(string[] args) {
      
      // Setup Base Model
      var model = new Base() {
        sdk = 2,
        version = 1,
        name = _PLUGIN_NAME,
        id = _PLUGIN_NAME
      };

      // Add Configuraiton
      // Add Plug Start Comand

      // Load asembly
      var c = MSFSTouchPortalPlugin.Objects.AutoPilot.AutoPilot.AP_AIRSPEED_HOLD;

      // Find assembly
      var a = Assembly.GetExecutingAssembly().GetReferencedAssemblies().Where(a => a.Name == _PLUGIN_NAME).FirstOrDefault();

      if (a == null) {
        throw new Exception("Unable to load assembly for reflection.");
      }

      var assembly = Assembly.Load(a);

      var q = assembly.GetTypes().ToList();

      // Get all classes with the TouchPortalCategory
      var s = q.Where(t => t.CustomAttributes.Any(att => att.AttributeType == typeof(TouchPortalCategoryAttribute))).ToList();

      // For each category, add to model
      s.ForEach(cat => {
        var att = (TouchPortalCategoryAttribute)Attribute.GetCustomAttribute(cat, typeof(TouchPortalCategoryAttribute));
        var category = new TouchPortalCategory() {
          id = $"{_PLUGIN_NAME}.{att.Id}",
          name = att.Name,
          imagepath = att.ImagePath
        };

        // Add actions
        var actions = cat.GetMembers().Where(t => t.CustomAttributes.Any(att => att.AttributeType == typeof(TouchPortalActionAttribute))).ToList();
        actions.ForEach(act => {
          var actionAttribute = (TouchPortalActionAttribute)Attribute.GetCustomAttribute(act, typeof(TouchPortalActionAttribute));
          var action = new TouchPortalAction() {
            id = $"{category.id}.Action.{actionAttribute.Id}",
            name = actionAttribute.Name,
            prefix = actionAttribute.Prefix,
            type = actionAttribute.Type,
            description = actionAttribute.Description,
            tryInline = true,
            format = actionAttribute.Format,
          };

          // TODO: Handle Data


          // Has Choices
          var choiceAttributes = act.GetCustomAttributes<TouchPortalActionChoiceAttribute>()?.ToList();

          if (choiceAttributes.Count > 0) {
            for (int i = 0; i < choiceAttributes.Count; i++) {
              var data = new TouchPortalActionData() {
                id = $"{action.id}.Data.{i}",
                type = "choice",
                label = "Action",
                defaultValue = choiceAttributes[i].DefaultValue,
                valueChoices = choiceAttributes[i].ChoiceValues
              };

              action.data.Add(data);
            }
            action.format = string.Format(action.format, action.data.Select(d => $"{{${d.id}$}}").ToArray());
          }

          category.actions.Add(action);
        });

        // TODO: Add states

        // TODO: Add events

        model.categories.Add(category);
      });

      var context = new ValidationContext(model, null, null);
      var errors = new Collection<ValidationResult>();
      var isValid = Validator.TryValidateObject(model, context, errors, true);

      if (!isValid) {
        throw new AggregateException(
          errors.Select((e) => new ValidationException(e.ErrorMessage)));
      } 

      var result = JsonConvert.SerializeObject(model, Formatting.Indented);
      File.WriteAllText("entry.tp", result);
      Console.WriteLine("entry.tp generated.");
    }
  }
}
