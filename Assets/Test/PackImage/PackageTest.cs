using System;
using System.Collections;
using System.Collections.Generic;
using DaVikingCode.AssetPacker;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PackageTest : MonoBehaviour
{
    private Canvas _launchLayer;
    private Image _image;
    private RawImage _rawImage;

    private AssetPacker _assetPacker;

    private UnityAction testAction;
    
    // Start is called before the first frame update
    void Start()
    { 
//        GameObject uiRoot = GameObject.Find("UIRoot");
//        DontDestroyOnLoad(uiRoot);
//        DontDestroyOnLoad(gameObject);
//
//        _launchLayer = uiRoot.transform.Find("LaunchLayer").GetComponent<Canvas>();
//
//        _image = _launchLayer.transform.Find("Image").GetComponent<Image>();
//        _rawImage = _launchLayer.transform.Find("RawImage").GetComponent<RawImage>();
//        
//        _assetPacker = gameObject.GetComponent<AssetPacker>();
//        _assetPacker.OnProcessCompleted += OnCompleted;
//        List<string> spriteNames = new List<string>();
//        for (int i = 0; i < 11; i++)
//        {   
//            spriteNames.Add($"textures/pack_imgs/walking{i+1:D4}.png");
//        }
//        _assetPacker.AddTexturesToPack(spriteNames.ToArray());
//        _assetPacker.Process();
    }

    void OnCompleted(string cacheName) {
        Debug.Log("Process ----- done");
        var sprite = _assetPacker.GetSprite("walking0011");
        _image.sprite = sprite;

        for (int i = 4; i < 9; i++)
        {
            Image img = Instantiate(_image, _image.transform.parent);
            img.name = $"walking{i:D4}";
            img.sprite = _assetPacker.GetSprite(img.name);
        }
    }
    
    private void OnGUI()
    {
        if (GUILayout.Button("测试"))
        {
            Debug.Log("测试------");
            DynamicImage img = GameObject.Find("UIRoot/LaunchLayer/DynamicImage").GetComponent<DynamicImage>();
            img.SetTexturePath("textures/pack_imgs/walking0010.png");
        }
    }
}
