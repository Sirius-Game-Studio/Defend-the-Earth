using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;

namespace SpaceGraphicsToolkit
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(SgtInputManager))]
	public class SgtInputManager_Editor : SgtEditor<SgtInputManager>
	{
		protected override void OnInspector()
		{
			EditorGUILayout.HelpBox("This component automatically converts mouse and touch inputs into a simple format.", MessageType.Info);

			DrawDefault("SimulateMultiFingers", "This allows you to simulate multi touch inputs on devices that don't support them (e.g. desktop).");
			DrawDefault("PinchTwistKey", "This allows you to set which key is required to simulate multi key twisting.");
			DrawDefault("MultiDragKey", "This allows you to set which key is required to simulate multi key dragging.");
		}
	}
}
#endif

namespace SpaceGraphicsToolkit
{
	/// <summary>This component converts mouse and touch inputs into a single interface.</summary>
	[HelpURL(SgtHelper.HelpUrlPrefix + "SgtInputManager")]
	[AddComponentMenu(SgtHelper.ComponentMenuPrefix + "Input Manager")]
	public class SgtInputManager : SgtLinkedBehaviour<SgtInputManager>
	{
		public class Finger
		{
			public int     Index;
			public bool    Marked;
			public float   Age;
			public bool    StartedOverGui;
			public Vector2 StartScreenPosition;
			public Vector2 LastScreenPosition;
			public Vector2 ScreenPosition;
		}

		/// <summary>This allows you to simulate multi touch inputs on devices that don't support them (e.g. desktop).</summary>
		public bool SimulateMultiFingers = true;

		/// <summary>This allows you to set which key is required to simulate multi key twisting.</summary>
		public KeyCode PinchTwistKey = KeyCode.LeftControl;

		/// <summary>This allows you to set which key is required to simulate multi key dragging.</summary>
		public KeyCode MultiDragKey = KeyCode.LeftAlt;

		public static List<Finger> Fingers = new List<Finger>();

		public static System.Action<Finger> OnFingerTap;

		private static List<RaycastResult> tempRaycastResults = new List<RaycastResult>(10);

		private static PointerEventData tempPointerEventData;

		private static EventSystem tempEventSystem;

		private static List<Finger> filteredFingers = new List<Finger>();

		public static bool AnyMouseButtonSet
		{
			get
			{
				for (var i = 0; i < 4; i++)
				{
					if (Input.GetMouseButton(i) == true)
					{
						return true;
					}
				}

				return false;
			}
		}

		public static List<Finger> GetFingers(bool ignoreIfStartedOverGui)
		{
			filteredFingers.Clear();

			for (var i = 0; i < Fingers.Count; i++)
			{
				var finger = Fingers[i];

				if (ignoreIfStartedOverGui == true && finger.StartedOverGui == true)
				{
					continue;
				}

				filteredFingers.Add(finger);
			}

			return filteredFingers;
		}

		public static Vector2 GetScaledDelta(List<Finger> fingers)
		{
			var total = Vector2.zero;
			var count = 0;

			for (var i = fingers.Count - 1; i >= 0; i--)
			{
				var finger = fingers[i];

				if (finger != null)
				{
					total += finger.ScreenPosition - finger.LastScreenPosition;
					count += 1;
				}
			}

			if (count > 0)
			{
				var dpi = Screen.dpi;

				if (dpi <= 0)
				{
					dpi = 1.0f;
				}

				total *= Mathf.Sqrt(200.0f) / Mathf.Sqrt(dpi);
				total /= count;

			}

			return total;
		}

		public static Vector2 GetLastScreenCenter(List<Finger> fingers)
		{
			var total = Vector2.zero;
			var count = 0;

			for (var i = fingers.Count - 1; i >= 0; i--)
			{
				var finger = fingers[i];

				if (finger != null)
				{
					total += finger.LastScreenPosition;
					count += 1;
				}
			}

			return count > 0 ? total / count : total;
		}

		public static Vector2 GetScreenCenter(List<Finger> fingers)
		{
			var total = Vector2.zero;
			var count = 0;

			for (var i = fingers.Count - 1; i >= 0; i--)
			{
				var finger = fingers[i];

				if (finger != null)
				{
					total += finger.ScreenPosition;
					count += 1;
				}
			}

			return count > 0 ? total / count : total;
		}

		public static float GetScreenDistance(List<Finger> fingers, Vector2 center)
		{
			var total = 0.0f;
			var count = 0;

			for (var i = fingers.Count - 1; i >= 0; i--)
			{
				var finger = fingers[i];

				if (finger != null)
				{
					total += Vector2.Distance(center, finger.ScreenPosition);
					count += 1;
				}
			}

			return count > 0 ? total / count : total;
		}

		public static float GetLastScreenDistance(List<Finger> fingers, Vector2 center)
		{
			var total = 0.0f;
			var count = 0;

			for (var i = fingers.Count - 1; i >= 0; i--)
			{
				var finger = fingers[i];

				if (finger != null)
				{
					total += Vector2.Distance(center, finger.LastScreenPosition);
					count += 1;
				}
			}

			return count > 0 ? total / count : total;
		}

		public static float GetPinchScale(List<Finger> fingers, float wheelSensitivity = 0.0f)
		{
			var center       = GetScreenCenter(fingers);
			var lastCenter   = GetLastScreenCenter(fingers);
			var distance     = GetScreenDistance(fingers, center);
			var lastDistance = GetLastScreenDistance(fingers, lastCenter);

			if (lastDistance > 0.0f)
			{
				return distance / lastDistance;
			}

			if (wheelSensitivity != 0.0f)
			{
				var scroll = Input.mouseScrollDelta.y;

				if (scroll > 0.0f)
				{
					return 1.0f - wheelSensitivity;
				}

				if (scroll < 0.0f)
				{
					return 1.0f + wheelSensitivity;
				}
			}

			return 1.0f;
		}

		public static bool PointOverGui(Vector2 screenPosition)
		{
			return RaycastGui(screenPosition).Count > 0;
		}

		public static List<RaycastResult> RaycastGui(Vector2 screenPosition)
		{
			return RaycastGui(screenPosition, 1 << 5);
		}

		public static List<RaycastResult> RaycastGui(Vector2 screenPosition, LayerMask layerMask)
		{
			tempRaycastResults.Clear();

			var currentEventSystem = EventSystem.current;

			if (currentEventSystem != null)
			{
				// Create point event data for this event system?
				if (currentEventSystem != tempEventSystem)
				{
					tempEventSystem = currentEventSystem;

					if (tempPointerEventData == null)
					{
						tempPointerEventData = new PointerEventData(tempEventSystem);
					}
					else
					{
						tempPointerEventData.Reset();
					}
				}

				// Raycast event system at the specified point
				tempPointerEventData.position = screenPosition;

				currentEventSystem.RaycastAll(tempPointerEventData, tempRaycastResults);

				// Loop through all results and remove any that don't match the layer mask
				if (tempRaycastResults.Count > 0)
				{
					for (var i = tempRaycastResults.Count - 1; i >= 0; i--)
					{
						var raycastResult = tempRaycastResults[i];
						var raycastLayer  = 1 << raycastResult.gameObject.layer;

						if ((raycastLayer & layerMask) == 0)
						{
							tempRaycastResults.RemoveAt(i);
						}
					}
				}
			}

			return tempRaycastResults;
		}

		protected override void OnEnable()
		{
			if (InstanceCount > 0)
			{
				Debug.LogWarning("Your scene already contains an instance of SgtInputSystem!", FirstInstance);
			}

			base.OnEnable();
		}

		protected virtual void Update()
		{
			if (this == FirstInstance)
			{
				Mark();
				{
					Poll();
				}
				Sweep();
			}
		}

		private void Mark()
		{
			for (var i = Fingers.Count - 1; i >= 0; i--)
			{
				Fingers[i].Marked = true;
			}
		}

		private void Poll()
		{
			// Update real fingers
			if (Input.touchCount > 0)
			{
				for (var i = 0; i < Input.touchCount; i++)
				{
					var touch = Input.GetTouch(i);

					// Only poll fingers that are active?
					if (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Stationary || touch.phase == TouchPhase.Moved)
					{
						AddFinger(touch.fingerId, touch.position);
					}
				}
			}
			// If there are no real touches, simulate some from the mouse?
			else if (AnyMouseButtonSet == true)
			{
				var screen        = new Rect(0, 0, Screen.width, Screen.height);
				var mousePosition = (Vector2)Input.mousePosition;

				// Is the mouse within the screen?
				if (screen.Contains(mousePosition) == true)
				{
					AddFinger(0, mousePosition);

					if (SimulateMultiFingers == true)
					{
						if (Input.GetKey(PinchTwistKey) == true)
						{
							var center = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);

							AddFinger(1, center - (mousePosition - center));
						}
						else if (Input.GetKey(MultiDragKey) == true)
						{
							AddFinger(1, mousePosition);
						}
					}
				}
			}
		}

		private void AddFinger(int index, Vector2 screenPosition)
		{
			var finger = FindFinger(index);

			if (finger != null)
			{
				finger.Index              = index;
				finger.Marked             = false;
				finger.Age               += Time.deltaTime;
				finger.LastScreenPosition = finger.ScreenPosition;
				finger.ScreenPosition     = screenPosition;
			}
			else
			{
				finger = SgtPoolClass<Finger>.Pop() ?? new Finger();

				finger.Index               = index;
				finger.Marked              = false;
				finger.Age                 = 0.0f;
				finger.StartScreenPosition = screenPosition;
				finger.LastScreenPosition  = screenPosition;
				finger.ScreenPosition      = screenPosition;
				finger.StartedOverGui      = PointOverGui(screenPosition);

				Fingers.Add(finger);
			}
		}

		private Finger FindFinger(int index)
		{
			for (var i = Fingers.Count - 1; i >= 0; i--)
			{
				var finger = Fingers[i];

				if (finger.Index == index)
				{
					return finger;
				}
			}

			return null;
		}

		private void Sweep()
		{
			for (var i = Fingers.Count - 1; i >= 0; i--)
			{
				var finger = Fingers[i];

				if (finger.Marked == true)
				{
					// Tap?
					if (finger.Age <= 0.5f)
					{
						var screenMovement  = Vector2.Distance(finger.StartScreenPosition, finger.ScreenPosition);
						var screenThreshold = Mathf.Min(Screen.width, Screen.height) * 0.025f;

						if (screenMovement <= screenThreshold)
						{
							if (OnFingerTap != null) OnFingerTap(finger);
						}
					}

					// Remove and pool
					Fingers.RemoveAt(i);

					SgtPoolClass<Finger>.Add(finger);
				}
			}
		}
	}
}