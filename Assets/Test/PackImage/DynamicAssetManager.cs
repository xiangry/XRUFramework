using System.Collections.Generic;
using DaVikingCode.AssetPacker;
using UnityEditor.SceneManagement;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.Events;

public class DynamicAssetManager : MonoSingleton<DynamicAssetManager>
{
    Dictionary<string, AssetPacker> _assetPackers = new Dictionary<string, AssetPacker>(); 
    Dictionary<string, AssetPacker> _loadingPacker = new Dictionary<string, AssetPacker>(); 
    
    public override void Dispose()
    {
        throw new System.NotImplementedException();
    }

    public void GetAssetPackerByName(string name, UnityAction<AssetPacker> action)
    {
        AssetPacker assetPacker;
        if (_assetPackers.TryGetValue(name, out assetPacker))
        {
            action.Invoke(assetPacker);
        }
        else
        {
            GameObject obj = new GameObject("name", typeof(AssetPacker));
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localRotation = Quaternion.identity;
            obj.transform.localScale = Vector3.one;
            AssetPacker packer = obj.GetComponent<AssetPacker>();
            _loadingPacker.Add(name, packer);
            packer.OnProcessCompleted.AddListener(OnPackerProcessCompleted);
            packer.Process();
        }
    }

    public void OnPackerProcessCompleted()
    {
        _loadingPacker.Remove(name);
        _assetPackers.Add(name, packer);
        action.Invoke(packer);
    }
    
    public Sprite GetAssetSpriteByName(string packerName, string spriteName)
    {
        AssetPacker assetPacker;
        if (_assetPackers.TryGetValue(packerName, out assetPacker))
        {
            return assetPacker.GetSprite(spriteName);
        }
        return null;
    }
}
