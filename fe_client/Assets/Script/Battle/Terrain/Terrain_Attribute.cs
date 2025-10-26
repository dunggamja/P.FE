using System;
using System.Collections;
using System.Collections.Generic;
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



        static readonly EnumTerrainAttribute[] s_terrain_attribute_sort_order =  new EnumTerrainAttribute[]
        {
            EnumTerrainAttribute.Invalid, 
            EnumTerrainAttribute.FlyerOnly, 
            EnumTerrainAttribute.Water, 
            EnumTerrainAttribute.Water_Shallow,
            EnumTerrainAttribute.Ground_Climb,
            EnumTerrainAttribute.Ground_Forest,
            EnumTerrainAttribute.Ground_Dirt,
            EnumTerrainAttribute.Ground
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

        public static (int cost, EnumTerrainAttribute attribute) Calculate_MoveCost(int _path_owner_attribute, Int64 _terrain_attribute)
        {
            // TODO: 나중에 데이터로 관리 가능하도록 빼는게 좋을듯? ScriptableObject...
            // move_cost == 0, 이동 불가

            if ((_terrain_attribute & (1 << (int)EnumTerrainAttribute.Invalid)) != 0)
            {
                // 이동 불가 지역
                return (0, EnumTerrainAttribute.Invalid);
            }

            else if ((_terrain_attribute & (1 << (int)EnumTerrainAttribute.FlyerOnly)) != 0)
            {
                // 비행 유닛만 가능.
                if ((_path_owner_attribute & (1 << (int)EnumPathOwnerAttribute.Flyer)) == 0)
                    return (0, EnumTerrainAttribute.FlyerOnly);

                // 이동 Cost
                return (1, EnumTerrainAttribute.FlyerOnly);
            }

            else if ((_terrain_attribute & (1 << (int)EnumTerrainAttribute.Water)) != 0)
            {
                // 물 지형
                if (((_path_owner_attribute & (1 << (int)EnumPathOwnerAttribute.Flyer)) == 0)
                &&  ((_path_owner_attribute & (1 << (int)EnumPathOwnerAttribute.Swimmer)) == 0))
                    return (0, EnumTerrainAttribute.Water);
                
                // 물 지형 이동 Cost
                return (1, EnumTerrainAttribute.Water);
            }

            else if ((_terrain_attribute & (1 << (int)EnumTerrainAttribute.Water_Shallow)) != 0)
            {
                // 물가
                if (((_path_owner_attribute & (1 << (int)EnumPathOwnerAttribute.Flyer))  == 0)
                &&   ((_path_owner_attribute & (1 << (int)EnumPathOwnerAttribute.Swimmer))  == 0)
                &&   ((_path_owner_attribute & (1 << (int)EnumPathOwnerAttribute.Ground)) == 0))
                    return (0, EnumTerrainAttribute.Water_Shallow);

                // 물가 지형 이동 Cost
                return (1, EnumTerrainAttribute.Water_Shallow);
            }

            else if ((_terrain_attribute & (1 << (int)EnumTerrainAttribute.Ground)) != 0)
            {
                // 땅                
                {
                    // 비병은 지형 Cost 1로 통과가능.
                    if ((_path_owner_attribute & (1 << (int)EnumPathOwnerAttribute.Flyer)) != 0)
                        return (1, EnumTerrainAttribute.Ground);

                    // 땅 이동이 가능한 지 체크.
                    if ((_path_owner_attribute & (1 << (int)EnumPathOwnerAttribute.Ground)) == 0)
                        return (0, EnumTerrainAttribute.Ground);
                }

                {
                    // 경사진 지형.
                    if ((_terrain_attribute & (1 << (int)EnumTerrainAttribute.Ground_Climb)) != 0)
                    {
                        // 경사면 이동 가능한지 체크.
                        if ((_path_owner_attribute & (1 << (int)EnumPathOwnerAttribute.Climber)) == 0)
                            return (0, EnumTerrainAttribute.Ground_Climb);

                        // 경사면 이동 Cost
                        return (3, EnumTerrainAttribute.Ground_Climb);
                    }
                    // 숲 지형
                    else if ((_terrain_attribute & (1 << (int)EnumTerrainAttribute.Ground_Forest)) != 0)
                    {
                        // 숲 지형 이동 Cost
                        return (2, EnumTerrainAttribute.Ground_Forest);
                    }
                }


                // 일반 땅 지형 이동 Cost
                return (1, EnumTerrainAttribute.Ground);
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





       
    }
}
