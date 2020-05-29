using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DynamicAtlasDemo : MonoBehaviour
{
    public GameObject image;
    public Transform imageParent;
    private int index = 0;
    private List<string> paths = new List<string>();
    private List<UIDynamicRawImage> images = new List<UIDynamicRawImage>();

    private int pathIndex = 0;

    public void OnAddClick()
    {
        index++;
        var item = imageParent.AddChild(image.gameObject);
        var pos = image.transform.localPosition;
        item.transform.localPosition = new Vector3(pos.x + (index + 1) * 100, pos.y + (index + 1) * 100, 0);

        UIDynamicRawImage data = item.transform.Find("RawImage/DynamicImage").GetComponent<UIDynamicRawImage>();
        string imageName = paths[pathIndex];
        data.SetImage(imageName);
        data.gameObject.name = imageName;
        pathIndex++;
        if (pathIndex >= paths.Count)
        {
            pathIndex = 0;
        }
        images.Add(data);
    }

    private GUIStyle _guiStyle;

    private void Start()
    {
        _guiStyle = new GUIStyle("测试");
        _guiStyle.fontSize = 30;
    }

    private void OnGUI()
    {
        if (GUILayout.Button("测试", _guiStyle))
        {
            GameObject grid = GameObject.Find("UIRoot/Grid");
            UIDynamicRawImage[] dynamicRaws = grid.GetComponentsInChildren<UIDynamicRawImage>(true);
            foreach (var raw in dynamicRaws)
            {
                Debug.Log($"raw.currentPath ===== {raw.currentPath}");
                raw.SetImage(raw.currentPath);
            }
//            UIDynamicRawImage data = grid.transform.Find("RawImage/DynamicImage").GetComponent<UIDynamicRawImage>();
//            string imageName = paths[pathIndex];
//            data.SetImage(imageName);
//            data.gameObject.name = imageName;
//            pathIndex++;
//            if (pathIndex >= paths.Count)
//            {
//                pathIndex = 0;
//            }
//            images.Add(data);
        }
    }


    public void OnSubClick()
    {
        if (images.Count == 0)
        {
            return;
        }
        //int removeIndex = Random.Range(0, images.Count - 1);
        int removeIndex = 0;
        UIDynamicRawImage obj = images[removeIndex];
        images.RemoveAt(removeIndex);
        GameObject.Destroy(obj.transform.parent.parent.gameObject);
    }

    private void Awake()
    {
        paths.Add("testres/test.png");
        for (int i = 1; i < 6; i++)
        {
            paths.Add("testres/BUFF/" + (12100000 + i) + ".png");
        }
        for (int i = 1; i < 8; i++)
        {
            paths.Add("testres/NPC/" + (11000685 + i) + ".png");
        }
        for (int i = 1; i < 10; i++)
        {
            paths.Add("testres/skill/" + (11200000 + i) + ".png");
        }
    }
   
}
