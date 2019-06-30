using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

namespace SpaceGraphicsToolkit
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(SgtFloatingWarpButton))]
	public class SgtFloatingWarpButton_Editor : SgtEditor<SgtFloatingWarpButton>
	{
		protected override void OnInspector()
		{
			BeginError(Any(t => t.Target == null));
				DrawDefault("Target", "The point that will be warped to.");
			EndError();
			BeginError(Any(t => t.Warp == null));
				DrawDefault("Warp", "The warp effect that will be used.");
			EndError();
		}
	}
}
#endif

namespace SpaceGraphicsToolkit
{
	/// <summary>This component allows you to warp to the target when clicking a button.
	/// NOTE: The button's OnClick event must be linked to the Click method.</summary>
	[ExecuteInEditMode]
	[HelpURL(SgtHelper.HelpUrlPrefix + "SgtFloatingWarpButton")]
	[AddComponentMenu(SgtHelper.ComponentMenuPrefix + "Floating Warp Button")]
	public class SgtFloatingWarpButton : MonoBehaviour
	{
		/// <summary>The point that will be warped to.</summary>
		public SgtFloatingTarget Target;

		/// <summary>The warp effect that will be used.</summary>
		public SgtFloatingWarp Warp;

		public void Click()
		{
			Warp.WarpTo(Target.CachedPoint.Position, Target.WarpDistance);
		}
	}
}