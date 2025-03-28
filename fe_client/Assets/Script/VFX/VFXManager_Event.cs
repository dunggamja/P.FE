using UnityEngine;


[EventReceiverAttribute(typeof(VFXTransformEvent))]
public partial class VFXManager
{
    public void OnReceiveEvent(IEventParam _event)
    {
        switch (_event)
        {
            case VFXTransformEvent transform_event:
                OnReceiveEvent_VFXTransform(transform_event);
                break;
        }
    }


    void OnReceiveEvent_VFXTransform(VFXTransformEvent _event)
    {
        if (_event == null)
            return;

        var vfx_object = SeekVFX(_event.ID);
        if (vfx_object == null)
            return;

        if (_event.Parent.apply)
            vfx_object.SetParent(_event.Parent.value);

        if (_event.Position.apply)
            vfx_object.SetPosition(_event.Position.value);

        if (_event.Rotation.apply)
            vfx_object.SetRotation(_event.Rotation.value);

        if (_event.Scale.apply)
            vfx_object.SetScale(_event.Scale.value);

        // TODO: Time 처리 필요.

        // 이펙트 위치 변경.
        //_event.Transform.position = _event.Position;
    }
    
}
