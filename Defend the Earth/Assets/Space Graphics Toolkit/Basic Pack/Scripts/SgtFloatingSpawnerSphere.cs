using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

namespace SpaceGraphicsToolkit
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(SgtFloatingSpawnerSphere))]
	public class SgtFloatingSpawnerSphere_Editor : SgtFloatingSpawner_Editor<SgtFloatingSpawnerSphere>
	{
		protected override void OnInspector()
		{
			base.OnInspector();

			Separator();

			DrawDefault("Count", "The amount of prefabs that will be spawned.");
			BeginError(Any(t => t.Radius <= 0.0));
				DrawDefault("Radius", "The maximum distance away the prefabs can spawn in meters.");
			EndError();
			DrawDefault("Offset", "The higher this value, the more likely the spawned objects will be pushed to the edge of the radius.");
			BeginError(Any(t => t.VelocityScale < 0.0f));
				DrawDefault("VelocityScale", "This allows you to set how much orbital velocity the spawned objects get if they have a Rigidbody attached.");
			EndError();
		}
	}
}
#endif

namespace SpaceGraphicsToolkit
{
	/// <summary>This component will automatically spawn prefabs in a circle around the attached SgtFloatingPoint.</summary>
	[HelpURL(SgtHelper.HelpUrlPrefix + "SgtFloatingSpawnerSphere")]
	[AddComponentMenu(SgtHelper.ComponentMenuPrefix + "Floating Spawner Sphere")]
	public class SgtFloatingSpawnerSphere : SgtFloatingSpawner
	{
		/// <summary>The amount of prefabs that will be spawned.</summary>
		public int Count = 10;

		/// <summary>The maximum distance away the prefabs can spawn in meters.</summary>
		public SgtLength Radius = new SgtLength(2000000.0, SgtLength.ScaleType.Meter);

		/// <summary>The higher this value, the more likely the spawned objects will be pushed to the edge of the radius.</summary>
		[Range(0.0f, 1.0f)]
		public float Offset;

		/// <summary>This allows you to set how much orbital velocity the spawned objects get if they have a Rigidbody attached.</summary>
		public float VelocityScale;

		protected virtual void OnEnable()
		{
			var parent      = GetComponent<SgtFloatingObject>();
			var parentPoint = parent.Point;

			BuildSpawnList();

			SgtHelper.BeginRandomSeed(parent.Seed);
			{
				var rad = (double)Radius;

				for (var i = 0; i < Count; i++)
				{
					var position  = parentPoint.Position;
					var direction = Random.onUnitSphere;
					var distance  = Mathf.Lerp(Offset, 1.0f, Random.value);
					var offset    = distance * direction;

					position.LocalX += offset.x * rad;
					position.LocalY += offset.y * rad;
					position.LocalZ += offset.z * rad;
					position.SnapLocal();

					var clone = SpawnAt(position);

					if (VelocityScale > 0.0f)
					{
						var rigidbody = clone.GetComponent<Rigidbody>();

						if (rigidbody != null)
						{
							var cross = Vector3.Cross(direction, Random.onUnitSphere).normalized;

							rigidbody.velocity = (cross * VelocityScale) / (distance * distance);
						}
					}
				}
			}
			SgtHelper.EndRandomSeed();
		}
	}
}