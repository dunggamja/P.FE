using UnityEngine;


public enum EnumInputParamType
{
    None,

    Grid_Move,    // 그리드 이동
    Grid_Pointer, // 그리드 포인터
    Grid_Delta,   // 그리드 델타
    Grid_Select,  // 그리드 선택
    Grid_Cancel,  // 그리드 취소

    UI_Move,      // UI 이동
    UI_Pointer,   // UI 포인터
    UI_Delta,     // UI 델타
    UI_Select,    // UI 선택
    UI_Cancel,    // UI 취소
}


public abstract class InputParam //: IPoolObject
{
    public abstract EnumInputParamType ParamType { get; }

    // protected abstract void OnReset();

    // public void Reset()
    // {
    //     // OnReset();
    // }

}

public class InputParam_Grid_Move : InputParam
{
    public override EnumInputParamType ParamType => EnumInputParamType.Grid_Move;

    public Vector2 Direction { get; private set; }

    public InputParam_Grid_Move(Vector2 _direction)
    {
        Direction = _direction;
    }
    
  
}
