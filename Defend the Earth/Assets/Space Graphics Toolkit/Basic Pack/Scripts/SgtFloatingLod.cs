using UnityEngine;

#if UNITY_EDITOR
namespace SpaceGraphicsToolkit
{
	using UnityEditor;

	[CanEditMultipleObjects]
	[CustomEditor(typeof(SgtFloatingLod))]
	public class SgtFloatingLod_Editor : SgtEditor<SgtFloatingLod>
	{
		protected override void OnInspector()
		{
			DrawDefault("DistanceMin", "The minimum spawning distance in meters.");
			BeginError(Any(t => t.Prefab == null));
				BeginIndent();
					DrawDefault("Prefab", "The object that will be spawned when within distance.");
				EndIndent();
			EndError();
			DrawDefault("DistanceMax", "The maximum spawning distance in meters.");
			DrawDefault("EnableInEditor", "Spawn or despawn the LOD in the editor?");
		}
	}
}
#endif

namespace SpaceGraphicsToolkit
{
	/// <summary>This component will automatically spawn & despawn a prefab based on its float origin distance to the SgtFloatingOrigin.</summary>
	[ExecuteInEditMode]
	[RequireComponent(typeof(SgtFloatingObject))]
	[HelpURL(SgtHelper.HelpUrlPrefix + "SgtFloatingLod")]
	[AddComponentMenu(SgtHelper.ComponentMenuPrefix + "Floating LOD")]
	public class SgtFloatingLod : MonoBehaviour
	{
		/// <summary>The minimum spawning distance in meters.</summary>
		public SgtLength DistanceMin;

		/// <summary>The object that will be spawned when within distance.</summary>
		public SgtFloatingObject Prefab;

		/// <summary>The maximum spawning distance in meters.</summary>
		public SgtLength DistanceMax;

		/// <summary>Spawn or despawn the LOD in the editor?</summary>
		public bool EnableInEditor;

		[SerializeField]
		private SgtFloatingObject instance;

		[System.NonSerialized]
		private SgtFloatingObject cachedObject;

		protected virtual void OnEnable()
		{
			cachedObject = GetComponent<SgtFloatingObject>();

			cachedObject.OnDistance += UpdateDistance;
		}

		protected virtual void OnDisable()
		{
			cachedObject.OnDistance -= UpdateDistance;

			if (instance != null)
			{
				SgtHelper.Destroy(instance.gameObject);

				instance = null;
			}
		}

		private void UpdateDistance(double distance)
		{
#if UNITY_EDITOR
			if (Application.isPlaying == false && EnableInEditor == false)
			{
				return;
			}
#endif
			if (distance >= DistanceMin && distance < DistanceMax)
			{
				if (instance == null)
				{
					// Store old values and override
					var oldPoint = Prefab.Point;

					Prefab.Point = cachedObject.Point;

					// Spawn
					instance = Instantiate(Prefab, SgtFloatingRoot.Root);

					// Revert values
					Prefab.Point = oldPoint;

					instance.UpdatePosition();

					// Invoke spawn
					if (instance.OnSpawn != null)
					{
						instance.OnSpawn.Invoke(cachedObject.Seed);
					}
				}

				if (instance.OnDistance != null)
				{
					instance.OnDistance.Invoke(distance);
				}
			}
			else
			{
				if (instance != null)
				{
					SgtHelper.Destroy(instance.gameObject);

					instance = null;
				}
			}
		}
	}
}