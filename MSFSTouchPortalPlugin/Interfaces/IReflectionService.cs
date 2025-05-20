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

using MSFSTouchPortalPlugin.Attributes;
using MSFSTouchPortalPlugin.Enums;
using MSFSTouchPortalPlugin.Types;
using System;
using System.Collections.Generic;

namespace MSFSTouchPortalPlugin.Interfaces
{
  internal interface IReflectionService {
    string PluginId { get; set; }
    Dictionary<string, ActionEventType> GetActionEvents();
    Dictionary<string, PluginSetting> GetSettings();
    IEnumerable<TouchPortalActionAttribute> GetActionAttributes(Groups catId);
    IEnumerable<TouchPortalConnectorAttribute> GetConnectorAttributes(Groups catId);
    IEnumerable<TouchPortalCategoryAttribute> GetCategoryAttributes();
    IEnumerable<TouchPortalEvent> GetEvents(Groups catId);
    void InitEvents();
  }
}
