using System.Collections;
using System.Collections.Generic;
using Battle;
using UnityEngine;


public class Terrain_Attribute : Terrain
{
    public Terrain_Attribute(int _width, int _height)
    : base(_width, _height)
    {
    }

    public bool HasAttribute(int _x, int _y, Battle.EnumTerrainAttribute _attribute_type)
    {
        return HasAttribute(_x, _y, (int)_attribute_type);
    }

    public void SetAttribute(int _x, int _y, Battle.EnumTerrainAttribute _attribute_type)
    {
        SetAttribute(_x, _y, (int)_attribute_type);
    }

    public void RemoveAttribute(int _x, int _y, Battle.EnumTerrainAttribute _attribute_type)
    {
        RemoveAttribute(_x, _y, (int)_attribute_type);
    }

    public static int Calculate_MoveCost(int _path_owner_attribute, int _terrain_attribute)
    {
        // TODO: 나중에 데이터로 관리 가능하도록 빼는게 좋을듯? ScriptableObject...
        // move_cost == 0, 이동 불가

        if ((_terrain_attribute & (int)EnumTerrainAttribute.Invalid) != 0)
        {
            // 이동 불가 지역
            return 0;
        }

        else if ((_terrain_attribute & (int)EnumTerrainAttribute.FlyerOnly) != 0)
        {
            // 비행 유닛만 가능.
            if ((_path_owner_attribute & (int)EnumPathOwnerAttribute.Flyer) == 0)
                return 0;

            // 이동 Cost
            return 1;
        }

        else if ((_terrain_attribute & (int)EnumTerrainAttribute.Water) != 0)
        {
            // 물 지형
            if (((_path_owner_attribute & (int)EnumPathOwnerAttribute.Flyer) == 0)
            &&  ((_path_owner_attribute & (int)EnumPathOwnerAttribute.Water) == 0))
                return 0;
            
            // 물 지형 이동 Cost
            return 1;
        }

        else if ((_terrain_attribute & (int)EnumTerrainAttribute.WaterSide) != 0)
        {
            // 물가
             if (((_path_owner_attribute & (int)EnumPathOwnerAttribute.Flyer)  == 0)
            &&   ((_path_owner_attribute & (int)EnumPathOwnerAttribute.Water)  == 0)
            &&   ((_path_owner_attribute & (int)EnumPathOwnerAttribute.Ground) == 0))
                return 0;

            // 물가 지형 이동 Cost
            return 1;
        }

        else if ((_terrain_attribute & (int)EnumTerrainAttribute.Ground) != 0)
        {
            // 땅

            
            {
                // 비병은 지형 Cost 1로 통과가능.
                if ((_path_owner_attribute & (int)EnumPathOwnerAttribute.Flyer) != 0)
                    return 1;

                // 땅 이동이 가능한 지 체크.
                if ((_path_owner_attribute & (int)EnumPathOwnerAttribute.Ground) == 0)
                    return 0;
            }

            {
                // 경사진 지형.
                if ((_terrain_attribute & (int)EnumTerrainAttribute.Slope) != 0)
                {
                    // 경사면 이동 가능한지 체크.
                    if ((_path_owner_attribute & (int)EnumPathOwnerAttribute.Slope) == 0)
                        return 0;

                    // 경사면 이동 Cost
                    return 3;
                }
                // 숲 지형
                else if ((_terrain_attribute & (int)EnumTerrainAttribute.Forest) != 0)
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
