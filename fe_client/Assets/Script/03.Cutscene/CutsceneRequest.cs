using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lua;
using Battle;
using Cysharp.Threading.Tasks;
using System.Threading;


public struct CutsceneRequest
{
    public EnumCutsceneRequestEvent RequestType;
    public TAG_INFO                 Detail;


    public static CutsceneRequest Create(EnumCutsceneRequestEvent _request_type, TAG_INFO _detail = default)
    {
        return new CutsceneRequest()
        {
            RequestType = _request_type,
            Detail      = _detail,
        };
    }
}