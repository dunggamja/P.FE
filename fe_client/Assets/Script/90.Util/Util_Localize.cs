using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;

public static partial class Util
{
    public static async UniTask<string> GetNameTextAsync(this Item _item)
    {
        if (_item == null)
        {
            return string.Empty;
        }

        var localize_key = _item.GetLocalizeName();

        var item_value   = _item.Value;

        var text = await LocalizationManager.Instance.GetTextAsync(localize_key.Table, localize_key.Key);


        // Value 값이 있으면 추가.
        if (item_value != 0)
        {
            return $"{text} {item_value}";
        }
        else
        {
            return text;
        }
    }

    public static async UniTask<string> GetDescTextAsync(this Item _item, bool _show_count = false)
    {
        if (_item == null)
        {
            return string.Empty;
        }
        
        var localize_key = _item.GetLocalizeDesc();
        var item_value   = _item.Value;

        var text = await LocalizationManager.Instance.GetTextAsync(localize_key.Table, localize_key.Key);

        if (item_value != 0)
        {
            return $"{text} {item_value}";
        }
        else
        {
            return text;
        }
    }

    public static async UniTask<string> GetDescTextAsync(this Item _item)
    {
        if (_item == null)
        {
            return string.Empty;
        }
        
        var localize_key = _item.GetLocalizeDesc();

        return await LocalizationManager.Instance.GetTextAsync(localize_key.Table, localize_key.Key);
    }


    public static LocalizeKey GetLocalizeName(this Item _item)
    {
        if (_item == null)
        {
            // null ����ó��.
            return LocalizeKey.Create("localization_base", "ui_empty");
        }
        
        var (table, key) = DataManager.Instance.ItemSheet.GetLocalizeName(_item.Kind);
        return LocalizeKey.Create(table, key);
    }

    public static LocalizeKey GetLocalizeDesc(this Item _item)
    {
        if (_item == null)
        {
            // null ����ó��.
            return LocalizeKey.Create("localization_base", "ui_empty");
        }
        
        var (table, key) = DataManager.Instance.ItemSheet.GetLocalizeDesc(_item.Kind);
        return LocalizeKey.Create(table, key);
    }
}