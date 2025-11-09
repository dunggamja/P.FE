using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public static partial class Util
{
    static public List<string> SplitText(string _text, char _seperator) //where T : struct, Enum
    {

        var list        = new List<string>();
        if (string.IsNullOrEmpty(_text) == false)
        {
            try
            {
                ReadOnlySpan<char> span_text  = _text.AsSpan();
                ReadOnlySpan<char> span_token = ReadOnlySpan<char>.Empty;

                while (span_text.IsEmpty == false)
                {
                    int index = span_text.IndexOf(_seperator);
                    if (index >= 0)
                    {
                        span_token = span_text.Slice(0, index);
                        span_text  = span_text.Slice(index + 1);
                    }
                    else
                    {
                        span_token = span_text;
                        span_text  = ReadOnlySpan<char>.Empty;
                    }
                    
                    list.Add(span_token.ToString());
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Split Error: {ex.Message}, text: {_text}");
            }
        }

        return list;
    } 

    static public List<T> SplitEnumText<T>(string _text, char _seperator) where T : struct, Enum
    {
        var    list = SplitText(_text, _seperator);
        return list.Select(item => Enum.Parse<T>(item)).ToList();
    }
}