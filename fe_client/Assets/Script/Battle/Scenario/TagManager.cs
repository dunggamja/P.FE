using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

   


namespace Battle
{
   public class TagData
   {
      public Int64            ID         { get; private set; } = 0;
      public EnumTagAttribute Attributes { get; private set; } = EnumTagAttribute.None;
      public string           Name       { get; private set; } = string.Empty;
      public TAG_TARGET_INFO  Owner      { get; private set; } = TAG_TARGET_INFO.Create_None();    
      public TAG_TARGET_INFO  Target     { get; private set; } = TAG_TARGET_INFO.Create_None();



      public bool Verify_Target(Entity _entity)
      {
         if (_entity == null)
            return false;

         if (Target.TagType == EnumTagTargetType.Entity && Target.TagValue == _entity.ID)
            return true;

         if (Target.TagType == EnumTagTargetType.Faction && _entity.GetFaction() == Target.TagValue)
            return true;

         if (Target.TagType == EnumTagTargetType.All)
            return true;

         return false;
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

      // public class Repo_Lookup_Target
      // {
      //    class Repo_Attribute : Dictionary<int, HashSet<Int64>> {}

      //    class Repo_TargetID : Dictionary<Int64, Repo_Attribute> {}

      //    class Repository : Dictionary<EnumLabelTargetType, Repo_TargetID> {}


      //    Repository m_repository = new();


      //    public void SetLabel(Label _label)
      //    {            
      //       if (_label == null)
      //          return;

      //       if (m_repository.TryGetValue(_label.Target.TargetType, out var repo_target_id) == false)
      //       {
      //          repo_target_id = new();
      //          m_repository.Add(_label.Target.TargetType, repo_target_id);
      //       }

      //       if (repo_target_id.TryGetValue(_label.Target.TargetID, out var repo_attribute) == false)
      //       {
      //          repo_attribute = new(); 
      //          repo_target_id.Add(_label.Target.TargetID, repo_attribute);
      //       }

      //       if (_label.Attributes != null)
      //       {
      //          foreach(var attribute in _label.Attributes)
      //          {
      //             if (repo_attribute.TryGetValue((int)attribute, out var list_id) == false)
      //             {
      //                list_id = new();
      //                repo_attribute.Add((int)attribute, list_id);
      //             }
      //             list_id.Add(_label.ID);
      //          }
      //       }
      //    }

      //    public void RemoveLabel(Label _label)
      //    {
      //       if (_label == null)
      //           return;

      //       if (m_repository.TryGetValue(_label.Target.TargetType, out var repo_target_id) == false)
      //          return;

      //       if (repo_target_id.TryGetValue(_label.Target.TargetID, out var repo_attribute) == false)
      //          return;

      //       if (_label.Attributes != null)
      //       {
      //          foreach(var attribute in _label.Attributes)
      //          {
      //             if (repo_attribute.TryGetValue((int)attribute, out var list_id) == false)
      //             {
      //                list_id.Remove(_label.ID);
      //             }
      //          }
      //       }                
      //    }
      
      //    public void CollectLabel(Label.TARGET_INFO _target_info, EnumLabelAttribute _attribute, List<Int64> _list_label_id)
      //    {
      //       if (m_repository.TryGetValue(_target_info.TargetType, out var repo_target_id) == false)
      //          return;

      //       if (repo_target_id.TryGetValue(_target_info.TargetID, out var repo_attribute) == false)
      //          return;

            

      //       if (_attribute == 0)
      //       {
      //          // 0이면 모든 속성을 가져와봅시다. 
      //          foreach(var list_id in repo_attribute.Values)
      //          {
      //             if (list_id != null)
      //             {
      //                if (_list_label_id != null)
      //                    _list_label_id.AddRange(list_id);
      //             }
      //          }
      //       }
      //       else
      //       {
      //          if (repo_attribute.TryGetValue((int)_attribute, out var list_id) == false)
      //             return;

      //          if (list_id != null)
      //          {
      //             if (_list_label_id != null)
      //                 _list_label_id.AddRange(list_id);
      //          }
      //       }
      //    }

      // }



      Dictionary<Int64, TagData> m_repository               = new();
      Repo_Lookup_Owner          m_repository_lookup_owner  = new();
      Repo_Lookup_Name           m_repository_lookup_name   = new();
      // Repo_Lookup_Target       m_repository_lookup_target = new();
      

      public void SetTag(TagData _tag)
      {
         if (_tag == null)
            return;

         if (m_repository.ContainsKey(_tag.ID))
            RemoveTag(_tag);

         m_repository_lookup_owner.SetTag(_tag);
         m_repository_lookup_name.SetTag(_tag);
         // m_repository_lookup_target.SetLabel(_label);
         m_repository.Add(_tag.ID, _tag);
      }

      public void RemoveTag(TagData _tag)  
      {
         if (_tag == null)
            return;

         m_repository_lookup_owner.RemoveTag(_tag);
         m_repository_lookup_name.RemoveTag(_tag);
         // m_repository_lookup_target.RemoveLabel(_label);
         m_repository.Remove(_tag.ID);
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


