/*
 * @Author: wangyun
 * @CreateTime: 2023-01-31 18:18:18 572
 * @LastEditor: wangyun
 * @EditTime: 2023-01-31 18:18:18 577
 */

using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace Control {
	[CanEditMultipleObjects]
	[CustomEditor(typeof(StateCtrlChildPath), true)]
	public class StateCtrlChildPathInspector : Editor {
		private const float REORDERABLE_LIST_THUMB_WIDTH = 16;
		private const float LIST_ELEMENT_HEIGHT = 20;
		
		private new StateCtrlChildPath target => base.target as StateCtrlChildPath;

		private readonly StateControllerSelectDrawer m_StateControllerSelectDrawer = new StateControllerSelectDrawer();
		private readonly AssetPathDrawer m_AssetPathDrawer = new AssetPathDrawer();
		private SerializedProperty m_StateControllerProperty;
		private SerializedProperty m_DefaultValueProperty;
		private SerializedProperty m_PrefabPathProperty;
		private SerializedProperty m_ValueValuesProperty;
		private List<int> m_ValueKeys;
		private List<string> m_ValueValues;

		private ReorderableList m_List;
		private bool m_IsListFold;
		
		private bool m_Editable;

		private void OnEnable() {
			m_StateControllerProperty = serializedObject.FindProperty("controller");
			m_DefaultValueProperty = serializedObject.FindProperty("m_DefaultValue");
			m_PrefabPathProperty = serializedObject.FindProperty("prefabPath");
			m_ValueValuesProperty = serializedObject.FindProperty("m_ValueValues");

			AssetPathAttribute assetPathAttribute = new AssetPathAttribute(typeof(GameObject));
			FieldInfo assetPathAttributeFI = m_AssetPathDrawer.GetType().GetField("m_Attribute", BindingFlags.Instance | BindingFlags.NonPublic);
			assetPathAttributeFI?.SetValue(m_AssetPathDrawer, assetPathAttribute);
			
			FieldInfo valueKeysFI = target.GetType().GetField("m_ValueKeys", BindingFlags.Instance | BindingFlags.NonPublic);
			m_ValueKeys = valueKeysFI?.GetValue(target) as List<int> ?? new List<int>();
			FieldInfo valueValuesFI = target.GetType().GetField("m_ValueValues", BindingFlags.Instance | BindingFlags.NonPublic);
			m_ValueValues = valueValuesFI?.GetValue(target) as List<string> ?? new List<string>();
			m_List = new ReorderableList(m_ValueKeys, null, true, false, false, false) {
				drawElementCallback = DrawValueKeysItems,
				elementHeight = LIST_ELEMENT_HEIGHT, footerHeight = 0, headerHeight = 0
			};
		}

		public override void OnInspectorGUI() {
			DrawProperty(m_StateControllerProperty, m_StateControllerSelectDrawer);
			DrawProperty(m_DefaultValueProperty, m_AssetPathDrawer);
			DrawProperty(m_PrefabPathProperty, m_AssetPathDrawer);

			CustomEditorGUI.BeginBackgroundColor(Color.gray * 0.15F);
			EditorGUILayout.BeginHorizontal("DD Background");
			CustomEditorGUI.EndBackgroundColor();
			bool newIsListFold = EditorGUILayout.Toggle(m_IsListFold, EditorStyles.foldout, GUILayout.Width(REORDERABLE_LIST_THUMB_WIDTH));
			if (newIsListFold != m_IsListFold) {
				Undo.RecordObject(this, "Fold");
				m_IsListFold = newIsListFold;
			}
			EditorGUILayout.LabelField("Keys", GUILayout.Width(EditorGUIUtility.labelWidth - REORDERABLE_LIST_THUMB_WIDTH - 5F));
			EditorGUILayout.LabelField("Values");
			EditorGUILayout.EndHorizontal();
			
			if (!m_Editable) {
				CustomEditorGUI.BeginDisabled(true);
			}
			if (m_IsListFold) {
				m_List.DoLayoutList();
			}
			if (!m_Editable) {
				CustomEditorGUI.EndDisabled();
			}
			m_Editable = CustomEditorGUI.Toggle(m_Editable, "编辑数据", CustomEditorGUI.COLOR_TOGGLE_CHECKED_EDITOR);
			if (GUI.changed) {
				serializedObject.ApplyModifiedProperties();
				EditorUtility.SetDirty(target);
			}
		}

		private static void DrawProperty(SerializedProperty property, PropertyDrawer drawer) {
			string propertyName = GetPropertyName(property);
			GUIContent content = EditorGUIUtility.TrTempContent(propertyName);
			float height = drawer.GetPropertyHeight(property, content);
			Rect rect = EditorGUILayout.GetControlRect(true, height);
			drawer.OnGUI(rect, property, content);
		}

		private static string GetPropertyName(SerializedProperty property) {
			string propertyName = property.name;
			if (propertyName.StartsWith("m_")) {
				propertyName = propertyName.Substring(2);
			}
			string firstChar = propertyName.Substring(0, 1);
			string newFirstChar = firstChar.ToUpper();
			if (newFirstChar != firstChar) {
				propertyName = newFirstChar + propertyName.Substring(1);
			}
			return propertyName;
		}
		
		private void DrawValueKeysItems(Rect rect, int index, bool isActive, bool isFocused) {
			Rect keyRect = new Rect(rect.x, rect.y + 1, EditorGUIUtility.labelWidth - REORDERABLE_LIST_THUMB_WIDTH - 5F, rect.height - 2);
			m_ValueKeys[index] = EditorGUI.IntField(keyRect, m_ValueKeys[index]);
			Rect valueRect = new Rect(keyRect.xMax + 2, rect.y + 1, rect.width - keyRect.width - 2 - 22 - 2, rect.height - 2);
			m_AssetPathDrawer.OnGUI(valueRect, m_ValueValuesProperty.GetArrayElementAtIndex(index), EditorGUIUtility.TrTempContent(string.Empty));
			Rect rightRect = new Rect(valueRect.xMax + 2, rect.y + 1, 28, rect.height - 2);
			if (GUI.Button(rightRect, EditorGUIUtility.IconContent("d_winbtn_win_close"))) {
				EditorApplication.delayCall += () => {
					m_ValueKeys.RemoveAt(index);
					m_ValueValues.RemoveAt(index);
				};
			}
		}
	}
}