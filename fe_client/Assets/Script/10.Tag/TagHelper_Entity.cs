using System;
using System.Collections;
using System.Collections.Generic;
using Lua;
using UnityEngine;
using Battle;

public static partial class TagHelper
{
    static bool Verify_TagType_Entity(EnumTagType _tag_type)
    {
        switch(_tag_type)
        {
        case EnumTagType.Entity:
        case EnumTagType.Entity_Faction:
        case EnumTagType.Entity_All:
        case EnumTagType.Entity_Group:
            return true;
        }

        return false;
    }


    public static void Collect_Entity(this TAG_INFO _tag_info, List<Entity> _list)
    {
        // 엔티티 타입인지 체크.
        if (Verify_TagType_Entity(_tag_info.TagType))
        {
            // 자식계층으로 탐색.
            using var list_children = ListPool<TAG_INFO>.AcquireWrapper();
            
            // 자기 자신도 추가.
            list_children.Value.Add(_tag_info);

            // 자식계층 태그 컬렉트.
            TagManager.Instance.CollectTag_Children(_tag_info, list_children.Value);

            // ENTITY 타입만 컬렉트
            foreach(var e in list_children.Value)
            {
                if (e.TagType != EnumTagType.Entity)
                    continue;

                var entity = EntityManager.Instance.GetEntity(e.TagValue);
                if (entity != null)
                    _list.Add(entity);
            }
        }
    }



    public static void SetupTag_Entity_Faction(Entity _entity)
    {
        if (_entity == null)
        return;

        //  ENTITY_FACTION, ENTITY_ALL 태그 관련 처리를 진행합니다.
        var tag_entity_all = TAG_INFO.Create(EnumTagType.Entity_All, 0);
        var tag_faction    = TAG_INFO.Create(EnumTagType.Entity_Faction, _entity.GetFaction());
        var tag_entity     = TAG_INFO.Create(_entity);
        
        // 셋업.
        TagManager.Instance.SetTag(TAG_DATA.Create(tag_entity_all, EnumTagAttributeType.HIERARCHY, tag_entity));
        TagManager.Instance.SetTag(TAG_DATA.Create(tag_faction,    EnumTagAttributeType.HIERARCHY, tag_entity));
    }

    public static void RemoveTag_Entity_Faction(Entity _entity)
    {
        if (_entity == null)
        return;

        //  ENTITY_FACTION, ENTITY_ALL 태그 관련 처리를 진행합니다.
        var tag_entity     = TAG_INFO.Create(_entity);

        
        // 부모 계층에서 TAG_INFO 컬렉트.
        using var list_remove = ListPool<TAG_INFO>.AcquireWrapper();
        TagManager.Instance.CollectTag_Parents(tag_entity, list_remove.Value);

        // ENTITY_FACTION, ENTITY_ALL 태그 제거.
        foreach(var e in list_remove.Value)
        {
            switch(e.TagType)
            {
                case EnumTagType.Entity_Faction:
                case EnumTagType.Entity_All:
                    TagManager.Instance.RemoveTag(TAG_DATA.Create(e, EnumTagAttributeType.HIERARCHY, tag_entity));
                    break;
            }
        }
    }

}