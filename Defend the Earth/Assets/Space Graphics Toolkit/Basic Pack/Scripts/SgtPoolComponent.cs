using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;

namespace SpaceGraphicsToolkit
{
	[CustomEditor(typeof(SgtPoolComponent))]
	public class SgtPoolComponent_Editor : SgtEditor<SgtPoolComponent>
	{
		protected override void OnInspector()
		{
			BeginDisabled();
				DrawDefault("TypeName", "The name of the type this pool manages.");
				DrawDefault("Elements", "The pooled elements in this pool.");
			EndDisabled();
			EditorGUILayout.HelpBox("SgtPoolComponent are not saved to your scene, so don't worry if you see it in edit mode.", MessageType.Info);
		}
	}
}
#endif

namespace SpaceGraphicsToolkit
{
	[ExecuteInEditMode]
	[AddComponentMenu("")]
	public class SgtPoolComponent : SgtLinkedBehaviour<SgtPoolComponent>
	{
		/// <summary>The name of the type this pool manages.</summary>
		public string TypeName;

		/// <summary>The pooled elements in this pool.</summary>
		public List<Component> Elements = new List<Component>();

		protected virtual void OnDestroy()
		{
			for (var i = Elements.Count - 1; i >= 0; i--)
			{
				var element = Elements[i];

				if (element != null)
				{
					DestroyImmediate(element.gameObject);
				}
			}
		}

#if UNITY_EDITOR
		protected virtual void Update()
		{
			// Auto destroy this pool if it exists in edit mode
			if (Application.isPlaying == false)
			{
				SgtHelper.Destroy(gameObject);
			}
		}
#endif

#if UNITY_EDITOR
		protected virtual void OnDrawGizmos()
		{
			// Auto destroy this pool if it exists in edit mode
			if (Application.isPlaying == false)
			{
				SgtHelper.Destroy(gameObject);
			}
		}
#endif
	}

	public static class SgtComponentPool<T>
		where T : Component
	{
		// The pool component for this static class
		private static SgtPoolComponent pool;

		// The amount of pooled objects
		public static int Count
		{
			get
			{
				UpdateComponent(false);

				if (pool != null)
				{
					return pool.Elements.Count;
				}

				return 0;
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
#if UNITY_EDITOR
				element.gameObject.hideFlags = HideFlags.DontSave;
#endif
				element.transform.SetParent(pool.transform, false);

				element.gameObject.SetActive(false);

				pool.Elements.Add(element);
			}

			return null;
		}

		public static void Cache()
		{
			UpdateComponent(true);

			var gameObject = new GameObject();
#if UNITY_EDITOR
			gameObject.hideFlags = HideFlags.DontSave;
#endif
			gameObject.transform.SetParent(pool.transform, false);

			gameObject.SetActive(false);

			var element = gameObject.AddComponent<T>();

			pool.Elements.Add(element);
		}
	
		public static T Pop(Transform parent, string name, int layer)
		{
			var element = Pop();

			if (element != null)
			{
				element.name = name;

				element.transform.SetParent(parent, false);

				element.gameObject.layer = layer;
			}
			else
			{
				var gameObject = new GameObject(name);

				gameObject.layer = layer;

				gameObject.transform.SetParent(parent, false);

				element = gameObject.AddComponent<T>();
			}

			return element;
		}

		public static T Pop()
		{
			return Pop(null);
		}

		public static T Pop(System.Predicate<T> match)
		{
			UpdateComponent(false);

			if (pool != null)
			{
				var elements = pool.Elements;
				var index    = elements.Count - 1;

				if (match != null)
				{
					for (var i = index; i >= 0; i--)
					{
						var element = elements[i];

						if (match((T)element) == true)
						{
							index = i; break;
						}
					}
				}

				if (index >= 0)
				{
					var element = (T)elements[index];

					elements.RemoveAt(index);

					if (element != null)
					{
#if UNITY_EDITOR
						element.gameObject.hideFlags = HideFlags.None;
#endif
						element.gameObject.SetActive(true);
					}

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
				var findPool = SgtPoolComponent.FirstInstance;

				for (var i = 0; i < SgtPoolComponent.InstanceCount; i++)
				{
					if (findPool.TypeName == typeName)
					{
						pool = findPool; return;
					}

					findPool = findPool.NextInstance;
				}

				if (allowCreation == true)
				{
					pool = new GameObject("SgtPoolComponent<" + typeName + ">").AddComponent<SgtPoolComponent>();

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