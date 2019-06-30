using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

namespace SpaceGraphicsToolkit
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(SgtFloatingWarp))]
	public abstract class SgtFloatingWarp_Editor<T> : SgtEditor<T>
		where T : SgtFloatingWarp
	{
		protected override void OnInspector()
		{
			BeginError(Any(t => t.Point == null));
				DrawDefault("Point", "The point that will be warped.");
			EndError();
		}
	}
}
#endif

namespace SpaceGraphicsToolkit
{
	/// <summary>This is the base class for all warp styles.</summary>
	public abstract class SgtFloatingWarp : MonoBehaviour
	{
		/// <summary>The point that will be warped.</summary>
		public SgtFloatingPoint Point;

		/// <summary>Allows you to warp to the target point with the specified separation distance.</summary>
		public void WarpTo(SgtPosition position, double distance)
		{
			// Make sure we don't warp directly onto the star
			var direction = SgtPosition.Direction(ref Point.Position, ref position);

			position.LocalX -= direction.x * distance;
			position.LocalY -= direction.y * distance;
			position.LocalZ -= direction.z * distance;
			position.SnapLocal();

			WarpTo(position);
		}

		public abstract bool CanAbortWarp
		{
			get;
		}

		public abstract void WarpTo(SgtPosition position);

		public abstract void AbortWarp();
	}
}