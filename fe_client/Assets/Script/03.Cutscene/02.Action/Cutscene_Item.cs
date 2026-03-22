using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lua;
using Battle;
using Cysharp.Threading.Tasks;
using System.Threading;
using R3;


public class Cutscene_Item_Change : Cutscene
{
    public TAG_INFO       Target  { get; private set; } = TAG_INFO.Create(EnumTagType.None, 0);
    public List<ItemData> Items   { get; private set; } = new();
    public bool           Acquire { get; private set; } = false;





    public Cutscene_Item_Change(CutsceneSequence _sequence, TAG_INFO _target, List<ItemData> _items, bool _acquire) : base(_sequence)
    {
        Target  = _target;
        Acquire = _acquire;

        if (_items != null)
            Items.AddRange(_items);
    }

    protected override void OnEnter()
    {
        // throw new NotImplementedException();
    }

    protected override async UniTask OnUpdate(CancellationToken _skip_token)
    {
        // 대상 유닛.
        using var list_collect = ListPool<Entity>.AcquireWrapper();
        TagHelper.Collect_Entity(Target, list_collect.Value);

        // 아이템 목록.
        using var list_item = ListPool<Item>.AcquireWrapper();
        foreach (var item in Items)
        {
            list_item.Value.Add(new Item(item));
        }

        // 아이템 획득/소모 처리.
        foreach (var e in list_collect.Value)
        {
            await ItemHelper.PlaySequence_Item_Change(e, list_item.Value, Acquire);
        }
    }

    protected override void OnExit()
    {
        // throw new NotImplementedException();
    }


}