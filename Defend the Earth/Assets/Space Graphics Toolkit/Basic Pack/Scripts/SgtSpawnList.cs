using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;

namespace SpaceGraphicsToolkit
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(SgtSpawnList))]
	public class SgtSpawnList_Editor : SgtEditor<SgtSpawnList>
	{
		protected override void OnInspector()
		{
			DrawDefault("Category", "The type of prefabs these are (e.g. Planet).");
			DrawDefault("Prefabs", "The prefabs beloning to this spawn list.");
		}
	}
}
#endif

namespace SpaceGraphicsToolkit
{
	/// <summary>This component allows you to create a list of SgtFloatingObject prefabs that are associated with a specific Category name.
	/// This allows you to easily manage what objects get spawned from each type of spawner.</summary>
	[ExecuteInEditMode]
	[HelpURL(SgtHelper.HelpUrlPrefix + "SgtSpawnList")]
	[AddComponentMenu(SgtHelper.ComponentMenuPrefix + "Spawn List")]
	public class SgtSpawnList : SgtLinkedBehaviour<SgtSpawnList>
	{
		/// <summary>The type of prefabs these are (e.g. Planet).</summary>
		public string Category;

		/// <summary>The prefabs beloning to this spawn list.</summary>
		public List<SgtFloatingObject> Prefabs;

		public static SgtSpawnList Create(int layer = 0, Transform parent = null)
		{
			return Create(layer, parent, Vector3.zero, Quaternion.identity, Vector3.one);
		}

		public static SgtSpawnList Create(int layer, Transform parent, Vector3 localPosition, Quaternion localRotation, Vector3 localScale)
		{
			var gameObject = SgtHelper.CreateGameObject("Spawn List", layer, parent, localPosition, localRotation, localScale);
			var spawnList  = gameObject.AddComponent<SgtSpawnList>();

			return spawnList;
		}

#if UNITY_EDITOR
		[MenuItem(SgtHelper.GameObjectMenuPrefix + "Spawn List", false, 10)]
		private static void CreateMenuItem()
		{
			var parent    = SgtHelper.GetSelectedParent();
			var spawnList = Create(parent != null ? parent.gameObject.layer : 0, parent);

			SgtHelper.SelectAndPing(spawnList);
		}
#endif
	}
}