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
  [AttributeUsage(AttributeTargets.All)]
  public class TouchPortalCategoryAttribute : Attribute
  {
    public Groups Id { get; set; }
    public string Name { get; set; }
    public string Imagepath { get; set; }

    public TouchPortalActionAttribute[] Actions { get; set; } = Array.Empty<TouchPortalActionAttribute>();
    public object[] States { get; set; } = Array.Empty<object>();
    public object[] Events { get; set; } = Array.Empty<object>();
    public TouchPortalConnectorAttribute[] Connectors { get; set; } = Array.Empty<TouchPortalConnectorAttribute>();

    public TouchPortalCategoryAttribute(Groups id) {
      Id = id;
      Name = Constants.Categories.CategoryName(id);
      Imagepath = Constants.Categories.CategoryImage(id);
    }
  }
}
