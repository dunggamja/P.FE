using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// public class Dialogue_RequestPlayEvent : IEventParam
// {
//     public EnumEventProcessTiming EventProcessTiming => EnumEventProcessTiming.OnNextUpdate;


//     public DIALOGUE_SEQUENCE Dialogue { get; private set; } = new ();

//     public Dialogue_RequestPlayEvent Set(DIALOGUE_SEQUENCE _dialogue)
//     {
//         Dialogue.AddDialogueData(_dialogue.DialogueData);
//         return this;
//     }

//     public void Release()
//     {
//         ObjectPool<Dialogue_RequestPlayEvent>.Return(this);
//     }

//     public void Reset()
//     {
//         Dialogue.Reset();
//     }
// }


// public class Dialogue_CompleteEvent : IEventParam
// {
//     public EnumEventProcessTiming EventProcessTiming => EnumEventProcessTiming.OnNextUpdate;

//     public Int64 DialogueID   { get; private set; } = 0;

//     public Dialogue_CompleteEvent Set(Int64 _dialogue_id)
//     {
//         DialogueID = _dialogue_id;
//         return this;
//     }

//     public void Release()
//     {
//         ObjectPool<Dialogue_CompleteEvent>.Return(this);
//     }

//     public void Reset()
//     {
//         DialogueID = 0;
//     }
// }