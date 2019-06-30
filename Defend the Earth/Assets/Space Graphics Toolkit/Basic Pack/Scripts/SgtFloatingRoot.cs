using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

namespace SpaceGraphicsToolkit
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(SgtFloatingRoot))]
	public class SgtFloatingRoot_Editor : SgtEditor<SgtFloatingRoot>
	{
		[InitializeOnLoad]
		public class ExecutionOrder
		{
			static ExecutionOrder()
			{
				ForceExecutionOrder(-100);
			}
		}

		protected override void OnInspector()
		{
			EditorGUILayout.HelpBox("All prefabs spawned from SgtFloatingLod and SgtFloatingSpawner___ will be attached to this GameObject.", MessageType.Info);
		}
	}
}
#endif

namespace SpaceGraphicsToolkit
{
	/// <summary>All prefabs spawned from SgtFloatingLod and SgtFloatingSpawner___ will be attached to this GameObject.</summary>
	[ExecuteInEditMode]
	[HelpURL(SgtHelper.HelpUrlPrefix + "SgtFloatingRoot")]
	[AddComponentMenu(SgtHelper.ComponentMenuPrefix + "Floating Root")]
	public class SgtFloatingRoot : SgtLinkedBehaviour<SgtFloatingRoot>
	{
		public static Transform Root
		{
			get
			{
				if (InstanceCount > 0)
				{
					return FirstInstance.transform;
				}

				Debug.LogWarning("Your scene contains no SgtFloatingRoot component, so all spawned SgtFloatingPoint prefabs will be placed in the scene root.");

				return null;
			}
		}

		protected override void OnEnable()
		{
			if (InstanceCount > 0)
			{
				Debug.LogWarning("Your scene already contains an instance of SgtFloatingRoot!", FirstInstance);
			}

			base.OnEnable();
		}
	}
}