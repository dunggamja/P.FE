using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Battle;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceProviders;


public class BootstrapLoader : MonoBehaviour
{
    async void Awake()
    {
        
        RegisterServices();

        await LoadScene("demo/scene");

        Debug.Log("BootstrapLoader: Scene loaded");
    }


    void RegisterServices()
    {
        
        ServiceLocator<CommandQueueHandler>.Register(ServiceLocator.GLOBAL, BattleSystemManager.Instance.CommandHandler);
    }

    async UniTask<SceneInstance> LoadScene(string _scene_name)
    {
        var    handle = Addressables.LoadSceneAsync(_scene_name, loadMode: UnityEngine.SceneManagement.LoadSceneMode.Single);
        await  handle.ToUniTask();
        return handle.Result;
    }

}