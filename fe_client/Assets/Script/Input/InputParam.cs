using UnityEngine;


public enum EnumInputParamType
{
    None,

    Grid_Move,    // �׸��� �̵�
    Grid_Pointer, // �׸��� ������
    Grid_Delta,   // �׸��� ��Ÿ
    Grid_Select,  // �׸��� ����
    Grid_Cancel,  // �׸��� ���

    UI_Move,      // UI �̵�
    UI_Pointer,   // UI ������
    UI_Delta,     // UI ��Ÿ
    UI_Select,    // UI ����
    UI_Cancel,    // UI ���
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
