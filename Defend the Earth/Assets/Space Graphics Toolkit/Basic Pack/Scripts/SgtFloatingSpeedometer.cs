using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;

namespace SpaceGraphicsToolkit
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(SgtFloatingSpeedometer))]
	public class SgtFloatingSpeedometer_Editor : SgtEditor<SgtFloatingSpeedometer>
	{
		protected override void OnInspector()
		{
			BeginError(Any(t => t.Point == null));
				DrawDefault("Point", "The point whose speed will be monitored.");
			EndError();
			DrawDefault("Title", "The speed will be output to this component.");
		}
	}
}
#endif

namespace SpaceGraphicsToolkit
{
	/// <summary>This component monitors Point for changes, and outputs the speed of those changes to the Title text.</summary>
	[ExecuteInEditMode]
	[HelpURL(SgtHelper.HelpUrlPrefix + "SgtFloatingSpeedometer")]
	[AddComponentMenu(SgtHelper.ComponentMenuPrefix + "Floating Speedometer")]
	public class SgtFloatingSpeedometer : SgtLinkedBehaviour<SgtFloatingSpeedometer>
	{
		/// <summary>The point whose speed will be monitored.</summary>
		public SgtFloatingPoint Point;

		/// <summary>The speed will be output to this component.</summary>
		public Text Title;

		[System.NonSerialized]
		private SgtFloatingObject cachedObject;

		[System.NonSerialized]
		private SgtPosition expectedPosition;

		[System.NonSerialized]
		private bool expectedPositionSet;

		protected virtual void Update()
		{
			if (Point != null && Title != null)
			{
				var currentPosition = Point.Position;

				if (expectedPositionSet == false)
				{
					expectedPositionSet = true;
					expectedPosition    = currentPosition;
				}

				var delta = SgtHelper.Divide(SgtPosition.Distance(ref expectedPosition, ref currentPosition), Time.deltaTime);

				Title.text = "Speed = " + System.Math.Round(delta).ToString("0") + "m/s";

				expectedPosition = currentPosition;
			}
		}
	}
}