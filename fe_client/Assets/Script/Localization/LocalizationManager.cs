using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using System;
using Cysharp.Threading.Tasks;
using R3;

public class LocalizationManager : Singleton<LocalizationManager>
{
    
    public string GetText(string _table, string _key)
    {
        return LocalizationSettings.StringDatabase.GetLocalizedString(_table, _key);
    }

    public async UniTask<string> GetTextAsync(string _table, string _key)
    {
        var    handle = LocalizationSettings.StringDatabase.GetLocalizedStringAsync(_table, _key);
        await  handle;
        return handle.Result;
    }

    public Observable<string> GetTextObservable(string _table, string _key)
    {
        return Observable.Create<string>(observer =>
        {
            GetTextAsync(_table, _key).ContinueWith(text =>
            {
                observer.OnNext(text);
                observer.OnCompleted();
            });

            return Disposable.Empty;
        });
    }
}
