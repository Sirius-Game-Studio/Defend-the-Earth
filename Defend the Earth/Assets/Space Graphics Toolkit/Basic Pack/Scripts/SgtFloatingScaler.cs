using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

namespace SpaceGraphicsToolkit
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(SgtFloatingScaler))]
	public class SgtFloatingScaler_Editor : SgtEditor<SgtFloatingScaler>
	{
		protected override void OnInspector()
		{
			DrawDefault("Scale", "The scale of the object when at DistanceMin.");
			BeginError(Any(t => t.ScaleMultiplier <= 0.0));
				DrawDefault("ScaleMultiplier", "Scale is multiplied by this, allowing you to more easily tweak large scales.");
			EndError();
			DrawDefault("ScaleByDistance", "Multiply the scale based on the distance to SgtFloatingOrigin?");
			if (Any(t => t.ScaleByDistance == true))
			{
				BeginIndent();
					BeginError(Any(t => t.DistanceMin < 0.0 || t.DistanceMin >= t.DistanceMax));
						DrawDefault("DistanceMin", "The distance where the scaling begins.");
						DrawDefault("DistanceMax", "The distance where the scaling stops.");
					EndError();
				EndIndent();
			}
			DrawDefault("RandomizeScale", "Should the ScaleMultiplier value be generated?");
			if (Any(t => t.RandomizeScale == true))
			{
				BeginIndent();
					BeginError(Any(t => t.ScaleMultiplierMin < 0.0 || t.ScaleMultiplierMin >= t.ScaleMultiplierMax));
						DrawDefault("ScaleMultiplierMin", "The distance where the scaling begins.");
						DrawDefault("ScaleMultiplierMax", "The distance where the scaling stops.");
					EndError();
				EndIndent();
			}
		}
	}
}
#endif

namespace SpaceGraphicsToolkit
{
	/// <summary>This component scales the current SgtFloatingObject based on its distance to the SgtFloatingOrigin.
	/// This scaling allows you to see the object from a greater distance than usual, which is very useful for star/planet/etc billboards you need to see from a distance.</summary>
	[ExecuteInEditMode]
	[RequireComponent(typeof(SgtFloatingObject))]
	[HelpURL(SgtHelper.HelpUrlPrefix + "SgtFloatingScaler")]
	[AddComponentMenu(SgtHelper.ComponentMenuPrefix + "Floating Scaler")]
	public class SgtFloatingScaler : MonoBehaviour
	{
		/// <summary>The scale of the object when at DistanceMin.</summary>
		public Vector3 Scale = Vector3.one;

		/// <summary>Scale is multiplied by this, allowing you to more easily tweak large scales.</summary>
		public double ScaleMultiplier = 1.0f;

		/// <summary>Multiply the scale based on the distance to SgtFloatingOrigin?</summary>
		public bool ScaleByDistance = true;

		/// <summary>The distance where the scaling begins.</summary>
		public SgtLength DistanceMin = 1000.0;

		/// <summary>The distance where the scaling stops.</summary>
		public SgtLength DistanceMax = 1000000.0;

		/// <summary>Should the ScaleMultiplier value be generated?</summary>
		public bool RandomizeScale;

		/// <summary>The minimum value of the generated ScaleMultiplier.</summary>
		public double ScaleMultiplierMin = 0.5;

		/// <summary>The maximum value of the generated ScaleMultiplier.</summary>
		public double ScaleMultiplierMax = 1.0;

		[System.NonSerialized]
		private SgtFloatingObject cachedObject;

		protected virtual void OnEnable()
		{
			cachedObject = GetComponent<SgtFloatingObject>();

			cachedObject.OnSpawn    += Generate;
			cachedObject.OnDistance += SetDistance;
		}

		protected virtual void OnDisable()
		{
			cachedObject.OnSpawn    -= Generate;
			cachedObject.OnDistance -= SetDistance;
		}

		private void Generate(int seed)
		{
			if (RandomizeScale == true)
			{
				ScaleMultiplier = ScaleMultiplierMin + (ScaleMultiplierMax - ScaleMultiplierMin) * Random.value;
			}
		}

		private void SetDistance(double distance)
		{
			if (ScaleByDistance == true)
			{
				if (distance <= DistanceMin)
				{
					transform.localScale = Vector3.zero;
				}
				else
				{
					var distanceRange = DistanceMax - DistanceMin;

					distance -= DistanceMin;

					if (distance >= distanceRange)
					{
						distance = distanceRange * 0.5f;
					}
					else
					{
						var distance01 = distance / distanceRange;

						distance -= distance * 0.5 * distance01;
					}

					var linear = distance * ScaleMultiplier;

					transform.localScale = Scale * (float)linear;
				}
			}
			else
			{
				transform.localScale = Scale * (float)ScaleMultiplier;
			}
		}
	}
}