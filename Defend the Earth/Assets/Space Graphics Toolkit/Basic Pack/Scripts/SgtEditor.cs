#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;

namespace SpaceGraphicsToolkit
{
	public abstract class SgtEditor<T> : Editor
		where T : MonoBehaviour
	{
		protected T Target;

		protected T[] Targets;

		private static List<Color> Colors = new List<Color>();

		private static GUIContent customContent = new GUIContent();

		public override void OnInspectorGUI()
		{
			EditorGUI.BeginChangeCheck();
			{
				Target  = (T)target;
				Targets = targets.Select(t => (T)t).ToArray();
				Colors.Clear();

				Separator();

				OnInspector();

				Separator();

				serializedObject.ApplyModifiedProperties();
			}
			if (EditorGUI.EndChangeCheck() == true)
			{
				GUI.changed = true; Repaint();

				foreach (var t in Targets)
				{
					SgtHelper.SetDirty(t);
				}
			}
		}

		protected virtual void OnSceneGUI()
		{
			Target = (T)target;

			OnScene();

			if (GUI.changed == true)
			{
				SgtHelper.SetDirty(target);
			}
		}

		protected static void ForceExecutionOrder(int order)
		{
			var scripts = MonoImporter.GetAllRuntimeMonoScripts();

			for (var i = scripts.Length - 1; i >= 0; i--)
			{
				var script = scripts[i];

				if (script != null && script.name == typeof(T).Name)
				{
					if (MonoImporter.GetExecutionOrder(script) != order)
					{
						MonoImporter.SetExecutionOrder(script, order);
					}

					return;
				}
			}
		}

		protected void Each(System.Action<T> update)
		{
			foreach (var t in Targets)
			{
				update(t);
			}
		}

		protected void DirtyEach(System.Action<T> update)
		{
			foreach (var t in Targets)
			{
				update(t);

				SgtHelper.SetDirty(t);
			}
		}

		protected bool Any(System.Func<T, bool> check)
		{
			foreach (var t in Targets)
			{
				if (check(t) == true)
				{
					return true;
				}
			}

			return false;
		}

		protected bool All(System.Func<T, bool> check)
		{
			foreach (var t in Targets)
			{
				if (check(t) == false)
				{
					return false;
				}
			}

			return true;
		}

		protected bool Button(string text)
		{
			var rect = SgtHelper.Reserve();

			return GUI.Button(rect, text);
		}

		protected bool HelpButton(string helpText, UnityEditor.MessageType type, string buttonText, float buttonWidth)
		{
			var clicked = false;

			EditorGUILayout.BeginHorizontal();
			{
				EditorGUILayout.HelpBox(helpText, type);

				var style = new GUIStyle(GUI.skin.button); style.wordWrap = true;

				clicked = GUILayout.Button(buttonText, style, GUILayout.Width(buttonWidth));
			}
			EditorGUILayout.EndHorizontal();

			return clicked;
		}

		protected void BeginError(bool error = true)
		{
			Colors.Add(GUI.color);

			GUI.color = error == true ? Color.red : Colors[0];
		}

		protected void EndError()
		{
			var index = Colors.Count - 1;

			GUI.color = Colors[index];

			Colors.RemoveAt(index);
		}

		protected void BeginDisabled(bool disabled = true)
		{
			EditorGUI.BeginDisabledGroup(disabled);
		}

		protected void EndDisabled()
		{
			EditorGUI.EndDisabledGroup();
		}

		protected bool DrawDefault(string propertyPath, string overrideTooltip = null)
		{
			EditorGUI.BeginChangeCheck();
			{
				var property = serializedObject.FindProperty(propertyPath);

				customContent.text    = property.displayName;
				customContent.tooltip = property.tooltip;

				if (string.IsNullOrEmpty(overrideTooltip) == false)
				{
					customContent.tooltip = overrideTooltip;
				}

				EditorGUILayout.PropertyField(property, customContent, true);
			}
			if (EditorGUI.EndChangeCheck() == true)
			{
				serializedObject.ApplyModifiedProperties();

				for (var i = Targets.Length - 1; i >= 0; i--)
				{
					SgtHelper.SetDirty(Targets[i]);
				}

				return true;
			}

			return false;
		}

		protected void DrawDefault(string propertyPath, ref bool modified, string overrideTooltip = null)
		{
			if (DrawDefault(propertyPath, overrideTooltip) == true)
			{
				modified = true;
			}
		}

		protected void DrawDefault(string propertyPath, ref bool modified1, ref bool modified2, string overrideTooltip = null)
		{
			if (DrawDefault(propertyPath, overrideTooltip) == true)
			{
				modified1 = true;
				modified2 = true;
			}
		}

		protected void Separator()
		{
			EditorGUILayout.Separator();
		}

		protected void BeginIndent()
		{
			EditorGUI.indentLevel += 1;
		}

		protected void EndIndent()
		{
			EditorGUI.indentLevel -= 1;
		}

		protected virtual void OnInspector()
		{
		}

		protected virtual void OnScene()
		{
		}

		protected void RequireCamera()
		{
			if (SgtCamera.InstanceCount == 0)
			{
				Separator();

				if (HelpButton("Your scene contains no SgtCameras", MessageType.Error, "Fix", 50.0f) == true)
				{
					SgtHelper.ClearSelection();

					foreach (var camera in Camera.allCameras)
					{
						SgtHelper.AddToSelection(camera.gameObject);

						SgtHelper.GetOrAddComponent<SgtCamera>(camera.gameObject);
					}
				}
			}
		}
	}
}
#endif
