using UnityEngine;
using System.Collections.Generic;

namespace SpaceGraphicsToolkit
{
	/// <summary>This class can be used to easily save & load the state of an object (e.g. transform position) based on the camera rendering state.</summary>
	public class SgtCameraState
	{
		/// <summary>The camera associated with this state.</summary>
		public Camera Camera;

		public static T Save<T>(ref List<T> cameraStates, Camera camera)
			where T : SgtCameraState, new()
		{
			if (cameraStates == null)
			{
				cameraStates = new List<T>();
			}

			for (var i = cameraStates.Count - 1; i >= 0; i--)
			{
				var cameraState = cameraStates[i];

				if (cameraState == null)
				{
					cameraStates.RemoveAt(i); continue;
				}

				if (cameraState.Camera == null)
				{
					SgtPoolClass<T>.Add(cameraState); cameraStates.RemoveAt(i); continue;
				}

				if (cameraState.Camera == camera)
				{
					return cameraState;
				}
			}

			var newCameraState = SgtPoolClass<T>.Pop() ?? new T();

			newCameraState.Camera = camera;

			cameraStates.Add(newCameraState);

			return newCameraState;
		}

		public static T Restore<T>(List<T> cameraStates, Camera camera)
			where T : SgtCameraState
		{
			if (cameraStates != null)
			{
				for (var i = cameraStates.Count - 1; i >= 0; i--)
				{
					var cameraState = cameraStates[i];

					if (cameraState.Camera == camera)
					{
						return cameraState;
					}
				}
			}

			return null;
		}

		public static void Clear<T>(List<T> cameraStates)
			where T : SgtCameraState
		{
			if (cameraStates != null)
			{
				for (var i = cameraStates.Count - 1; i >= 0; i--)
				{
					SgtPoolClass<T>.Add(cameraStates[i]);
				}

				cameraStates.Clear();
			}
		}
	}
}