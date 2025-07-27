using System;
using System.Collections;
using System.Collections.Generic;
using Battle;
using UnityEngine;


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
        // TODO: ���߿� �����ͷ� ���� �����ϵ��� ���°� ������? ScriptableObject...
        // move_cost == 0, �̵� �Ұ�

        if ((_terrain_attribute & (1 << (int)EnumTerrainAttribute.Invalid)) != 0)
        {
            // �̵� �Ұ� ����
            return 0;
        }

        else if ((_terrain_attribute & (1 << (int)EnumTerrainAttribute.FlyerOnly)) != 0)
        {
            // ���� ���ָ� ����.
            if ((_path_owner_attribute & (1 << (int)EnumPathOwnerAttribute.Flyer)) == 0)
                return 0;

            // �̵� Cost
            return 1;
        }

        else if ((_terrain_attribute & (1 << (int)EnumTerrainAttribute.Water)) != 0)
        {
            // �� ����
            if (((_path_owner_attribute & (1 << (int)EnumPathOwnerAttribute.Flyer)) == 0)
            &&  ((_path_owner_attribute & (1 << (int)EnumPathOwnerAttribute.Water)) == 0))
                return 0;
            
            // �� ���� �̵� Cost
            return 1;
        }

        else if ((_terrain_attribute & (1 << (int)EnumTerrainAttribute.WaterSide)) != 0)
        {
            // ����
             if (((_path_owner_attribute & (1 << (int)EnumPathOwnerAttribute.Flyer))  == 0)
            &&   ((_path_owner_attribute & (1 << (int)EnumPathOwnerAttribute.Water))  == 0)
            &&   ((_path_owner_attribute & (1 << (int)EnumPathOwnerAttribute.Ground)) == 0))
                return 0;

            // ���� ���� �̵� Cost
            return 1;
        }

        else if ((_terrain_attribute & (1 << (int)EnumTerrainAttribute.Ground)) != 0)
        {
            // ��

            
            {
                // ���� ���� Cost 1�� �������.
                if ((_path_owner_attribute & (1 << (int)EnumPathOwnerAttribute.Flyer)) != 0)
                    return 1;

                // �� �̵��� ������ �� üũ.
                if ((_path_owner_attribute & (1 << (int)EnumPathOwnerAttribute.Ground)) == 0)
                    return 0;
            }

            {
                // ����� ����.
                if ((_terrain_attribute & (1 << (int)EnumTerrainAttribute.Slope)) != 0)
                {
                    // ���� �̵� �������� üũ.
                    if ((_path_owner_attribute & (1 << (int)EnumPathOwnerAttribute.Slope)) == 0)
                        return 0;

                    // ���� �̵� Cost
                    return 3;
                }
                // �� ����
                else if ((_terrain_attribute & (1 << (int)EnumTerrainAttribute.Forest)) != 0)
                {
                    // �� ���� �̵� Cost
                    return 2;
                }
            }


            // �Ϲ� �� ���� �̵� Cost
            return 1;
        }


        // ���� ������ �� �Ǿ� ������ �̵� �Ұ�.        
        return 0;
    }
}
