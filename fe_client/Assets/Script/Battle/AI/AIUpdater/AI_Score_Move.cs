using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEditor.Localization.Plugins.XLIFF.V20;
using UnityEngine;

namespace Battle
{
  public class AI_Score_Move : IAIUpdater
  {
    public class Result : IPoolObject
    {
      public enum EnumScoreType
      {
        CloseToTarget,    // 목표 위치와 가까운지 체크.
        TerrainAdvantage, // 지형 이득 점수.
        TerrainDamage,    // 지형 데미지 점수.
        MAX,
      }

      static float Default_Score_Multiplier(EnumScoreType _type, float _score)
      {
        var multiplier = 0f;
        switch(_type)
        {
          case EnumScoreType.CloseToTarget:    multiplier = 1f;  break;
          case EnumScoreType.TerrainAdvantage: multiplier = 1f;  break;
          case EnumScoreType.TerrainDamage:    multiplier = -1f; break;
        }

        return multiplier * _score;
      }

      public (int x, int y) Position { get; private set; } = (0, 0);

      private float[]       m_score = new float[(int)EnumScoreType.MAX] ;

      public void CopyFrom(Result _o)
      {
        Position = _o.Position;
        Array.Copy(_o.m_score, m_score, m_score.Length);
      }


      public void Reset()
      {
          Position = (0, 0);
          Array.Clear(m_score, 0, m_score.Length);
      }


      public void SetPosition(int _x, int _y)
      {
        Position = (_x, _y);
      }


      public void SetScore(EnumScoreType _score_type, float _score_value)
      {
          var index = (int)_score_type;
          if (index < 0 || m_score.Length <= index)
              return;
          
          // 각 스코어는 0.0 ~ 1.0 까지만 유효.
          m_score[index] = Mathf.Clamp01(_score_value);                
      }



      public float CalculateScore(Func<EnumScoreType, float, float> _func_score_multiplier = null)
      {
          if (_func_score_multiplier == null)
              _func_score_multiplier = Default_Score_Multiplier;

          var score_total = 0f;
          var score_max   = 0f;


          // 점수 합산.
          for(int i = 0; i < m_score.Length; ++i)
          {
              score_total += Mathf.Max(_func_score_multiplier((EnumScoreType)i, m_score[i]), 0f);
              score_max   += Mathf.Max(_func_score_multiplier((EnumScoreType)i, 1f), 0f);
          }

          // 예외처리.
          if (score_max <= float.Epsilon)
              return 0f;

          // 점수는 0.0 ~ 1.0으로 제한.
          return Mathf.Clamp01(score_total / score_max);
      }


      public static float CalculateScore_CloseToTarget(int _x, int _y, List<PathNode> _path_to_target)
      {
          // 점수 계산 :
          //  a : (현재 위치에서 가장 가까운 경로의 이동 비용 - 현재 위치에서 가장 가까운 경로의 거리)
          //  b : (길찾기 마지막 경로까지의 이동 비용)
          //  score = a / b

          if (_path_to_target == null || _path_to_target.Count == 0)
            return 0f;


           // 이동이 불가능?
          var move_cost_max = _path_to_target[_path_to_target.Count - 1].MoveCost;
          if (move_cost_max <= 0)
            return 0f;


          // 현재 위치에서 가장 가까운 경로의 거리와 이동 비용을 찾습니다.
          var close_node_distance = int.MaxValue;
          var close_node_movecost = 0;
          for (int i = 0; i < _path_to_target.Count; ++i)
          {
              var position  = _path_to_target[i].GetPosition().PositionToCell();
              var distance  = PathAlgorithm.Distance(_x, _y, position.x, position.y);
              if (close_node_distance > distance)
              {
                  // 현재 위치에서 가장 가까운 경로의 거리
                  close_node_distance = distance;

                  // (현재 위치에서 가장 가까운 경로의 이동 비용 - 현재 위치에서 가장 가까운 경로의 거리)
                  close_node_movecost = _path_to_target[i].MoveCost - distance;
              }
          }

          // 점수 계산.
          return Mathf.Clamp01((float)close_node_movecost / move_cost_max);   
      }

      
    }

    public EnumAIAggressiveType AggressiveType { get; private set; } = EnumAIAggressiveType.Aggressive;

    public EnumAITargetType     TargetType     { get; private set; } = EnumAITargetType.None;



    class PositionVisitor : PathAlgorithm.IFloodFillVisitor, IPoolObject
    {
      public TerrainMap     TerrainMap     { get; set; }  
      public IPathOwner     Visitor        { get; set; }  
      public (int x, int y) Position       { get; set; }  
      public int            MoveDistance   { get; set; }   
      public bool           VisitOnlyEmptyCell => true;
      public bool           IsStop()           => false;


      
      public Result         BestResult        { get; set; } = new();
      public List<PathNode> BestPath          { get; set; } = new();
      // public (int x, int y) BestPosition      { get; set; } = (0, 0);
      // public float          BestScore         { get; set; } = 0f;

      public void Reset()
      {
        TerrainMap     = null;
        Visitor        = null;
        Position       = (0, 0);
        MoveDistance   = 0;

        // BestPosition   = (0, 0);
        // BestScore      = 0f;
        BestPath.Clear();
        BestResult.Reset();
      }

      public void Visit(int x, int y)
      {  
         var temp_result = ObjectPool<Result>.Acquire();       

         // 위치 셋팅.
         temp_result.SetPosition(x, y);

         // 타겟과의 거리에 대한 점수 셋팅.
         temp_result.SetScore(Result.EnumScoreType.CloseToTarget, Result.CalculateScore_CloseToTarget(x, y, BestPath));

         // 점수가 더 좋다면 교체.
         if (BestResult.CalculateScore() < temp_result.CalculateScore())
             BestResult.CopyFrom(temp_result);

         ObjectPool<Result>.Return(ref temp_result);
      }
    }


    public void Update(IAIDataManager _owner)
    {
      if (_owner == null)
        return;

      var entity = EntityManager.Instance.GetEntity(_owner.ID);
      if (entity == null)
        return;

      // 이동 행동이 불가능하면 종료.
      var is_moveable   = entity.HasCommandEnable(EnumCommandFlag.Move);
      if (is_moveable == false)
        return;

      
      // 타겟을 향해가 봅시다.
      switch(TargetType)
      {
        // 타겟을 향해서 이동.
        case EnumAITargetType.Target:   Process_Target(_owner);         break;
        // 특정 위치를 향해서 이동.
        case EnumAITargetType.Position: Process_TargetPosition(_owner); break;
        // 근처 가장 가까운 타겟을 향해서 이동.
        case EnumAITargetType.None:     Process_CloseTarget(_owner);     break;
      }


      

      

      

      //   score = w1 * (1f - distToNearestEnemyNorm) // 적에게 가까워질때의 점수
      // + w2 * (1f - threatExposure) // 적에게 위험을 받을때의 점수.
      // + w3 * terrainAdvantage      // 지형 이득 점수.
      // - w4 * moveCost;             // 이동 비용 점수.?? <- 필요한가?

        // 음... 어찌구분할지 잘 모르겠으니.. EnumAIType에 따라서 동작을 분류해봅시다.
        // 뭔가 

        // switch(_owner.AIType)
        // {
        //   case EnumAIType.Attack:
        //     Process_AIType_Attack(_owner);
        //     break;
        // }
        
    }


    void Process_Target(IAIDataManager _owner)
    {
      if (_owner == null)
        return;

        var target_list = _owner.AIData?.Targets?.AllTargetIDList;
        if (target_list == null)
          return;

        
    }

    void Process_TargetPosition(IAIDataManager _owner)
    {
      if (_owner == null)
        return;


        
    }

    void Process_CloseTarget(IAIDataManager _owner)
    {
        var list_path_nodes = ListPool<PathNode>.Acquire();
        var visitor         = ObjectPool<PositionVisitor>.Acquire();

        try
        {          
          // 가장 가까운 적에게 최대한 가까운 위치로 이동한다. 
          if (_owner == null)
            return;

          var entity = EntityManager.Instance.GetEntity(_owner.ID);
          if (entity == null)
            return;


          // 가장 가까운 적과 경로를 찾아봅시다.
          var target_id = Find_Closest_Enemy(entity, list_path_nodes);
          if (target_id == 0)
            return;

          // 해당 경로와 가장 가까운 위치를 찾아봅시다.
          visitor.TerrainMap     = entity.PathNodeManager.TerrainMap;
          visitor.Visitor        = entity;
          visitor.Position       = entity.Cell;
          visitor.MoveDistance   = entity.StatusManager.GetBuffedUnitStatus(EnumUnitStatus.Movement);
          visitor.BestPath.AddRange(list_path_nodes);
          
          PathAlgorithm.FloodFill(visitor);

          // 점수 셋팅.
          entity.BlackBoard.Score_Move.CopyFrom(visitor.BestResult);
          entity.BlackBoard.SetBPValue(EnumEntityBlackBoard.AIScore_Move, visitor.BestResult.CalculateScore());
        }
        finally
        {
          ListPool<PathNode>.Return(ref list_path_nodes);
          ObjectPool<PositionVisitor>.Return(ref visitor);
        }

    }

    

    Int64 Find_Closest_Enemy(Entity _entity, List<PathNode> _path_nodes)
    {
      if (_entity == null || _path_nodes == null)
          return 0;

        const int RANGE_STEP   = 5;
        const int RANGE_MAX    = 100;

        var   entity_position  = _entity.Cell;
        var   range_level      = 0;

        Int64 target_id        = 0;
        int   target_movecost  = 0;

        var   weapon_range_max = _entity.GetWeaponRange().max;       


        var terrain_map        = TerrainMapManager.Instance.TerrainMap;
        var terrain_map_width  = (terrain_map != null) ? terrain_map.Width  : 0;
        var terrain_map_height = (terrain_map != null) ? terrain_map.Height : 0;


        // 공격자의 위치를 중심으로, 주변으로 범위를 확장해가면서 적을 찾습니다.
        while(target_id == 0 && (range_level * RANGE_STEP) < RANGE_MAX)
        {
            var list_aabb              = HashSetPool<AABB>.Acquire();
            var list_target            = ListPool<Int64>.Acquire();
            var list_target_path_nodes = ListPool<PathNode>.Acquire();

            // y축 좌우.
            for (int y = -range_level; y <= range_level; ++y)
            {
              var center_x_l = entity_position.x - (RANGE_STEP * 2 * range_level);
              var center_x_r = entity_position.x + (RANGE_STEP * 2 * range_level);
              var center_y   = entity_position.y + (RANGE_STEP * 2 * y);

              var left_box = new AABB {
                      min = new Vector2(center_x_l - RANGE_STEP, center_y - RANGE_STEP),
                      max = new Vector2(center_x_l + RANGE_STEP, center_y + RANGE_STEP)
                    };

              var right_box = new AABB {
                      min = new Vector2(center_x_r - RANGE_STEP, center_y - RANGE_STEP),
                      max = new Vector2(center_x_r + RANGE_STEP, center_y + RANGE_STEP)
                    };

              // if (left_box.max.x >= 0 
              // &&  left_box.max.y >= 0 
              // &&  left_box.min.x < terrain_map_width 
              // &&  left_box.min.y < terrain_map_height)
              {
                list_aabb.Add(left_box);
              }

              // if (right_box.max.x >= 0 
              // &&  right_box.max.y >= 0 
              // &&  right_box.min.x < terrain_map_width 
              // &&  right_box.min.y < terrain_map_height)
              {
                list_aabb.Add(right_box);
              }

            }

            // x축 상하.
            for (int x = -range_level; x <= range_level; ++x)
            {
               var center_x   = entity_position.x + (RANGE_STEP * 2 * x);
               var center_y_t = entity_position.y + (RANGE_STEP * 2 * range_level);
               var center_y_b = entity_position.y - (RANGE_STEP * 2 * range_level);


              var top_box = new AABB {
                      min = new Vector2(center_x - RANGE_STEP, center_y_t - RANGE_STEP),
                      max = new Vector2(center_x + RANGE_STEP, center_y_t + RANGE_STEP)
                    };
            
              var bottom_box = new AABB {
                      min = new Vector2(center_x - RANGE_STEP, center_y_b - RANGE_STEP),
                      max = new Vector2(center_x + RANGE_STEP, center_y_b + RANGE_STEP)
                    };

              // if (top_box.max.x >= 0 
              // &&  top_box.max.y >= 0 
              // &&  top_box.min.x < terrain_map_width 
              // &&  top_box.min.y < terrain_map_height)
              {
                list_aabb.Add(top_box);
              }

              // if (bottom_box.max.x >= 0 
              // &&  bottom_box.max.y >= 0 
              // &&  bottom_box.min.x < terrain_map_width 
              // &&  bottom_box.min.y < terrain_map_height)
              {
                list_aabb.Add(bottom_box);
              }
            }


            foreach(var aabb in list_aabb)
            {
                list_target.Clear();
              

                // 근처에 있는 적을 찾아봅시다. 
                SpacePartitionManager.Instance.Query_Position_AABB(
                  list_target, aabb);


                foreach(var e in list_target)
                {
                    var entity_target = EntityManager.Instance.GetEntity(e);
                    if (entity_target == null)
                      continue;

                    // 같은 진영인 경우 제외.
                    var is_alliance = BattleSystemManager.Instance.IsFactionAlliance(_entity.GetFaction(), entity_target.GetFaction());
                    if (is_alliance)
                      continue;
                  
                    // 대상의 위치까지 길찾기 
                    var path_find = PathAlgorithm.PathFind(
                        _entity.PathNodeManager.TerrainMap,
                        _entity,
    
                        _entity.Cell,
                        entity_target.Cell,


                        // 도착 범위 체크.
                        PathAlgorithm.PathFindOption.Create()
                        .SetAllowApproximate()
                        // .SetGoalRange(5)
                        ,

                        // 길찾기 경로.
                        list_target_path_nodes
                      );

                    
                    // 길찾기 & 거리 체크
                    if (path_find.result)
                    {
                        if (target_id == 0 || target_movecost > path_find.move_cost)
                        {
                          target_id       = e;
                          target_movecost = path_find.move_cost;

                          // 길찾기 경로 저장.
                          _path_nodes.Clear();
                          _path_nodes.AddRange(list_target_path_nodes);
                        }
                    }
                }
            }

            // 탐색범위 증가.
            ++range_level;


            HashSetPool<AABB>.Return(ref list_aabb);
            ListPool<Int64>.Return(ref list_target);
            ListPool<PathNode>.Return(ref list_target_path_nodes);
        }

        return target_id;
    }
  }
}