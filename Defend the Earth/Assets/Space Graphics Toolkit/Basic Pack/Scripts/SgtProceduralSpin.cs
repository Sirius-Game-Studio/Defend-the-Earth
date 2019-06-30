using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

namespace SpaceGraphicsToolkit
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(SgtProceduralSpin))]
	public class SgtProceduralSpin_Editor : SgtEditor<SgtProceduralSpin>
	{
		protected override void OnInspector()
		{
			DrawDefault("CustomAxis", "Enable this if you want the spin to go around any axis.");
			if (Any(t => t.CustomAxis == true))
			{
				BeginIndent();
					DrawDefault("Axis", "The axis of the spin.");
				EndIndent();
			}
			DrawDefault("CustomSpeed", "Enable this if you want to specify the speed of rotation, otherwise a random one will be picked.");
			if (Any(t => t.CustomSpeed == true))
			{
				BeginIndent();
					DrawDefault("Speed", "The axis of the spin.");
				EndIndent();
			}
			if (Any(t => t.CustomSpeed == false))
			{
				BeginIndent();
					DrawDefault("SpeedMin", "Minimum degrees per second.");
					DrawDefault("SpeedMax", "Maximum degrees per second.");
				EndIndent();
			}
			DrawDefault("UseFloatingObject", "If you enable this then the procedural generation to be based on the SgtFloatingObject seed.");
			if (Any(t => t.UseFloatingObject == true && t.GetComponent<SgtFloatingObject>() == null))
			{
				EditorGUILayout.HelpBox("Your GameObject doesn't have the SgtFloatingObject component.", MessageType.Error);
			}
			DrawDefault("UseRigidbody", "If you enable this then the attached Rigidbody.velocity will be used for the spin.");
			if (Any(t => t.UseRigidbody == true && t.GetComponent<Rigidbody>() == null))
			{
				EditorGUILayout.HelpBox("Your GameObject doesn't have the Rigidbody component.", MessageType.Error);
			}
			DrawDefault("RandomlyRotate", "If you enable this then the transform will be randomly rotated on generation around its axis.");
		}
	}
}
#endif

namespace SpaceGraphicsToolkit
{
	/// <summary>This component rotates the current GameObject along a random axis, with a random speed.</summary>
	[HelpURL(SgtHelper.HelpUrlPrefix + "SgtProceduralSpin")]
	[AddComponentMenu(SgtHelper.ComponentMenuPrefix + "Procedural Spin")]
	public class SgtProceduralSpin : MonoBehaviour
	{
		/// <summary>Enable this if you want to specify the axis of rotation, otherwise a random one will be picked.</summary>
		public bool CustomAxis;

		/// <summary>The axis of the spin.</summary>
		public Vector3 Axis = Vector3.up;

		/// <summary>Enable this if you want to specify the speed of rotation, otherwise a random one will be picked.</summary>
		public bool CustomSpeed;

		/// <summary>The speed of the spin in degrees per second.</summary>
		public float Speed = 1.0f;

		/// <summary>Minimum degrees per second.</summary>
		public float SpeedMin;

		/// <summary>Maximum degrees per second.</summary>
		public float SpeedMax = 10.0f;

		/// <summary>If you enable this then the procedural generation to be based on the SgtFloatingObject.Seed.</summary>
		public bool UseFloatingObject;

		/// <summary>If you enable this then the attached Rigidbody.velocity will be used for the spin.</summary>
		public bool UseRigidbody;

		/// <summary>If you enable this then the transform will be randomly rotated on generation around its axis.</summary>
		public bool RandomlyRotate;

		[System.NonSerialized]
		private SgtFloatingObject cachedFloatingObject;

		[System.NonSerialized]
		private Transform cachedTransform;

		[ContextMenu("Generate")]
		public void Generate()
		{
			if (CustomAxis == false)
			{
				Axis = Random.onUnitSphere;
			}

			if (CustomSpeed == false)
			{
				Speed = Random.Range(SpeedMin, SpeedMax);
			}

			if (RandomlyRotate == true)
			{
				if (CustomAxis == true)
				{
					transform.localRotation *= Quaternion.AngleAxis(Random.value * 360.0f, Axis);
				}
				else
				{
					transform.localRotation = Random.rotation;
				}
			}

			if (UseRigidbody == true)
			{
				var rigidbody = GetComponent<Rigidbody>();

				if (rigidbody != null)
				{
					rigidbody.angularVelocity = Axis.normalized * Speed * Mathf.Deg2Rad;

					// Disable so Update isn't run
					enabled = false;
				}
			}
		}

		protected virtual void OnEnable()
		{
			if (UseFloatingObject == true)
			{
				cachedFloatingObject = GetComponent<SgtFloatingObject>();

				cachedFloatingObject.OnSpawn += SpawnSeed;
			}
			else
			{
				Generate();
			}

			cachedTransform = GetComponent<Transform>();
		}

		protected virtual void OnDisable()
		{
			if (UseFloatingObject == true)
			{
				cachedFloatingObject.OnSpawn -= SpawnSeed;
			}
		}

		protected virtual void Update()
		{
			cachedTransform.Rotate(Axis, Speed * Time.deltaTime);
		}

		private void SpawnSeed(int seed)
		{
			SgtHelper.BeginRandomSeed(seed);
			{
				Generate();
			}
			SgtHelper.EndRandomSeed();
		}
	}
}