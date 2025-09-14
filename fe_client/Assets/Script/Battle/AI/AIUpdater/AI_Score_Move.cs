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
        switch(_type)
        {
          case EnumScoreType.CloseToTarget:    return 1f;
          case EnumScoreType.TerrainAdvantage: return 1f;
          case EnumScoreType.TerrainDamage:    return -1f;
        }

        return 0f;
      }

      public (int x, int y) Position { get; private set; } = (0, 0);

      private float[]       m_score = new float[(int)EnumScoreType.MAX] ;


      public void Reset()
      {
          Position = (0, 0);
          Array.Clear(m_score, 0, m_score.Length);
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


      public (int x, int y) BestPosition      { get; set; } = (0, 0);
      public float          BestPositionScore { get; set; } = 0f;

      public void Reset()
      {
        TerrainMap        = null;
        Visitor           = null;
        Position          = (0, 0);
        MoveDistance      = 0;
        BestPosition      = (0, 0);
        BestPositionScore = 0;
      }

      public void Visit(int x, int y)
      {
         const float SCORE_MIN = 0.01f;



          // return true;
      }
    }


    public void Update(IAIDataManager _owner)
    {
      if (_owner == null)
        return;

      var entity = EntityManager.Instance.GetEntity(_owner.ID);
      if (entity == null)
        return;


      var score = 0f;
      
      // 타겟을 향해 가 봅시다.
      switch(TargetType)
      {
        // 타겟을 향해서 이동.
        case EnumAITargetType.Target:   score = Process_Target(_owner);         break;
        // 특정 위치를 향해서 이동.
        case EnumAITargetType.Position: score = Process_TargetPosition(_owner); break;
        // 근처 가장 가까운 타겟을 향해서 이동.
        case EnumAITargetType.None:     score = Process_CloseTarget(_owner);     break;
      }


      entity.BlackBoard.SetBPValue(EnumEntityBlackBoard.AIScore_Move, score);

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


    float Process_Target(IAIDataManager _owner)
    {
      if (_owner == null)
        return 0f;

        var target_list = _owner.AIData?.Targets?.AllTargetIDList;
        if (target_list == null)
          return 0f;

        
      return 0f;
    }

    float Process_TargetPosition(IAIDataManager _owner)
    {
      if (_owner == null)
        return 0f;


      return 0f;
        
    }

    float Process_CloseTarget(IAIDataManager _owner)
    {
        // 가장 가까운 적에게 최대한 가까운 위치로 이동한다. 
        if (_owner == null)
          return 0f;

        var entity = EntityManager.Instance.GetEntity(_owner.ID);
        if (entity == null)
          return 0f;

        var target_id = Find_Closest_Enemy(entity);
        if (target_id == 0)
          return 0f;

        var target_entity = EntityManager.Instance.GetEntity(target_id);
        if (target_entity == null)
          return 0f;


        var visitor = ObjectPool<PositionVisitor>.Acquire();
        visitor.TerrainMap     = entity.PathNodeManager.TerrainMap;
        visitor.Visitor        = entity;
        visitor.Position       = entity.Cell;
        visitor.MoveDistance   = entity.StatusManager.GetBuffedUnitStatus(EnumUnitStatus.Movement);

       

        try
        {
          PathAlgorithm.FloodFill(visitor);
            

        }
        finally
        {
          ObjectPool<PositionVisitor>.Return(ref visitor);
        }


        

        return 0f;
    }

    

    Int64 Find_Closest_Enemy(Entity _entity)
    {
      if (_entity == null)
          return 0;

        int RANGE_STEP   = 5;
        int RANGE_MAX    = 100;

        var entity_position = _entity.Cell;
        var range_level     = 0;

        Int64 target_id       = 0;
        int   target_distance = 0;


        while(target_id == 0 && (range_level * RANGE_STEP) < RANGE_MAX)
        {
            var list_center = HashSetPool<(int x, int y)>.Acquire();

            // y축 좌우.
            for (int y = -range_level; y <= range_level; ++y)
            {
               var center_x_l = entity_position.x - (RANGE_STEP * 2 * range_level);
               var center_x_r = entity_position.x + (RANGE_STEP * 2 * range_level);
               var center_y   = entity_position.y + (RANGE_STEP * 2 * y);

               list_center.Add((center_x_l, center_y));
               list_center.Add((center_x_r, center_y));
            }

            // x축 상하.
            for (int x = -range_level; x <= range_level; ++x)
            {
               var center_x   = entity_position.x + (RANGE_STEP * 2 * x);
               var center_y_t = entity_position.y + (RANGE_STEP * 2 * range_level);
               var center_y_b = entity_position.y - (RANGE_STEP * 2 * range_level);

               list_center.Add((center_x, center_y_t));
               list_center.Add((center_x, center_y_b));
            }


            foreach(var center in list_center)
            {
                var list_target = ListPool<Int64>.Acquire();

                // 근처에 있는 적을 찾아봅시다. 
                SpacePartitionManager.Instance.Query_Position_AABB(
                  list_target,
                  new AABB {
                    min = new Vector2(center.x - RANGE_STEP, center.y - RANGE_STEP),
                    max = new Vector2(center.x + RANGE_STEP, center.y + RANGE_STEP)
                  });


                foreach(var e in list_target)
                {
                    var entity_target = EntityManager.Instance.GetEntity(e);
                    if (entity_target == null)
                      continue;

                    // 같은 진영인 경우 제외.
                    if (entity_target.GetFaction() == _entity.GetFaction())
                      continue;
                  
                    // 경로 찾기.
                    var find_path = PathAlgorithm.PathFind(
                      _entity.PathNodeManager.TerrainMap,
                      _entity,

                      _entity.Cell,
                      entity_target.Cell,

                      PathAlgorithm.PathFindOption.EMPTY);

                    // 경로 찾기 결과가 있으면 저장.
                    if (find_path.result)
                    {
                        // 가장 가까운 적을 찾는다.
                        if (target_id == 0 || target_distance > find_path.move_distance)
                        {
                          target_id       = e;
                          target_distance = find_path.move_distance;
                        }
                    }
                }
            }

            // 탐색범위 증가.
            ++range_level;
        }

        return target_id;
    }
  }
}