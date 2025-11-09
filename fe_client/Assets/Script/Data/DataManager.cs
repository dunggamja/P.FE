using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

public class DataManager : Singleton<DataManager>
{
    

    public sheet_item ItemSheet { get; private set; }
    public sheet_unit UnitSheet { get; private set; }
    public sheet_buff BuffSheet { get; private set; }
    protected override void Init()
    {
        base.Init();
    }


    public async UniTask LoadSheetData()
    {
        ItemSheet = await AssetManager.Instance.LoadAssetAsync<sheet_item>(AssetName.SHEET_ITEM);
        UnitSheet = await AssetManager.Instance.LoadAssetAsync<sheet_unit>(AssetName.SHEET_UNIT);
        BuffSheet = await AssetManager.Instance.LoadAssetAsync<sheet_buff>(AssetName.SHEET_BUFF);


        ItemSheet.Initialize();     
        UnitSheet.Initialize();
        BuffSheet.Initialize();
    }
}



