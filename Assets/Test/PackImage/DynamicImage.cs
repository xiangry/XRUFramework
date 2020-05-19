using System.Collections;
using System.Collections.Generic;
using DaVikingCode.AssetPacker;
using UnityEngine;
using UnityEngine.UI;

public class DynamicImage : Image
{
    public string cacheTexture = "Temp";
    
    // Start is called before the first frame update
    void Start()
    {
        sprite = DynamicAssetManager.instance.GetAssetSpriteByName(cacheTexture, "walking0003");
    }

    void SyncSprite()
    {
        sprite = DynamicAssetManager.instance.GetAssetSpriteByName(cacheTexture, "walking0003");
        
        
    }

}
