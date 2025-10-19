using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public static partial class Util
{
    static public List<T> SplitText<T>(string _text, char _seperator) where T : struct, Enum
    {

        var list = new List<T>();

        if (string.IsNullOrEmpty(_text) == false)
        {
            try
            {
                var items = _text.Split(_seperator);
                foreach (var item in items)
                {
                    if (Enum.TryParse(item, out T result))
                        list.Add(result);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Split Error: {ex.Message}, text: {_text}");
            }
        }

        return list;
    } 
}