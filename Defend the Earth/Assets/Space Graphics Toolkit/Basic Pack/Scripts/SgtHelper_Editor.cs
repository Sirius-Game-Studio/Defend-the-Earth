#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace SpaceGraphicsToolkit
{
	public static partial class SgtHelper
	{
		private static string undoName;

		public static void BeginUndo(string newUndoName)
		{
			undoName = newUndoName;
		}

		public static Texture2D CreateTempTexture(int width, int height, string encoded)
		{
			var texture = new Texture2D(width, height, TextureFormat.ARGB32, false);

			texture.hideFlags = HideFlags.HideAndDontSave;
			texture.LoadImage(System.Convert.FromBase64String(encoded));
			texture.Apply();

			return texture;
		}

		public static Rect Reserve(float height = 16.0f)
		{
			var rect = default(Rect);

			rect = EditorGUILayout.BeginVertical();
			{
				EditorGUILayout.LabelField(string.Empty, GUILayout.Height(height));
			}
			EditorGUILayout.EndVertical();

			return rect;
		}

		public static void SetDirty<T>(T t)
			where T : Object
		{
			if (t != null)
			{
				EditorUtility.SetDirty(t);
			}
		}

		public static void SetDirty<T>(T[] ts)
			where T : Object
		{
			foreach (var t in ts)
			{
				SetDirty(t);
			}
		}

		public static T LoadFirstAsset<T>(string pattern) // e.g. "Name t:mesh"
			where T : Object
		{
			var guids = UnityEditor.AssetDatabase.FindAssets(pattern);

			if (guids.Length > 0)
			{
				var path = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[0]);

				return (T)UnityEditor.AssetDatabase.LoadAssetAtPath(path, typeof(T));
			}

			return null;
		}

		public static List<T> LoadAllAssets<T>(string pattern) // e.g. "*.prefab"
			where T : Object
		{
			var assets   = new List<T>();
			var basePath = Application.dataPath;
			var files    = new List<string>(); GetFilesRecursive(files, basePath, pattern);
			var sub      = basePath.Length - "Assets".Length;

			for (var i = 0; i < files.Count; i++)
			{
				EditorUtility.DisplayProgressBar("Loading Assets", "", (float)files.Count / (float)i);

				var file  = files[i];
				var path  = file.Substring(sub);
				var asset = LoadAsset<T>(path);

				if (asset != null)
				{
					assets.Add(asset);
				}
			}

			EditorUtility.ClearProgressBar();

			return assets;
		}

		public static T LoadAsset<T>(string path)
			where T : Object
		{
			return (T)AssetDatabase.LoadAssetAtPath(path, typeof(T));
		}

		private static void GetFilesRecursive(List<string> files, string path, string pattern)
		{
			files.AddRange(System.IO.Directory.GetFiles(path, pattern));

			var directories = System.IO.Directory.GetDirectories(path);

			foreach (var directory in directories)
			{
				GetFilesRecursive(files, directory, pattern);
			}
		}

		public static T GetAssetImporter<T>(Object asset)
			where T : AssetImporter
		{
			return GetAssetImporter<T>((AssetDatabase.GetAssetPath(asset)));
		}

		public static T GetAssetImporter<T>(string path)
			where T : AssetImporter
		{
			return AssetImporter.GetAtPath(path) as T;
		}

		public static void ReimportAsset(Object asset)
		{
			ReimportAsset(AssetDatabase.GetAssetPath(asset));
		}

		public static void ReimportAsset(string path)
		{
			AssetDatabase.ImportAsset(path);
		}

		public static void MakeTextureReadable(Texture texture)
		{
			if (texture != null)
			{
				var importer = GetAssetImporter<TextureImporter>(texture);

				if (importer != null && importer.isReadable == false)
				{
					importer.isReadable = true;

					ReimportAsset(importer.assetPath);
				}
			}
		}

		public static void MakeTextureTruecolor(Texture2D texture)
		{
			if (texture != null)
			{
				var importer = GetAssetImporter<TextureImporter>(texture);

				if (importer != null)
				{
					if (importer.textureCompression != TextureImporterCompression.Uncompressed)
					{
						importer.textureCompression = TextureImporterCompression.Uncompressed;

						ReimportAsset(importer.assetPath);
					}
				}
			}
		}

		public static void ClearSelection()
		{
			Selection.objects = new Object[0];
		}

		public static void AddToSelection(Object o)
		{
			var os = new List<Object>(Selection.objects);

			os.Add(o);

			Selection.objects = os.ToArray();
		}

		public static void SelectAndPing(Object o)
		{
			Selection.activeObject = o;

			EditorApplication.delayCall += () => EditorGUIUtility.PingObject(o);
		}

		public static Transform GetSelectedParent()
		{
			if (Selection.activeGameObject != null)
			{
				return Selection.activeGameObject.transform;
			}

			return null;
		}

		public static TextureImporter ExportTextureDialog(Texture2D texture2D, string title)
		{
			if (texture2D != null)
			{
				var root = Application.dataPath;
				var path = EditorUtility.SaveFilePanel("Export " + title, root, title, "png");

				if (string.IsNullOrEmpty(path) == false)
				{
					var data = texture2D.EncodeToPNG();

					System.IO.File.WriteAllBytes(path, data);

					Debug.Log("Exported " + title + " Texture to " + path);

					if (path.StartsWith(root) == true)
					{
						var local = path.Substring(root.Length - "Assets".Length);

						AssetDatabase.ImportAsset(local);

						return GetAssetImporter<TextureImporter>(local);
					}
				}
			}

			return null;
		}

		public static AssetImporter ExportAssetDialog(Object asset, string title)
		{
			if (asset != null)
			{
				var root = Application.dataPath;
				var path = EditorUtility.SaveFilePanel("Export " + title, root, title, "asset");

				if (string.IsNullOrEmpty(path) == false)
				{
					if (path.StartsWith(root) == true)
					{
						var local = path.Substring(root.Length - "Assets".Length);

						Debug.Log("Exported " + title + " Asset to " + local);

						var clone = Object.Instantiate(asset);

						AssetDatabase.CreateAsset(clone, local);

						return GetAssetImporter<AssetImporter>(local);
					}
				}
			}

			return null;
		}
	}
}
#endif