using UnityEngine;


public enum EnumInputParamType
{
    None,

    Grid_Move,      // �׸��� �̵�
    Grid_Pointer,   // �׸��� ������
    Grid_Select,    // �׸��� ����
    Grid_Cancel,    // �׸��� ���
    // Grid_Delta,     // �׸��� ��Ÿ
    // Grid_AnyButton, // �׸��� ��ư

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


public class InputParam_Grid_Pointer : InputParam
{
    public override EnumInputParamType ParamType => EnumInputParamType.Grid_Pointer;

    public Vector2 Position { get; private set; }

    public InputParam_Grid_Pointer(Vector2 _position)
    {
        Position = _position;
    }
}

public class InputParam_Grid_Select : InputParam
{
    public override EnumInputParamType ParamType => EnumInputParamType.Grid_Select;
}

public class InputParam_Grid_Cancel : InputParam
{
    public override EnumInputParamType ParamType => EnumInputParamType.Grid_Cancel;
}


// public class InputParam_Grid_Delta : InputParam
// {
//     public override EnumInputParamType ParamType => EnumInputParamType.Grid_Delta;
// }

// public class InputParam_Grid_AnyButton : InputParam
// {
//     public override EnumInputParamType ParamType => EnumInputParamType.Grid_AnyButton;
// }






