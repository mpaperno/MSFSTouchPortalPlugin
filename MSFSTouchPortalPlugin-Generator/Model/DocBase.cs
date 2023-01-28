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

using System.Collections.Generic;

namespace MSFSTouchPortalPlugin_Generator.Model {
  public class DocBase {
    public string Title { get; set; }
    public string Overview { get; set; }
    public string Version { get; set; }
    public string DocsUrl { get; set; }
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
    public MSFSTouchPortalPlugin.Enums.Groups CategoryId { get; set; }
    public string Id { get; set; }
    public string Name { get; set; }
    public List<DocAction> Actions { get; set; } = new();
    public List<DocConnector> Connectors { get; set; } = new();
    public List<DocState> States { get; set; } = new();
    public List<DocEvent> Events { get; set; } = new();
  }

  public class DocActionBase
  {
    public string Name { get; set; }
    public string Description { get; set; }
    public string Format { get; set; }
    public List<DocActionData> Data { get; set; } = new();
  }

  public class DocAction : DocActionBase
  {
    public string Type { get; set; }
    public bool HasHoldFunctionality { get; set; } = false;
    public List<DocActionMapping> Mappings { get; set; } = new();
  }

  public class DocConnector : DocActionBase
  { }

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
    public string FormattingString { get; set; }
  }

  public class DocEvent
  {
    public string Id { get; set; }
    public string Name { get; set; }
    public string Format { get; set; }
    public string ValueType { get; set; }
    public virtual string[] ValueChoices { get; set; }
    public string ValueStateId { get; set; }
    public Dictionary<string, string> ChoiceMappings { get; set; }
  }

}
