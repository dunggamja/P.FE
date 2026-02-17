using System;
using System.Collections;
using System.Collections.Generic;
using Battle;
using UnityEngine;


[Serializable]
public class sheet_chapter_map
{
    public Int32       CHAPTER;          // 장 번호 (1, 2, 3...)
    public string      MEMO;             // 메모/설명
    public Int32       STAGE_NUMBER;     // 스테이지 번호
    public int         STAGE_TYPE;       // "main:1" 또는 "side:2"
    public bool        FREE;             // 필수 여부 (메인은 항상 false, 사이드는 true/false)
    public string      MAP_CONDITION_NAME;  // 맵 등장 조건
    public string      MAP_FILE_NAME;    // 맵 파일명 (예: "01_main", "01_side_01")
    public string      MAP_SHEET_NAME;   // 맵 설정 파일명
    public string      MAP_FACTION_NAME; // 진영 설정 파일명
    public string      MAP_SCRIPT_NAME;  // 진영 설정 파일명

    public bool        IsMainStage => STAGE_TYPE == (int)EnumChapterStageType.Main;
    public bool        IsSideStage => STAGE_TYPE == (int)EnumChapterStageType.Side;
    public bool        IsFreeStage => IsMainStage == false && FREE == true;

    public EnumChapterStageType StageType => (EnumChapterStageType)STAGE_TYPE;
}


[ExcelAsset]
public class sheet_chapter : ScriptableObject
{
    public List<sheet_chapter_map> stages = new();

    private Dictionary<int, List<sheet_chapter_map>> m_cache_stages = new();

    public void Initialize()
    {
        m_cache_stages.Clear();
        if (stages != null)
        {
            foreach (var stage in stages)
            {
                if (m_cache_stages.TryGetValue(stage.CHAPTER, out var list_stages) == false)
                {
                    list_stages = new List<sheet_chapter_map>();
                    m_cache_stages.Add(stage.CHAPTER, list_stages);
                }

                list_stages.Add(stage);
            }

            foreach ((var _, var list_stage) in m_cache_stages)
            {
                list_stage.Sort((a, b) => a.STAGE_NUMBER.CompareTo(b.STAGE_NUMBER));            
            }
        }
    }
}