/*
 * @Author: wangyun
 * @CreateTime: 2022-12-09 14:16:58 253
 * @LastEditor: wangyun
 * @EditTime: 2023-03-05 16:55:13 023
 */

using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using DG.Tweening;
using Sirenix.Utilities;
using Sirenix.OdinInspector.Editor;

using UObject = UnityEngine.Object;

namespace Control {
	public partial class ProcessStepDrawerBase<TStep> : OdinValueDrawer<TStep> where TStep : ProcessStepBase {
		// ReSharper disable once StaticMemberInGenericType
		protected static float s_ContextWidth;

		protected TStep Target => ValueEntry.SmartValue;

		protected override void DrawPropertyLayout(GUIContent _) {
			s_ContextWidth = CustomEditorGUI.GetContextWidth();
			
			EditorGUI.BeginChangeCheck();
			
			CustomEditorGUI.BeginLabelWidth(60F);
			DrawTitle();
			DrawTime();
			DrawType();
			DrawFeature();
			CustomEditorGUI.EndLabelWidth();
			
			if (EditorGUI.EndChangeCheck()) {
				SetDirty();
			}
		}

		protected virtual void DrawTitle() {
			string newTitle = EditorGUILayout.TextField("描述", Target.title);
			if (newTitle != Target.title) {
				Property.RecordForUndo("Title");
				Target.title = newTitle;
			}
		}

		protected virtual void DrawTime() {
			EditorGUILayout.BeginHorizontal();
			float newTime = EditorGUILayout.FloatField("时间", Target.time);
			if (Mathf.Abs(newTime - Target.time) > Mathf.Epsilon) {
				Property.RecordForUndo("Time");
				Target.time = newTime;
			}
				
			int delayFramesInt = Mathf.FloorToInt(Target.delayFrames);
			bool waitEndOfFrame = Target.delayFrames > delayFramesInt;
				
			float prevLabelWidth = EditorGUIUtility.labelWidth;
			EditorGUIUtility.labelWidth = 26F;
			GUILayoutOption widthIntField = GUILayout.Width(s_ContextWidth * 0.3F + 2F - 3F - 16F + 5F - 3F - 33F);
			int newDelayFrames = Mathf.Max(EditorGUILayout.IntField("延迟", delayFramesInt, widthIntField), 0);
			if (newDelayFrames != delayFramesInt) {
				Property.RecordForUndo("DelayFrames");
				Target.delayFrames += newDelayFrames - delayFramesInt;
				delayFramesInt = newDelayFrames;
			}
			EditorGUIUtility.labelWidth = prevLabelWidth;
				
			GUILayout.Space(-2F);
				
			EditorGUILayout.LabelField("帧", GUILayout.Width(16F));
				
			GUILayout.Space(-5F);

			if (DrawToggle(waitEndOfFrame, "末尾", GUILayout.Width(33F)) != waitEndOfFrame) {
				Property.RecordForUndo("DelayFrames");
				Target.delayFrames = waitEndOfFrame ? delayFramesInt : delayFramesInt + 0.5F;
			}
			EditorGUILayout.EndHorizontal();
		}

		protected virtual void DrawType() {
			EditorGUILayout.BeginHorizontal();
			Color oldColor = GUI.backgroundColor;
			Color newColor = Color.red * 1F + Color.green * 0.4F;
			newColor.a = 0.2F;
			GUI.backgroundColor = newColor;
			(List<string> names, List<int> values) = GetTypeOptions();
			ProcessStepTypeBase baseType = (ProcessStepTypeBase) EditorGUILayout.IntPopup("类型", (int) Target.Type, names.ToArray(), values.ToArray());
			GUI.backgroundColor = oldColor;
			if (baseType != Target.Type) {
				Property.RecordForUndo("Type");
				Target.Type = baseType;
			}
				
			if (GUILayout.Button("执行", GUILayout.Width(s_ContextWidth * 0.18F))) {
				if (Target.obj) {
					Undo.RecordObject(Target.obj, "Execute");
				}
				foreach (var objArgument in Target.objArguments) {
					if (objArgument) {
						Undo.RecordObject(objArgument, "Execute");
					}
				}
				Target.DoStep(GetTargetExecutor());
			}
			PropertyInfo isExecutedPI = typeof(ProcessStepBase).GetProperty("IsExecuted", BindingFlags.Instance | BindingFlags.NonPublic);
			bool isExecuted = isExecutedPI?.GetValue(Target) is true;
			GUILayoutOption widthToggle = GUILayout.Width(s_ContextWidth * 0.12F - 3F);
			if (DrawToggle(isExecuted, isExecuted ? "●" : "○", widthToggle) != isExecuted) {
				isExecutedPI?.SetValue(Target, false);
			}
			EditorGUILayout.EndHorizontal();
		}
		protected virtual (List<string>, List<int>) GetTypeOptions() {
			Array allTypes = Enum.GetValues(typeof(ProcessStepTypeBase));
			List<int> values = allTypes.Convert(_type => (int) (ProcessStepTypeBase) _type).ToList();
			List<string> names = allTypes.Convert(_type => 
					s_TypeNameDict.TryGetValue((ProcessStepTypeBase) _type, out string _name) ? _name : _type.ToString()).ToList();
			for (int i = 1, length = allTypes.Length; i < length; ++i) {
				if (values[i - 1] / 10 != values[i] / 10) {
					values.Insert(i, -1);
					names.Insert(i, string.Empty);
					++i;
					++length;
				}
			}
			return (names, values);
		}
		
		protected virtual void DrawFeature() {
			switch (Target.Type) {
				case ProcessStepTypeBase.EXECUTOR:
					DrawExecutor();
					break;
				case ProcessStepTypeBase.STATE_CONTROLLER:
					DrawStateController();
					break;
				case ProcessStepTypeBase.PROGRESS_CONTROLLER:
					DrawProgressController();
					break;
				case ProcessStepTypeBase.CUSTOM_EVENT:
					DrawCustomEvent();
					break;
				
				case ProcessStepTypeBase.INSTANTIATE:
					DrawInstantiate();
					break;
				case ProcessStepTypeBase.DESTROY:
					DrawDestroy();
					break;
				case ProcessStepTypeBase.PARENT:
					DrawParent();
					break;
				case ProcessStepTypeBase.ACTIVE:
					DrawActive();
					break;
				case ProcessStepTypeBase.ENABLED:
					DrawEnabled();
					break;
				
				case ProcessStepTypeBase.TRANSFORM:
					DrawTransform();
					break;
				case ProcessStepTypeBase.LOOK_AT:
					DrawLookAt();
					break;
				case ProcessStepTypeBase.CAMERA_ANCHOR:
					DrawCameraAnchor();
					break;
				
				case ProcessStepTypeBase.AUDIO_ONE_SHOT:
					DrawAudioOneShot();
					break;
				case ProcessStepTypeBase.AUDIO_SOURCE_CTRL:
					DrawAudioSourceCtrl();
					break;
				case ProcessStepTypeBase.AUDIOS_PLAY:
					DrawAudiosPlay();
					break;
				
				case ProcessStepTypeBase.ANIMATOR_PARAMETERS:
					DrawAnimatorParameters();
					break;
				case ProcessStepTypeBase.ANIMATOR_CONTROLLER:
					DrawAnimatorController();
					break;
				case ProcessStepTypeBase.ANIMATOR_AVATAR:
					DrawAnimatorAvatar();
					break;
				case ProcessStepTypeBase.ANIMATOR_APPLY_ROOT_MOTION:
					DrawAnimatorApplyRootMotion();
					break;
				
				case ProcessStepTypeBase.PLAYABLE_CTRL:
					DrawPlayableCtrl();
					break;
				case ProcessStepTypeBase.PLAYABLE_GOTO:
					DrawPlayableGoto();
					break;
				
				case ProcessStepTypeBase.DO_TWEEN_RESTART:
					DrawDOTweenRestart();
					break;
				case ProcessStepTypeBase.DO_TWEEN_CTRL:
					DrawDOTweenCtrl();
					break;
				case ProcessStepTypeBase.DO_TWEEN_GOTO:
					DrawDOTweenGoto();
					break;
				case ProcessStepTypeBase.DO_TWEEN_LIFE:
					DrawDOTweenLife();
					break;
				
#if SPINE_EXIST
				case ProcessStepTypeBase.SPINE_ANIMATION_SET_VALUE:
					DrawSpineSetValue();
					break;
				case ProcessStepTypeBase.SPINE_ANIMATION_RESTART:
					DrawSpineRestart();
					break;
				case ProcessStepTypeBase.SPINE_ANIMATION_STOP:
					DrawSpineStop();
					break;
#endif
				
#if LUA_BEHAVIOUR_EXIST
				case ProcessStepTypeBase.LUA_CODE_EXECUTE:
					DrawLuaCodeExecute();
					break;
				case ProcessStepTypeBase.LUA_SET_VALUE:
					DrawLuaSetValue();
					break;
				case ProcessStepTypeBase.LUA_FUNCTION_INVOKE:
					DrawLuaFunctionInvoke();
					break;
#endif
				
				case ProcessStepTypeBase.UNITY_EVENT:
					DrawUnityEvent();
					break;
			}
		}

		protected void DrawUnityEvent() {
			int valueCount = Property.SerializationRoot.ValueEntry.ValueCount;
			ExecutorProcessBase<ProcessStepBase> executor = null;
			for (int i = 0; i < valueCount; i++) {
				executor = Property.SerializationRoot.ValueEntry.WeakValues[i] as ExecutorProcessBase<ProcessStepBase>;
				if (executor != null) {
					break;
				}
			}
			if (executor != null) {
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("事件", CustomEditorGUI.LabelWidthOption);
				
				int index = executor.steps.IndexOf(Target);
				if (index != -1) {
					SerializedObject serializedObject = new SerializedObject(executor);
					SerializedProperty serializedProperty = serializedObject.FindProperty("steps")
							.GetArrayElementAtIndex(index)
							.FindPropertyRelative("unityEvent");
					EditorGUILayout.PropertyField(serializedProperty);
					serializedObject.ApplyModifiedProperties();
				}
				EditorGUILayout.EndHorizontal();
			}
		}

		protected void DrawTween() {
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("缓动", CustomEditorGUI.LabelWidthOption);
			bool newTween = DrawToggle(Target.tween, Target.tween ? "√" : "X", BTN_WIDTH_OPTION);
			if (newTween != Target.tween) {
				Property.RecordForUndo("Tween");
				Target.tween = newTween;
			}
			EditorGUILayout.EndHorizontal();

			if (newTween) {
				float newDelay = EditorGUILayout.FloatField("延迟", Target.tweenDelay);
				if (!Mathf.Approximately(newDelay, Target.tweenDelay)) {
					Property.RecordForUndo("TweenDelay");
					Target.tweenDelay = newDelay;
				}
			
				float newDuration = EditorGUILayout.FloatField("时长", Target.tweenDuration);
				if (!Mathf.Approximately(newDuration, Target.tweenDuration)) {
					Property.RecordForUndo("TweenDuration");
					Target.tweenDuration = newDuration;
				}
			
				Ease newEase = (Ease) EditorGUILayout.EnumPopup("函数", Target.tweenEase);
				if (newEase != Target.tweenEase) {
					Property.RecordForUndo("TweenEase");
					Target.tweenEase = newEase;
				}

				if (newEase == Ease.INTERNAL_Custom) {
					AnimationCurve newCurve = EditorGUILayout.CurveField("曲线", Target.tweenEaseCurve);
					if (!newCurve.Equals(Target.tweenEaseCurve)) {
						Property.RecordForUndo("TweenEaseCurve");
						Target.tweenEaseCurve = newCurve;
					}
				}
			}
		}

		protected T DrawObjectFieldWithThisBtn<T>(string text, UObject obj) where T : UObject {
			EditorGUILayout.BeginHorizontal();
			T newObj = DrawObjectField<T>(text, obj);
			GameObject go = GetTargetExecutor()?.gameObject;
			if (go) {
				if (typeof(T) == typeof(GameObject)) {
					CustomEditorGUI.BeginDisabled(newObj == go);
					if (GUILayout.Button("ThisGo", GUILayout.Width(s_ContextWidth * 0.12F - 3F))) {
						newObj = go as T;
					}
					CustomEditorGUI.EndDisabled();
				} else if (typeof(Component).IsAssignableFrom(typeof(T))) {
					T comp = go.GetComponent<T>();
					CustomEditorGUI.BeginDisabled(!comp || comp == newObj);
					if (GUILayout.Button("ThisGo", GUILayout.Width(s_ContextWidth * 0.12F - 3F))) {
						newObj = comp;
					}
					CustomEditorGUI.EndDisabled();
				}
			}
			EditorGUILayout.EndHorizontal();
			return newObj;
		}

		protected T DrawCompFieldWithThisBtn<T>(string text, UObject obj, params Type[] types) where T : Component {
			EditorGUILayout.BeginHorizontal();
			T newObj = DrawCompField<T>(text, obj, types);
			if (!newObj) {
				GameObject go = GetTargetExecutor()?.gameObject;
				if (go) {
					T comp = go.GetComponent<T>();
					CustomEditorGUI.BeginDisabled(!comp);
					if (GUILayout.Button("ThisGo", GUILayout.Width(s_ContextWidth * 0.12F - 3F))) {
						newObj = comp;
					}
					CustomEditorGUI.EndDisabled();
				}
			}
			EditorGUILayout.EndHorizontal();
			return newObj;
		}

		protected void SetDirty() {
			BaseExecutor executor = GetTargetExecutor();
			if (executor != null) {
				EditorUtility.SetDirty(executor);
			}
		}

		protected BaseExecutor GetTargetExecutor() {
			BaseExecutor executor = null;
			IPropertyValueEntry valueEntry = Property.SerializationRoot.ValueEntry;
			for (int i = 0, valueCount = valueEntry.ValueCount; i < valueCount; i++) {
				executor = valueEntry.WeakValues[i] as BaseExecutor;
				if (executor != null) {
					break;
				}
			}
			return executor;
		}
		
		protected static int DrawEnumButtons(string text, int value, string[] displayedOptions, int[] optionValues, bool isMask = false, params GUILayoutOption[] options) {
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(text, CustomEditorGUI.LabelWidthOption);
			for (int i = 0, nameLength = displayedOptions.Length, valueLength = optionValues.Length; i < valueLength; ++i) {
				if (isMask) {
					bool selected = (value & optionValues[i]) != 0;
					if (DrawToggle(selected, i < nameLength ? displayedOptions[i] : string.Empty, options) != selected) {
						if (selected) {
							value ^= optionValues[i];
						} else {
							value |= optionValues[i];
						}
					}
				} else {
					if (DrawToggle(value == optionValues[i], i < nameLength ? displayedOptions[i] : string.Empty, options)) {
						value = optionValues[i];
					}
				}
			}
			EditorGUILayout.EndHorizontal();
			return value;
		}

		// ReSharper disable once StaticMemberInGenericType
		protected static readonly GUILayoutOption BTN_WIDTH_OPTION = GUILayout.Width(80F);
		
		protected static bool DrawToggle(bool value, string text, params GUILayoutOption[] options) {
			return CustomEditorGUI.Toggle(value, text, CustomEditorGUI.COLOR_TOGGLE_CHECKED, options);
		}

		protected static T DrawObjectField<T>(string text, UObject obj) where T : UObject {
			T newObj = string.IsNullOrEmpty(text) ?
					EditorGUILayout.ObjectField(obj, typeof(T), true) as T :
					EditorGUILayout.ObjectField(text, obj, typeof(T), true) as T;
			return newObj;
		}
		protected static T DrawCompField<T>(string text, UObject obj, params Type[] types) where T : Component {
			EditorGUILayout.BeginHorizontal();
			Type _returnType = typeof(T);
			T newObj = string.IsNullOrEmpty(text) ?
					EditorGUILayout.ObjectField(obj, _returnType, true) as T :
					EditorGUILayout.ObjectField(text, obj, _returnType, true) as T;
			
			// ObjectField传入的Type限制了拖进来的要么是T要么是null，不存在非T非null的情况。
			if (newObj) {
				List<Component> comps = new List<Component>(newObj.GetComponents(_returnType));
				if (types.Length > 0) {
					comps = comps.FindAll(_comp => Array.Exists(types, _type => _type.IsInstanceOfType(_comp)));
				}
				comps.Insert(0, null);
				int compCount = comps.Count;
				List<string> names = new List<string>(compCount) {"0.无"};
				for (int index = 1; index < compCount; ++index) {
					string name = index + "." + comps[index].GetType().Name;
					string customLabel = CustomEditorGUI.GetCustomLabel(comps[index]);
					if (customLabel != null) {
						name += " - " + customLabel;
					}
					names.Add(name);
				}
				int dataIndex = EditorGUILayout.Popup(comps.IndexOf(newObj), names.ToArray(), GUILayout.Width(s_ContextWidth * 0.3F));
				if (dataIndex == -1) {
					newObj = (compCount > 1 ? comps[1] : obj) as T;
				} else {
					newObj = comps[dataIndex] as T;
				}
			}
			EditorGUILayout.EndHorizontal();
			return newObj;
		}
		
		protected static T DrawObjectField<T>(Rect rect, string text, UObject obj) where T : UObject {
			T newObj = string.IsNullOrEmpty(text) ?
					EditorGUI.ObjectField(rect, obj, typeof(T), true) as T :
					EditorGUI.ObjectField(rect, text, obj, typeof(T), true) as T;
			return newObj;
		}
		protected static T DrawCompField<T>(Rect rect, string text, UObject obj, params Type[] types) where T : Component {
			Type _returnType = typeof(T);
			if (obj) {
				Rect leftRect = new Rect(rect.x, rect.y, rect.width * 0.6F, rect.height);
				Rect rightRect = new Rect(leftRect.x + leftRect.width, rect.y, rect.width - leftRect.width, rect.height);
				
				T newObj = string.IsNullOrEmpty(text) ?
						EditorGUI.ObjectField(leftRect, obj, _returnType, true) as T :
						EditorGUI.ObjectField(leftRect, text, obj, _returnType, true) as T;
				
				// ObjectField传入的Type限制了拖进来的要么是T要么是null，不存在非T非null的情况。
				if (newObj) {
					List<Component> comps = new List<Component>(newObj.GetComponents(_returnType));
					if (types.Length > 0) {
						comps = comps.FindAll(_comp => Array.Exists(types, _type => _type.IsInstanceOfType(_comp)));
					}
					comps.Insert(0, null);
					int compCount = comps.Count;
					List<string> names = new List<string>(compCount) {"0.无"};
					for (int index = 1; index < compCount; ++index) {
						string name = index + "." + comps[index].GetType().Name;
						string customLabel = CustomEditorGUI.GetCustomLabel(comps[index]);
						if (customLabel != null) {
							name += " - " + customLabel;
						}
						names.Add(name);
					}
					int dataIndex = EditorGUI.Popup(rightRect, comps.IndexOf(newObj), names.ToArray());
					if (dataIndex == -1) {
						newObj = (compCount > 1 ? comps[1] : obj) as T;
					} else {
						newObj = comps[dataIndex] as T;
					}
				}
				return newObj;
			} else {
				T newObj = string.IsNullOrEmpty(text) ?
						EditorGUI.ObjectField(rect, obj, _returnType, true) as T :
						EditorGUI.ObjectField(rect, text, obj, _returnType, true) as T;
				return newObj;
			}
		}
		
		// ReSharper disable once StaticMemberInGenericType
		protected static readonly Dictionary<ProcessStepTypeBase, string> s_TypeNameDict = new Dictionary<ProcessStepTypeBase, string> {
			{ProcessStepTypeBase.EXECUTOR, "执行"},
			{ProcessStepTypeBase.STATE_CONTROLLER, "状态控制"},
			{ProcessStepTypeBase.PROGRESS_CONTROLLER, "进度控制"},
			
			{ProcessStepTypeBase.INSTANTIATE, "组织 - 实例化"},
			{ProcessStepTypeBase.DESTROY, "组织 - 销毁"},
			{ProcessStepTypeBase.PARENT, "组织 - 换父节点"},
			
			{ProcessStepTypeBase.ACTIVE, "开关 - 活跃状态"},
			{ProcessStepTypeBase.ENABLED, "开关 - 启用状态"},
			
			{ProcessStepTypeBase.TRANSFORM, "变换 - 变换"},
			{ProcessStepTypeBase.LOOK_AT, "变换 - 朝向"},
			{ProcessStepTypeBase.CAMERA_ANCHOR, "变换 - 相对摄像机"},
			
			{ProcessStepTypeBase.AUDIO_ONE_SHOT, "音频 - 单次播放"},
			{ProcessStepTypeBase.AUDIO_SOURCE_CTRL, "音频 - 播放控制"},
			{ProcessStepTypeBase.AUDIOS_PLAY, "音频 - 播放多个"},
			
			{ProcessStepTypeBase.ANIMATOR_PARAMETERS, "动画 - 参数控制"},
			{ProcessStepTypeBase.ANIMATOR_CONTROLLER, "动画 - 换状态机"},
			{ProcessStepTypeBase.ANIMATOR_AVATAR, "动画 - 换形象"},
			{ProcessStepTypeBase.ANIMATOR_APPLY_ROOT_MOTION, "动画 - 根移动"},
			
			{ProcessStepTypeBase.PLAYABLE_CTRL, "时间轴 - 播放控制"},
			{ProcessStepTypeBase.PLAYABLE_GOTO, "时间轴 - 跳转进度"},
			
			{ProcessStepTypeBase.DO_TWEEN_RESTART, "缓动 - 从头开始"},
			{ProcessStepTypeBase.DO_TWEEN_CTRL, "缓动 - 播放控制"},
			{ProcessStepTypeBase.DO_TWEEN_GOTO, "缓动 - 跳转进度"},
			{ProcessStepTypeBase.DO_TWEEN_LIFE, "缓动 - 杀死或重生"},
			
#if SPINE_EXIST
			{ProcessStepTypeBase.SPINE_ANIMATION_SET_VALUE, "骨骼动画 - 设置值"},
			{ProcessStepTypeBase.SPINE_ANIMATION_RESTART, "骨骼动画 - 从头开始"},
			{ProcessStepTypeBase.SPINE_ANIMATION_STOP, "骨骼动画 - 结束"},
#endif
			
#if LUA_BEHAVIOUR_EXIST
			{ProcessStepTypeBase.LUA_CODE_EXECUTE, "Lua - 代码执行"},
			{ProcessStepTypeBase.LUA_SET_VALUE, "Lua - 赋值"},
			{ProcessStepTypeBase.LUA_FUNCTION_INVOKE, "Lua - 函数调用"},
#endif
			
			{ProcessStepTypeBase.CUSTOM_EVENT, "自定义事件"},
			{ProcessStepTypeBase.UNITY_EVENT, "Unity事件"},
		};
	}
}