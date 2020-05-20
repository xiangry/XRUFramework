using DaVikingCode.RectanglePacking;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace DaVikingCode.AssetPacker {

	public class AssetPacker : MonoBehaviour {

		public UnityAction<string> OnProcessCompleted;
		public float pixelsPerUnit = 100.0f;

		public bool useCache = false;
		public string cacheName = "";
		public int cacheVersion = 1;
		public bool deletePreviousCacheVersion = true;

		protected Dictionary<string, Sprite> mSprites = new Dictionary<string, Sprite>();
		protected List<TextureToPack> itemsToRaster = new List<TextureToPack>();

		protected bool allow4096Textures = false;
		public int defalutTextureSize = 1024;
		public int padding = 1;

		public void AddTextureToPack(string file, string customID = null) {
			itemsToRaster.Add(new TextureToPack(file, customID != null ? customID : Path.GetFileNameWithoutExtension(file)));
		}

		public void AddTexturesToPack(string[] files) {

			foreach (string file in files)
				AddTextureToPack(file);
		}
		
		public void AddTextureToPack(string name, Texture2D texture2D, string customID = null)
		{

			Sprite spr;
//			textures.Add(texture2D);
//			images.Add(name);
		}

		public void Process(bool allow4096Textures = false) {

			this.allow4096Textures = allow4096Textures;

			if (useCache) {

				if (cacheName == "")
					throw new Exception("No cache name specified");

				string path = Application.persistentDataPath + "/AssetPacker/" + cacheName + "/" + cacheVersion + "/";

				bool cacheExist = Directory.Exists(path);

				if (!cacheExist)
					StartCoroutine(createPack(path));
				else
					StartCoroutine(loadPack(path));
				
			} else
				StartCoroutine(createPack());
			
		}

		protected IEnumerator createPack(string savePath = "") {

			if (savePath != "") {

				if (deletePreviousCacheVersion && Directory.Exists(Application.persistentDataPath + "/AssetPacker/" + cacheName + "/"))
					foreach (string dirPath in Directory.GetDirectories(Application.persistentDataPath + "/AssetPacker/" + cacheName + "/", "*", SearchOption.AllDirectories))
						Directory.Delete(dirPath, true);

				Directory.CreateDirectory(savePath);
			}

			List<Texture2D> textures = new List<Texture2D>();
			List<string> images = new List<string>();

			foreach (TextureToPack itemToRaster in itemsToRaster)
			{
				Debug.Log($"add package file {itemToRaster.file}");
				var ao = Addressables.LoadAssetAsync<Texture2D>(itemToRaster.file);
				yield return ao;
				if (ao.IsDone && ao.IsValid())
				{
					textures.Add(ao.Result);
					images.Add(itemToRaster.id);
				}
			}

			int textureSize = allow4096Textures ? 4096 : defalutTextureSize;

			List<Rect> rectangles = new List<Rect>();
			for (int i = 0; i < textures.Count; i++)
				if (textures[i].width > textureSize || textures[i].height > textureSize)
				{
					Debug.LogError($"纹理超出了 {textureSize}");
					throw new Exception("A texture size is bigger than the sprite sheet size!");
				}
				else
					rectangles.Add(new Rect(0, 0, textures[i].width, textures[i].height));

			int numSpriteSheet = 0;
			while (rectangles.Count > 0) {

				Texture2D texture = new Texture2D(textureSize, textureSize, TextureFormat.ARGB32, false);
				Color32[] fillColor = texture.GetPixels32();
				for (int i = 0; i < fillColor.Length; ++i)
					fillColor[i] = Color.clear;

				RectanglePacker packer = new RectanglePacker(texture.width, texture.height, padding);

				for (int i = 0; i < rectangles.Count; i++)
					packer.insertRectangle((int) rectangles[i].width, (int) rectangles[i].height, i);

				packer.packRectangles();

				if (packer.rectangleCount > 0) {

					texture.SetPixels32(fillColor);
					IntegerRectangle rect = new IntegerRectangle();
					List<TextureAsset> textureAssets = new List<TextureAsset>();

					List<Rect> garbageRect = new List<Rect>();
					List<Texture2D> garabeTextures = new List<Texture2D>();
					List<string> garbageImages = new List<string>();

					for (int j = 0; j < packer.rectangleCount; j++) {

						rect = packer.getRectangle(j, rect);

						int index = packer.getRectangleId(j);

						texture.SetPixels32(rect.x, rect.y, rect.width, rect.height, textures[index].GetPixels32());

						TextureAsset textureAsset = new TextureAsset();
						textureAsset.x = rect.x;
						textureAsset.y = rect.y;
						textureAsset.width = rect.width;
						textureAsset.height = rect.height;
						textureAsset.name = images[index];

						textureAssets.Add(textureAsset);

						garbageRect.Add(rectangles[index]);
						garabeTextures.Add(textures[index]);
						garbageImages.Add(images[index]);
					}

					foreach (Rect garbage in garbageRect)
						rectangles.Remove(garbage);

					foreach (Texture2D garbage in garabeTextures)
						textures.Remove(garbage);

					foreach (string garbage in garbageImages)
						images.Remove(garbage);

					texture.Apply();

					if (savePath != "") {

						File.WriteAllBytes(savePath + "/data" + numSpriteSheet + ".png", texture.EncodeToPNG());
						File.WriteAllText(savePath + "/data" + numSpriteSheet + ".json", JsonUtility.ToJson(new TextureAssets(textureAssets.ToArray())));
						++numSpriteSheet;
					}

					foreach (TextureAsset textureAsset in textureAssets)
					{
						if (!mSprites.ContainsKey(textureAsset.name))
						{
							mSprites.Add(textureAsset.name, Sprite.Create(texture, new Rect(textureAsset.x, textureAsset.y, textureAsset.width, textureAsset.height), Vector2.zero, pixelsPerUnit, 0, SpriteMeshType.FullRect));
						}
					}

				}

			}

			OnProcessCompleted.Invoke(cacheName);
		}

		protected IEnumerator loadPack(string savePath) {
			int numFiles = Directory.GetFiles(savePath).Length;
			Debug.Log($"loadPack savePath:{savePath}  fileNums:{numFiles}");
			if (numFiles >= 2)
			{
				for (int i = 0; i < numFiles / 2; ++i)
				{
					var textureUrl = "file:///" + savePath + "/data" + i + ".png";
					var textureRequest = UnityWebRequestTexture.GetTexture(textureUrl);
					yield return textureRequest.SendWebRequest();
				
					var jsonUrl = "file:///" + savePath + "/data" + i + ".json";
					var jsonRequest = UnityWebRequest.Get(jsonUrl);
					yield return jsonRequest.SendWebRequest();
					if (!jsonRequest.isNetworkError && !textureRequest.isNetworkError)
					{
						string text = jsonRequest.downloadHandler.text;
						Debug.Log($" json text:{text}");
						TextureAssets textureAssets = JsonUtility.FromJson<TextureAssets>(text);

						Texture2D t = DownloadHandlerTexture.GetContent(textureRequest); // prevent creating a new Texture2D each time.
						foreach (TextureAsset textureAsset in textureAssets.assets)
						if (!mSprites.ContainsKey(textureAsset.name))
						{
							mSprites.Add(textureAsset.name, Sprite.Create(t, new Rect(textureAsset.x, textureAsset.y, textureAsset.width, textureAsset.height), Vector2.zero, pixelsPerUnit, 0, SpriteMeshType.FullRect));
						}
					}
					textureRequest.Dispose();
					jsonRequest.Dispose();
				}
				yield return null;
				OnProcessCompleted.Invoke(cacheName);
			}
			else
			{
				yield return createPack(savePath);
			}
		}

		public void Dispose() {

			foreach (var asset in mSprites)
				Destroy(asset.Value.texture);

			mSprites.Clear();
		}

		void Destroy() {

			Dispose();
		}

		public Sprite GetSprite(string id) {

			Sprite sprite = null;

			mSprites.TryGetValue (id, out sprite);

			return sprite;
		}

		public Sprite[] GetSprites(string prefix) {

			List<string> spriteNames = new List<string>();
			foreach (var asset in mSprites)
				if (asset.Key.StartsWith(prefix))
					spriteNames.Add(asset.Key);

			spriteNames.Sort(StringComparer.Ordinal);

			List<Sprite> sprites = new List<Sprite>();
			Sprite sprite;
			for (int i = 0; i < spriteNames.Count; ++i) {

				mSprites.TryGetValue(spriteNames[i], out sprite);

				sprites.Add(sprite);
			}

			return sprites.ToArray();
		}
		
		/// <summary>
		/// 运行模式下Texture转换成Texture2D
		/// </summary>
		/// <param name="texture"></param>
		/// <returns></returns>
		private Texture2D TextureToTexture2D(Texture texture) {
			Texture2D texture2D = new Texture2D(texture.width, texture.height, TextureFormat.RGBA32, false);
			RenderTexture currentRT = RenderTexture.active;
			RenderTexture renderTexture = RenderTexture.GetTemporary(texture.width, texture.height, 32);
			Graphics.Blit(texture, renderTexture);

			RenderTexture.active = renderTexture;
			texture2D.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
			texture2D.Apply();

			RenderTexture.active = currentRT;
			RenderTexture.ReleaseTemporary(renderTexture);

			return texture2D;
		}
	}
}
