using Battle;
using Sirenix.OdinInspector;
using UnityEngine;



public class VFXShape : VFXObject
{
    public new class Param : VFXObject.Param
    {       
    }
    

    [SerializeField]
    private Shapes.Quad m_quad = null;

    const float GAP_Y_MAX = 1.0f;


    protected override void ApplySnapToTerrain()
    {
        //base.ApplySnapToTerrain();

        if (m_quad == null)
            return;

        // Debug.LogWarning($"ApplySnapToTerrain_Before: {m_quad.A}, {m_quad.B}, {m_quad.C}, {m_quad.D}");
        
        var mat_local_to_world = m_quad.transform.localToWorldMatrix;
        var mat_world_to_local = m_quad.transform.worldToLocalMatrix;



        var point_a = mat_local_to_world.MultiplyPoint(m_quad.A);
        var point_b = mat_local_to_world.MultiplyPoint(m_quad.B);
        var point_c = mat_local_to_world.MultiplyPoint(m_quad.C);
        var point_d = mat_local_to_world.MultiplyPoint(m_quad.D);

        // 중점을 기준으로 지형인지 체크.
        var center_point          = (point_a + point_b + point_c + point_d) / 4f;
        var center_height         = SnapToTerrain(center_point).y;
        var center_height_terrain = SnapToTerrain(center_point, true).y;        

        var is_center_terrain = center_height_terrain == center_height;
        if (is_center_terrain)
        {
            // 중점이 지형이면 지형 높이 사용하는 것으로 변경.
            point_a = SnapToTerrain(point_a, true);
            point_b = SnapToTerrain(point_b, true);
            point_c = SnapToTerrain(point_c, true);
            point_d = SnapToTerrain(point_d, true);
        }   
        else
        {
            // 중점이 지형이 아니라면 해당 오브젝트의 높이를 사용.
            point_a.y = center_height;
            point_b.y = center_height;
            point_c.y = center_height;
            point_d.y = center_height;
        }

        m_quad.A    = mat_world_to_local.MultiplyPoint(point_a);
        m_quad.B    = mat_world_to_local.MultiplyPoint(point_b);
        m_quad.C    = mat_world_to_local.MultiplyPoint(point_c);
        m_quad.D    = mat_world_to_local.MultiplyPoint(point_d);

        // Debug.LogWarning($"ApplySnapToTerrain_After: {m_quad.A}, {m_quad.B}, {m_quad.C}, {m_quad.D}");
    }
}


