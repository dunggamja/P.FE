using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public abstract class HUDBase : GUIPage
{
    public virtual bool  IsAutoDestroy  => false;

    private Int64 m_follow_world_object_id = 0;

    public void SetFollowWorldObjectID(Int64 _world_object_id)
    {
        m_follow_world_object_id = _world_object_id;
    }

    (bool result, Vector3 world_position) FollowWorldObjectPosition()
    {
       if (m_follow_world_object_id == 0)
       {
          return (false, Vector3.zero);
       }

       var world_object = WorldObjectManager.Instance.Seek(m_follow_world_object_id);
       if (world_object == null)
       {
          return (false, Vector3.zero);
       }

       return (true, world_object.transform.position);
    }
    
    void LateUpdate()
    {
        var (follow_result, follow_position) = FollowWorldObjectPosition();
        if  (follow_result)
        {
            transform.position = follow_position;
        }      


        // billboard 처리.
        var camera = GUIManager.Instance.HUDCamera;
        if (camera != null)
        {
            transform.rotation = camera.transform.rotation;
        }
    }


    protected override void OnLoop()
    {
        base.OnLoop();

        if (CheckAutoDestroy())
        {
           GUIManager.Instance.CloseUI(ID);
        }
    }

    bool CheckAutoDestroy()
    {
        // 자동삭제 조건 체크.
        if (IsAutoDestroy == false)
          return false;


        // Follow World Object 체크.
        if (m_follow_world_object_id > 0)
        {
            if (FollowWorldObjectPosition().result == false)
              return true;
        }

        return false;
    }
}