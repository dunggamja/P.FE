using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lua;
using Battle;
using Cysharp.Threading.Tasks;
using System.Threading;
using R3;


public class Cutscene_VFX_TileSelect : Cutscene
{
//    public override EnumCutsceneType Type => EnumCutsceneType.VFX_TileSelect;
    public bool           Create   { get; private set; } = false;
    public int            Index    { get; private set; } = 0;
    public TAG_INFO       Tag      { get; private set; } = TAG_INFO.Create(EnumTagType.None, 0);

    protected int BlackBoardKey
    {
        get
        {
            var index = (int)EnumCutsceneLocalMemory.VFX_Tile_Select_Begin + Index;
            if (index < (int)EnumCutsceneLocalMemory.VFX_Tile_Select_Begin
            ||  index > (int)EnumCutsceneLocalMemory.VFX_Tile_Select_End)
            {
                Debug.LogError($"Cutscene_VFX_TileSelect: BlackBoard_Index is out of range: {index}");
                return 0;
            }


            return index;
        }
    }

    public Cutscene_VFX_TileSelect(CutsceneSequence _sequence, 
    int            _index, 
    bool           _create, 
    TAG_INFO       _tag) : base(_sequence)
    {
        Index    = _index;  
        Create   = _create;
        Tag      = _tag;
    }

    protected override void OnEnter()
    {
    }

    protected override UniTask OnUpdate(CancellationToken _skip_token)
    {
        // INDEX가 겹치는 값이 있으면 해제해줍시다.
        var vfx_id   = Sequence.Memory.GetValue(BlackBoardKey);
        var position = TagHelper.Peek_Position(Tag);


        if (Create)
        {
            // 커서 VFX 생성.
            if (vfx_id == 0)
            {
                vfx_id = VFXHelper.CreateTileSelctVFX(position);
                Sequence.Memory.SetValue(BlackBoardKey, vfx_id);
            }

            // 커서 VFX 위치 갱신 및 카메라 위치 갱신 이벤트 발생.
            VFXHelper.UpdateTileSelectVFX(vfx_id, position);
        }
        else
        {
            // 커서 VFX 해제.
            if (vfx_id != 0)
            {
                VFXHelper.ReleaseTileSelectVFX(ref vfx_id);
                Sequence.Memory.SetValue(BlackBoardKey, 0);
            }
        }

        return UniTask.CompletedTask;
    }

    protected override void OnExit()
    {
        
    }
}