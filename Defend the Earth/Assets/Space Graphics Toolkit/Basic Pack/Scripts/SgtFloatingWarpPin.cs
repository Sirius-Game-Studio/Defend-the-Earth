using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;

namespace SpaceGraphicsToolkit
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(SgtFloatingWarpPin))]
	public class SgtFloatingWarpPin_Editor : SgtEditor<SgtFloatingWarpPin>
	{
		protected override void OnInspector()
		{
			DrawDefault("PickDistance", "The maximum distance between the tap/click point at the SgtWarpTarget in scaled screen space.");
			DrawDefault("CurrentTarget", "The currently picked target.");

			Separator();

			BeginError(Any(t => t.Parent == null));
				DrawDefault("Parent", "The parent rect of the pin.");
			EndError();
			BeginError(Any(t => t.Rect == null));
				DrawDefault("Rect", "The main rect of the pin that will be placed on the screen on top of the CurrentTarget.");
			EndError();
			BeginError(Any(t => t.Title == null));
				DrawDefault("Title", "The name of the pin.");
			EndError();
			BeginError(Any(t => t.Group == null));
				DrawDefault("Group", "The group that will control hide/show of the pin.");
			EndError();
			BeginError(Any(t => t.Warp == null));
				DrawDefault("Warp", "The warp component that will be used.");
			EndError();
			DrawDefault("HideIfTooClose", "The the pin if we're within warping distance?");
		}
	}
}
#endif

namespace SpaceGraphicsToolkit
{
	/// <summary>This component moves Rect above the currently picked SgtFloatingTarget. You can tap/click the screen to update the picked target.</summary>
	[ExecuteInEditMode]
	[HelpURL(SgtHelper.HelpUrlPrefix + "SgtFloatingWarpPin")]
	[AddComponentMenu(SgtHelper.ComponentMenuPrefix + "Floating Warp Pin")]
	public class SgtFloatingWarpPin : MonoBehaviour
	{
		/// <summary>The maximum distance between the tap/click point at the SgtWarpTarget in scaled screen space.</summary>
		public float PickDistance = 0.025f;

		/// <summary>The currently picked target.</summary>
		public SgtFloatingTarget CurrentTarget;

		/// <summary>The parent rect of the pin.</summary>
		public RectTransform Parent;

		/// <summary>The main rect of the pin that will be placed on the screen on top of the CurrentTarget.</summary>
		public RectTransform Rect;

		/// <summary>The name of the pin.</summary>
		public Text Title;

		/// <summary>The group that will control hide/show of the pin.</summary>
		public CanvasGroup Group;

		/// <summary>The warp component that will be used.</summary>
		public SgtFloatingWarp Warp;

		/// <summary>The the pin if we're within warping distance?</summary>
		public bool HideIfTooClose = true;

		[HideInInspector]
		public float Alpha;

		public void ClickWarp()
		{
			if (CurrentTarget != null)
			{
				Warp.WarpTo(CurrentTarget.CachedPoint.Position, CurrentTarget.WarpDistance);
			}
		}

		public void Pick(Vector2 pickScreenPoint)
		{
			var floatingCamera = default(SgtFloatingCamera);

			if (SgtFloatingCamera.TryGetCamera(1, ref floatingCamera) == true)
			{
				var warpTarget   = SgtFloatingTarget.FirstInstance;
				var bestTarget   = default(SgtFloatingTarget);
				var bestDistance = float.PositiveInfinity;

				for (var i = 0; i < SgtFloatingTarget.InstanceCount; i++)
				{
					var localPosition = floatingCamera.CalculatePosition(ref warpTarget.CachedPoint.Position);
					var screenPoint   = floatingCamera.CachedCamera.WorldToScreenPoint(localPosition);

					if (screenPoint.z >= 0.0f)
					{
						var distance = ((Vector2)screenPoint - pickScreenPoint).sqrMagnitude;

						if (distance <= bestDistance)
						{
							bestDistance = distance;
							bestTarget   = warpTarget;
						}
					}

					warpTarget = warpTarget.NextInstance;
				}

				if (bestTarget != null)
				{
					var pickThreshold = Mathf.Min(Screen.width, Screen.height) * PickDistance;

					if (bestDistance <= pickThreshold * pickThreshold)
					{
						CurrentTarget = bestTarget;
					}
				}
				else
				{
					CurrentTarget = null;
				}
			}
		}

		protected virtual void OnEnable()
		{
			SgtInputManager.OnFingerTap += FingerTap;
		}

		protected virtual void OnDisable()
		{
			SgtInputManager.OnFingerTap += FingerTap;
		}

		protected virtual void LateUpdate()
		{
			var floatingCamera = default(SgtFloatingCamera);
			var targetAlpha    = 0.0f;

			if (SgtFloatingCamera.TryGetCamera(1, ref floatingCamera) == true)
			{
				if (CurrentTarget != null)
				{
					var localPosition = floatingCamera.CalculatePosition(ref CurrentTarget.CachedPoint.Position);
					var screenPoint   = floatingCamera.CachedCamera.WorldToScreenPoint(localPosition);

					if (screenPoint.z >= 0.0f)
					{
						var anchoredPosition = default(Vector2);

						if (RectTransformUtility.ScreenPointToLocalPointInRectangle(Parent, screenPoint, null, out anchoredPosition) == true)
						{
							Rect.anchoredPosition = anchoredPosition;
						}

						targetAlpha = 1.0f;

						if (HideIfTooClose == true)
						{
							if (SgtPosition.SqrDistance(ref SgtFloatingOrigin.CurrentPoint.Position, ref CurrentTarget.CachedPoint.Position) <= CurrentTarget.WarpDistance * CurrentTarget.WarpDistance)
							{
								targetAlpha = 0.0f;
							}
						}
					}
					else
					{
						Alpha = 0.0f;
					}

					Title.text = CurrentTarget.WarpName;
				}
			}

			Alpha = SgtHelper.Dampen(Alpha, targetAlpha, 10.0f, Time.deltaTime);

			Group.alpha          = Alpha;
			Group.blocksRaycasts = targetAlpha > 0.0f;
		}

		private void FingerTap(SgtInputManager.Finger finger)
		{
			if (finger.StartedOverGui == false)
			{
				Pick(finger.ScreenPosition);
			}
		}
	}
}