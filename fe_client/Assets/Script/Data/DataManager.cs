using Cysharp.Threading.Tasks;
using UnityEngine;

public class DataManager : Singleton<DataManager>
{
    

    public sheet_weapon WeaponSheet { get; private set; }

    protected override void Init()
    {
        base.Init();

    }


    public async UniTask LoadSheetData()
    {
        WeaponSheet = await AssetManager.Instance.LoadAssetAsync<sheet_weapon>(AssetName.SHEET_WEAPON);

        
        WeaponSheet.Initialize();     
    }
}



