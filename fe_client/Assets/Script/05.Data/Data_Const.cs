using System;
using System.Collections.Generic;
using UnityEngine;

public static class AssetName
{
    public const string TEST_PREFAB      = "local_base/test";
    public const string TILE_HIGHLIGHT   = "vfx/tile_highlight";
    public const string TILE_SELECTION   = "vfx/tile_selection";    // 타일 선택.

    public const string TILE_EFFECT_PATH  = "vfx/tile_effect_path"; // 이동 경로.
    public const string TILE_EFFECT_BLUE  = "vfx/tile_effect_blue"; // 이동 범위 표시.
    public const string TILE_EFFECT_RED   = "vfx/tile_effect_red";  // 공격 범위 표시.
    public const string TILE_EFFECT_GREEN = "vfx/tile_effect_green"; // 지팡이 범위 표시.
    public const string TILE_EFFECT       = "vfx/tile_effect";


    // public const string SHEET_WEAPON     = "sheet/weapon";
    public const string SHEET_ITEM       = "sheet/item";
    public const string SHEET_UNIT       = "sheet/unit";
    public const string SHEET_BUFF       = "sheet/buff";

}

public static class Data_Const
{
    public const Int32 KIND_WEAPON_SWORD_IRON = 1;
    public const Int32 KIND_WEAPON_SWORD_KILL = 2;

    public const char SHEET_SEPERATOR = ';';
    public const char SHEET_LOCALIZATION_SEPERATOR = '/';



    public const int  UNIT_INVENTORY_MAX = 8;

    public const int  CHAPTER_NUMBER_MAX = 1000;
    public const int  STAGE_NUMBER_MAX   = 1000;

}

