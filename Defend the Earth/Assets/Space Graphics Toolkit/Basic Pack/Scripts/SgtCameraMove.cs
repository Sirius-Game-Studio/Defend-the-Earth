using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

namespace SpaceGraphicsToolkit
{
	using UnityEditor;

	[CanEditMultipleObjects]
	[CustomEditor(typeof(SgtCameraMove))]
	public class SgtCameraMove_Editor : SgtEditor<SgtCameraMove>
	{
		protected override void OnInspector()
		{
			BeginError(Any(t => t.KeySensitivity == 0.0f));
				DrawDefault("KeySensitivity", "The distance the camera moves per second with keyboard inputs.");
			EndError();
			BeginError(Any(t => t.PanSensitivity == 0.0f));
				DrawDefault("PanSensitivity", "The distance the camera moves relative to the finger drag.");
			EndError();
			BeginError(Any(t => t.PinchSensitivity == 0.0f));
				DrawDefault("PinchSensitivity", "The distance the camera moves relative to the finger pinch scale.");
			EndError();
			DrawDefault("WheelSensitivity", "If you want the mouse wheel to simulate pinching then set the strength of it here.");
			DrawDefault("Dampening", "How quickly the position goes to the target value (-1 = instant).");
		}
	}
}
#endif

namespace SpaceGraphicsToolkit
{
	/// <summary>This component allows you to move the current GameObject based on WASD/mouse/finger drags. NOTE: This requires the SgtInputManager in your scene to function.</summary>
	[HelpURL(SgtHelper.HelpUrlPrefix + "SgtCameraMove")]
	[AddComponentMenu(SgtHelper.ComponentMenuPrefix + "Camera Move")]
	public class SgtCameraMove : MonoBehaviour
	{
		/// <summary>The distance the camera moves per second with keyboard inputs.</summary>
		public float KeySensitivity = 100.0f;

		/// <summary>The distance the camera moves relative to the finger drag.</summary>
		public float PanSensitivity = 1.0f;

		/// <summary>The distance the camera moves relative to the finger pinch scale.</summary>
		public float PinchSensitivity = 200.0f;

		/// <summary>If you want the mouse wheel to simulate pinching then set the strength of it here.</summary>
		[Range(-1.0f, 1.0f)]
		public float WheelSensitivity;

		/// <summary>How quickly the position goes to the target value (-1 = instant).</summary>
		public float Dampening = 10.0f;

		[System.NonSerialized]
		private Vector3 remainingDelta;

		protected virtual void Update()
		{
			AddToDelta();
			DampenDelta();
		}

		private void AddToDelta()
		{
			// Get delta from fingers
			var fingers = SgtInputManager.GetFingers(true);
			var deltaXY = SgtInputManager.GetScaledDelta(fingers) * PanSensitivity;
			var deltaZ  = (SgtInputManager.GetPinchScale(fingers, WheelSensitivity) - 1.0f) * PinchSensitivity;

			if (fingers.Count < 2)
			{
				deltaXY = Vector2.zero;
			}

			// Add delta from keyboard
			deltaXY.x += Input.GetAxisRaw("Horizontal") * KeySensitivity * Time.deltaTime;
			deltaZ    += Input.GetAxisRaw("Vertical") * KeySensitivity * Time.deltaTime;

			// Store old position
			var oldPosition = transform.localPosition;

			// Translate
			transform.Translate(deltaXY.x, deltaXY.y, deltaZ, Space.Self);

			// Add to remaining
			remainingDelta += transform.localPosition - oldPosition;

			// Revert position
			transform.localPosition = oldPosition;
		}

		private void DampenDelta()
		{
			// Dampen remaining delta
			var factor   = SgtHelper.DampenFactor(Dampening, Time.deltaTime);
			var newDelta = Vector3.Lerp(remainingDelta, Vector3.zero, factor);

			// Translate by difference
			transform.localPosition += remainingDelta - newDelta;

			// Update remaining
			remainingDelta = newDelta;
		}
	}
}