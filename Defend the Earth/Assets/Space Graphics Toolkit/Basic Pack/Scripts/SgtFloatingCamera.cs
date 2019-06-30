using UnityEngine;

#if UNITY_EDITOR
namespace SpaceGraphicsToolkit
{
	using UnityEditor;

	[CanEditMultipleObjects]
	[CustomEditor(typeof(SgtFloatingCamera))]
	public class SgtFloatingCamera_Editor : SgtEditor<SgtFloatingCamera>
	{
		protected override void OnInspector()
		{
			BeginError(Any(t => t.Scale <= 0));
				DrawDefault("Scale", "The scale of this camera (e.g. 10 = objects should be 10% of normal size, 100 = 1%, etc)");
			EndError();
			DrawDefault("MonitorPosition", "If this GameObject's position changes, should the SgtFloatingOrigin's SgtFloatingPoint be adjusted accordingly?");
			DrawDefault("SnappedPoint", "Every time this camera's position gets snapped, its position at that time is stored here. This allows other objects to correctly position themselves relative to this.");
		}
	}
}
#endif

namespace SpaceGraphicsToolkit
{
	/// <summary>This component marks the current GameObject as a camera. This means as soon as the transform.position strays too far from the origin (0,0,0), it will snap back to the origin.
	/// After it snaps back, the SnappedPoint field will be updated with the current position of the SgtFloatingOrigin component.</summary>
	[ExecuteInEditMode]
	[HelpURL(SgtHelper.HelpUrlPrefix + "SgtFloatingCamera")]
	[AddComponentMenu(SgtHelper.ComponentMenuPrefix + "Floating Camera")]
	public class SgtFloatingCamera : SgtLinkedBehaviour<SgtFloatingCamera>
	{
		/// <summary>When the transform.position.magnitude exceeds this radius, the position will be snapped back to the origin.</summary>
		public static readonly double SnapRadius = 100.0;

		/// <summary>Called when this camera's position snaps back to the origin.</summary>
		public static System.Action<SgtFloatingCamera> OnPositionChanged;
		public static System.Action<SgtFloatingCamera> LateOnPositionChanged;

		/// <summary>"The scale of this camera (e.g. 10 = objects should be 10% of normal size, 100 = 1%, etc)"</summary>
		public long Scale = 1000;

		/// <summary>If this GameObject's position changes, should the SgtFloatingOrigin's SgtFloatingPoint be adjusted accordingly?</summary>
		public bool MonitorPosition;

		/// <summary>Every time this camera's position gets snapped, its position at that time is stored here. This allows other objects to correctly position themselves relative to this.</summary>
		public SgtPosition SnappedPoint;

		// Used to handle adjustments to the Position
		[SerializeField]
		private Vector3 expectedPosition;

		[SerializeField]
		private bool expectedPositionSet;

		[System.NonSerialized]
		private Camera cachedCamera;

		[System.NonSerialized]
		private bool cachedCameraSet;

		public Camera CachedCamera
		{
			get
			{
				if (cachedCameraSet == false)
				{
					cachedCamera    = GetComponent<Camera>();
					cachedCameraSet = true;
				}

				return cachedCamera;
			}
		}

		/// <summary>This will find the active and enabled camera with the matching scale, or return false.</summary>
		public static bool TryGetCamera(long scale, ref SgtFloatingCamera matchingCamera)
		{
			var camera = FirstInstance;

			for (var i = 0; i < InstanceCount; i++)
			{
				if (camera.Scale == scale)
				{
					matchingCamera = camera;

					return true;
				}

				camera = camera.NextInstance;
			}

			return false;
		}

		// Get an SgtPosition from a camera-relative position
		public SgtPosition GetPosition(Vector3 localPosition)
		{
			var o = default(SgtPosition);

			o.LocalX = localPosition.x * (double)Scale;
			o.LocalY = localPosition.y * (double)Scale;
			o.LocalZ = localPosition.z * (double)Scale;

			o.SnapLocal();

			return o;
		}

		// Calculate the camera-relative position of an SgtPosition
		public Vector3 CalculatePosition(ref SgtPosition input)
		{
			var offsetX = (input.GlobalX - SnappedPoint.GlobalX) * SgtPosition.CellSize + (input.LocalX - SnappedPoint.LocalX);
			var offsetY = (input.GlobalY - SnappedPoint.GlobalY) * SgtPosition.CellSize + (input.LocalY - SnappedPoint.LocalY);
			var offsetZ = (input.GlobalZ - SnappedPoint.GlobalZ) * SgtPosition.CellSize + (input.LocalZ - SnappedPoint.LocalZ);
			var scaledX = offsetX / Scale;
			var scaledY = offsetY / Scale;
			var scaledZ = offsetZ / Scale;

			return new Vector3((float)scaledX, (float)scaledY, (float)scaledZ);
		}

		[ContextMenu("Snap")]
		public void Snap()
		{
			if (SgtFloatingOrigin.CurrentPointSet == true)
			{
				SnappedPoint = SgtFloatingOrigin.CurrentPoint.Position;

				transform.position = expectedPosition = Vector3.zero;

				if (OnPositionChanged != null) OnPositionChanged(this);
				if (LateOnPositionChanged != null) LateOnPositionChanged(this);
			}
		}

		protected override void OnEnable()
		{
			base.OnEnable();

			Camera.onPreCull += PreCull;

			Snap();
		}

		protected override void OnDisable()
		{
			base.OnDisable();

			Camera.onPreCull -= PreCull;
		}

		protected virtual void LateUpdate()
		{
			if (expectedPositionSet == true)
			{
				if (MonitorPosition == true)
				{
					var delta = transform.position - expectedPosition;

					SgtFloatingOrigin.CurrentPoint.Position.LocalX += delta.x * Scale;
					SgtFloatingOrigin.CurrentPoint.Position.LocalY += delta.y * Scale;
					SgtFloatingOrigin.CurrentPoint.Position.LocalZ += delta.z * Scale;
				}
			}
			else
			{
				expectedPositionSet = true;
			}

			transform.position = expectedPosition = CalculatePosition(ref SgtFloatingOrigin.CurrentPoint.Position);
		}

		private void PreCull(Camera camera)
		{
			SgtFloatingOrigin.CurrentPoint.Position.SnapLocal();

			transform.position = expectedPosition = CalculatePosition(ref SgtFloatingOrigin.CurrentPoint.Position);

			// Rotate to main camera?
			var mainCamera = Camera.main;

			if (mainCamera != null)
			{
				transform.rotation = mainCamera.transform.rotation;
			}

			// Snap scaled camera position?
			var position = transform.position;

			if (position.magnitude > SnapRadius)
			{
				Snap();
			}
		}
	}
}