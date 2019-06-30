using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

namespace SpaceGraphicsToolkit
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(SgtFloatingLight))]
	public class SgtFloatingLight_Editor : SgtEditor<SgtFloatingLight>
	{
		protected override void OnInspector()
		{
			EditorGUILayout.HelpBox("This component will rotate the current GameObject toward the SgtFloatingOrigin point. This makes directional lights compatible with the floating origin system.", MessageType.Info);
		}
	}
}
#endif

namespace SpaceGraphicsToolkit
{
	/// <summary>This component will rotate the current GameObject toward the SgtFloatingOrigin point. This makes directional lights compatible with the floating origin system.</summary>
	[ExecuteInEditMode]
	[RequireComponent(typeof(SgtFloatingObject))]
	[HelpURL(SgtHelper.HelpUrlPrefix + "SgtFloatingLight")]
	[AddComponentMenu(SgtHelper.ComponentMenuPrefix + "Floating Light")]
	public class SgtFloatingLight : SgtLinkedBehaviour<SgtFloatingLight>
	{
		[System.NonSerialized]
		private SgtFloatingObject cachedObject;

		protected override void OnEnable()
		{
			base.OnEnable();

			Camera.onPreCull += PreCull;
			SgtFloatingCamera.OnPositionChanged += FloatingCameraPositionChanged;

			cachedObject = GetComponent<SgtFloatingObject>();
		}

		protected override void OnDisable()
		{
			base.OnDisable();

			Camera.onPreCull -= PreCull;
			SgtFloatingCamera.OnPositionChanged -= FloatingCameraPositionChanged;
		}

		private void PreCull(Camera camera)
		{
			var direction = SgtPosition.Direction(ref cachedObject.Point.Position, ref SgtFloatingOrigin.CurrentPoint.Position);

			transform.forward = direction;
		}

		private void FloatingCameraPositionChanged(SgtFloatingCamera floatingCamera)
		{
			var direction = SgtPosition.Direction(ref cachedObject.Point.Position, ref SgtFloatingOrigin.CurrentPoint.Position);

			transform.forward = direction;
		}
	}
}