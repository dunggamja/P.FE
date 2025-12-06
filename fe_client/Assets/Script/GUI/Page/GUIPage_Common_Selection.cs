using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using R3;
using UnityEngine.UI;
using Battle;

[EventReceiver(
    typeof(GUI_Menu_MoveEvent), 
    typeof(GUI_Menu_SelectEvent),
    typeof(GUI_Menu_CancelEvent),
    typeof(Battle_Scene_ChangeEvent)
    // typeof(Battle_Command_Event)
    )]
public class GUIPage_Common_Selection : GUIPage, IEventReceiver
{
    public class PARAM : GUIOpenParam
    {
        // public Int64 EntityID { get; private set; }

        public class ITEM_DATA
        {
            public int                Index              { get; private set; } = 0;
            public Observable<string> TextSubscription   { get; private set; } = null;
            public Action             OnSelectAction     { get; private set; } = null;
            // public Observable<int>    CursorSubscription { get; private set; } = null;
            // public Observable<int>    SelectSubscription { get; private set; } = null;

            public static ITEM_DATA Create(
                int                _index, 
                Observable<string> _text_subscription, 
                Action             _on_select_action)
                // Observable<int>    _cursor_subscription,
            {
                return new ITEM_DATA()
                {
                    Index              = _index,
                    TextSubscription   = _text_subscription,
                    OnSelectAction     = _on_select_action
                    // CursorSubscription = _cursor_subscription,
                };
            }
        }



        public override EnumGUIType GUIType => EnumGUIType.Popup;

        private PARAM() 
        : base(
            // id      
            GUIPage.GenerateID(),    

            // asset path
            "gui/page/common_selection",   

            // is input enabled
            true,

            // is multiple open
            false                     
            )      
        { 
            
        }


        public List<ITEM_DATA> ItemDatas { get; private set; } = new();


        static public PARAM Create(List<ITEM_DATA> _item_datas)
        {
            return new PARAM()
            {
                ItemDatas = new List<ITEM_DATA>(_item_datas),
            };
        }


        static public PARAM Create(List<EnumItemActionType> _list_action_type, Action<EnumItemActionType> _on_select_action)
        {
            using var list_item_datas = ListPool<ITEM_DATA>.AcquireWrapper();

            if (_list_action_type != null)
            {
                int index = 0;
                foreach(var action_type in _list_action_type)
                {
                    var localize_key = ItemHelper.GetLocalizeName(action_type);
                    var text_subject = LocalizationManager.Instance.GetTextObservable(localize_key.Table, localize_key.Key);


                    list_item_datas.Value.Add(ITEM_DATA.Create(index++, text_subject, ()=> _on_select_action?.Invoke(action_type)));
                }
            }

            return PARAM.Create(list_item_datas.Value);
        }
    }



    [SerializeField]
    private RectTransform                 m_grid_selection_root_rect;
    
    [SerializeField]
    private RectTransform                 m_grid_selection_root_bg_rect;

    [SerializeField]
    private GridLayoutGroup               m_grid_selection_root;

    [SerializeField]
    private GUIElement_Grid_Item_MenuText m_grid_selection_item;



    private List<PARAM.ITEM_DATA>         m_item_datas             = new();
    private BehaviorSubject<int>          m_cursor_index_subject   = new(0);
    private (bool init, Vector2 value)    m_grid_selection_padding = (false, Vector2.zero);


    public void OnReceiveEvent(IEventParam _event)
    {
        switch (_event)
        {
            case GUI_Menu_MoveEvent menu_move_event:
                OnReceiveEvent_GUI_Menu_MoveEvent(menu_move_event);
                break;

            case GUI_Menu_SelectEvent menu_select_event:
                OnReceiveEvent_GUI_Menu_SelectEvent(menu_select_event);
                break;

            case Battle_Scene_ChangeEvent:
                GUIManager.Instance.CloseUI(ID);
                break;

            case GUI_Menu_CancelEvent menu_cancel_event:
                OnReceiveEvent_GUI_Menu_CancelEvent(menu_cancel_event);
                break;
        }
    }



    protected override void OnOpen(GUIOpenParam _param)
    {
        var param    = _param as PARAM;
        m_item_datas = param?.ItemDatas ?? new List<PARAM.ITEM_DATA>();


        UpdateItemDatas();
        UpdateLayout();
    }


    protected override void OnClose()
    {
    }

    protected override void OnPostProcess_Close()
    {
    }

    void UpdateItemDatas()
    {
        m_grid_selection_root_rect.DestroyAllChildren();
        for (int i = 0; i < m_item_datas.Count; i++)
        {
            var item_data  = m_item_datas[i];
            var clonedItem = Instantiate(m_grid_selection_item, m_grid_selection_root_rect.transform);
            clonedItem.Initialize(i, m_cursor_index_subject, item_data.TextSubscription, null);
        }
    }

    void UpdateLayout()
    {
        if (m_grid_selection_padding.init == false)
        {
            var padding              = m_grid_selection_root_bg_rect.sizeDelta - m_grid_selection_root_rect.sizeDelta;
            padding.x                = Mathf.Abs(padding.x);
            padding.y                = Mathf.Abs(padding.y);
            m_grid_selection_padding = (true, padding);
        }

        var padding_height = m_grid_selection_root.padding.top + m_grid_selection_root.padding.bottom;
        var child_count    = m_grid_selection_root.transform.childCount;
        var cell_height    = m_grid_selection_root.cellSize.y;
        var spacing_height = m_grid_selection_root.spacing.y;
        var total_height   = (child_count * cell_height) + Mathf.Max(0, spacing_height * (child_count - 1)) + padding_height;

        m_grid_selection_root_rect.sizeDelta    = new Vector2(m_grid_selection_root_rect.sizeDelta.x, total_height);
        m_grid_selection_root_bg_rect.sizeDelta = new Vector2(m_grid_selection_root_bg_rect.sizeDelta.x, total_height + m_grid_selection_padding.value.y);

    }

    void ClampCursorIndex()
    {
        var cur_index = m_cursor_index_subject.Value;
        var new_index = Math.Clamp(cur_index, 0, Math.Max(0, m_item_datas.Count - 1));
        if (new_index != cur_index)
            m_cursor_index_subject.OnNext(new_index);
    }


    private void OnReceiveEvent_GUI_Menu_CancelEvent(GUI_Menu_CancelEvent _event)
    {
        if (_event == null || _event.GUI_ID != ID)
            return;

        GUIManager.Instance.CloseUI(ID);
    }

    private void OnReceiveEvent_GUI_Menu_SelectEvent(GUI_Menu_SelectEvent _event)
    {
        if (_event == null || _event.GUI_ID != ID)
            return;

        var cursor_index = m_cursor_index_subject.Value;
        if (cursor_index < 0 || cursor_index >= m_item_datas.Count)
            return;

        var item_data = m_item_datas[cursor_index];
        if (item_data == null)
            return;

        item_data.OnSelectAction?.Invoke();

        GUIManager.Instance.CloseUI(ID);
    }

    private void OnReceiveEvent_GUI_Menu_MoveEvent(GUI_Menu_MoveEvent _event)
    {
        if (_event == null || _event.GUI_ID != ID)
            return;

        var move_direction = _event.MoveDirection;
        if (move_direction == Vector2Int.zero)
            return;

        // 메뉴 이동 방향이 없으면 종료.
        if (move_direction.y  == 0)
            return;

        var cur_index = m_cursor_index_subject.Value;
        var new_index = cur_index + (move_direction.y < 0 ? 1 : -1);

        // 선택 인덱스 설정.
        m_cursor_index_subject.OnNext(new_index);

        ClampCursorIndex();
    }
}
