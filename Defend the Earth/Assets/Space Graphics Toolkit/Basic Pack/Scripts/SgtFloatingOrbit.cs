using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

namespace SpaceGraphicsToolkit
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(SgtFloatingOrbit))]
	public class SgtFloatingOrbit_Editor : SgtEditor<SgtFloatingOrbit>
	{
		protected override void OnInspector()
		{
			BeginError(Any(t => t.ParentPoint == null));
				Each(t => t.UnregisterParentPoint());
				DrawDefault("parentPoint", "The point this orbit will go around. NOTE: This should be null/None if it will be spawned by SgtFloatingSpawnerOrbit.");
				Each(t => t.RegisterParentPoint());
			EndError();
			if (Any(t => t.ParentPoint == null))
			{
				EditorGUILayout.HelpBox("ParentPoint should only be None/null if this prefab will be spawned from the SgtFloatingSpawnerOrbit component. If not, you should add one to the parent GameObject.", MessageType.Info);
			}
			DrawDefault("Radius", "The radius of the orbit in meters.");
			DrawDefault("Oblateness", "How squashed the orbit is.");
			DrawDefault("Tilt", "The local rotation of the orbit in degrees.");
			DrawDefault("Angle", "The curent position along the orbit in degrees.");
			DrawDefault("DegreesPerSecond", "The orbit speed.");
			DrawDefault("Visual", "If you want to display the orbit path, set the visual here. The visual should be in a child GameObject.");
		}
	}
}
#endif

namespace SpaceGraphicsToolkit
{
	/// <summary>This component will orbit the attached SgtFloatingPoint around the parent SgtFloatingPoint.</summary>
	[ExecuteInEditMode]
	[RequireComponent(typeof(SgtFloatingPoint))]
	[HelpURL(SgtHelper.HelpUrlPrefix + "SgtFloatingOrbit")]
	[AddComponentMenu(SgtHelper.ComponentMenuPrefix + "Floating Orbit")]
	public class SgtFloatingOrbit : MonoBehaviour
	{
		/// <summary>The radius of the orbit in meters.</summary>
		public double Radius = 1.0f;

		/// <summary>How squashed the orbit is.</summary>
		[Range(0.0f, 1.0f)]
		public float Oblateness;

		/// <summary>The local rotation of the orbit in degrees.</summary>
		public Vector3 Tilt;

		/// <summary>The curent position along the orbit in degrees.</summary>
		public double Angle;

		/// <summary>The orbit speed.</summary>
		public double DegreesPerSecond = 10.0f;

		/// <summary>If you want to display the orbit path, set the visual here. The visual should be in a child GameObject.</summary>
		public SgtFloatingOrbitVisual Visual;

		[SerializeField]
		private SgtFloatingPoint parentPoint;

		[System.NonSerialized]
		private SgtFloatingPoint cachedPoint;

		[System.NonSerialized]
		private bool cachedPointSet;

		/// <summary>The center orbit point. NOTE: This should be null/None if it will be spawned by SgtFloatingSpawnerOrbit.</summary>
		public SgtFloatingPoint ParentPoint
		{
			set
			{
				if (parentPoint != value)
				{
					UnregisterParentPoint();

					parentPoint = value;

					RegisterParentPoint();
				}
			}

			get
			{
				return parentPoint;
			}
		}

		public void RegisterParentPoint()
		{
			if (parentPoint != null)
			{
				parentPoint.OnPositionChanged += ParentPositionChanged;
			}
		}

		public void UnregisterParentPoint()
		{
			if (parentPoint != null)
			{
				parentPoint.OnPositionChanged -= ParentPositionChanged;
			}
		}

		public void UpdateOrbit()
		{
			var position = ParentPoint.Position;
			var rotation = ParentPoint.transform.rotation * Quaternion.Euler(Tilt);
			var r1       = Radius;
			var r2       = Radius * (1.0f - Oblateness);
			var localX   = System.Math.Sin(Angle * Mathf.Deg2Rad) * r1;
			var localY   = 0.0;
			var localZ   = System.Math.Cos(Angle * Mathf.Deg2Rad) * r2;

			Rotate(rotation, ref localX, ref localY, ref localZ);

			position.LocalX += localX;
			position.LocalY += localY;
			position.LocalZ += localZ;
			position.SnapLocal();

			cachedPoint.Position = position;
			cachedPoint.PositionChanged();

			if (SgtHelper.Enabled(Visual) == true)
			{
				Visual.Draw(this);
			}
		}

		// Rotates x and y only
		public static void Rotate(Quaternion q, ref double x, ref double y, ref double z)
		{
			var num01 = q.x * 2f;
			var num02 = q.y * 2f;
			var num03 = q.z * 2f;
			var num04 = q.x * num01;
			var num05 = q.y * num02;
			var num06 = q.z * num03;
			var num07 = q.x * num02;
			var num08 = q.x * num03;
			var num09 = q.y * num03;
			var num10 = q.w * num01;
			var num11 = q.w * num02;
			var num12 = q.w * num03;

			var rX = (1f - (num05 + num06)) * x + (num08 + num11) * z;
			var rY = (num07 + num12) * x + (num09 - num10) * z;
			var rZ = (num08 - num11) * x + (1f - (num04 + num05)) * z;

			x = rX;
			y = rY;
			z = rZ;
		}

		protected virtual void OnEnable()
		{
			if (cachedPointSet == false)
			{
				cachedPoint    = GetComponent<SgtFloatingPoint>();
				cachedPointSet = true;
			}

#if UNITY_EDITOR
			if (parentPoint == null)
			{
				var parent = transform.parent;

				if (parent != null)
				{
					parentPoint = GetComponent<SgtFloatingPoint>();
				}
			}
#endif

			RegisterParentPoint();

			SgtFloatingCamera.OnPositionChanged += PositionChanged;
		}

		protected virtual void OnDisable()
		{
			UnregisterParentPoint();

			SgtFloatingCamera.OnPositionChanged -= PositionChanged;
		}

		protected virtual void LateUpdate()
		{
			if (Application.isPlaying == true)
			{
				Angle += DegreesPerSecond * Time.deltaTime;
			}

			UpdateOrbit();
		}

		private void ParentPositionChanged()
		{
			LateUpdate();
		}

		private void PositionChanged(SgtFloatingCamera camera)
		{
			if (SgtHelper.Enabled(Visual) == true && camera.Scale == Visual.Scale)
			{
				UpdateOrbit();
			}
		}
	}
}