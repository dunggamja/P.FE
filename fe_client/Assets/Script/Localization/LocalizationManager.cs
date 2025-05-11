using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using System;
using Cysharp.Threading.Tasks;
using R3;

public class LocalizationManager : Singleton<LocalizationManager>
{
    
    public string GetText(string _key)
    {
        return LocalizationSettings.StringDatabase.GetLocalizedString(_key);
    }

    public async UniTask<string> GetTextAsync(string _key)
    {
        var    handle = LocalizationSettings.StringDatabase.GetLocalizedStringAsync(_key);
        await  handle;
        return handle.Result;
    }

    public IObservable<string> GetTextObservable(string _key)
    {
        return GetTextAsync(_key).ToObservable();
    }

}
