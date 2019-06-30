using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;

namespace SpaceGraphicsToolkit
{
	public abstract class SgtFloatingSpawner_Editor<T> : SgtEditor<T>
		where T : SgtFloatingSpawner
	{
		protected override void OnInspector()
		{
			var missing = true;

			if (Any(t => string.IsNullOrEmpty(t.Category) == false))
			{
				missing = false;
			}

			if (Any(t => t.Prefabs != null && t.Prefabs.Count > 0))
			{
				missing = false;
			}

			BeginError(missing);
				DrawDefault("Category", "If you want to define prefabs externally, then you can use the SgtSpawnList component with a matching Category name.");
				DrawDefault("Prefabs", "If you aren't using a spawn list category, or just want to augment the spawn list, then define the prefabs you want to spawn here.");
			EndError();
		}
	}
}
#endif

namespace SpaceGraphicsToolkit
{
	/// <summary>This is the base class for all floating origin spawners, providing a useful methods for spawning and handling prefabs.</summary>
	[RequireComponent(typeof(SgtFloatingObject))]
	public abstract class SgtFloatingSpawner : MonoBehaviour
	{
		/// <summary>If you want to define prefabs externally, then you can use the SgtSpawnList component with a matching Category name.</summary>
		public string Category;

		/// <summary>If you aren't using a spawn list category, or just want to augment the spawn list, then define the prefabs you want to spawn here.</summary>
		public List<SgtFloatingObject> Prefabs;

		private static List<SgtFloatingObject> prefabs = new List<SgtFloatingObject>();

		[SerializeField]
		private List<SgtFloatingObject> instances;

		protected virtual void OnDisable()
		{
			if (instances != null)
			{
				for (var i = instances.Count - 1; i >= 0; i--)
				{
					var instance = instances[i];

					if (instance != null)
					{
						SgtHelper.Destroy(instance.gameObject);
					}
				}

				instances.Clear();
			}
		}

		protected bool BuildSpawnList()
		{
			if (instances == null)
			{
				instances = new List<SgtFloatingObject>();
			}

			prefabs.Clear();

			if (string.IsNullOrEmpty(Category) == false)
			{
				var spawnList = SgtSpawnList.FirstInstance;

				while (spawnList != null)
				{
					if (spawnList.Category == Category)
					{
						BuildSpawnList(spawnList.Prefabs);
					}

					spawnList = spawnList.NextInstance;
				}
			}

			BuildSpawnList(Prefabs);

			return prefabs.Count > 0;
		}

		protected SgtFloatingObject SpawnAt(SgtPosition position)
		{
			var index       = Random.Range(0, prefabs.Count);
			var prefab      = prefabs[index];
			var prefabPoint = prefab.Point;
			var oldSeed     = prefab.Seed;
			var oldPosition = prefabPoint.Position;

			prefab.Seed = Random.Range(int.MinValue, int.MaxValue);
			prefabPoint.Position = position;

			var instance = Instantiate(prefab, SgtFloatingRoot.Root);

			prefab.Seed = oldSeed;
			prefabPoint.Position = oldPosition;

			instances.Add(instance);

			if (instance.OnSpawn != null)
			{
				instance.OnSpawn(instance.Seed);
			}

			return instance;
		}

		private static void BuildSpawnList(List<SgtFloatingObject> floatingObjects)
		{
			if (floatingObjects != null)
			{
				for (var i = floatingObjects.Count - 1; i >= 0; i--)
				{
					var floatingObject = floatingObjects[i];

					if (floatingObject != null)
					{
						prefabs.Add(floatingObject);
					}
				}
			}
		}
	}
}