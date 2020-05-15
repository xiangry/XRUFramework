using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLaunch : MonoBehaviour
{
    private GameObject UIRoot = null;
    
    // Start is called before the first frame update
    IEnumerator Start()
    {
        UIRoot = GameObject.Find("UIRoot");
        DontDestroyOnLoad(UIRoot);

        
        
        yield return OpenView();
    }

    
    
    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator OpenView()
    {
        var loader = KVResourceMgr.Instance.LoadAssetAsync("prefabs/launch.prefab");
        yield return loader;
        GameObject obj = loader.asset as GameObject;
        if (obj)
        {
            obj.transform.SetParent(UIRoot.transform);
        }
    }
}
