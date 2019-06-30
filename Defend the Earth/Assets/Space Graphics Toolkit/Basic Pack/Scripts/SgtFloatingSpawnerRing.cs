using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

namespace SpaceGraphicsToolkit
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(SgtFloatingSpawnerRing))]
	public class SgtFloatingSpawnerRing_Editor : SgtFloatingSpawner_Editor<SgtFloatingSpawnerRing>
	{
		protected override void OnInspector()
		{
			base.OnInspector();

			Separator();

			DrawDefault("Count", "The amount of prefabs that will be spawned.");
			BeginError(Any(t => t.RadiusMin <= 0.0 || t.RadiusMin > t.RadiusMax));
				DrawDefault("RadiusMin", "The minimum distance away the prefabs can spawn in meters.");
				DrawDefault("RadiusMax");
			EndError();
		}
	}
}
#endif

namespace SpaceGraphicsToolkit
{
	/// <summary>This component will automatically spawn prefabs in a ring around the attached SgtFloatingPoint.</summary>
	[HelpURL(SgtHelper.HelpUrlPrefix + "SgtFloatingSpawnerRing")]
	[AddComponentMenu(SgtHelper.ComponentMenuPrefix + "Floating Spawner Ring")]
	public class SgtFloatingSpawnerRing : SgtFloatingSpawner
	{
		/// <summary>The amount of prefabs that will be spawned.</summary>
		public int Count = 10;

		/// <summary>The minimum distance away the prefabs can spawn.</summary>
		public SgtLength RadiusMin = 200000.0;

		/// <summary>The maximum distance away the prefabs can spawn in meters.</summary>
		public SgtLength RadiusMax = 2000000.0;

		protected virtual void OnEnable()
		{
			var parent      = GetComponent<SgtFloatingObject>();
			var parentPoint = parent.Point;

			BuildSpawnList();

			SgtHelper.BeginRandomSeed(parent.Seed);
			{
				var radMin = (double)RadiusMin;
				var radMax = (double)RadiusMax;
				var radRng = radMax - radMin;

				for (var i = 0; i < Count; i++)
				{
					var position = parentPoint.Position;
					var angle    = Random.Range(-Mathf.PI, Mathf.PI);
					var offset   = transform.rotation * new Vector3(Mathf.Sin(angle), 0.0f, Mathf.Cos(angle));
					var radius   = radMin + radRng * Random.value;

					position.LocalX += offset.x * radius;
					position.LocalY += offset.y * radius;
					position.LocalZ += offset.z * radius;
					position.SnapLocal();

					SpawnAt(position);
				}
			}
			SgtHelper.EndRandomSeed();
		}
	}
}