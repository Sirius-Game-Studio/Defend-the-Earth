using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;

namespace SpaceGraphicsToolkit
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(SgtFloatingOrbitVisual))]
	public class SgtFloatingOrbitVisual_Editor : SgtEditor<SgtFloatingOrbitVisual>
	{
		protected override void OnInspector()
		{
			DrawDefault("Thickness", "The thickness of the visual ring in local space.");
			DrawDefault("Points", "The amount of points used to draw the orbit.");
			DrawDefault("Scale", "The camera scale used to render the ring.");
			DrawDefault("Colors", "The color of the orbit ring as it goes around the orbit.");
		}
	}
}
#endif

namespace SpaceGraphicsToolkit
{
	/// <summary>This component draws an orbit in 3D space.</summary>
	[ExecuteInEditMode]
	[RequireComponent(typeof(MeshFilter))]
	[RequireComponent(typeof(MeshRenderer))]
	[HelpURL(SgtHelper.HelpUrlPrefix + "SgtFloatingOrbitVisual")]
	[AddComponentMenu(SgtHelper.ComponentMenuPrefix + "Floating Orbit Visual")]
	public class SgtFloatingOrbitVisual : MonoBehaviour
	{
		/// <summary>The thickness of the visual ring in local space.</summary>
		public SgtLength Thickness = 100000.0f;

		/// <summary>The amount of points used to draw the orbit.</summary>
		public int Points = 360;

		/// <summary>The camera scale used to render the ring.</summary>
		public long Scale = 1;

		/// <summary>The color of the orbit ring as it goes around the orbit.</summary>
		public Gradient Colors;

		[System.NonSerialized]
		private MeshFilter cachedMeshFilter;

		[System.NonSerialized]
		private bool cachedMeshFilterSet;

		[System.NonSerialized]
		private Mesh visualMesh;

		[System.NonSerialized]
		private List<Vector3> positions = new List<Vector3>(360 * 2);

		[System.NonSerialized]
		private List<Vector2> coords = new List<Vector2>(360 * 2);

		[System.NonSerialized]
		private List<Color> colors = new List<Color>(360 * 2);

		[System.NonSerialized]
		private List<int> indices = new List<int>(360 * 6);

		public void Draw(SgtFloatingOrbit orbit)
		{
			if (cachedMeshFilterSet == false)
			{
				cachedMeshFilter    = GetComponent<MeshFilter>();
				cachedMeshFilterSet = true;
			}

			if (visualMesh == null)
			{
				visualMesh = cachedMeshFilter.sharedMesh = SgtHelper.CreateTempMesh("Orbit Visual");
			}

			positions.Clear();
			coords.Clear();
			colors.Clear();
			indices.Clear();

			var camera = default(SgtFloatingCamera);

			if (SgtFloatingCamera.TryGetCamera(Scale, ref camera) == true)
			{
				var position = camera.CalculatePosition(ref orbit.ParentPoint.Position);
				var rotation = orbit.ParentPoint.transform.rotation * Quaternion.Euler(orbit.Tilt);
				var r1       = orbit.Radius;
				var r2       = orbit.Radius * (1.0f - orbit.Oblateness);
				var i1       = r1 - Thickness * 0.5;
				var i2       = r2 - Thickness * 0.5;
				var o1       = i1 + Thickness;
				var o2       = i2 + Thickness;
				var step     = 360.0 / Points;
				var stepI    = 1.0f / (Points - 1);

				for (var i = 0; i < Points; i++)
				{
					var angle = (orbit.Angle - i * step) * Mathf.Deg2Rad;
					var sin   = System.Math.Sin(angle);
					var cos   = System.Math.Cos(angle);

					// Inner
					{
						var point   = position;
						var offsetX = sin * i1;
						var offsetY = 0.0;
						var offsetZ = cos * i2;

						SgtFloatingOrbit.Rotate(rotation, ref offsetX, ref offsetY, ref offsetZ);

						point.x += (float)offsetX;
						point.y += (float)offsetY;
						point.z += (float)offsetZ;

						point = transform.InverseTransformPoint(point);

						positions.Add(point);
					}

					// Outer
					{
						var point   = position;
						var offsetX = sin * o1;
						var offsetY = 0.0;
						var offsetZ = cos * o2;

						SgtFloatingOrbit.Rotate(rotation, ref offsetX, ref offsetY, ref offsetZ);

						point.x += (float)offsetX;
						point.y += (float)offsetY;
						point.z += (float)offsetZ;

						point = transform.InverseTransformPoint(point);

						positions.Add(point);
					}

					var u     = stepI * i;
					var color = Colors.Evaluate(u);

					coords.Add(new Vector2(u, 0.0f));
					coords.Add(new Vector2(u, 1.0f));

					colors.Add(color);
					colors.Add(color);
				}

				for (var i = 0; i < Points; i++)
				{
					var indexA = i * 2 + 0;
					var indexB = i * 2 + 1; 
					var indexC = i * 2 + 2; indexC %= Points * 2;
					var indexD = i * 2 + 3; indexD %= Points * 2;

					indices.Add(indexA);
					indices.Add(indexB);
					indices.Add(indexC);
					indices.Add(indexD);
					indices.Add(indexC);
					indices.Add(indexB);
				}

				visualMesh.SetVertices(positions);
				visualMesh.SetTriangles(indices, 0);
				visualMesh.SetUVs(0, coords);
				visualMesh.SetColors(colors);
				visualMesh.RecalculateBounds();
			}
		}
	}
}