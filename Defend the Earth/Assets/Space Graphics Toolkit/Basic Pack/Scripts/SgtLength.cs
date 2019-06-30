using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

namespace SpaceGraphicsToolkit
{
	[CustomPropertyDrawer(typeof(SgtLength))]
	public class SgtLengthDrawer : PropertyDrawer
	{
		[System.NonSerialized]
		private int index;

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			var rect1 = position; rect1.xMax = position.xMax - 60;
			var rect2 = position; rect2.xMin = position.xMax - 58;
			
			EditorGUI.PropertyField(rect1, property.FindPropertyRelative("Value"), label);
			EditorGUI.PropertyField(rect2, property.FindPropertyRelative("Scale"), GUIContent.none);
		}
	}
}
#endif

namespace SpaceGraphicsToolkit
{
	[System.Serializable]
	public struct SgtLength
	{
		public enum ScaleType
		{
			Meter,
			Kilometer,
			AU,
			Lightyear,
			Parsec,
			GigaParsec
		}
		
		public double    Value;
		public ScaleType Scale;

		public SgtLength(double newValue, ScaleType newScale)
		{
			Value = newValue;
			Scale = newScale;
		}

		public static implicit operator double(SgtLength length)
		{
			switch (length.Scale)
			{
				case ScaleType.Meter:      return length.Value;
				case ScaleType.Kilometer:  return length.Value * 1000.0;
				case ScaleType.AU:         return length.Value * 149600000000.0;
				case ScaleType.Lightyear:  return length.Value * 9461000000000000.0;
				case ScaleType.Parsec:     return length.Value * 30856740000000000.0;
				case ScaleType.GigaParsec: return length.Value * 30860903000000000000000.0;
			}

			return default(double);
		}

		public static implicit operator SgtLength(double length)
		{
			return new SgtLength(length, ScaleType.Meter);
		}
	}
}