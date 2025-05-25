using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
  public class Command_Abort : Command
  {
    public bool IsPendingOnly { get; private set; } = false;

    public Command_Abort(Int64 _owner_id, bool _is_pending_only)
      : base(_owner_id)
    {
       IsPendingOnly = _is_pending_only;
    }

    protected override void OnEnter()
    {
      throw new NotImplementedException();
    }

    protected override bool OnUpdate()
    {
      throw new NotImplementedException();
    }

    protected override void OnExit(bool _is_abort)
    {
      throw new NotImplementedException();
    }
  }

}

