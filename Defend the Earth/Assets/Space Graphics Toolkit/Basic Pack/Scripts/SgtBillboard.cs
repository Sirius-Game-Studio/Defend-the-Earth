using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;

namespace SpaceGraphicsToolkit
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(SgtBillboard))]
	public class SgtBillboard_Editor : SgtEditor<SgtBillboard>
	{
		protected override void OnInspector()
		{
			DrawDefault("RollWithCamera", "If the camera rolls, should this billboard roll with it?");
			DrawDefault("AvoidClipping", "If your billboard is clipping out of view at extreme angles, then enable this.");
		}
	}
}
#endif

namespace SpaceGraphicsToolkit
{
	/// <summary>This component turns the current GameObject into a billboard.
	/// NOTE: If you're using the floating origin system then you should use the SgtFloatingBillboard component instead.</summary>
	[ExecuteInEditMode]
	[HelpURL(SgtHelper.HelpUrlPrefix + "SgtBillboard")]
	[AddComponentMenu(SgtHelper.ComponentMenuPrefix + "Billboard")]
	public class SgtBillboard : MonoBehaviour
	{
		public class CameraState : SgtCameraState
		{
			public Quaternion LocalRotation;
		}

		/// <summary>If the camera rolls, should this billboard roll with it?</summary>
		public bool RollWithCamera;

		/// <summary>If your billboard is clipping out of view at extreme angles, then enable this.</summary>
		public bool AvoidClipping;

		[System.NonSerialized]
		private List<CameraState> cameraStates;

		[System.NonSerialized]
		public Transform cachedTransform;

		protected virtual void OnEnable()
		{
			Camera.onPreCull   += CameraPreCull;
			Camera.onPreRender += CameraPreRender;

			cachedTransform = GetComponent<Transform>();
		}

		protected virtual void OnDisable()
		{
			Camera.onPreCull   -= CameraPreCull;
			Camera.onPreRender -= CameraPreRender;
		}

		private void CameraPreCull(Camera camera)
		{
			Revert();
			{
				var cameraRotation = camera.transform.rotation;
				var rollRotation   = cameraRotation;
				var observer       = default(SgtCamera);
				var rotation       = default(Quaternion);

				if (SgtCamera.TryFind(camera, ref observer) == true)
				{
					rollRotation *= observer.RollQuaternion;
				}

				if (RollWithCamera == true)
				{
					rotation = rollRotation;
				}
				else
				{
					rotation = cameraRotation;
				}

				if (AvoidClipping == true)
				{
					var directionA = Vector3.Normalize(transform.position - camera.transform.position);
					var directionB = rotation * Vector3.forward;
					var theta      = Vector3.Angle(directionA, directionB);
					var axis       = Vector3.Cross(directionA, directionB);

					rotation = Quaternion.AngleAxis(theta, -axis) * rotation;
				}

				cachedTransform.rotation = rotation;
			}
			Save(camera);
		}

		private void CameraPreRender(Camera camera)
		{
			Restore(camera);
		}

		public void Save(Camera camera)
		{
			var cameraState = SgtCameraState.Save(ref cameraStates, camera);
		
			cameraState.LocalRotation = transform.localRotation;
		}

		private void Restore(Camera camera)
		{
			var cameraState = SgtCameraState.Restore(cameraStates, camera);

			if (cameraState != null)
			{
				transform.localRotation = cameraState.LocalRotation;
			}
		}

		public void Revert()
		{
			transform.localRotation = Quaternion.identity;
		}
	}
}