using System;
using System.Collections;
using System.Collections.Generic;
using Battle;
using Unity.Mathematics;
using UnityEngine;


// 
namespace Battle
{
    public enum EnumInfluenceType
    {
      None,


    //   // 위협도: 높을수록 해당 위치를 공격 가능한 유닛이 많음. (유닛으로 인한 길막은 계산 안함. 지형만 계산.)
    //   Threat, 

    // //   // 실제 위협도 : 유닛으로 인한 길막까지 계산된 실제 위험도. <- 요건 상호간의 영향을 주고 받는게 많아서 
    // //   // 영향도 맵으로 계산하는게 힘들 것 같다...  일단 제외.
    // //   Threat_Real, 

    //   // 거리. : 해당 위치에서 가장 가까운 적과의 거리. (0은 없음을 뜻함.)
    //   Distance, 
    }


    public class Terrain_Influence
    {
        int                                  m_width;
        int                                  m_height;
        int                                  m_block_size;

        Dictionary<EnumInfluenceType, Dictionary<int, TerrainBlockManager>> 
                                             m_faction_influence_map = new();


       



        public Terrain_Influence(int _width, int _height, int _block_size)    
        {
            m_width      = _width;
            m_height     = _height;
            m_block_size = _block_size;        
        }


        public void SetInfluenceCell(EnumInfluenceType _influence_type, int _faction, int _x, int _y, Int64 _value)
        {
            if (!m_faction_influence_map.ContainsKey(_influence_type))
                 m_faction_influence_map.Add(_influence_type, new Dictionary<int, TerrainBlockManager>());

            if (! m_faction_influence_map[_influence_type].ContainsKey(_faction))
                m_faction_influence_map[_influence_type].Add(_faction, new TerrainBlockManager(m_width, m_height, m_block_size));


            m_faction_influence_map[_influence_type][_faction].SetCellData(_x, _y, _value);
        }


        public Int64 GetInfluenceCell(EnumInfluenceType _influence_type, int _faction, int _x, int _y)
        {
            if (!m_faction_influence_map.ContainsKey(_influence_type))
                return 0;

            if (!m_faction_influence_map[_influence_type].ContainsKey(_faction))
                return 0;

            return m_faction_influence_map[_influence_type][_faction].GetCellData(_x, _y);
        }

        
    }
}



    // 일단 단순히 증감을 통해서만 계산해본다...

    // public class BlockInfluencer
    // {
    //     public (int x, int y) BlockIndex       { get; private set; } = (0, 0);

    //     public HashSet<Int64> ListInfluencers { get; private set; } = new();

    //     public bool AddInfluencer(Int64 _influencer_id)
    //     {
    //         return ListInfluencers.Add(_influencer_id);
    //     }

    //     public bool RemoveInfluencer(Int64 _influencer_id)
    //     {
    //         return ListInfluencers.Remove(_influencer_id);
    //     }

    //     public bool HasInfluencer(Int64 _influencer_id)
    //     {
    //         return ListInfluencers.Contains(_influencer_id);
    //     }
    // }


 // 해당 블록에 영향을 미치고 있는 entity 목록
        // Dictionary<EnumInfluenceType, Dictionary<(int block_x, int block_y), BlockInfluencer>> 
        //                                      m_block_influencer_map = new();

        // 갱신이 필요한 entity 목록
        // HashSet<Int64>                       m_update_flag_influencer = new();     

        // BlockInfluencer GetBlockInfluencer(EnumInfluenceType _influence_type, int _block_x, int _block_y)
        // {
        //     if (!m_block_influencer_map.ContainsKey(_influence_type))
        //         return null;
                

        //     if (!m_block_influencer_map[_influence_type].ContainsKey((_block_x, _block_y)))
        //         return null;

        //     return m_block_influencer_map[_influence_type][(_block_x, _block_y)];
        // }

        // BlockInfluencer TryAddBlockInfluencer(EnumInfluenceType _influence_type, int _block_x, int _block_y)
        // {
        //     var block_influencer = GetBlockInfluencer(_influence_type, _block_x, _block_y);
        //     if (block_influencer != null)
        //         return block_influencer;

        //     block_influencer = new BlockInfluencer();

        //     if (!m_block_influencer_map.ContainsKey(_influence_type))
        //         m_block_influencer_map.Add(_influence_type, new Dictionary<(int block_x, int block_y), BlockInfluencer>());

        //     if (!m_block_influencer_map[_influence_type].ContainsKey((_block_x, _block_y)))
        //          m_block_influencer_map[_influence_type].Add((_block_x, _block_y), block_influencer);

        //     return block_influencer;
        // }




        // public TerrainBlock GetInfluenceBlock(EnumInfluenceType _influence_type, int _faction, int _x, int _y)
        // {            
        // }


        // public void UpdateInfluencer()
        // {
        //     var list_block_update = ListPool<(EnumInfluenceType, (int x, int y))>.Acquire();
        //     var list_influencer_update = HashSetPool<Int64>.Acquire();
            
        //     // 변경사항이 entity 목록을 추출합니다.
        //     UpdateInfluencer_Collect(ref list_block_update, ref list_influencer_update);

        //     // 각 entity 들이 속한 블록들을 
        //     foreach((var enfluence_type, var block_index) in list_block_update)
        //     {
                


        //         // var entity = EntityManager.Instance.GetEntity(entity_id);
        //         // if (entity != null)
        //         // {
        //         //     var influence_distance = 0;
        //         // }
        //     }


        //     // // ..정리..
        //     // m_update_flag_influencer.Clear();
        //     // HashSetPool<Int64>.Return(ref list_influencer_update);
        //     // ListPool<(EnumInfluenceType, (int x, int y))>.Return(ref list_block_update);
        // }

        // void UpdateInfluencer_Collect(
        //     ref List<(EnumInfluenceType, (int x, int y))> _list_block_update,
        //     ref HashSet<Int64> _list_influencer_update)
        // {
            


        //     // // 변경사항이 발생한 블록을 목록을 추출합니다.
        //     // foreach((var enfluence_type, var block_map) in m_block_influencer_map)
        //     // {
        //     //     foreach((var block_index, var block_influencer) in block_map)
        //     //     {
        //     //         bool is_update_block = false;

        //     //         foreach(var influencer_id in m_update_flag_influencer)
        //     //         {
        //     //             if (block_influencer.RemoveInfluencer(influencer_id))
        //     //                 is_update_block = true;
        //     //         }

        //     //         if (is_update_block)
        //     //             _list_block_update.Add((enfluence_type, block_index));
        //     //     }
        //     // }

        //     // // 변경사항이 발생한 블록들을 사정거리내에 두고 있던 entity 목록을 취합합니다. 
        //     // foreach((var enfluence_type, var block_index) in _list_block_update)
        //     // {
        //     //     var block_influencer = GetBlockInfluencer(enfluence_type, block_index.x, block_index.y);
        //     //     if (block_influencer != null)
        //     //     {
        //     //         foreach(var influencer_id in block_influencer.ListInfluencers)
        //     //         {
        //     //             _list_influencer_update.Add(influencer_id);
        //     //         }
        //     //     }                
        //     // }


        // }

