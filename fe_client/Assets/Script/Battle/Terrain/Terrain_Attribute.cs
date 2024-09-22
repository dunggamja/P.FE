using System.Collections;
using System.Collections.Generic;
using Battle;
using UnityEngine;

public partial class TerrainAttribute
{
    public static int Calculate_MoveCost(int _path_owner_attribute, int _terrain_attribute)
    {
        // TODO: ���߿� �����ͷ� ���� �����ϵ��� ���°� ������?
        // move_cost == 0, �̵� �Ұ�

        if ((_terrain_attribute & (int)EnumTerrainAttribute.Invalid) != 0)
        {
            // �̵� �Ұ� ����
            return 0;
        }

        else if ((_terrain_attribute & (int)EnumTerrainAttribute.FlyerOnly) != 0)
        {
            // ���� ���ָ� ����.
            if ((_path_owner_attribute & (int)EnumPathOwnerAttribute.Flyer) == 0)
                return 0;

            // �̵� Cost
            return 1;
        }

        else if ((_terrain_attribute & (int)EnumTerrainAttribute.Water) != 0)
        {
            // �� ����
            if (((_path_owner_attribute & (int)EnumPathOwnerAttribute.Flyer) == 0)
            &&  ((_path_owner_attribute & (int)EnumPathOwnerAttribute.Water) == 0))
                return 0;
            
            // �� ���� �̵� Cost
            return 1;
        }

        else if ((_terrain_attribute & (int)EnumTerrainAttribute.WaterSide) != 0)
        {
            // ����
             if (((_path_owner_attribute & (int)EnumPathOwnerAttribute.Flyer)  == 0)
            &&   ((_path_owner_attribute & (int)EnumPathOwnerAttribute.Water)  == 0)
            &&   ((_path_owner_attribute & (int)EnumPathOwnerAttribute.Ground) == 0))
                return 0;

            // ���� ���� �̵� Cost
            return 1;
        }

        else if ((_terrain_attribute & (int)EnumTerrainAttribute.Ground) != 0)
        {
            // ��

            
            {
                // ���� ���� Cost 1�� �������.
                if ((_path_owner_attribute & (int)EnumPathOwnerAttribute.Flyer) != 0)
                    return 1;

                // �� �̵��� ������ �� üũ.
                if ((_path_owner_attribute & (int)EnumPathOwnerAttribute.Ground) == 0)
                    return 0;
            }

            {
                // ����� ����.
                if ((_terrain_attribute & (int)EnumTerrainAttribute.Slope) != 0)
                {
                    // ���� �̵� �������� üũ.
                    if ((_path_owner_attribute & (int)EnumPathOwnerAttribute.Slope) == 0)
                        return 0;

                    // ���� �̵� Cost
                    return 3;
                }
                // �� ����
                else if ((_terrain_attribute & (int)EnumTerrainAttribute.Forest) != 0)
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