using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

   


namespace Battle
{
   public class TagData
   {
      public Int64              ID           { get; private set; } = 0;
      public EnumTagAttribute   Attributes   { get; private set; } = EnumTagAttribute.None;
      public string             Name         { get; private set; } = string.Empty;
      public TAG_TARGET_INFO    Owner        { get; private set; } = TAG_TARGET_INFO.Create_None();    
      public TAG_TARGET_INFO    Target       { get; private set; } = TAG_TARGET_INFO.Create_None();

      public EnumTagProductType ProductType  { get; private set; } = EnumTagProductType.None;
      public Int64              ProductValue { get; private set; } = 0;



      public bool Verify_Target_Entity(Entity _entity)
      {
         return Target.Verify_Entity(_entity);
      }

      public bool Verify_Target_Faction(int _faction_id)
      {
         return Target.Verify_Faction(_faction_id);
      }

      public bool Verify_Target_Position(int _x, int _y)
      {
         return Target.Verify_Position(_x, _y);
      }
   }


   public class TagManager : Singleton<TagManager>
   {
      public class Repo_Lookup_Owner
      {
         // class Repo_Attribute : Dictionary<int, HashSet<Int64>> {}
         class Repo_OwnerID : Dictionary<Int64, HashSet<Int64>> {}

         class Repository : Dictionary<EnumTagTargetType, Repo_OwnerID> {}

         Repository m_repository = new();

         public void SetTag(TagData _tag)
         {
            if (_tag == null)
               return;

            if (m_repository.TryGetValue(_tag.Owner.TagType, out var repo_owner_id) == false)
            {
               repo_owner_id = new();
               m_repository.Add(_tag.Owner.TagType, repo_owner_id);
            }

            if (repo_owner_id.TryGetValue(_tag.Owner.TagValue, out var repo_label_id) == false)
            {
               repo_label_id = new();
               repo_owner_id.Add(_tag.Owner.TagValue, repo_label_id);
            }

            repo_label_id.Add(_tag.ID);
         }

         public void RemoveTag(TagData _tag)
         {
            if (_tag == null)
               return;

            if (m_repository.TryGetValue(_tag.Owner.TagType, out var repo_owner_id) == false)
               return;

            if (repo_owner_id.TryGetValue(_tag.Owner.TagValue, out var repo_label_id) == false)
               return;

            repo_label_id.Remove(_tag.ID);
         }


         public void CollectTag(TAG_TARGET_INFO _owner_info, List<Int64> _list_label_id)
         {
            if (m_repository.TryGetValue(_owner_info.TagType, out var repo_owner_id) == false)
               return;

            if (repo_owner_id.TryGetValue(_owner_info.TagValue, out var repo_label_id) == false)
               return;

            if (_list_label_id == null || repo_label_id == null)
               return;

            _list_label_id.AddRange(repo_label_id);
         }
      }

      public class Repo_Lookup_Name
      {
         class Repository : Dictionary<string, HashSet<Int64>> {}

         Repository m_repository = new();

         public void SetTag(TagData _tag)
         {
            if (_tag == null)
               return;

            if (m_repository.TryGetValue(_tag.Name, out var repo_tag_id) == false)
            {
               repo_tag_id = new();
               m_repository.Add(_tag.Name, repo_tag_id);
            }

            repo_tag_id.Add(_tag.ID);
         }

         public void RemoveTag(TagData _tag)
         {
            if (_tag == null)
               return;

            if (m_repository.TryGetValue(_tag.Name, out var repo_tag_id) == false)
               return;

            repo_tag_id.Remove(_tag.ID);
         }

         public void CollectTag(string _name, List<Int64> _list_tag_id)
         {
            if (m_repository.TryGetValue(_name, out var repo_tag_id) == false)
               return;

            _list_tag_id.AddRange(repo_tag_id);
         }
      }


      Dictionary<Int64, TagData> m_repository               = new();
      Repo_Lookup_Owner          m_repository_lookup_owner  = new();
      Repo_Lookup_Name           m_repository_lookup_name   = new();
      // Repo_Lookup_Target       m_repository_lookup_target = new();
      

      public void SetTag(TagData _tag)
      {
         if (_tag == null)
            return;

         if (GetTag(_tag.ID) != null)
             RemoveTag(_tag.ID);

         m_repository_lookup_owner.SetTag(_tag);
         m_repository_lookup_name.SetTag(_tag);
         // m_repository_lookup_target.SetLabel(_label);
         m_repository.Add(_tag.ID, _tag);
      }

      public void RemoveTag(Int64 _tag_id)  
      {
         var tag = GetTag(_tag_id);
         if (tag == null)
            return;

         m_repository_lookup_owner.RemoveTag(tag);
         m_repository_lookup_name.RemoveTag(tag);
         // m_repository_lookup_target.RemoveLabel(_label);
         m_repository.Remove(_tag_id);
      }


      public TagData GetTag(Int64 _tag_id)
      {
         if (m_repository.TryGetValue(_tag_id, out var label))
            return label;

         return null;
      }

      public void CollectTag(Int64 _owner_entity_id, List<TagData> _result)
      {
         using var list_label_id = ListPool<Int64>.AcquireWrapper();        

         m_repository_lookup_owner.CollectTag(TAG_TARGET_INFO.Create_Entity(_owner_entity_id), list_label_id.Value);

         foreach(var label_id in list_label_id.Value)
         {
            var label = GetTag(label_id);
            if (label != null)
               _result.Add(label);
         }
      }

      public void CollectTag_ByName(string _name, List<TagData> _result)
      {
         using var list_tag_id = ListPool<Int64>.AcquireWrapper();

         m_repository_lookup_name.CollectTag(_name, list_tag_id.Value);

         foreach(var tag_id in list_tag_id.Value)
         {
            var tag = GetTag(tag_id);
            if (tag != null)
               _result.Add(tag);
         }
      }


      // public void Collect_Target(Int64 _entity_id, EnumLabelAttribute _attribute, List<Label> _result)
      // {
      //    var entity = EntityManager.Instance.GetEntity(_entity_id);
      //    if (entity == null)
      //       return;

      //    CollectLabel_Target(Label.TARGET_INFO.Create(EnumLabelTargetType.All, 0), _attribute, _result);

      //    CollectLabel_Target(Label.TARGET_INFO.Create(EnumLabelTargetType.Faction, entity.GetFaction()), _attribute, _result);

      //    CollectLabel_Target(Label.TARGET_INFO.Create(EnumLabelTargetType.Entity, entity.ID), _attribute, _result);  
      // }

      // void CollectLabel_Target(Label.TARGET_INFO _target_info, EnumLabelAttribute _attribute, List<Label> _result)
      // {
      //    using var list_label_id = ListPool<Int64>.AcquireWrapper();

      //    m_repository_lookup_target.CollectLabel(_target_info, _attribute, list_label_id.Value);

      //    foreach(var label_id in list_label_id.Value)
      //    {
      //       var label = GetLabel(label_id);
      //       if (label != null)
      //          _result.Add(label);
      //    }
      // }
   }

}


