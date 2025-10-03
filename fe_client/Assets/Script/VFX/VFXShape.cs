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

        point_a     = SnapToTerrain(point_a);
        point_b     = SnapToTerrain(point_b);
        point_c     = SnapToTerrain(point_c);
        point_d     = SnapToTerrain(point_d);

        m_quad.A    = mat_world_to_local.MultiplyPoint(point_a);
        m_quad.B    = mat_world_to_local.MultiplyPoint(point_b);
        m_quad.C    = mat_world_to_local.MultiplyPoint(point_c);
        m_quad.D    = mat_world_to_local.MultiplyPoint(point_d);

        // Debug.LogWarning($"ApplySnapToTerrain_After: {m_quad.A}, {m_quad.B}, {m_quad.C}, {m_quad.D}");
    }
}


