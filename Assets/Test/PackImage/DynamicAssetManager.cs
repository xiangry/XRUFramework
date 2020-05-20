using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using DaVikingCode.AssetPacker;
using UnityEditor.SceneManagement;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.Events;

public class AssetPackerLoader : IDisposable
{
    public AssetPacker assetPacker;
    public string cacheName;
    public UnityAction<AssetPacker> completeAction;

    public AssetPackerLoader(string name, AssetPacker packer, UnityAction<AssetPacker> action)
    {
        cacheName = name;
        assetPacker = packer;
        completeAction = action;
    }

    public void Dispose()
    {
        cacheName = null;
        assetPacker = null;
        completeAction = null;
    }
}

public class SpriteAdder : IDisposable
{
    public string textureName;
    public UnityAction<Sprite> completeAction;

    public SpriteAdder(string name, UnityAction<Sprite> action)
    {
        textureName = name;
        completeAction = action;
    }

    public void Dispose()
    {
        textureName = null;
        completeAction = null;
    }
}

public class DynamicAssetManager : MonoSingleton<DynamicAssetManager>
{
    Dictionary<string, AssetPacker> _assetPackers = new Dictionary<string, AssetPacker>(); 
    Dictionary<string, AssetPackerLoader> _loadingPacker = new Dictionary<string, AssetPackerLoader>(); 
    Dictionary<string, SpriteAdder> _spriteAdder = new Dictionary<string, SpriteAdder>(); 
    
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
            GameObject obj = new GameObject(name);
            obj.transform.parent = transform;
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localRotation = Quaternion.identity;
            obj.transform.localScale = Vector3.one;
            AssetPacker packer = obj.AddComponent<AssetPacker>();
            packer.cacheName = name;
            packer.useCache = false;
            packer.cacheVersion = 1;
            _loadingPacker.Add(name, new AssetPackerLoader(name, packer, action));
            packer.OnProcessCompleted = OnPackerProcessCompleted; 
            packer.Process();
        }
    }

    public void OnPackerProcessCompleted(string cacheName)
    {
        AssetPackerLoader loader;
        if (_loadingPacker.TryGetValue(cacheName, out loader))
        {
            loader.completeAction.Invoke(loader.assetPacker);
            _assetPackers.Add(cacheName, loader.assetPacker);
            loader.Dispose();
            _loadingPacker.Remove(cacheName);
        }
        else
        {
            Debug.LogError($"没有保存的加载中的AssetPacker:{cacheName}");
            AssetPacker packer;
            if (_assetPackers.TryGetValue(cacheName, out packer))
            {
                Debug.Log($" cacheName  {packer}");
                SpriteAdder adder;
                if (_spriteAdder.TryGetValue(cacheName, out adder))
                {
                    Debug.Log($" cacheName  {adder} {adder.textureName}  {packer.GetSprite(adder.textureName)}");
                    adder.completeAction(packer.GetSprite(adder.textureName));
                }
            }
        }
    }
    
    public void GetAssetSpriteByName(string packerName, string spriteName, UnityAction<Sprite> action)
    {
        GetAssetPackerByName(packerName, assetPacker =>
        {
            Sprite sprite = assetPacker.GetSprite(spriteName);
            if (sprite)
            {
                action.Invoke(sprite);
            }
            else
            {
                Debug.Log($"AssetPacker({packerName})找不到sprite:{spriteName}");
            }
        });
    }
    
    public void AddAssetSpriteByName(string packerName, string texturePath, UnityAction<Sprite> action)
    {
        GetAssetPackerByName(packerName, assetPacker =>
        {
            var sname = Path.GetFileNameWithoutExtension(texturePath);
            Sprite sprite = assetPacker.GetSprite(sname);
            if (sprite)
            {
                action.Invoke(sprite);
            }
            else
            {
                Debug.Log($"AssetPacker({packerName})找不到sprite:{texturePath}");
                assetPacker.AddTextureToPack(texturePath);
                _spriteAdder.Add(packerName, new SpriteAdder(sname, action));
                assetPacker.Process();
            }
        });
    }
}
