using System;
using UnityEngine;
using UnityEngine.UI;

[EventReceiver()]
public class GUIPage_Unit_Command : GUIPage, IEventReceiver
{
    public class PARAM : GUIOpenParam
    {
        public Int64 EntityID { get; private set; }

        private PARAM(Int64 _entity_id) 
        : base(
            GUIPage.GenerateID(),    // id      
            "gui/page/unit_command", // asset path
            EnumGUIType.Screen)      // gui type
        { 
            EntityID = _entity_id;  
        }


        static public PARAM Create(Int64 _entity_id)
        {
            return new PARAM(_entity_id);
        }
    }

    class MENU_ITEM_DATA
    {
        public int    Index { get; private set; } = 0;
        public string Text  { get; private set; } = "";

        public MENU_ITEM_DATA(int _index, string _text)
        {
            Index = _index;
            Text  = _text;
        }
    }


    [SerializeField]
    private RectTransform                 m_grid_menu_root_bg_rect;

    [SerializeField]
    private RectTransform                 m_grid_menu_root_rect;

    [SerializeField]
    private GridLayoutGroup               m_grid_menu_root;

    [SerializeField]
    private GUIElement_Grid_Item_MenuText m_grid_menu_item;


    private Int64                       m_entity_id = 0;           
    private MENU_ITEM_DATA[]            m_menu_item_datas;
    private (bool init, Vector2 value)  m_grid_menu_padding = (false, Vector2.zero);



    protected override void OnOpen(GUIOpenParam _param)
    {
        var param   = _param as PARAM;
        m_entity_id = param?.EntityID ?? 0;

        // 메뉴 아이템 생성
        UpdateMenuItems();  

        // 레이아웃 갱신
        UpdateLayout();


        

        // var entity = EntityManager.Instance.Find(_param.EntityID);
        
        // var entity = Battle.EntityManager.Instance.Find(_param);
        // if (entity == null)
        // {
        //     Debug.LogError($"Entity not found: {_param.EntityID}");
        //     return;
        // }
    }

    protected override void OnClose()
    {
    }

    protected override void OnPostProcess_Close()
    {
    }



    public void OnReceiveEvent(IEventParam _event)
    {
        // throw new NotImplementedException();
    }

    private void UpdateMenuItems()
    {
         // TESTCODE: 메뉴 아이템 데이터 생성.
        m_menu_item_datas = new MENU_ITEM_DATA[]
        {
            new MENU_ITEM_DATA(0, "Attack"),
            new MENU_ITEM_DATA(1, "Wait"),
            // new MENU_ITEM_DATA(2, "Move"),
            // new MENU_ITEM_DATA(3, "Skill"),
            // new MENU_ITEM_DATA(4, "Item")
        };

        // 메뉴 아이템 생성.
        for (int i = 0; i < m_menu_item_datas.Length; i++)
        {
            var clonedItem = Instantiate(m_grid_menu_item, m_grid_menu_root.transform);
            clonedItem.SetText(m_menu_item_datas[i].Text);
            clonedItem.gameObject.SetActive(true);
        }
    }

    private void UpdateLayout()
    {
        if (m_grid_menu_padding.init == false)
        {
            var padding = m_grid_menu_root_bg_rect.sizeDelta - m_grid_menu_root_rect.sizeDelta;
            padding.x   = Mathf.Abs(padding.x);
            padding.y   = Mathf.Abs(padding.y);

            m_grid_menu_padding = (true, padding);
        }

        var padding_height = m_grid_menu_root.padding.top + m_grid_menu_root.padding.bottom;
        var child_count    = m_grid_menu_root.transform.childCount;
        var cell_height    = m_grid_menu_root.cellSize.y;
        var spacing_height = m_grid_menu_root.spacing.y;
        var total_height   = (child_count * cell_height) + Mathf.Max(0, spacing_height * (child_count - 1)) + padding_height;

        m_grid_menu_root_rect.sizeDelta    = new Vector2(m_grid_menu_root_rect.sizeDelta.x, total_height);
        m_grid_menu_root_bg_rect.sizeDelta = new Vector2(m_grid_menu_root_bg_rect.sizeDelta.x, total_height + m_grid_menu_padding.value.y);
    }
}
