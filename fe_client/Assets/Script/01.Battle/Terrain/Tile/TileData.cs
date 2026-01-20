using UnityEngine;
using Battle;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif


namespace Battle
{

   [CreateAssetMenu(fileName = "TileData", menuName = "ScriptableObjects/Battle/TileData")]
   public class TileData : ScriptableObject
   {
      [Serializable]
      public struct TileItem
      {
         public EnumTerrainAttribute Attribute;
         public TileBase Tile;
      }


      public List<TileItem> m_tiles = null;


      public bool HasAttribute(EnumTerrainAttribute _attribute, TileBase _tile)
      {
         if (m_tiles == null)
            return false;

         foreach (var item in m_tiles)
         {
            if ((item.Attribute == _attribute) && (item.Tile == _tile))
               return true;
         }

         return false;
      }

      public EnumTerrainAttribute GetTerrainAttribute(TileBase _tile)
      {
         if (_tile == null)
            return EnumTerrainAttribute.Invalid;


         foreach (var item in m_tiles)
         {
            if (item.Tile == _tile)
               return item.Attribute;
         }

         return EnumTerrainAttribute.Invalid;
      }


      [ContextMenu("SortTiles")]
      public void SortTiles()
      {
         m_tiles.Sort((a, b) => a.Attribute.CompareTo(b.Attribute));
      }      
   }


   #if UNITY_EDITOR
   [CustomEditor(typeof(TileData))]
   public class TileDataEditor : Editor
   {
      public override void OnInspectorGUI()
      {
         base.OnInspectorGUI();

         if (GUILayout.Button("SortTiles"))
         {
            ((TileData)target).SortTiles();
         }
      }
   }
   #endif

}