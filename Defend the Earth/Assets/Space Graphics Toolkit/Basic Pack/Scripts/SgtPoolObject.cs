using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;

namespace SpaceGraphicsToolkit
{
	[CustomEditor(typeof(SgtPoolObject))]
	public class SgtPoolObject_Editor : SgtEditor<SgtPoolObject>
	{
		protected override void OnInspector()
		{
			BeginDisabled();
				DrawDefault("TypeName", "The name of the type this pool manages.");
				DrawDefault("Elements", "The pooled elements in this pool.");
			EndDisabled();
			EditorGUILayout.HelpBox("SgtPoolObject are not saved to your scene, so don't worry if you see it in edit mode.", MessageType.Info);
		}
	}
}
#endif

namespace SpaceGraphicsToolkit
{
	[ExecuteInEditMode]
	[AddComponentMenu("")]
	public class SgtPoolObject : SgtLinkedBehaviour<SgtPoolObject>
	{
		/// <summary>The name of the type this pool manages.</summary>
		public string TypeName;

		/// <summary>The pooled elements in this pool.</summary>
		public List<Object> Elements = new List<Object>();

		protected virtual void OnDestroy()
		{
			for (var i = Elements.Count - 1; i >= 0; i--)
			{
				Object.DestroyImmediate(Elements[i]);
			}
		}

#if UNITY_EDITOR
		protected virtual void OnDrawGizmos()
		{
			if (Application.isPlaying == false)
			{
				SgtHelper.Destroy(gameObject);
			}
		}
#endif
	}

	public static class SgtObjectPool<T>
		where T : Object
	{
		private static SgtPoolObject pool;
	
		static SgtObjectPool()
		{
			if (typeof(T).IsSubclassOf(typeof(Component)))
			{
				Debug.LogError("Attempting to use " + typeof(T).Name + " with SgtPoolObject. Use SgtPoolComponent instead.");
			}
		}

		public static T Add(T entry)
		{
			return Add(entry, null);
		}

		public static T Add(T element, System.Action<T> onAdd)
		{
			if (element != null)
			{
				if (onAdd != null)
				{
					onAdd(element);
				}

				UpdateComponent(true);
			
				pool.Elements.Add(element);
			}

			return null;
		}
	
		public static T Pop()
		{
			UpdateComponent(false);
		
			if (pool != null)
			{
				var elements = pool.Elements;
				var count    = elements.Count;

				if (count > 0)
				{
					var index   = count - 1;
					var element = (T)elements[index];

					elements.RemoveAt(index);
#if UNITY_EDITOR
					if (element != null)
					{
						element.hideFlags = HideFlags.None;
					}
#endif
					return element;
				}
			}

			return null;
		}
	
		private static void UpdateComponent(bool allowCreation)
		{
			if (pool == null)
			{
				var typeName = typeof(T).Name;
				var findPool = SgtPoolObject.FirstInstance;

				for (var i = 0; i < SgtPoolObject.InstanceCount; i++)
				{
					if (findPool.TypeName == typeName)
					{
						pool = findPool; return;
					}

					findPool = findPool.NextInstance;
				}

				if (allowCreation == true)
				{
					pool = new GameObject("SgtPoolObject<" + typeName + ">").AddComponent<SgtPoolObject>();

					pool.TypeName = typeName;

					if (Application.isPlaying == true)
					{
						Object.DontDestroyOnLoad(pool);
					}
#if UNITY_EDITOR
					pool.gameObject.hideFlags = HideFlags.DontSave;
#endif
				}
			}
		}
	}
}