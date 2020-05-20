using System.Collections;
using System.Collections.Generic;
using DaVikingCode.AssetPacker;
using UnityEngine;
using UnityEngine.UI;

public class DynamicImage : Image
{
    public string cacheTexture = "Temp";
    public string imagePath = "";

    void Awake()
    {
        SetTexturePath(imagePath);
        base.Awake();
    }


    public void SetTexturePath(string path)
    {
        if (Application.isPlaying)
        {
            Debug.Log($"DynamicImage  {path}");
            DynamicAssetManager.Instance.AddAssetSpriteByName(cacheTexture, path, sprite =>
            {
                Debug.Log($"AddAssetSpriteByName === {sprite}");
                this.sprite = sprite;
            });
        }
    }
}
