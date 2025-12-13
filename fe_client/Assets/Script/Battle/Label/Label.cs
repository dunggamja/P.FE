using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

   


namespace Battle
{
   public class Label
   {
      public struct OWNER_INFO
      {
         public  EnumLabelOwnerType OwnerType;
         public  Int64              OwnerValue;

         public Int64          EntityID => OwnerType == EnumLabelOwnerType.Entity   ? OwnerValue : 0;
         public (int x, int y) Position => OwnerType == EnumLabelOwnerType.Position ? ValueToPosition(OwnerValue) : (0, 0);


         private static (int x, int y) ValueToPosition(Int64 _id)
         {
            return ((int)(_id / Constants.MAX_MAP_SIZE), (int)(_id % Constants.MAX_MAP_SIZE));
         }

         private static Int64 PositionToValue(int _x, int _y)
         {
            return _x * Constants.MAX_MAP_SIZE + _y;
         }



         public static OWNER_INFO Create_Entity(Int64 _owner_id)
         {
            return new OWNER_INFO { OwnerType = EnumLabelOwnerType.Entity, OwnerValue = _owner_id };
         }

         public static OWNER_INFO Create_Position(int _x, int _y)
         {
            return new OWNER_INFO { OwnerType = EnumLabelOwnerType.Position, OwnerValue = PositionToValue(_x, _y) };
         }
      }


      public struct TARGET_INFO
      {
         public EnumLabelTargetType TargetType;
         public Int64               TargetID ;

         public static TARGET_INFO Create(EnumLabelTargetType _target_type, Int64 _target_id)
         {
            return new TARGET_INFO { TargetType = _target_type, TargetID = _target_id };
         }
      }


      public Int64                       ID         { get; private set; } = 0;
      public OWNER_INFO                  Owner      { get; private set; } = OWNER_INFO.Create_Entity(0);    
      public TARGET_INFO                 Target     { get; private set; } = TARGET_INFO.Create(EnumLabelTargetType.None, 0);
      public HashSet<EnumLabelAttribute> Attributes { get; private set; } = new();



      public bool Verify_Target(Entity _entity)
      {
         if (_entity == null)
            return false;

         if (Target.TargetType == EnumLabelTargetType.Entity && Target.TargetID == _entity.ID)
            return true;

         if (Target.TargetType == EnumLabelTargetType.Faction && _entity.GetFaction() == Target.TargetID)
            return true;

         if (Target.TargetType == EnumLabelTargetType.All)
            return true;

         return false;
      }

      public bool HasAttribute(EnumLabelAttribute _attribute)
      {
         return Attributes != null && Attributes.Contains(_attribute);
      }
   }


   public class LabelManager : Singleton<LabelManager>
   {
      public class Repo_Lookup_Owner
      {
         // class Repo_Attribute : Dictionary<int, HashSet<Int64>> {}
         class Repo_OwnerID : Dictionary<Int64, HashSet<Int64>> {}

         class Repository : Dictionary<EnumLabelOwnerType, Repo_OwnerID> {}

         Repository m_repository = new();

         public void SetLabel(Label _label)
         {
            if (_label == null)
               return;

            if (m_repository.TryGetValue(_label.Owner.OwnerType, out var repo_owner_id) == false)
            {
               repo_owner_id = new();
               m_repository.Add(_label.Owner.OwnerType, repo_owner_id);
            }

            if (repo_owner_id.TryGetValue(_label.Owner.OwnerValue, out var repo_label_id) == false)
            {
               repo_label_id = new();
               repo_owner_id.Add(_label.Owner.OwnerValue, repo_label_id);
            }

            repo_label_id.Add(_label.ID);
         }

         public void RemoveLabel(Label _label)
         {
            if (_label == null)
               return;

            if (m_repository.TryGetValue(_label.Owner.OwnerType, out var repo_owner_id) == false)
               return;

            if (repo_owner_id.TryGetValue(_label.Owner.OwnerValue, out var repo_label_id) == false)
               return;

            repo_label_id.Remove(_label.ID);
         }


         public void CollectLabel(Label.OWNER_INFO _owner_info, List<Int64> _list_label_id)
         {
            if (m_repository.TryGetValue(_owner_info.OwnerType, out var repo_owner_id) == false)
               return;

            if (repo_owner_id.TryGetValue(_owner_info.OwnerValue, out var repo_label_id) == false)
               return;

            if (_list_label_id == null || repo_label_id == null)
               return;

            _list_label_id.AddRange(repo_label_id);
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



      Dictionary<Int64, Label> m_repository               = new();
      Repo_Lookup_Owner        m_repository_lookup_owner  = new();
      // Repo_Lookup_Target       m_repository_lookup_target = new();
      

      public void SetLabel(Label _label)
      {
         if (_label == null)
            return;

         if (m_repository.ContainsKey(_label.ID))
            RemoveLabel(_label);

         m_repository_lookup_owner.SetLabel(_label);
         // m_repository_lookup_target.SetLabel(_label);
         m_repository.Add(_label.ID, _label);
      }

      public void RemoveLabel(Label _label)  
      {
         if (_label == null)
            return;

         m_repository_lookup_owner.RemoveLabel(_label);
         // m_repository_lookup_target.RemoveLabel(_label);
         m_repository.Remove(_label.ID);
      }


      public Label GetLabel(Int64 _label_id)
      {
         if (m_repository.TryGetValue(_label_id, out var label))
            return label;

         return null;
      }

      public void Collect_Owner(Int64 _entity_id, List<Label> _result)
      {
         using var list_label_id = ListPool<Int64>.AcquireWrapper();        

         m_repository_lookup_owner.CollectLabel(Label.OWNER_INFO.Create_Entity(_entity_id), list_label_id.Value);

         foreach(var label_id in list_label_id.Value)
         {
            var label = GetLabel(label_id);
            if (label != null)
               _result.Add(label);
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


