using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;


public class HUD_Unit : HUDBase
{
    public class PARAM : GUIOpenParam
    {
        public override EnumGUIType GUIType => EnumGUIType.HUD;

        public Int64 EntityID { get; private set; }

        private PARAM(Int64 _entity_id) 
        : base(
            // id      
            GUIPage.GenerateID(),           

            // asset path
            "gui/hud/unit",

            // is input enabled
            false)
        {
            EntityID = _entity_id;
        }

        static public PARAM Create(Int64 _entity_id)
        {
            return new PARAM(_entity_id);
        }

    }


    protected override void OnClose()
    {
        throw new NotImplementedException();
    }

    protected override void OnOpen(GUIOpenParam _param)
    {
        throw new NotImplementedException();
    }

    protected override void OnPostProcess_Close()
    {
        throw new NotImplementedException();
    }
}