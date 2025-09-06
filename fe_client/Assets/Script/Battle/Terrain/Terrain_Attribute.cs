using System;
using System.Collections;
using System.Collections.Generic;
using Battle;
using UnityEngine;

namespace Battle
{
    
    public class Terrain_Attribute : TerrainBlockManager
    {
        public Terrain_Attribute(int _width, int _height, int _block_size)
        : base(_width, _height, _block_size)
        {
        }

        public Int64 GetAttribute(int _x, int _y)
        {
            return GetCellData(_x, _y);
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

        public static int Calculate_MoveCost(int _path_owner_attribute, Int64 _terrain_attribute)
        {
            // TODO: 나중에 데이터로 관리 가능하도록 빼는게 좋을듯? ScriptableObject...
            // move_cost == 0, 이동 불가

            if ((_terrain_attribute & (1 << (int)EnumTerrainAttribute.Invalid)) != 0)
            {
                // 이동 불가 지역
                return 0;
            }

            else if ((_terrain_attribute & (1 << (int)EnumTerrainAttribute.FlyerOnly)) != 0)
            {
                // 비행 유닛만 가능.
                if ((_path_owner_attribute & (1 << (int)EnumPathOwnerAttribute.Flyer)) == 0)
                    return 0;

                // 이동 Cost
                return 1;
            }

            else if ((_terrain_attribute & (1 << (int)EnumTerrainAttribute.Water)) != 0)
            {
                // 물 지형
                if (((_path_owner_attribute & (1 << (int)EnumPathOwnerAttribute.Flyer)) == 0)
                &&  ((_path_owner_attribute & (1 << (int)EnumPathOwnerAttribute.Water)) == 0))
                    return 0;
                
                // 물 지형 이동 Cost
                return 1;
            }

            else if ((_terrain_attribute & (1 << (int)EnumTerrainAttribute.WaterSide)) != 0)
            {
                // 물가
                if (((_path_owner_attribute & (1 << (int)EnumPathOwnerAttribute.Flyer))  == 0)
                &&   ((_path_owner_attribute & (1 << (int)EnumPathOwnerAttribute.Water))  == 0)
                &&   ((_path_owner_attribute & (1 << (int)EnumPathOwnerAttribute.Ground)) == 0))
                    return 0;

                // 물가 지형 이동 Cost
                return 1;
            }

            else if ((_terrain_attribute & (1 << (int)EnumTerrainAttribute.Ground)) != 0)
            {
                // 땅

                
                {
                    // 비병은 지형 Cost 1로 통과가능.
                    if ((_path_owner_attribute & (1 << (int)EnumPathOwnerAttribute.Flyer)) != 0)
                        return 1;

                    // 땅 이동이 가능한 지 체크.
                    if ((_path_owner_attribute & (1 << (int)EnumPathOwnerAttribute.Ground)) == 0)
                        return 0;
                }

                {
                    // 경사진 지형.
                    if ((_terrain_attribute & (1 << (int)EnumTerrainAttribute.Slope)) != 0)
                    {
                        // 경사면 이동 가능한지 체크.
                        if ((_path_owner_attribute & (1 << (int)EnumPathOwnerAttribute.Slope)) == 0)
                            return 0;

                        // 경사면 이동 Cost
                        return 3;
                    }
                    // 숲 지형
                    else if ((_terrain_attribute & (1 << (int)EnumTerrainAttribute.Forest)) != 0)
                    {
                        // 숲 지형 이동 Cost
                        return 2;
                    }
                }


                // 일반 땅 지형 이동 Cost
                return 1;
            }


            // 지형 셋팅이 안 되어 있으면 이동 불가.        
            return 0;
        }
    }
}
