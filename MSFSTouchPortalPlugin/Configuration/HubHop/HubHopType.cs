/*
This file is part of the MSFS Touch Portal Plugin project.
https://github.com/mpaperno/MSFSTouchPortalPlugin

COPYRIGHT: (c) Maxim Paperno; All Rights Reserved.

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

using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace MSFSTouchPortalPlugin.Configuration
{
  // note the EnumMember attribute Values need to match the HubHop API names.
  public enum HubHopType : ushort
  {
    None = 0,

    [Display(Name = "Output Value", Description = "The value of a variable or result of a calculation.")]
    [EnumMember(Value = "Output")]
    Output             = 0x01,

    [Display(Name = "Input Key", Description = "An input type event taking no value, like a key press.")]
    [EnumMember(Value = "Input")]
    Input              = 0x02,

    [Display(Name = "Input with Value", Description = "An input type event taking a value.")]
    [EnumMember(Value = "Input (Potentiometer)")]
    InputPotentiometer = 0x04,

    [Display(Name = "All Input Types", Description = "Both kinda of input events, with and w/out a value.")]
    [EnumMember(Value="All Inputs")]
    AllInputs          = Input | InputPotentiometer,

    [Display(Name = "Any Type", Description = "Any/all events.")]
    //[EnumMember(Value="Any")]  // not used in the HH API
    Any                = AllInputs | Output
  }
}
