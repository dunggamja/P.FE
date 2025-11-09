using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static partial class Util
{
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