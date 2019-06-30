using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

namespace SpaceGraphicsToolkit
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(SgtCamera))]
	public class SgtCamera_Editor : SgtEditor<SgtCamera>
	{
		protected override void OnInspector()
		{
			DrawDefault("RollAngle", "The amount of degrees this camera has rolled (used to counteract billboard non-rotation).");
		}
	}
}
#endif

namespace SpaceGraphicsToolkit
{
	/// <summary>This component monitors the attached Camera for modifications in roll angle, and stores the total change.</summary>
	[ExecuteInEditMode]
	[DisallowMultipleComponent]
	[RequireComponent(typeof(Camera))]
	[HelpURL(SgtHelper.HelpUrlPrefix + "SgtCamera")]
	[AddComponentMenu(SgtHelper.ComponentMenuPrefix + "Camera")]
	public class SgtCamera : SgtLinkedBehaviour<SgtCamera>
	{
		public static System.Action<SgtCamera> OnCameraPreCull;

		public static System.Action<SgtCamera> OnCameraPreRender;

		public static System.Action<SgtCamera> OnCameraPostRender;

		/// <summary>The amount of degrees this camera has rolled (used to counteract billboard non-rotation).</summary>
		public float RollAngle;

		// A quaternion of the current roll angle
		public Quaternion RollQuaternion = Quaternion.identity;

		// A matrix of the current roll angle
		public Matrix4x4 RollMatrix = Matrix4x4.identity;

		// The change in position of this GameObject over the past frame
		[System.NonSerialized]
		public Vector3 DeltaPosition;

		// The current velocity of this GameObject per second
		[System.NonSerialized]
		public Vector3 Velocity;

		// Previous frame rotation
		[System.NonSerialized]
		public Quaternion OldRotation = Quaternion.identity;

		// Previous frame position
		[System.NonSerialized]
		public Vector3 OldPosition;

		// The camera this camera is attached to
		[System.NonSerialized]
		public Camera cachedCamera;

		[System.NonSerialized]
		public bool cachedCameraSet;

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

		// Find the camera attached to a specific camera, if it exists
		public static bool TryFind(Camera unityCamera, ref SgtCamera foundCamera)
		{
			var camera = FirstInstance;

			for (var i = 0; i < InstanceCount; i++)
			{
				if (camera.CachedCamera == unityCamera)
				{
					foundCamera = camera; return true;
				}

				camera = camera.NextInstance;
			}

			return false;
		}

		protected override void OnEnable()
		{
			base.OnEnable();

			OldRotation = transform.rotation;
			OldPosition = transform.position;

			SgtFloatingCamera.OnPositionChanged += FloatingCameraPositionChanged;
		}

		protected override void OnDisable()
		{
			base.OnDisable();

			SgtFloatingCamera.OnPositionChanged -= FloatingCameraPositionChanged;
		}

		protected virtual void OnPreCull()
		{
			if (OnCameraPreCull != null) OnCameraPreCull(this);
		}

		protected virtual void OnPreRender()
		{
			if (OnCameraPreRender != null) OnCameraPreRender(this);
		}

		protected virtual void OnPostRender()
		{
			if (OnCameraPostRender != null) OnCameraPostRender(this);
		}

		protected virtual void LateUpdate()
		{
			var newRotation   = transform.rotation;
			var newPosition   = transform.position;
			var deltaRotation = Quaternion.Inverse(OldRotation) * newRotation;
			var deltaPosition = OldPosition - newPosition;

			OldRotation    = newRotation;
			OldPosition    = newPosition;
			RollAngle      = (RollAngle - deltaRotation.eulerAngles.z) % 360.0f;
			RollQuaternion = Quaternion.Euler(0.0f, 0.0f, RollAngle);
			RollMatrix     = SgtHelper.Rotation(RollQuaternion);
			DeltaPosition  = deltaPosition;
			Velocity       = SgtHelper.Reciprocal(Time.deltaTime) * deltaPosition;
		}

		private void FloatingCameraPositionChanged(SgtFloatingCamera floatingCamera)
		{
			if (floatingCamera.gameObject == gameObject)
			{
				OldPosition = transform.position;
			}
		}
	}
}