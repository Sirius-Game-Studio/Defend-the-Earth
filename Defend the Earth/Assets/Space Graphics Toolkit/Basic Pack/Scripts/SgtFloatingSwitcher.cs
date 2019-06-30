using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

namespace SpaceGraphicsToolkit
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(SgtFloatingSwitcher))]
	public class SgtFloatingSwitcher_Editor : SgtEditor<SgtFloatingSwitcher>
	{
		protected override void OnInspector()
		{
			if (Targets.Length == 1)
			{
				var steps = Target.Steps;

				if (steps != null)
				{
					var levelsProp = serializedObject.FindProperty("Steps");

					for (var i = 0; i < steps.Count; i++)
					{
						var level     = steps[i];
						var levelProp = levelsProp.GetArrayElementAtIndex(i);

						EditorGUILayout.PropertyField(levelProp, true);

						Separator();
					}
				}
			}
			else
			{
				BeginError(Any(t => t.Steps == null || t.Steps.Count == 0));
					DrawDefault("Steps", "The different scales that will be switched between.");
				EndError();

				Separator();
			}

			if (Button("Add Step") == true)
			{
				DirtyEach(t => { if (t.Steps == null) t.Steps = new List<SgtFloatingSwitcher.Step>(); t.Steps.Add(new SgtFloatingSwitcher.Step()); });
			}
		}
	}
}
#endif

namespace SpaceGraphicsToolkit
{
	/// <summary>This component automatically switches the layer and scale of the attached SgtFloatingObject.</summary>
	[ExecuteInEditMode]
	[RequireComponent(typeof(SgtFloatingObject))]
	[HelpURL(SgtHelper.HelpUrlPrefix + "SgtFloatingSwitcher")]
	[AddComponentMenu(SgtHelper.ComponentMenuPrefix + "Floating Switcher")]
	public class SgtFloatingSwitcher : MonoBehaviour
	{
		[System.Serializable]
		public class Step
		{
			public double DistanceMin;
			public long   Scale;
			public double DistanceMax;
		}

		/// <summary>The different scales that will be switched between.</summary>
		public List<Step> Steps;

		[System.NonSerialized]
		private SgtFloatingObject cachedObject;

		[System.NonSerialized]
		private bool cachedObjectSet;

		public void ApplyLayer(Transform t, int layer)
		{
			t.gameObject.layer = layer;

			for (var i = t.childCount - 1; i >= 0; i--)
			{
				ApplyLayer(t.GetChild(i), layer);
			}
		}

		public void SetDistance(double distance)
		{
			if (cachedObjectSet == false)
			{
				cachedObject    = GetComponent<SgtFloatingObject>();
				cachedObjectSet = true;
			}

			// Assume these exist to speed things up
			//if (Steps != null)
			{
				for (var i = Steps.Count - 1; i >= 0; i--)
				{
					var step = Steps[i];

					if (distance >= step.DistanceMin && distance < step.DistanceMax)
					{
						if (step.Scale != cachedObject.Scale)
						{
							ApplyScale(step.Scale);
						}

						break;
					}
				}
			}
		}

		private void ApplyScale(long newScale)
		{
			var floatingCamera = default(SgtFloatingCamera);

			if (SgtFloatingCamera.TryGetCamera(newScale, ref floatingCamera) == true)
			{
				ApplyLayer(transform, floatingCamera.gameObject.layer);

				if (cachedObjectSet == false)
				{
					cachedObject    = GetComponent<SgtFloatingObject>();
					cachedObjectSet = true;
				}

				cachedObject.UpdatePosition(floatingCamera);

				cachedObject.Scale = newScale;

				//cachedObject.UpdateScale();
			}

			if (cachedObjectSet == false)
			{
				cachedObject    = GetComponent<SgtFloatingObject>();
				cachedObjectSet = true;
			}
		}
	}
}