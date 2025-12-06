using Battle;
using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
// using Sirenix.OdinInspector;

public class DataManager : Singleton<DataManager>
{
    

    public sheet_item ItemSheet { get; private set; }
    public sheet_unit UnitSheet { get; private set; }
    public sheet_buff BuffSheet { get; private set; }

	//  public sheet_map_setting MapSettingSheet { get; private set; }


	 // 맵 설정과 관련된 데이터. (맵파일과 연관되어있다.)
	 private sheet_map_setting         m_map_setting     = null;
	 private sheet_map_faction_setting m_faction_setting = null;

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



