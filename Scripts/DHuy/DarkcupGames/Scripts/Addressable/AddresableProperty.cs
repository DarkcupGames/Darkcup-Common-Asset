using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;

public class AddresableProperty : ObjectProperties
{
    public const int PRE_LOAD_ASSET_AMOUNT = 5;
    public static AddresableProperty Instance { get; private set; }
    [SerializeField] private AssetReference[] addresableRefs;
    public UnityEvent<float> OnDoneLoadingEach;
    [SerializeField] private int currentAssetLoaded;
    public int maxLevelIndex { get { return addresableRefs.Length; } }

    protected override void Awake ()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
        ready = false;
        SceneManager.activeSceneChanged += ClearEvent;
    }

    public override async void Init ()
    {
        await LoadAllRef ();
    }

    public async Task LoadAsset (int index, Action<GameObject> onLoadComplete)
    {
        if (index < 0 || index >= addresableRefs.Length)
        {
            Debug.LogError ("invalid index " + index);
            return;
        }
        if (collection.ContainsKey (index))
        {
            onLoadComplete?.Invoke (collection[index]);
            return;
        }
        if (addresableRefs[index].IsValid ())
        {
            var obj = (GameObject)addresableRefs[index].Asset;
            if (collection.ContainsKey (index) == false) collection.Add (index, obj);
            onLoadComplete?.Invoke (collection[index]);
            return;
        }
        var handle = addresableRefs[index].LoadAssetAsync<GameObject> ();
        await handle.Task;
        if (handle.Status == AsyncOperationStatus.Failed)
        {
            Debug.LogError ($"Fail to load reference of {addresableRefs[index].Asset.name}");
            return;
        }
        collection.Add (index, handle.Result);
        onLoadComplete?.Invoke (collection[index]);
    }

    public async void PreLoad(int preloadAmount = PRE_LOAD_ASSET_AMOUNT, Action onPreloadComplete = null)
    {
        await PreLoadAsset(preloadAmount, onPreloadComplete);
    }

    private async Task PreLoadAsset (int preloadAmount, Action onPreloadComplete)
    {
        var currentLoaded = currentAssetLoaded;
        var loadCount = 0;
        var amount = currentAssetLoaded + preloadAmount;
        if(amount > addresableRefs.Length) amount = addresableRefs.Length;
        if(currentLoaded == amount)
        {
            Debug.Log ("All assets has been loaded");
            return;
        }
        for (int i = currentLoaded; i < amount; i++)
        {
            if (addresableRefs[i].IsValid())
            {
                AddToCollection (i, (GameObject)addresableRefs[i].Asset, ref loadCount);
                continue;
            }
            var handle = addresableRefs[i].LoadAssetAsync<GameObject> ();
            await handle.Task;
            if (handle.Status == AsyncOperationStatus.Failed) Debug.LogError ($"Fail to load reference of {addresableRefs[i].Asset.name}");
            else AddToCollection (i, handle.Result, ref loadCount);
            OnDoneLoadingEach?.Invoke ((float)loadCount / preloadAmount);
        }
        ready = true;
        Debug.Log ($"Preload Completed {loadCount} assets. Total completed {currentAssetLoaded} assets");
        onPreloadComplete?.Invoke ();
    }

    private void AddToCollection(int key, GameObject obj, ref int loadCount)
    {
        if (collection.ContainsKey (key))
        {
            Debug.Log ($"Duplicate key of object name {addresableRefs[key].Asset.name}. Skip add to collection");
            return;
        }
        collection.Add (key, obj);
        currentAssetLoaded++;
        loadCount++;
    }

    private async Task LoadAllRef ()
    {
        for (int i = 0; i < addresableRefs.Length; i++)
        {
            var handle = addresableRefs[i].LoadAssetAsync<GameObject> ();
            await handle.Task;
            if (handle.Status == AsyncOperationStatus.Failed) Debug.LogError ($"Fail to load reference of {addresableRefs[i].Asset.name}");
            else
            {
                if (collection.ContainsKey (i))
                {
                    Debug.LogError ($"Duplicate key of object name {addresableRefs[i].Asset.name}");
                    return;
                }
                if (collection.ContainsValue ((GameObject)addresableRefs[i].Asset))
                {
                    Debug.LogError ($"Duplicate value of object name {addresableRefs[i].Asset.name}");
                    return;
                }
                collection.Add (i, handle.Result);
            }
        }
        ready = true;
        onLoadComplete?.Invoke ();
    }

    private void ClearEvent(UnityEngine.SceneManagement.Scene thisScene, UnityEngine.SceneManagement.Scene nextScene)
    {
        onLoadComplete.RemoveAllListeners ();
    }
}
