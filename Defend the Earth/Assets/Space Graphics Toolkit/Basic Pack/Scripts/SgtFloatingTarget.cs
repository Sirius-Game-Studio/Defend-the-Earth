using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

namespace SpaceGraphicsToolkit
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(SgtFloatingTarget))]
	public class SgtWarpTarget_Editor : SgtEditor<SgtFloatingTarget>
	{
		protected override void OnInspector()
		{
			BeginError(Any(t => string.IsNullOrEmpty(t.WarpName)));
				DrawDefault("WarpName", "The shorthand name for this warp target.");
			EndError();
			BeginError(Any(t => t.WarpDistance < 0.0));
				DrawDefault("WarpDistance", "The distance from this SgtFloatingPoint we should warp to, to prevent you warping too close.");
			EndError();
		}
	}
}
#endif

namespace SpaceGraphicsToolkit
{
	/// <summary>This component marks the attached LeanFloatingPoint component as a warpable target point. This allows you to pick the target using the SgtWarpPin component.</summary>
	[RequireComponent(typeof(SgtFloatingPoint))]
	[HelpURL(SgtHelper.HelpUrlPrefix + "SgtFloatingTarget")]
	[AddComponentMenu(SgtHelper.ComponentMenuPrefix + "Floating Target")]
	public class SgtFloatingTarget : SgtLinkedBehaviour<SgtFloatingTarget>
	{
		/// <summary>The shorthand name for this warp target.</summary>
		public string WarpName;

		/// <summary>The distance from this SgtFloatingPoint we should warp to, to prevent you warping too close.</summary>
		public SgtLength WarpDistance = 1000.0;

		[System.NonSerialized]
		private SgtFloatingPoint cachedPoint;

		[System.NonSerialized]
		private bool cachedPointSet;

		public SgtFloatingPoint CachedPoint
		{
			get
			{
				if (cachedPointSet == false)
				{
					cachedPoint    = GetComponent<SgtFloatingPoint>();
					cachedPointSet = true;
				}

				return cachedPoint;
			}
		}
	}
}