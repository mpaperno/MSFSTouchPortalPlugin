using MSFSTouchPortalPlugin.Attributes;
using System;

namespace MSFSTouchPortalPlugin.Constants {
  public enum Definition {
    Init = 0,
    GroundVelocity,
  }

  public enum Request {
    Init = 0,
    GroundVelocity
  }

  public static class SimVars {
    [SimVarDataRequest]
    public static SimVarItem GroundVelocity = new SimVarItem() { def = Definition.GroundVelocity, req = Request.GroundVelocity, SimVarName = "GROUND VELOCITY", Unit = Units.Feet, CanSet = false };
  }

public class SimVarItem {
    public Definition def;
    public Request req;
    public string SimVarName;
    public string Unit;
    public bool CanSet = false;
    public bool PendingRequest = false;
    public DateTime LastPending = DateTime.Now;
    public string TouchPortalStateMapping;

    public void SetPending(bool val) {
      PendingRequest = val;

      if (val) {
        LastPending = DateTime.Now;
      }
    }

    /// <summary>
    /// If pending for more than 30 seconds, timeout
    /// </summary>
    public void PendingTimeout() {
      if (PendingRequest) {
        if (DateTime.Now > LastPending.AddSeconds(30)) {
          SetPending(false);
        }
      }
    }
  }
}
