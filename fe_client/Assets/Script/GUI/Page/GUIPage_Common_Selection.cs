using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using R3;
using UnityEngine.UI;
using Battle;


public class GUIPage_Common_Selection : GUIPage
{
    public class PARAM : GUIOpenParam
    {
        // public Int64 EntityID { get; private set; }
        public override EnumGUIType GUIType => EnumGUIType.Popup;

        private PARAM() 
        : base(
            // id      
            GUIPage.GenerateID(),    

            // asset path
            "gui/page/common_selection",   

            // is input enabled
            true,

            // is multiple open
            false                     
            )      
        { 
            
        }


        static public PARAM Create()
        {
            return new PARAM();
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
