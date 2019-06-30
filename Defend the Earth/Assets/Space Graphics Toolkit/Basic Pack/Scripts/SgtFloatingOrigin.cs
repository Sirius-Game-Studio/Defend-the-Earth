using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

namespace SpaceGraphicsToolkit
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(SgtFloatingOrigin))]
	public class SgtFloatingMain_Editor : SgtEditor<SgtFloatingOrigin>
	{
		[InitializeOnLoad]
		public class ExecutionOrder
		{
			static ExecutionOrder()
			{
				ForceExecutionOrder(-200);
			}
		}

		protected override void OnInspector()
		{
			EditorGUILayout.HelpBox("This component marks the attached SgtFloatingPoint as the floating origin system's main point.", MessageType.Info);
		}
	}
}
#endif

namespace SpaceGraphicsToolkit
{
	/// <summary>This component marks the attached SgtFloatingPoint as the floating origin system's main point.</summary>
	[ExecuteInEditMode]
	[RequireComponent(typeof(SgtFloatingPoint))]
	[HelpURL(SgtHelper.HelpUrlPrefix + "SgtFloatingOrigin")]
	[AddComponentMenu(SgtHelper.ComponentMenuPrefix + "Floating Origin")]
	public class SgtFloatingOrigin : SgtLinkedBehaviour<SgtFloatingOrigin>
	{
		/// <summary>Is CurrentPoint non-null?</summary>
		public static bool CurrentPointSet;

		/// <summary>If there is an active and enabled SgtFloatingOrigin instance, its attached SgtFloatingPoint will be set here.</summary>
		public static SgtFloatingPoint CurrentPoint;

		[System.NonSerialized]
		private SgtFloatingPoint cachedPoint;

		protected override void OnEnable()
		{
			if (InstanceCount > 0)
			{
				Debug.LogWarning("Your scene already contains an instance of SgtFloatingOrigin!", FirstInstance);
			}

			base.OnEnable();

			cachedPoint = GetComponent<SgtFloatingPoint>();

			// Make this the current point
			CurrentPoint    = cachedPoint;
			CurrentPointSet = true;
		}

		protected override void OnDisable()
		{
			base.OnDisable();

			// Revert CurrentPoint?
			if (FirstInstance != null)
			{
				CurrentPoint = FirstInstance.cachedPoint;
			}
			else
			{
				CurrentPoint    = null;
				CurrentPointSet = false;
			}
		}
	}
}