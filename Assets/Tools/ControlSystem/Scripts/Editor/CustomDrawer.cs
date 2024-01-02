/*
 * @Author: wangyun
 * @CreateTime: 2022-03-30 16:21:41 627
 * @LastEditor: wangyun
 * @EditTime: 2022-05-10 21:50:47 060
 */

using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;

using UObject = UnityEngine.Object;

namespace Control {
	[CustomPropertyDrawer(typeof(BlankAttribute))]
	public class BlankDrawer : PropertyDrawer {
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
		}
		
		public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
			return -2;
		}
	}

	[CustomPropertyDrawer(typeof(StateControllerSelectAttribute))]
	public class StateControllerSelectDrawer : PropertyDrawer {
		private const float CAPTURE_BTN_OFFSET = 40F;
		private readonly float s_LineHeight = EditorGUIUtility.singleLineHeight;
		private readonly float s_SpaceV = EditorGUIUtility.standardVerticalSpacing;

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			// 显示控制器选择下拉控件
			Component target = (Component) property.serializedObject.targetObject;
			List<StateController> controllers = new List<StateController> { null };
			List<string> titles = new List<string> { "无" };
			Transform trans = target.transform;
			int depth = 0;
			while (trans != null) {
				StateController[] _controllers = trans.GetComponents<StateController>();
				Dictionary<string, int> titleCountDict = new Dictionary<string, int>();
				foreach (var _controller in _controllers) {
					string title = string.IsNullOrEmpty(_controller.title) ? "匿名 " + -depth : _controller.title + " " + -depth;
					if (titleCountDict.TryGetValue(title, out int count)) {
						titleCountDict[title] = count + 1;
						title = $"{title}({count})";
					} else {
						titleCountDict.Add(title, 1);
					}
					controllers.Add(_controller);
					titles.Add(title);
				}
				trans = trans.parent;
				depth++;
			}
			StateController controller = property.objectReferenceValue as StateController;
			position.height = s_LineHeight;
			int index = controllers.IndexOf(controller);
			int newIndex = EditorGUI.Popup(position, label.text, index, titles.ToArray());
			if (newIndex != index) {
				property.objectReferenceValue = controllers[newIndex];
			}

			if (newIndex > 0) {
				controller = controllers[newIndex];
				List<State> states = controller.states;

				// 显示控制器状态选择下拉控件
				int stateCount = states.Count;
				string[] options = new string[stateCount];
				bool anyNameExist = states.Exists(state => !string.IsNullOrEmpty(state.name));
				for (int j = 0; j < stateCount; ++j) {
					string ext = anyNameExist ? states[j].name : states[j].desc;
					if (!string.IsNullOrEmpty(ext)) {
						ext = ":" + ext;
					}
					options[j] = j + ext;
				}
				int[] values = new int[stateCount];
				for (int j = 0; j < stateCount; ++j) {
					values[j] = j;
				}
				position.y += s_LineHeight + s_SpaceV;
				position.width -= CAPTURE_BTN_OFFSET;
				int newStateIndex = EditorGUI.IntPopup(position, "     Index", controller.Index, options, values);
				if (newStateIndex != controller.Index) {
					Undo.RecordObject(controller, "State.Select");
					controller.Index = newStateIndex;
					CustomEditorGUI.RepaintScene();
				}
				
				// 显示控制器记录状态控件
				position.x += position.width;
				position.width = CAPTURE_BTN_OFFSET;
				if (GUI.Button(position, "记录")) {
					if (target is BaseStateCtrl baseStateCtrl) {
						Undo.RecordObject(baseStateCtrl, "StateCapture");
						baseStateCtrl.Capture(states[controller.Index].uid);
					}
				}
			}
		}
		
		public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
			StateController controller = property.objectReferenceValue as StateController;
			return controller ? s_LineHeight + s_SpaceV + s_LineHeight : s_LineHeight;
		}
	}
	
	[CustomPropertyDrawer(typeof(ProgressControllerSelectAttribute))]
	public class ProgressControllerSelectDrawer : PropertyDrawer {
		private const float CAPTURE_BTN_OFFSET = 40F;
		private readonly float s_LineHeight = EditorGUIUtility.singleLineHeight;
		private readonly float s_SpaceV = EditorGUIUtility.standardVerticalSpacing;
		
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			// 显示控制器选择下拉控件
			Component target = (Component) property.serializedObject.targetObject;
			List<ProgressController> controllers = new List<ProgressController> { null };
			List<string> titles = new List<string> { "无" };
			Transform trans = target.transform;
			int depth = 0;
			while (trans != null) {
				ProgressController[] _controllers = trans.GetComponents<ProgressController>();
				Dictionary<string, int> titleCountDict = new Dictionary<string, int>();
				foreach (var _controller in _controllers) {
					string title = string.IsNullOrEmpty(_controller.title) ? "匿名 " + -depth : _controller.title + " " + -depth;
					if (titleCountDict.TryGetValue(title, out int count)) {
						titleCountDict[title] = count + 1;
						title = $"{title}({count})";
					} else {
						titleCountDict.Add(title, 1);
					}
					controllers.Add(_controller);
					titles.Add(title);
				}
				trans = trans.parent;
				depth++;
			}
			ProgressController controller = property.objectReferenceValue as ProgressController;
			position.height = s_LineHeight;
			int index = controllers.IndexOf(controller);
			int newIndex = EditorGUI.Popup(position, label.text, index, titles.ToArray());
			if (newIndex != index) {
				property.objectReferenceValue = controllers[newIndex];
			}

			if (newIndex > 0) {
				controller = controllers[newIndex];
				
				// 显示控制器进度选择滑动控件
				position.y += s_LineHeight + s_SpaceV;
				position.width -= CAPTURE_BTN_OFFSET;
				FieldInfo snapCountField = typeof(ProgressController).GetField("snapCount", BindingFlags.Instance | BindingFlags.NonPublic);
				int snapCount = snapCountField == null ? 0 : (int) snapCountField.GetValue(controller);
				float newProgress = EditorGUI.Slider(position, "     Progress", controller.Progress, 0, 1);
				if (snapCount > 0) {
					float length = 1F / snapCount;
					newProgress = Mathf.RoundToInt(newProgress / length) * length;
				}
				if (Mathf.Abs(newProgress - controller.Progress) > Mathf.Epsilon) {
					Undo.RecordObject(controller, "Progress.Change");
					controller.Progress = newProgress;
					CustomEditorGUI.RepaintScene();
				}
				
				// 显示控制器记录状态控件
				position.x += position.width;
				position.width = CAPTURE_BTN_OFFSET;
				if (GUI.Button(position, "记录")) {
					if (target is BaseProgressCtrl baseProgressCtrl) {
						Undo.RecordObject(baseProgressCtrl, "ProgressCapture");
						baseProgressCtrl.Capture(controller.Progress);
					}
				}
			}
		}
		
		public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
			ProgressController controller = property.objectReferenceValue as ProgressController;
			return controller ? s_LineHeight + s_SpaceV + s_LineHeight : s_LineHeight;
		}
	}

	[CustomPropertyDrawer(typeof(AnimatorParamSelectAttribute))]
	public class AnimatorParamSelectDrawer : PropertyDrawer {
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			Component target = (Component) property.serializedObject.targetObject;
			Animator anim = null;
			string animatorVarName = ((AnimatorParamSelectAttribute) attribute).AnimatorVarName;
			if (string.IsNullOrEmpty(animatorVarName)) {
				FieldInfo[] fis = target.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				Type animatorType = typeof(Animator);
				foreach (var fi in fis) {
					if (animatorType.IsAssignableFrom(fi.FieldType)) {
						anim = fi.GetValue(target) as Animator;
						break;
					}
				}
			} else {
				FieldInfo fi = target.GetType().GetField(animatorVarName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				anim = fi?.GetValue(target) as Animator;
			}
			// if (anim && anim.gameObject.activeInHierarchy && anim.runtimeAnimatorController) {
			if (anim && anim.gameObject.activeInHierarchy && anim.runtimeAnimatorController is AnimatorController controller) {
				// if (!anim.isInitialized) {
				// 	// 随便找个字段改一下
				// 	SerializedObject serializedObject = new SerializedObject(anim);
				// 	SerializedProperty serializedProperty = serializedObject.FindProperty("m_ApplyRootMotion");
				// 	bool willRevert = serializedProperty.isInstantiatedPrefab && !serializedProperty.prefabOverride;
				// 	serializedProperty.boolValue = !serializedProperty.boolValue;
				// 	serializedProperty.boolValue = !serializedProperty.boolValue;
				// 	if (willRevert && serializedProperty.prefabOverride) {
				// 		PrefabUtility.RevertPropertyOverride(serializedProperty, InteractionMode.AutomatedAction);
				// 	} else {
				// 		serializedObject.ApplyModifiedProperties();
				// 	}
				// }
				List<AnimatorControllerParameter> parameters = new List<AnimatorControllerParameter>(controller.parameters);
				AnimatorControllerParameterType type = ((AnimatorParamSelectAttribute) attribute).Type;
				if (type > 0) {
					parameters = parameters.FindAll(parameter => parameter.type == type);
				}
				if (parameters.Count > 0) {
					List<string> paramNames = parameters.ConvertAll(parameter => parameter.name);
					int index = paramNames.IndexOf(property.stringValue);
					int newIndex = EditorGUI.Popup(position, label.text, index, paramNames.ToArray());
					if (newIndex != index) {
						property.stringValue = paramNames[newIndex];
					}
					return;
				}
			}
			EditorGUI.BeginDisabledGroup(true);
			EditorGUI.Popup(position, label.text, 0, new []{property.stringValue});
			EditorGUI.EndDisabledGroup();
		}
	}

	[CustomPropertyDrawer(typeof(SelfAnimatorParamSelectAttribute))]
	public class SelfAnimatorParamSelectDrawer : PropertyDrawer {
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			Component target = (Component) property.serializedObject.targetObject;
			Animator anim = target.GetComponent<Animator>();
			// if (anim && anim.gameObject.activeInHierarchy && anim.runtimeAnimatorController) {
			if (anim && anim.gameObject.activeInHierarchy && anim.runtimeAnimatorController is AnimatorController controller) {
				// if (!anim.isInitialized) {
				// 	// 随便找个字段改一下
				// 	SerializedObject serializedObject = new SerializedObject(anim);
				// 	SerializedProperty serializedProperty = serializedObject.FindProperty("m_ApplyRootMotion");
				// 	bool willRevert = serializedProperty.isInstantiatedPrefab && !serializedProperty.prefabOverride;
				// 	serializedProperty.boolValue = !serializedProperty.boolValue;
				// 	serializedProperty.boolValue = !serializedProperty.boolValue;
				// 	if (willRevert && serializedProperty.prefabOverride) {
				// 		PrefabUtility.RevertPropertyOverride(serializedProperty, InteractionMode.AutomatedAction);
				// 	} else {
				// 		serializedObject.ApplyModifiedProperties();
				// 	}
				// }
				List<AnimatorControllerParameter> parameters = new List<AnimatorControllerParameter>(controller.parameters);
				AnimatorControllerParameterType type = ((SelfAnimatorParamSelectAttribute) attribute).Type;
				if (type > 0) {
					parameters = parameters.FindAll(parameter => parameter.type == type);
				}
				if (parameters.Count > 0) {
					List<string> paramNames = parameters.ConvertAll(parameter => parameter.name);
					int index = paramNames.IndexOf(property.stringValue);
					int newIndex = EditorGUI.Popup(position, label.text, index, paramNames.ToArray());
					if (newIndex != index) {
						property.stringValue = paramNames[newIndex];
					}
					return;
				}
			}
			EditorGUI.BeginDisabledGroup(true);
			EditorGUI.Popup(position, label.text, 0, new []{property.stringValue});
			EditorGUI.EndDisabledGroup();
		}
	}

	[CustomPropertyDrawer(typeof(ComponentSelectAttribute))]
	public class ComponentSelectDrawer : PropertyDrawer {
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			Type type = fieldInfo.FieldType;
			if (type.IsArray) {
				type = type.GetElementType();
			} else if (type.IsGenericType) {
				type = type.GetGenericArguments()[0];
			}
			if (!typeof(Component).IsAssignableFrom(type)) {
				// 修饰的是个非Component字段，无效。
				EditorGUI.PropertyField(position, property, label);
				return;
			}
			
			Component comp = property.objectReferenceValue as Component;
			if (comp) {
				Rect leftPosition = new Rect(position.x, position.y, (position.width + EditorGUIUtility.labelWidth) * 0.5F, position.height);
				Rect rightPosition = new Rect(leftPosition.x + leftPosition.width, position.y, position.width - leftPosition.width, position.height);
			
				EditorGUI.PropertyField(leftPosition, property, label);
				Component newComp = property.objectReferenceValue as Component;
				// 字段类型是Component或其子类，限制了拖进来的要么是Component要么是null，不存在非Component非null的情况。
				if (newComp) {
					List<Component> comps = new List<Component>(newComp.GetComponents(type));
					if (((ComponentSelectAttribute) attribute).ExceptSelf) {
						Component self = property.serializedObject.targetObject as Component;
						comps.Remove(self);
					}
					Type[] constraintTypes = ((ComponentSelectAttribute) attribute).ConstraintTypes;
					if (constraintTypes.Length > 0) {
						for (int i = comps.Count - 1; i >= 0; --i) {
							if (!Array.Exists(constraintTypes, t => t.IsInstanceOfType(comps[i]))) {
								comps.RemoveAt(i);
							}
						}
					}
					comps.Insert(0, null);
					int compCount = comps.Count;
					List<string> names = new List<string>(compCount) {"0.None"};
					for (int index = 1; index < compCount; ++index) {
						string name = index + "." + comps[index].GetType().Name;
						string customLabel = CustomEditorGUI.GetCustomLabel(comps[index]);
						if (customLabel != null) {
							name += " - " + customLabel;
						}
						names.Add(name);
					}
					int dataIndex = EditorGUI.Popup(rightPosition, comps.IndexOf(newComp), names.ToArray());
					if (dataIndex == -1) {
						newComp = (compCount > 1 ? comps[1] : comp);
					} else {
						newComp = comps[dataIndex];
					}
					if (newComp != property.objectReferenceValue) {
						property.objectReferenceValue = newComp;
					}
				}
			} else {
				const float buttonWidth = 60F;
				Rect leftPosition = new Rect(position.x, position.y, position.width - buttonWidth, position.height);
				Rect rightPosition = new Rect(leftPosition.x + leftPosition.width, position.y, buttonWidth, position.height);
			
				EditorGUI.PropertyField(leftPosition, property, label);
				
				Component target = (Component) property.serializedObject.targetObject;
				List<Component> comps = new List<Component>(target.GetComponents(type));
				if (((ComponentSelectAttribute) attribute).ExceptSelf) {
					comps.Remove((Component) property.serializedObject.targetObject);
				}
				Type[] constraintTypes = ((ComponentSelectAttribute) attribute).ConstraintTypes;
				int constraintTypeCount = constraintTypes.Length;
				if (constraintTypeCount > 0) {
					for (int i = comps.Count - 1; i >= 0; --i) {
						if (!Array.Exists(constraintTypes, t => t.IsInstanceOfType(comps[i]))) {
							if (property.objectReferenceValue == comps[i]) {
								property.objectReferenceValue = null;
							}
							comps.RemoveAt(i);
						}
					}
				}
				bool prevEnabled = GUI.enabled;
				GUI.enabled = prevEnabled && comps.Count > 0;
				if (GUI.Button(rightPosition, "ThisGo")) {
					property.objectReferenceValue = comps[0];
				}
				GUI.enabled = prevEnabled;
			}
		}
	}

	[CustomPropertyDrawer(typeof(ObjectSelectAttribute))]
	public class ObjectSelectDrawer : PropertyDrawer {
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			UObject obj = property.objectReferenceValue;
			if (obj is GameObject || obj is Component) {
				Rect leftPosition = position;
				leftPosition.width = (position.width + EditorGUIUtility.labelWidth) * 0.5F;
				Rect rightPosition = leftPosition;
				rightPosition.x += rightPosition.width;
				rightPosition.width = position.width - leftPosition.width;
			
				EditorGUI.PropertyField(leftPosition, property, label);

				List<Component> comps = null;
				switch (obj) {
					case GameObject go:
						comps = new List<Component>(go.GetComponents<Component>());
						break;
					case Component comp:
						comps = new List<Component>(comp.GetComponents<Component>());
						break;
				}
				if (((ObjectSelectAttribute) attribute).ExceptSelf) {
					Component self = property.serializedObject.targetObject as Component;
					comps.Remove(self);
					if (property.objectReferenceValue == self) {
						property.objectReferenceValue = obj;
					}
				}
				Type[] constraintTypes = ((ObjectSelectAttribute) attribute).ConstraintTypes;
				int constraintTypeCount = constraintTypes.Length;
				if (constraintTypeCount > 0) {
					for (int i = comps.Count - 1; i >= 0; --i) {
						bool isMatched = false;
						for (int j = 0; j < constraintTypeCount; ++j) {
							if (constraintTypes[j].IsInstanceOfType(comps[i])) {
								isMatched = true;
							}
						}
						if (!isMatched) {
							if (property.objectReferenceValue == comps[i]) {
								property.objectReferenceValue = obj;
							}
							comps.RemoveAt(i);
						}
					}
				}
				int compCount = comps.Count;
				List<UObject> objs = new List<UObject>(compCount + 2) { null, comps[0].gameObject };
				objs.AddRange(comps);
				string[] compNames = new string[compCount + 2];
				int[] compIndexes = new int[compCount + 2];
				compNames[0] = "0.None";
				compIndexes[0] = 0;
				compNames[1] = "1.GameObject";
				compIndexes[1] = 1;
				for (int index = 0; index < compCount; ++index) {
					string customLabel = CustomEditorGUI.GetCustomLabel(comps[index]);
					string name = index + 2 + "." + comps[index].GetType().Name;
					if (customLabel != null) {
						name += " - " + customLabel;
					}
					compNames[index + 2] = name;
					compIndexes[index + 2] = index + 2;
				}
				int currentIndex = objs.IndexOf(obj);
				int dataIndex = EditorGUI.IntPopup(rightPosition, currentIndex, compNames, compIndexes);
				if (dataIndex != currentIndex) {
					property.objectReferenceValue = objs[dataIndex];
				}
			} else {
				EditorGUI.PropertyField(position, property, label);
			}
		}
	}

	[CustomPropertyDrawer(typeof(CanResetCurveAttribute))]
	public class CanResetCurveDrawer : PropertyDrawer {
		private const float RESET_BTN_WIDTH = 50F;
		private const float FOLD_BTN_WIDTH = 22F;
		private const float FOLD_BTN_OFFSET = 38F;
		private readonly float s_LineHeight = EditorGUIUtility.singleLineHeight;
		private readonly float s_SpaceV = EditorGUIUtility.standardVerticalSpacing;
		private readonly float s_SpaceH = 2F;
		
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			position.height = base.GetPropertyHeight(property, label);
			
			Rect curvePosition = position;
			curvePosition.width -= RESET_BTN_WIDTH;
			EditorGUI.PropertyField(curvePosition, property, label);
			AnimationCurve curve = property.animationCurveValue;
			Keyframe[] curveKeys = curve.keys;
			int curveLength = curveKeys.Length;
			Rect resetBtnPosition = position;
			resetBtnPosition.x += curvePosition.width;
			resetBtnPosition.width = RESET_BTN_WIDTH;
			if (GUI.Button(resetBtnPosition, "Reset")) {
				if (curveLength != 1 || curveKeys[0].time != 0 || curveKeys[0].value != 0) {
					curve = new AnimationCurve(new Keyframe(0, 0));
					property.animationCurveValue = curve;
					curveKeys = curve.keys;
					curveLength = 1;
				}
			}
			Rect foldBtnPosition = position;
			foldBtnPosition.x += FOLD_BTN_OFFSET;
			foldBtnPosition.width = FOLD_BTN_WIDTH;
			bool isFolded = EditorPrefs.GetBool("CanResetCurve.IsFolded");
			if (GUI.Button(foldBtnPosition, isFolded ? "\u25C4" : "\u25BC")) {
				isFolded = !isFolded;
				EditorPrefs.SetBool("CanResetCurve.IsFolded", isFolded);
			}

			if (!isFolded && !GUI.changed) {
				Rect indexPosition = position;
				indexPosition.height = s_LineHeight;
				indexPosition.x += FOLD_BTN_OFFSET;
				indexPosition.width = EditorGUIUtility.labelWidth - FOLD_BTN_OFFSET;
				Rect value1Position = indexPosition;
				value1Position.x += indexPosition.width + s_SpaceH;
				value1Position.width = (position.x + position.width - value1Position.x - s_SpaceH) * 0.5F;
				Rect value2Position = value1Position;
				value2Position.x += value1Position.width + s_SpaceH;
				
				EditorGUIUtility.labelWidth = 40F;
				for (int i = 0; i < curveLength; ++i) {
					indexPosition.y += s_LineHeight + s_SpaceV;
					value1Position.y += s_LineHeight + s_SpaceV;
					value2Position.y += s_LineHeight + s_SpaceV;
					EditorGUI.LabelField(indexPosition, string.Empty + i);
					curveKeys[i].time = EditorGUI.FloatField(value1Position, "Time", curveKeys[i].time);
					curveKeys[i].value = EditorGUI.FloatField(value2Position, "Value", curveKeys[i].value);
					
					indexPosition.y += s_LineHeight + s_SpaceV;
					value1Position.y += s_LineHeight + s_SpaceV;
					value2Position.y += s_LineHeight + s_SpaceV;
					curveKeys[i].inTangent = EditorGUI.FloatField(value1Position, "In", curveKeys[i].inTangent);
					curveKeys[i].outTangent = EditorGUI.FloatField(value2Position, "Out", curveKeys[i].outTangent);
				}
				if (GUI.changed) {
					curve = new AnimationCurve(curveKeys);
					property.animationCurveValue = curve;
				}
			}
		}
		
		public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
			float baseHeight = base.GetPropertyHeight(property, label);
			int lineCount = 0;
			bool isFolded = EditorPrefs.GetBool("CanResetCurve.IsFolded");
			if (!isFolded) {
				AnimationCurve curve = property.animationCurveValue;
				Keyframe[] curveKeys = curve.keys;
				lineCount += curveKeys.Length + curveKeys.Length;
			}
			return lineCount * (s_SpaceV + s_LineHeight) + baseHeight;
		}
	}

	[CustomPropertyDrawer(typeof(TailSpaceAttribute))]
	public class TailSpaceDrawer : PropertyDrawer {
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			EditorGUI.PropertyField(position, property, label);
			GUILayout.Space(((TailSpaceAttribute) attribute).Space);
		}
	}

	[CustomPropertyDrawer(typeof(LimitedEditableAttribute))]
	public class LimitedEditableDrawer : PropertyDrawer {
		private const float BUTTON_WIDTH = 32F;
		private readonly float s_LineHeight = EditorGUIUtility.singleLineHeight;
		private readonly float s_SpaceV = EditorGUIUtility.standardVerticalSpacing;
		private bool m_Editable;
		
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			if (!m_Editable) {
				Rect buttonPosition = position;
				buttonPosition.x += EditorGUIUtility.labelWidth - BUTTON_WIDTH;
				buttonPosition.width = BUTTON_WIDTH;
				buttonPosition.height = s_LineHeight;
				if (GUI.Button(buttonPosition, "Edit")) {
					m_Editable = true;
				}
			}
			bool prevEnabled = GUI.enabled;
			GUI.enabled = m_Editable;
			EditorGUI.PropertyField(position, property, label, true);
			GUI.enabled = prevEnabled;
		}
		
		public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
			float baseHeight = base.GetPropertyHeight(property, label);
			int lineCount = property.CountInProperty();
			return (lineCount - 1) * (s_SpaceV + s_LineHeight) + baseHeight;
		}
	}

	[CustomPropertyDrawer(typeof(AssetPathAttribute))]
	public class AssetPathDrawer : PropertyDrawer {
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			string path = property.stringValue;
			UObject obj = AssetDatabase.LoadAssetAtPath<UObject>(path);
			AssetPathAttribute assetPathAttribute = (AssetPathAttribute) attribute;
			UObject newObj = EditorGUI.ObjectField(position, label, obj, assetPathAttribute.mainType ?? typeof(UObject));
			if (newObj
				&& assetPathAttribute.constraintTypes != null
				&& Array.TrueForAll(assetPathAttribute.constraintTypes, type => !type.IsInstanceOfType(newObj))
			) {
				newObj = obj;
			}
			if (newObj != obj) {
				string newPath = AssetDatabase.GetAssetPath(newObj);
				property.stringValue = newPath;
			}
		}
	}
}