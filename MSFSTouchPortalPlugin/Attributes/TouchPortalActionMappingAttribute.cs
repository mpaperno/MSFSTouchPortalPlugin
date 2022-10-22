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

using MSFSTouchPortalPlugin.Enums;
using System;

namespace MSFSTouchPortalPlugin.Attributes
{
  [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
  public class TouchPortalActionMappingAttribute : Attribute
  {
    public Enum EnumId { get; set; } = default;
    public string ActionId { get; set; }
    public string[] Values { get; set; } = Array.Empty<string>();
    public uint[] EventValues { get; set; } = null;
    public bool Deprecated { get; set; } = false;  // exclude from generated entry.tp and docs if true, but preserve mapping for backwards compat.

    public TouchPortalActionMappingAttribute(string actionId, string value, uint eventValue = 0) :
      this(actionId, new [] { value }, eventValue) { }
    public TouchPortalActionMappingAttribute(string actionId, string value1, string value2, uint eventValue = 0) :
      this(actionId, new[] { value1, value2 }, eventValue) { }
    public TouchPortalActionMappingAttribute(string actionId, string value1, string value2, string value3, uint eventValue = 0) :
      this(actionId, new[] { value1, value2, value3 }, eventValue) { }

    public TouchPortalActionMappingAttribute(string actionId, string[] values, uint eventValue) :
      this(actionId, values, eventValue > 0 ? new[] { eventValue } : null) { }

    public TouchPortalActionMappingAttribute(string actionId, string[] values = null, uint[] eventValues = null)
    {
      ActionId = actionId;
      Values = values ?? Array.Empty<string>();
      EventValues = eventValues;
    }

    public TouchPortalActionMappingAttribute(PluginActions actionId, string value) : this(actionId, new[] { value }) { }
    public TouchPortalActionMappingAttribute(PluginActions actionId, string[] values = null) : this(actionId.ToString(), values) {
      EnumId = actionId;
    }

  }
}
