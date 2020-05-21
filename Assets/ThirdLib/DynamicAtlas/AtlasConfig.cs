﻿

using UnityEngine;

public class AtlasConfig 
{
    public static bool kUsingCopyTexture = true;//是否使用CopyTexture接口
#if UNITY_ANDROID
        public const TextureFormat kTextureFormat = TextureFormat.ARGB32;//android,ios的图片格式选择
#else
         public const TextureFormat kTextureFormat = TextureFormat.ARGB32;
#endif

#if UNITY_ANDROID
    public const RenderTextureFormat kRenderTextureFormat = RenderTextureFormat.ARGB32;//android,ios的图片RenderTextureFormat
#else
        public const RenderTextureFormat kRenderTextureFormat = RenderTextureFormat.ARGB32;
#endif
}
