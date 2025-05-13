using UnityEngine;


[EventReceiver(typeof(VFX_TransformEvent))]
public partial class VFXManager
{
    public void OnReceiveEvent(IEventParam _event)
    {
        switch (_event)
        {
            case VFX_TransformEvent transform_event:
                OnReceiveEvent_VFXTransform(transform_event);
                break;
        }
    }


    void OnReceiveEvent_VFXTransform(VFX_TransformEvent _event)
    {
        if (_event == null)
            return;

        var vfx_object = SeekVFX(_event.ID);
        if (vfx_object == null)
            return;

        if (_event.Parent.apply)
            vfx_object.SetParent(_event.Parent.value);

        if (_event.Position.apply)
            vfx_object.SetPosition(_event.Position.value, _event.Position.time);

        if (_event.Rotation.apply)
            vfx_object.SetRotation(_event.Rotation.value, _event.Rotation.time);

        if (_event.Scale.apply)
            vfx_object.SetScale(_event.Scale.value, _event.Scale.time);

        // 이펙트 위치 변경.
        //_event.Transform.position = _event.Position;
    }
    
}
