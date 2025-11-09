using System;
using System.Collections;
using System.Collections.Generic;
using Battle;
using UnityEngine;

[Serializable]
public class sheet_buff_value
{
	public Int32       KIND;
	public string      MEMO;
	public string      SITUATION;
   public string      TARGET;
   public string      STATUS;
   public string      OPTION;
   public int         VALUE_MULTIPLY;
   public int         VALUE_ADD;

   public EnumSituationType cache_situation { get; private set; }
   public EnumBuffTarget    cache_target    { get; private set; }
   public EnumBuffStatus    cache_status    { get; private set; }
   // public BuffOption        cache_option    { get; private set; }


   public void Cache()
   {
      Enum.TryParse<EnumSituationType>(SITUATION, out var situation);
      Enum.TryParse<EnumBuffTarget>(TARGET, out var target);
      Enum.TryParse<EnumBuffStatus>(STATUS, out var status);
      

      cache_situation = situation;
      cache_target    = target;
      cache_status    = status;


      // foreach (var option in Util.SplitEnumText<EnumBuffOption>(OPTION, Data_Const.SHEET_SEPERATOR))
      // {
      //    cache_option.SetValue(option, true);
      // }
      // cache_option.SetValue()

   }
}


[ExcelAsset]
public class sheet_buff : ScriptableObject
{
   public List<sheet_buff_value> buff;

   private Dictionary<Int32, sheet_buff_value> m_cache_buff = new();


   public void Initialize()
   {
      m_cache_buff.Clear();
      if (buff != null)
      {
         foreach (var item in buff)
         {
            if (null == item)
               continue;

            item.Cache();
            m_cache_buff.Add(item.KIND, item);
         }
      }
   }

   public BuffTarget GetBuffTarget(Int32 _kind)
   {
      if (m_cache_buff.TryGetValue(_kind, out var result))
      {
         return BuffTarget.Create(result.cache_situation, result.cache_target, result.cache_status);
      }

      return BuffTarget.Empty;
   }

   public BuffValue GetBuffValue(Int32 _kind)
   {
      if (m_cache_buff.TryGetValue(_kind, out var result))
      {
         return new BuffValue { Multiply = result.VALUE_MULTIPLY, Add = result.VALUE_ADD };
      }

      return BuffValue.Empty;
   }


}