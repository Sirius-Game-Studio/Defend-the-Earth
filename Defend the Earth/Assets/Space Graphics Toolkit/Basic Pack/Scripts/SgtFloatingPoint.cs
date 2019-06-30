using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

namespace SpaceGraphicsToolkit
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(SgtFloatingPoint))]
	public class SgtFloatingPoint_Editor : SgtEditor<SgtFloatingPoint>
	{
		protected override void OnInspector()
		{
			var modified = false;

			modified |= DrawDefault("Position.LocalX");
			modified |= DrawDefault("Position.LocalY");
			modified |= DrawDefault("Position.LocalZ");
			modified |= DrawDefault("Position.GlobalX");
			modified |= DrawDefault("Position.GlobalY");
			modified |= DrawDefault("Position.GlobalZ");

			if (modified == true)
			{
				Each(t => t.PositionChanged());
			}
		}
	}
}
#endif

namespace SpaceGraphicsToolkit
{
	/// <summary>This component wraps SgtPosition into a component, and defines a single point in the floating origin system.
	/// Normal transform position coordinates are stored using floats (Vector3), but SgtPosition coordinates are stored using a long and a double pair.
	/// The long is used to specify the current grid cell, and the double is used to specify the high precision relative offset to the grid cell.
	/// Combined, these values allow simulation of the whole observable universe.</summary>
	[HelpURL(SgtHelper.HelpUrlPrefix + "SgtFloatingPoint")]
	[AddComponentMenu(SgtHelper.ComponentMenuPrefix + "Floating Point")]
	public class SgtFloatingPoint : MonoBehaviour
	{
		/// <summary>The position wrapped by this component.</summary>
		public SgtPosition Position;

		/// <summary>Whenever the Position values are modified, this gets called. This is useful for components that depend on this position being known at all times (e.g. SgtFloatingOrbit).</summary>
		public System.Action OnPositionChanged;

		/// <summary>Call this method after you've finished modifying the Position, and it will notify all event listeners.</summary>
		public void PositionChanged()
		{
			if (OnPositionChanged != null)
			{
				OnPositionChanged();
			}
		}

#if UNITY_EDITOR
		protected virtual void OnValidate()
		{
			PositionChanged();
		}
#endif
	}
}