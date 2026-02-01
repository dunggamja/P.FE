using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Battle;
using UnityEngine;

namespace Battle
{
    
    public class Terrain_Attribute : TerrainBlockManager
    {
        // 기본 속성.
        Int64[,]                                 m_attribute_mask_static   = null;
        // 동적 속성.
        Dictionary<EnumTerrainAttribute, int[,]> m_attribute_count_dynamic = new();



        static readonly EnumTerrainAttribute[]   s_terrain_attribute_sort_order =  new EnumTerrainAttribute[]
        {
            EnumTerrainAttribute.Invalid, 
            EnumTerrainAttribute.FlyerOnly, 
            EnumTerrainAttribute.Water, 
            EnumTerrainAttribute.Water_Shallow,
            EnumTerrainAttribute.Ground_Indoor,
            EnumTerrainAttribute.Ground_Climb,
            EnumTerrainAttribute.Ground_Forest,
            EnumTerrainAttribute.Ground_Dirt,
            EnumTerrainAttribute.Ground,
        };

        public static ReadOnlySpan<EnumTerrainAttribute> TerrainAttributeSortOrder => s_terrain_attribute_sort_order;

        



        public Terrain_Attribute(int _width, int _height, int _block_size)
        : base(_width, _height, _block_size)
        {
            m_attribute_mask_static   = new Int64[Width, Height];
            m_attribute_count_dynamic = new();
        }

        public bool HasAttribute(int _x, int _y, EnumTerrainAttribute _attribute_type)
        {
            return HasBitIndex(_x, _y, (int)_attribute_type);
        }


        public void SetAttribute(int _x, int _y, Battle.EnumTerrainAttribute _attribute_type)
        {
            SetBitIndex(_x, _y, (int)_attribute_type);
        }

        public void RemoveAttribute(int _x, int _y, Battle.EnumTerrainAttribute _attribute_type)
        {
            RemoveBitIndex(_x, _y, (int)_attribute_type);
        }

        public int GetDodgeBonus(int _x, int _y)
        {
            int dodge_bonus = 0;

            // 숲 지형에서 회피 보너스 적용.
            if (HasAttribute(_x, _y, EnumTerrainAttribute.Ground_Forest))
                dodge_bonus += 20;

            return dodge_bonus;
            
        }

        public static (int cost, EnumTerrainAttribute attribute) Calculate_MoveCost(
            IPathOwner _path_owner,
            Int64      _terrain_attribute,
            bool       _is_occupy)
        {
            if (_path_owner == null)
                return (0, EnumTerrainAttribute.Invalid);

            var terrain_kind       = _path_owner.PathTerrainKind;

            // 탑승 상태일경우, 실내 지형을 점유할수 없습니다.
            var cant_occupy_indoor = (_is_occupy && _path_owner.PathMounted);


            // 위에서부터 순서대로 체크합니다.
            Span<EnumTerrainAttribute> span_terrain_attribute = stackalloc EnumTerrainAttribute[] 
            {
                EnumTerrainAttribute.Invalid,
                EnumTerrainAttribute.FlyerOnly,
                EnumTerrainAttribute.Water,
                EnumTerrainAttribute.Water_Shallow,
                EnumTerrainAttribute.Ground_Indoor,
                EnumTerrainAttribute.Ground_Climb,
                EnumTerrainAttribute.Ground_Forest,
                EnumTerrainAttribute.Ground_Dirt,
                EnumTerrainAttribute.Ground
            };

            foreach(var e in span_terrain_attribute)
            {
                // 지형 속성 체크.
                if ((_terrain_attribute & (1 << (int)e)) != 0)
                {
                    // 실내 지형으로 이동 가능한지 체크.
                    if (cant_occupy_indoor)
                    {
                        if (e == EnumTerrainAttribute.Ground_Indoor)
                        {
                            return (0, EnumTerrainAttribute.Invalid);
                        }
                    }

                    var cost = DataManager.Instance.UnitSheet.GetTerrainCost(terrain_kind, e);
                    return (cost, e);
                }
            }

            // 지형 셋팅이 안 되어 있으면 이동 불가.        
            return (0, EnumTerrainAttribute.Invalid);
        }
    



        public void SetAttributeMask_Static(int _x, int _y, Int64 _attribute_mask)
        {
            m_attribute_mask_static[_x, _y] = _attribute_mask;
        }

        public Int64 GetAttributeMask_Static(int _x, int _y)
        {
            return m_attribute_mask_static[_x, _y];
        }
    

        public int GetCellAttributeCount(int _x, int _y, EnumTerrainAttribute _attribute_type)
        {
            if (!m_attribute_count_dynamic.ContainsKey(_attribute_type))
                return 0;

            return m_attribute_count_dynamic[_attribute_type][_x, _y];
        }


        public void IncreaseCellAttributeCount(int _x, int _y, EnumTerrainAttribute _attribute_type, int _count)
        {
            if (!m_attribute_count_dynamic.ContainsKey(_attribute_type))
                m_attribute_count_dynamic.Add(_attribute_type, new int[Width, Height]);

            m_attribute_count_dynamic[_attribute_type][_x, _y] += _count;
        }

        public void DecreaseCellAttributeCount(int _x, int _y, EnumTerrainAttribute _attribute_type)
        {
            if (!m_attribute_count_dynamic.ContainsKey(_attribute_type))
                return;

            m_attribute_count_dynamic[_attribute_type][_x, _y] -= 1;

            if (m_attribute_count_dynamic[_attribute_type][_x, _y] < 0)
            {
                Debug.LogError($"DecreaseCellMetadata under 0: {_attribute_type}, {_x}, {_y}, {m_attribute_count_dynamic[_attribute_type][_x, _y]}");
                m_attribute_count_dynamic[_attribute_type][_x, _y] = 0;
            }
        }


        public void RefreshCell_FromMetadata(int _x, int _y)
        {
            // Attribute들 비트 마스크를 생성.
            Int64 cell_data = 0;
            foreach(var e in TerrainAttributeSortOrder)
            {
                if (GetCellAttributeCount(_x, _y, e) > 0)
                    cell_data |= (1L << (int)e);
            }

            cell_data |= GetAttributeMask_Static(_x, _y);


            // 셀 데이터 갱신.
            SetCellData(_x, _y, cell_data);
        }


        new public Terrain_Attribute_IO Save()
        {
            var attribute_count_dynamic = new Dictionary<EnumTerrainAttribute, int[,]>();
            foreach(var e in m_attribute_count_dynamic)
            {
                attribute_count_dynamic.Add(e.Key, (int[,])e.Value.Clone());
            }


            return new Terrain_Attribute_IO()
            {
                Base                  = base.Save(),
                AttributeMaskStatic   = (Int64[,])m_attribute_mask_static.Clone(),
                AttributeCountDynamic = attribute_count_dynamic,
            };
        }
        
        public void Load(Terrain_Attribute_IO _snapshot)
        {
            base.Load(_snapshot.Base);
            m_attribute_mask_static   = _snapshot.AttributeMaskStatic;
            m_attribute_count_dynamic = _snapshot.AttributeCountDynamic;
        }

       
    }


    public class Terrain_Attribute_IO 
    {
        public TerrainBlockManager_IO                   Base                  { get; set; }
        public Int64[,]                                 AttributeMaskStatic   { get; set; }
        public Dictionary<EnumTerrainAttribute, int[,]> AttributeCountDynamic { get; set; }       
    }


}




