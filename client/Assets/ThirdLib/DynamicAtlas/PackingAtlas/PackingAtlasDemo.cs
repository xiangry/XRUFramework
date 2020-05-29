using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class PackingAtlasDemo : MonoBehaviour {

    public GameObject root;
    public Transform imageParent;
    private int index = 0;
    private List<string> paths = new List<string>();
    private List<UIPackingRawImage> images = new List<UIPackingRawImage>();
    private Dictionary<Texture2D, string> texture2dDic = new Dictionary<Texture2D, string>();
    private List<Texture2D> texture2dList = new List<Texture2D>();
    private int taskIndex = 0;
    private UIPackingRawImage baseUIPackingRawImage;
    private List<GameObject> objs = new List<GameObject>();

    void OnLoadImageCallBack(string newPath, Texture2D texture2D)
    {
        if (texture2D != null)
        {
            texture2dDic[texture2D] = newPath;
        }
        else
        {
            Debug.Log("can not load obj:" + newPath);
        }
        taskIndex--;
        texture2dList.Clear();
        if (taskIndex == 0)//全都加载完
        {
            foreach (var item in texture2dDic)
            {
                texture2dList.Add(item.Key);
            }
            texture2dList.Sort((emp1, emp2) => emp1.width.CompareTo(emp2.width));
            int count = texture2dList.Count;

            for (int i = 0; i < count; i++)
            {
                index++;
                Texture2D tex2D = texture2dList[i];
                var item = imageParent.AddChild(root.gameObject);
                objs.Add(item);
                var pos = imageParent.transform.localPosition;

                item.transform.localPosition = new Vector3(pos.x + (index + 1) * 100, pos.y + (index + 1) * 100, 0);

                UIPackingRawImage image = item.transform.Find("RawImage/DynamicImage").GetComponent<UIPackingRawImage>();
                image.SetGroup(baseUIPackingRawImage.mGroup);
                image.SetImage(texture2dDic[tex2D], tex2D);
            }
            texture2dDic.Clear();
            texture2dList.Clear();
        }

    }
    public void OnAddClick()
    {
        int count = paths.Count;
        taskIndex = count;
        for (int i = 0; i < count; i++)
        {
            string path = paths[i];
            var ao = Addressables.LoadAssetAsync<Texture2D>(path);
            ao.Completed += handle =>
            {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    OnLoadImageCallBack(path, handle.Result);
                }
            };
        }
    }

    public void OnSubClick()
    {
        var m_Atlas = DynamicAtlasManager.Instance.GetPackingAtlas(baseUIPackingRawImage.mGroup);
        m_Atlas.ClearAtlas();
        foreach (var item in objs)
        {
            GameObject.Destroy(item);
        }
        objs.Clear();
    }

    private void Awake()
    {
        baseUIPackingRawImage = root.transform.Find("RawImage/DynamicImage").GetComponent<UIPackingRawImage>();
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
            paths.Add("testres/skill/" + (11220000 + i) + ".png");
        }
    }
}
