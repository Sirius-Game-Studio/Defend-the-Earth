using UnityEngine;

#if UNITY_EDITOR
namespace SpaceGraphicsToolkit
{
	using UnityEditor;

	[CanEditMultipleObjects]
	[CustomEditor(typeof(SgtFloatingObject))]
	public class SgtFloatingObject_Editor : SgtEditor<SgtFloatingObject>
	{
		protected override void OnInspector()
		{
			BeginError(Any(t => t.Point == null));
				Each(t => t.UnregisterPoint());
					DrawDefault("Point", "This allows you to set the position of this object inside the floating origin system. NOTE: If this object is spawned from the SgtFloatingLod component, then leave this as null/None, because it will automatically be assigned on spawn.");
				Each(t => t.RegisterPoint());
			EndError();
			if (Any(t => t.Point == null))
			{
				EditorGUILayout.HelpBox("Point should only be None/null if this prefab will be spawned from the SgtFloatingLod component. If not, you should add one to this or a parent GameObject.", MessageType.Info);
			}
			DrawDefault("Seed", "This allows you to set the random seed used during procedural generation. If this object is spawned from an SgtFloatingSpawner___ component, then this will automatically be set.");
			DrawDefault("Scale", "The SgtFloatingCamera.Scale this object belongs to. See the SgtFloatingCamera component for more details.");
			DrawDefault("MonitorPosition", "If this transform.position changes (e.g. rigidbody physics), should the change be applied to the associated Point?");
		}
	}
}
#endif

namespace SpaceGraphicsToolkit
{
	/// <summary>This component allows you to turn a normal GameObject into one that works with the floating origin system.
	/// Keep in mind the transform.position will be altered based on camera movement, so certain components may not work correctly without modification.
	/// For example, if you make this GameObject lerp between two Vector3 positions, then those positions will be incorrect when the floating origin snaps to a new position.
	/// To correctly handle this scenario, you need to hook into the SgtFloatingCamera.OnPositionChanged event, and calculate new positions using the CalculatePosition method from the passed SgtFloatingCamera instance.</summary>
	[ExecuteInEditMode]
	[HelpURL(SgtHelper.HelpUrlPrefix + "SgtFloatingObject")]
	[AddComponentMenu(SgtHelper.ComponentMenuPrefix + "Floating Object")]
	public class SgtFloatingObject : MonoBehaviour
	{
		/// <summary>This allows you to set the position of this object inside the floating origin system. NOTE: If this object is spawned from the SgtFloatingLod component, then leave this as null/None, because it will automatically be assigned on spawn.</summary>
		public SgtFloatingPoint Point;

		/// <summary>This allows you to set the random seed used during procedural generation. If this object is spawned from an SgtFloatingSpawner___ component, then this will automatically be set.</summary>
		public SgtSeed Seed;

		/// <summary>The SgtFloatingCamera.Scale this object belongs to. See the SgtFloatingCamera component for more details.</summary>
		public long Scale = 1;

		/// <summary>If this transform.position changes (e.g. rigidbody physics), should the change be applied to the associated Point?</summary>
		public bool MonitorPosition;

		/// <summary>If this object is spawned from an SgtFloatingSpawner___ component, then this will be called with the new Seed value.</summary>
		public System.Action<int> OnSpawn;

		/// <summary>This will be called every Update the object is active and enabled.</summary>
		public System.Action<double> OnDistance;

		[SerializeField]
		private Vector3 expectedPosition;

		[SerializeField]
		private bool expectedPositionSet;

		[ContextMenu("Update Position")]
		public void UpdatePosition()
		{
			var camera = default(SgtFloatingCamera);

			if (SgtFloatingCamera.TryGetCamera(Scale, ref camera) == true)
			{
				UpdatePosition(camera);
			}
		}

		public void UpdatePosition(SgtFloatingCamera camera)
		{
			transform.position = expectedPosition = camera.CalculatePosition(ref Point.Position);
		}

		public void RegisterPoint()
		{
#if UNITY_EDITOR
			if (Point == null)
			{
				var point = GetComponentInParent<SgtFloatingPoint>();

				if (point != null)
				{
					Point = point;
				}
			}

			if (Point == null)
			{
				return;
			}
#endif
			Point.OnPositionChanged += PointPositionChanged;
		}

		public void UnregisterPoint()
		{
#if UNITY_EDITOR
			if (Point == null)
			{
				return;
			}
#endif
			Point.OnPositionChanged -= PointPositionChanged;
		}

#if UNITY_EDITOR
		protected virtual void Reset()
		{
			if (Point == null)
			{
				Point = GetComponent<SgtFloatingPoint>();
			}
		}
#endif

		protected virtual void OnEnable()
		{
			SgtFloatingCamera.OnPositionChanged += FloatingCameraPositionChanged;

			RegisterPoint();
			UpdatePosition();
		}

		protected virtual void OnDisable()
		{
			SgtFloatingCamera.OnPositionChanged -= FloatingCameraPositionChanged;

			UnregisterPoint();
		}

		protected virtual void Update()
		{
			if (MonitorPosition == true)
			{
				var position = transform.position;

				if (expectedPositionSet == true)
				{
					if (expectedPosition.x != position.x || expectedPosition.y != position.y || expectedPosition.z != position.z)
					{
						Point.Position.LocalX += position.x - expectedPosition.x;
						Point.Position.LocalY += position.y - expectedPosition.y;
						Point.Position.LocalZ += position.z - expectedPosition.z;

						expectedPosition = position;

						Point.Position.SnapLocal();

						Point.PositionChanged();
					}
					else
					{
						expectedPosition = position;
					}
				}
				else
				{
					expectedPositionSet = true;
					expectedPosition    = position;
				}
			}
			else
			{
				expectedPositionSet = false;
			}

			if (OnDistance != null)
			{
				var position = SgtFloatingOrigin.CurrentPoint.Position;
				var distance = SgtPosition.Distance(ref position, ref Point.Position);

				OnDistance.Invoke(distance);
			}
		}

		private void FloatingCameraPositionChanged(SgtFloatingCamera camera)
		{
			if (camera.Scale == Scale)
			{
				UpdatePosition(camera);
			}
		}

		private void PointPositionChanged()
		{
			var camera = default(SgtFloatingCamera);

			if (SgtFloatingCamera.TryGetCamera(Scale, ref camera) == true)
			{
				transform.position = expectedPosition = camera.CalculatePosition(ref Point.Position);
			}
		}
	}
}