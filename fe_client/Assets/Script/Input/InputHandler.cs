using UnityEngine;

public enum EnumInputHandlerType
{
    None,

    UI,   // UI
    Grid, // �׸��� �̵�
}


// �׸��� ���� or UI ����

public abstract class InputHandler
{
    protected abstract void OnInitialize();
    // protected abstract void OnLoop();
    protected abstract void OnRelease();

    // public void OnInput()
}

public class InputHandler_UI : InputHandler
{
    protected override void OnInitialize()
    {
    }

    protected override void OnRelease()
    {
    }
}

public class InputHandler_Grid : InputHandler
{
    protected override void OnInitialize()
    {
    }

    protected override void OnRelease()
    {
    }
}
    