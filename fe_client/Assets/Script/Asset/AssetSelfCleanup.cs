using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Unity.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AssetSelfCleanup : MonoBehaviour
{
    private void OnDestroy()
    {
        Addressables.ReleaseInstance(gameObject);
    }
}