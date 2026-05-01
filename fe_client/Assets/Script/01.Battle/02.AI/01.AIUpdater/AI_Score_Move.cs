using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEditor.Localization.Plugins.XLIFF.V20;
using UnityEngine;

namespace Battle
{
  public class AI_Score_Move : AI_Score_Base
  {
    public enum EnumBehavior
    {
       // 가장 가까운 적에게 이동.
       Closest_Enemy,

       // 타겟과의 가까워지는 것을 최우선.
       Closest_Target,

       // 랜덤으로 이동.
       Random,

       //  Hit&Run: 공격 후 피해량이 적을 것으로 예상되는 지점으로 이동.
       HitAndRun,
    }




    public class Result : IPoolObject
    {
      public enum EnumScoreType
      {
        Goal,             // 목표 위치와 가까울수록 높은 점수.
        TerrainAdvantage, // 지형 이득 점수.
        TerrainDamage,    // 지형 데미지 점수.
        MAX,
      }

      static float Default_Score_Multiplier(EnumScoreType _type, float _score)
      {
        var multiplier = 0f;
        switch(_type)
        {
          case EnumScoreType.Goal:             multiplier = 1f;  break;
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


      public bool ApplyHigherScore(Result _o)
      {
          if (_o == null)
          return false;

          // 점수가 동일하거나 낮으면 패스.
          if (_o.CalculateScore() <= CalculateScore())
          return false;
      
          // 점수가 더 좋으면 교체.
          CopyFrom(_o);
          return true;
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
    }


    // 타겟과의 거리를 계산.
    class DistanceFromTargetVisitor : PathAlgorithm.IFloodFillVisitor, IPoolObject
    {
      public TerrainMap     TerrainMap     { get; set; }  
      public IPathOwner     Visitor        { get; set; }  
      public (int x, int y) Position       { get; set; }  
      public int            MoveDistance   => int.MaxValue;  // 무제한 이동.
      public bool           VisitOnlyEmptyCell => false;
      public bool           IsStop()
      {
         // 전체 맵 순회를 하도록 하자.
         return false;
      }


      public Dictionary<(int x, int y), int> DistanceFromTarget { get; private set; } = new();


      int GetMoveCostFromTarget(int _x, int _y)
      {
        if (DistanceFromTarget.TryGetValue((_x, _y), out var distance))
            return distance;

        return -1;
      }

      public float CalcScore(IPathOwner _visitor, TerrainMap _terrain_map, (int x, int y) _position)
      {
        var     move_cost = GetMoveCostFromTarget(_position.x, _position.y);
        return (move_cost >= 0) ? 1f / (1f + move_cost) : 0f;
      }



      public void Visit(PathAlgorithm.IFloodFillVisitor.VisitNode _node)
      {
         DistanceFromTarget.Add((_node.x, _node.y), _node.cost);
      }

      public void Reset()
      {
        TerrainMap = null;
        Visitor    = null;
        Position   = (0, 0);
        DistanceFromTarget.Clear();
      }
    }


    // 위치에 대한 점수를 계산.
    class PositionVisitor : PathAlgorithm.IFloodFillVisitor, IPoolObject
    {
      public TerrainMap     TerrainMap     { get; set; }  
      public IPathOwner     Visitor        { get; set; }  
      public (int x, int y) Position       { get; set; }  
      public int            MoveDistance   { get; set; }   
      public bool           VisitOnlyEmptyCell => true;
      public bool           IsStop()           => false;

      public delegate float FuncCalcScore(IPathOwner _visitor, TerrainMap _terrain_map, (int x, int y) _position);


      
      public Result                BestResult            { get; set; } = new();
      // public Func<int, int, int>   GetMoveCostFromTarget { get; set; } = null;
      public FuncCalcScore         CalcScore             { get; set; } = null;
      
      
      // public (int x, int y) BestPosition      { get; set; } = (0, 0);
      // public float          BestScore         { get; set; } = 0f;

      public void Reset()
      {
        TerrainMap            = null;
        Visitor               = null;
        Position              = (0, 0);
        MoveDistance          = 0;
        CalcScore             = null;

        // BestPosition   = (0, 0);
        // BestScore      = 0f;
        BestResult.Reset();
      }

      public void Visit(PathAlgorithm.IFloodFillVisitor.VisitNode _node)
      {  
         using var temp_result = ObjectPool<Result>.AcquireWrapper();       

         // 위치 셋팅.
         temp_result.Value.SetPosition(_node.x, _node.y);


         // 타겟과 가까울수록 높은 점수.
         //var move_cost  = GetMoveCostFromTarget?.Invoke(_node.x, _node.y) ?? -1;         
         //  var move_score = (move_cost >= 0) ? 1f / (1f + move_cost) : 0f;
         var move_score = CalcScore?.Invoke(Visitor, TerrainMap, (_node.x, _node.y)) ?? 0f;

         // 타겟과의 거리에 대한 점수 셋팅.
         temp_result.Value.SetScore(Result.EnumScoreType.Goal, move_score);

         // 점수가 더 좋다면 교체.
         BestResult.ApplyHigherScore(temp_result.Value);
      }
    }


    public EnumBehavior     BehaviorType     { get; private set; } = EnumBehavior.Closest_Enemy;

    public AI_Score_Move(EnumBehavior _behavior_type)
    {
      BehaviorType = _behavior_type;
    }

    
    protected override bool OnUpdate(IAIDataManager _param)
    {
      if (_param == null)
        return false;

      var entity = EntityManager.Instance.GetEntity(_param.ID);
      if (entity == null)
        return false;

      // 이동 행동이 불가능하면 종료.
      var is_moveable   = entity.HasCommandEnable(EnumCommandFlag.Move);
      if (is_moveable == false)
        return false;


      switch(BehaviorType)
      {
        case EnumBehavior.Closest_Enemy:
        case EnumBehavior.Closest_Target:
          return Process_Closest_Target(_param, BehaviorType);

        case EnumBehavior.Random:
          return Process_Random(_param);

        case EnumBehavior.HitAndRun:
          return Process_HitAndRun(_param);
      }

      return false;
    }


    bool Process_Advance(IAIDataManager _owner)
    {
      if (_owner == null)
        return false;

      // 타겟을 찾는다.
      var target_id = Process_Target_FindTarget(_owner);


      return false;
    }

    Int64 Process_Target_FindTarget(IAIDataManager _owner)
    {
      if (_owner == null)
        return 0;

      // TODO: 도발 등 상태이상에 걸린 경우.

      // TODO: 시나리오 상 정해진 타겟을 찾아봅시다.


      return 0;
    }



    bool Process_Closest_Target(IAIDataManager _owner, EnumBehavior _behavior_type)
    {
        using var visit_target    = ObjectPool<DistanceFromTargetVisitor>.AcquireWrapper();
        using var visit_position  = ObjectPool<PositionVisitor>.AcquireWrapper();

        {          
          // 가장 가까운 적에게 최대한 가까운 위치로 이동한다. 
          if (_owner == null)
            return false;

          var entity = EntityManager.Instance.GetEntity(_owner.ID);
          if (entity == null)
            return false;

          // 가장 가까운 타겟을 찾아봅시다.
          var target = Find_Closest_Target(entity);         
          if (target == null)
            return false;

          // 타겟으로부터 각 셀들의 거리를 미리 계산해둡니다.
          visit_target.Value.TerrainMap     = entity.PathNodeManager.TerrainMap;
          visit_target.Value.Visitor        = entity;
          visit_target.Value.Position       = target.Cell;
          PathAlgorithm.FloodFill(visit_target.Value);


          // 해당 경로와 가장 가까운 위치를 찾아봅시다.
          visit_position.Value.TerrainMap            = entity.PathNodeManager.TerrainMap;
          visit_position.Value.Visitor               = entity;
          visit_position.Value.Position              = entity.Cell;
          visit_position.Value.MoveDistance          = entity.PathMoveRange;

          // 점수 계산 공식. (타겟과 거리가 가까울수록 높은 점수를 줍니다.)
          visit_position.Value.CalcScore = visit_target.Value.CalcScore;
          
          // 타겟으로부터 가장 최적의 위치를 찾아봅시다.
          PathAlgorithm.FloodFill(visit_position.Value);

          // 점수 셋팅.
          var ai_blackboard = entity.AIManager.AIBlackBoard;
          if (ai_blackboard.Score_Move.ApplyHigherScore(visit_position.Value.BestResult))
              ai_blackboard.SetBPValue(EnumAIBlackBoard.Score_Move, visit_position.Value.BestResult.CalculateScore());
        }

        return true;
    }


    Entity Find_Closest_Target(Entity _entity)
    {
      // 가장 가까운 적을 찾아봅시다.
      if (_entity == null)
          return null;

        // 최대 100칸 이내의 적을 5칸씩 범위를 확장해가면서 찾습니다.
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
            using var list_aabb              = HashSetPool<AABB>.AcquireWrapper();
            using var list_target            = ListPool<Int64>.AcquireWrapper();
            using var list_target_path_nodes = ListPool<PathNode>.AcquireWrapper();

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

              list_aabb.Value.Add(left_box);
              list_aabb.Value.Add(right_box);

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

              list_aabb.Value.Add(top_box);
              list_aabb.Value.Add(bottom_box);
            }


            foreach(var aabb in list_aabb.Value)
            {
                list_target.Value.Clear();
              

                // 근처에 있는 적을 찾아봅시다. 
                SpacePartitionManager.Instance.Query_Position_AABB(
                  list_target.Value, aabb);


                foreach(var e in list_target.Value)
                {
                    var entity_target = EntityManager.Instance.GetEntity(e);
                    if (entity_target == null)
                      continue;

                    // 공격 가능한 타겟인지 체크.
                    if (Verify_Enemy(_entity, entity_target) == false)
                      continue;

                  
                    // 대상의 위치까지 길찾기 실행.
                    var path_find = PathAlgorithm.PathFind(
                        _entity.PathNodeManager.TerrainMap,
                        _entity,
    
                        _entity.Cell,
                        entity_target.Cell,


                        // 도착 범위 체크.
                        PathAlgorithm.PathFindOption.Create()

                        // 길막을 무시하고 일단 길을 찾습니다.
                        .SetIgnoreOccupancy()
                        // .SetGoalRange(5)
                        ,

                        // 길찾기 경로.
                        list_target_path_nodes.Value
                      );

                    
                    // 길찾기 & 거리 체크
                    if (path_find.result)
                    {
                        if (target_id == 0 || target_movecost > path_find.move_cost)
                        {
                          target_id       = e;
                          target_movecost = path_find.move_cost;
                        }
                    }
                }
            }

            // 탐색범위 증가.
            ++range_level;            
        }


        return EntityManager.Instance.GetEntity(target_id);
    }
  
  
    void Process_SafePosition(IAIDataManager _owner)
    {
      if (_owner == null)
        return;
    }

    bool Process_Random(IAIDataManager _owner)
    {
      // 
      if (_owner == null)
        return false;

      var entity = EntityManager.Instance.GetEntity(_owner.ID);
      if (entity == null)
        return false;

      using var visit_position  = ObjectPool<PositionVisitor>.AcquireWrapper();


      // 빈 좌표들에 랜덤한 점수를 줘봅시다.
      visit_position.Value.TerrainMap            = entity.PathNodeManager.TerrainMap;
      visit_position.Value.Visitor               = entity;
      visit_position.Value.Position              = entity.Cell;
      visit_position.Value.MoveDistance          = entity.PathMoveRange;

      // 점수 계산 공식. (랜덤 점수입니다.)
      visit_position.Value.CalcScore = (_, _, _position) => Util.Random01();
      
      // 빈 좌표 탐색.
      PathAlgorithm.FloodFill(visit_position.Value);

      // 점수 셋팅.
      var ai_blackboard = entity.AIManager.AIBlackBoard;
      if (ai_blackboard.Score_Move.ApplyHigherScore(visit_position.Value.BestResult))
          ai_blackboard.SetBPValue(EnumAIBlackBoard.Score_Move, visit_position.Value.BestResult.CalculateScore());
  
      return true;
    }


    bool Process_HitAndRun(IAIDataManager _owner)
    {
      if (_owner == null)
        return false;

      var entity = EntityManager.Instance.GetEntity(_owner.ID);
      if (entity == null)
        return false;

      // 1. 안전도. 공격 위협에서 안전한 위치를 찾아봅시다. [적용]
      // 2. 자리 비우기. 공격했던 자리를 비우는 것을 우선합니다. [일단 미적용]
      // 3. 복귀. 원래 위치해 있던 곳으로 복귀합니다. [일단 미적용]
      {
        using var visit_position  = ObjectPool<PositionVisitor>.AcquireWrapper();
        
        visit_position.Value.TerrainMap            = entity.PathNodeManager.TerrainMap;
        visit_position.Value.Visitor               = entity;
        visit_position.Value.Position              = entity.Cell;
        visit_position.Value.MoveDistance          = entity.PathMoveRange;

        // 점수 계산 공식. (안전도)
        visit_position.Value.CalcScore = CalcScore_SafePosition;
        
        // 빈 좌표 탐색.
        PathAlgorithm.FloodFill(visit_position.Value);

        // 점수 셋팅.
        var ai_blackboard = entity.AIManager.AIBlackBoard;
        if (ai_blackboard.Score_Move.ApplyHigherScore(visit_position.Value.BestResult))
        {
            ai_blackboard.SetBPValue(EnumAIBlackBoard.Score_Move, visit_position.Value.BestResult.CalculateScore());

            // Debug.Log($"SafePosition From: {entity.ID}, {entity.Cell.x}, {entity.Cell.y}");
            // Debug.Log($"SafePosition To: {visit_position.Value.BestResult.Position.x}, {visit_position.Value.BestResult.Position.y}");            
            // Debug.Log($"SafePosition Score: {visit_position.Value.BestResult.CalculateScore()}");
        }
      }

      return true;
    }

    float CalcScore_SafePosition(IPathOwner _visitor, TerrainMap _terrain_map, (int x, int y) _position)
    {
      var entity = _visitor as Entity;
      if (entity == null)
        return 0f;


      // 10명정도에게 얻어 맞으면 최고로 위험한 위치라고 가정하자.
      var max_threat_score = 10f;

      // 
      var threat_score     = 0f;
      var entity_faction   = entity.GetFaction();

      // 위협 후보들 필터링.      
      using var list_threaten = ListPool<Int64>.AcquireWrapper();
      SpacePartitionManager.Instance.Query_Threaten(
         list_threaten.Value, 
         _position, 
         (_faction) => BattleSystemManager.Instance.IsEnemy(entity_faction, _faction));


      foreach(var e in list_threaten.Value)
      {
        var enemy = EntityManager.Instance.GetEntity(e);
        if (enemy == null)
          continue;

        // 적과의 거리 체크.
        var distance    = PathAlgorithm.Distance(_position, enemy.Cell);

        // 적의 사정거리 체크.
        var enemy_range = enemy.PathMoveRange + enemy.GetWeaponRange().max;

        // 적의 사정거리보다 내가 멀리있다면 패스.
        if (enemy_range < distance)
          continue;

        // 거리에 대한 점수.
        var distance_score = (float)distance / (enemy_range + 1);
        
        // 거리가 가까우면 위협 점수를 높게 줍시다.
        threat_score += 1f - Mathf.Clamp01(distance_score);
      }



      // 안전점수 = 1f - (위협점수/최대 위협점수)
      var safe_score = 1f - (Mathf.Clamp(threat_score, 0f, max_threat_score) / max_threat_score);

      // Debug.Log($"SafePosition, Cell: {_position.x}, {_position.y}, Score: {safe_score}");

      return Mathf.Clamp01(safe_score);
    }






    private bool Verify_Enemy(Entity _entity, Entity _target) 
    {
        if (_entity == null || _target == null)
            return false;

        // 적인지 체크.
        if (AIHelper.Verify_IsEnemy(_entity.ID, _target.ID) == false)
            return false;

        // 무시 대상인지 체크
        if (AIHelper.Verify_Target_Ignore(_entity, _target))
            return false;

        
        if (BehaviorType == EnumBehavior.Closest_Target)
        {
            // 포커싱 대상인지 체크.
            if (AIHelper.Verify_Target_Focus(_entity, _target) == false)
                return false;
        }

        return true;
    }
  }
}