// /*
//  * @Author: wangyun
//  * @CreateTime: 2022-07-03 14:29:49 572
//  * @LastEditor: wangyun
//  * @EditTime: 2022-07-27 18:07:04 604
//  */
//
// #if UNITY_EDITOR
// using System;
// using System.Linq;
// using System.Reflection;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.Playables;
// using UnityEditor;
// using UnityEditor.Animations;
// using DG.Tweening;
// using DG.Tweening.Core;
// using Sirenix.Utilities;
// using Sirenix.OdinInspector;
// using Sirenix.OdinInspector.Editor;
// using Sirenix.Utilities.Editor;
//
// using UObject = UnityEngine.Object;
//
// namespace Control {
// 	public partial class ProcessStep {
// 		private static float s_ContextWidth;
//
// 		[OnInspectorGUI]
// 		private void OnInspectorGUI(InspectorProperty property) {
// 			EditorGUIUtility.labelWidth = 60F;
// 			s_ContextWidth = GetContextWidth();
// 			
// 			EditorGUI.BeginChangeCheck();
// 			
// 			string newTitle = EditorGUILayout.TextField("描述", title);
// 			if (newTitle != title) {
// 				property.RecordForUndo("Title");
// 				title = newTitle;
// 			}
// 			
// 			EditorGUILayout.BeginHorizontal();
// 				float newTime = EditorGUILayout.FloatField("时间", time);
// 				if (Mathf.Abs(newTime - time) > Mathf.Epsilon) {
// 					property.RecordForUndo("Time");
// 					time = newTime;
// 				}
// 				
// 				int delayFramesInt = Mathf.FloorToInt(delayFrames);
// 				bool waitEndOfFrame = delayFrames > delayFramesInt;
// 				
// 				float prevLabelWidth = EditorGUIUtility.labelWidth;
// 				EditorGUIUtility.labelWidth = 26F;
// 				GUILayoutOption widthIntField = GUILayout.Width(s_ContextWidth * 0.3F + 2F - 3F - 16F + 5F - 3F - 33F);
// 				int newDelayFrames = Mathf.Max(EditorGUILayout.IntField("延迟", delayFramesInt, widthIntField), 0);
// 				if (newDelayFrames != delayFramesInt) {
// 					property.RecordForUndo("DelayFrames");
// 					delayFrames += newDelayFrames - delayFramesInt;
// 					delayFramesInt = newDelayFrames;
// 				}
// 				EditorGUIUtility.labelWidth = prevLabelWidth;
// 				
// 				GUILayout.Space(-2F);
// 				
// 				EditorGUILayout.LabelField("帧", GUILayout.Width(16F));
// 				
// 				GUILayout.Space(-5F);
//
// 				if (DrawToggle(waitEndOfFrame, "末尾", GUILayout.Width(33F)) != waitEndOfFrame) {
// 					property.RecordForUndo("DelayFrames");
// 					delayFrames = waitEndOfFrame ? delayFramesInt : delayFramesInt + 0.5F;
// 				}
// 			EditorGUILayout.EndHorizontal();
// 			
// 			EditorGUILayout.BeginHorizontal();
// 				Color oldColor = GUI.backgroundColor;
// 				Color newColor = Color.red * 1F + Color.green * 0.4F;
// 				newColor.a = 0.2F;
// 				GUI.backgroundColor = newColor;
// 				// ProcessStepType newType = (ProcessStepType) EditorGUILayout.EnumPopup("类型", type);
// 				Array allTypes = Enum.GetValues(typeof(ProcessStepType));
// 				int[] values = allTypes.Convert(_type => (int) (ProcessStepType) _type).ToArray();
// 				string[] names = allTypes.Convert(_type => 
// 						s_TypeNameDict.TryGetValue((ProcessStepType) _type, out string _name) ? _name : _type.ToString()).ToArray();
// 				ProcessStepType newType = (ProcessStepType) EditorGUILayout.IntPopup("类型", (int) type, names, values);
// 				GUI.backgroundColor = oldColor;
// 				if (newType != type) {
// 					property.RecordForUndo("Type");
// 					type = newType;
// 					ResetByType();
// 				}
// 				
// 				if (GUILayout.Button("触发", GUILayout.Width(s_ContextWidth * 0.18F))) {
// 					int valueCount = property.SerializationRoot.ValueEntry.ValueCount;
// 					TriggerCtrlProcess trigger = null;
// 					for (int i = 0; i < valueCount; i++) {
// 						trigger = property.SerializationRoot.ValueEntry.WeakValues[i] as TriggerCtrlProcess;
// 						if (trigger != null) {
// 							break;
// 						}
// 					}
// 					if (trigger != null) {
// 						const BindingFlags FLAGS = BindingFlags.Instance | BindingFlags.NonPublic;
// 						MethodInfo mi = typeof(TriggerCtrlProcess).GetMethod("DoStep", FLAGS);
// 						mi?.Invoke(trigger, new object[] { this });
// 					}
// 				}
// 				GUILayoutOption widthToggle = GUILayout.Width(s_ContextWidth * 0.12F - 3F);
// 				if (DrawToggle(IsTriggered, IsTriggered ? "●" : "○", widthToggle) != IsTriggered) {
// 					IsTriggered = false;
// 				}
// 			EditorGUILayout.EndHorizontal();
// 			
// 			switch (type) {
// 				case ProcessStepType.TRIGGER:
// 					DrawTrigger(property);
// 					break;
// 				case ProcessStepType.STATE_CONTROLLER:
// 					DrawStateController(property);
// 					break;
// 				case ProcessStepType.PROGRESS_CONTROLLER:
// 					DrawProgressController(property);
// 					break;
// 				case ProcessStepType.DO_TWEEN_RESTART:
// 					DrawDOTweenRestart(property);
// 					break;
// 				case ProcessStepType.DO_TWEEN_GOTO:
// 					DrawDOTweenGoto(property);
// 					break;
// 				case ProcessStepType.DO_TWEEN_PAUSE:
// 					DrawDOTweenPause(property);
// 					break;
// 				case ProcessStepType.DO_TWEEN_RESUME:
// 					DrawDOTweenResume(property);
// 					break;
// 				case ProcessStepType.DO_TWEEN_KILL:
// 					DrawDOTweenKill(property);
// 					break;
// 				case ProcessStepType.ANIMATOR:
// 					DrawAnimator(property);
// 					break;
// 				case ProcessStepType.ANIMATOR_CONTROLLER:
// 					DrawAnimatorController(property);
// 					break;
// 				case ProcessStepType.ANIMATOR_APPLY_ROOT_MOTION:
// 					DrawAnimatorApplyRootMotion(property);
// 					break;
// 				case ProcessStepType.AUDIO_ONE_SHOT:
// 					DrawAudioOneShot(property);
// 					break;
// 				case ProcessStepType.AUDIO_PLAY:
// 					DrawAudioSource(property);
// 					break;
// 				case ProcessStepType.AUDIO_STOP:
// 					DrawAudioSource(property);
// 					break;
// 				case ProcessStepType.ACTIVE:
// 					DrawActive(property);
// 					break;
// 				case ProcessStepType.ENABLED:
// 					DrawEnabled(property);
// 					break;
// 				case ProcessStepType.INSTANTIATE:
// 					DrawInstantiate(property);
// 					break;
// 				case ProcessStepType.DESTROY:
// 					DrawDestroy(property);
// 					break;
// 				case ProcessStepType.PARENT:
// 					DrawParent(property);
// 					break;
// 				case ProcessStepType.PLAYABLE_PLAY:
// 					DrawPlayable(property);
// 					break;
// 				case ProcessStepType.PLAYABLE_STOP:
// 					DrawPlayable(property);
// 					break;
// 				case ProcessStepType.PLAYABLE_PAUSE:
// 					DrawPlayable(property);
// 					break;
// 				case ProcessStepType.PLAYABLE_RESUME:
// 					DrawPlayable(property);
// 					break;
// 				case ProcessStepType.PLAYABLE_GOTO:
// 					DrawPlayableGoto(property);
// 					break;
// 				case ProcessStepType.CUSTOM_EVENT:
// 					DrawCustomEvent(property);
// 					break;
// 				case ProcessStepType.UNITY_EVENT:
// 					DrawUnityEvent(property);
// 					break;
// 			}
// 			
// 			if (EditorGUI.EndChangeCheck()) {
// 				SetDirty(property);
// 			}
// 		}
//
// 		private void ResetByType() {
// 			switch (type) {
// 				case ProcessStepType.UNITY_EVENT:
// 					obj = null;
// 					break;
// 				default:
// 					unityEvent = null;
// 					break;
// 			}
// 			switch (type) {
// 				case ProcessStepType.ANIMATOR:
// 				case ProcessStepType.AUDIO_ONE_SHOT:
// 				case ProcessStepType.INSTANTIATE:
// 				case ProcessStepType.CUSTOM_EVENT:
// 					fArgument = 1;
// 					break;
// 				case ProcessStepType.AUDIO_PLAY:
// 				case ProcessStepType.AUDIO_STOP:
// 					fArgument = 0.25F;
// 					break;
// 				default:
// 					fArgument = 0;
// 					break;
// 			}
// 			switch (type) {
// 				case ProcessStepType.DO_TWEEN_RESTART:
// 				case ProcessStepType.DO_TWEEN_GOTO:
// 					sArgument = "1";
// 					break;
// 				default:
// 					sArgument = null;
// 					break;
// 			}
// 			switch (type) {
// 				case ProcessStepType.INSTANTIATE:
// 				case ProcessStepType.PARENT:
// 					break;
// 				default:
// 					objArgument = null;
// 					break;
// 			}
// 		}
//
// 		private void DrawTrigger(InspectorProperty property) {
// 			BaseTriggerCtrl newObj = DrawCompField<BaseTriggerCtrl>("触发器", obj);
// 			if (newObj != obj) {
// 				property.RecordForUndo("Obj");
// 				obj = newObj;
// 			}
// 		}
//
// 		private void DrawStateController(InspectorProperty property) {
// 			StateController newObj = DrawCompField<StateController>("控制器", obj);
// 			if (newObj != obj) {
// 				property.RecordForUndo("Obj");
// 				obj = newObj;
// 			}
// 			if (newObj != null) {
// 				EditorGUILayout.BeginHorizontal();
// 				bool random = !string.IsNullOrEmpty(sArgument) && sArgument != "0";
// 				
// 				List<State> states = newObj.states;
// 				int _stateCount = states.Count;
// 				if (random) {
// 					_stateCount = Mathf.Min(_stateCount, 24);	// float精度为2^24 - 1
// 					string[] options = new string[_stateCount];
// 					bool anyNameExist = states.Exists(state => !string.IsNullOrEmpty(state.name));
// 					for (int j = 0; j < _stateCount; ++j) {
// 						string ext = anyNameExist ? states[j].name : states[j].desc;
// 						if (!string.IsNullOrEmpty(ext)) {
// 							ext = ":" + ext;
// 						}
// 						options[j] = j + ext;
// 					}
// 					GUIContent label = new GUIContent("状态", "数据以Float类型储存，尾数部分为23位，所以最多只支持24个状态");
// 					int indexMask = (int) fArgument;
// 					int newIndexMask = EditorGUILayout.MaskField(label, indexMask, options);
// 					if (newIndexMask == (1 << _stateCount) - 1) {
// 						newIndexMask = -1;
// 					}
// 					if (newIndexMask != indexMask) {
// 						property.RecordForUndo("FArgument");
// 						fArgument = newIndexMask;
// 					}
// 				} else {
// 					string[] options = new string[_stateCount];
// 					int[] values = new int[_stateCount];
// 					bool anyNameExist = states.Exists(state => !string.IsNullOrEmpty(state.name));
// 					for (int j = 0; j < _stateCount; ++j) {
// 						string ext = anyNameExist ? states[j].name : states[j].desc;
// 						if (!string.IsNullOrEmpty(ext)) {
// 							ext = ":" + ext;
// 						}
// 						options[j] = j + ext;
// 						values[j] = j;
// 					}
// 					int stateIndex = (int) fArgument;
// 					int newStateIndex = EditorGUILayout.IntPopup("状态", stateIndex, options, values);
// 					if (newStateIndex != stateIndex) {
// 						property.RecordForUndo("FArgument");
// 						fArgument = newStateIndex;
// 					}
// 				}
// 				
// 				bool newRandom = DrawToggle(random, "随机", BTN_WIDTH_OPTION);
// 				if (newRandom != random) {
// 					property.RecordForUndo("SArgument");
// 					sArgument = newRandom ? "1" : null;
// 					fArgument = newRandom ? -1 : 0;
// 				}
// 				
// 				GUILayoutOption width = GUILayout.Width(s_ContextWidth * 0.3F - 80F - 3F);
// 				EditorGUILayout.LabelField(string.Empty, width);
// 				EditorGUILayout.EndHorizontal();
// 			}
// 		}
//
// 		private void DrawProgressController(InspectorProperty property) {
// 			ProgressController newObj = DrawCompField<ProgressController>("控制器", obj);
// 			if (newObj != obj) {
// 				property.RecordForUndo("Obj");
// 				obj = newObj;
// 			}
// 			if (newObj != null) {
// 				EditorGUILayout.BeginHorizontal();
// 				bool random = sArgument == "1";
// 				
// 				bool prevEnabled = GUI.enabled;
// 				GUI.enabled = !random && prevEnabled;
// 				float newArgument = EditorGUILayout.Slider("进度", fArgument, 0, 1);
// 				if (Math.Abs(newArgument - fArgument) > Mathf.Epsilon) {
// 					property.RecordForUndo("FArgument");
// 					fArgument = newArgument;
// 				}
// 				GUI.enabled = prevEnabled;
// 				
// 				bool newRandom = DrawToggle(random, "随机", BTN_WIDTH_OPTION);
// 				if (newRandom != random) {
// 					property.RecordForUndo("SArgument");
// 					sArgument = newRandom ? "1" : "0";
// 				}
// 				
// 				GUILayoutOption width = GUILayout.Width(s_ContextWidth * 0.3F - 80F - 3F);
// 				EditorGUILayout.LabelField("", width);
// 				EditorGUILayout.EndHorizontal();
// 			}
// 		}
//
// 		private void DrawDOTweenRestart(InspectorProperty property) {
// 			ABSAnimationComponent newObj = DrawCompField<ABSAnimationComponent>("缓动器", obj);
// 			if (newObj != obj) {
// 				property.RecordForUndo("Obj");
// 				obj = newObj;
// 			}
// 			if (newObj != null) {
// 				EditorGUILayout.BeginHorizontal();
// 				EditorGUILayout.LabelField("参数", CustomEditorGUI.LabelWidthOption);
//
// 				bool oldSArgument = sArgument == "1";
// 				bool newSArgument = DrawToggle(oldSArgument, "Include Delay", GUILayout.Width(90F));
// 				if (oldSArgument != newSArgument) {
// 					property.RecordForUndo("SArgument");
// 					sArgument = newSArgument ? "1" : "0";
// 				}
// 					
// 				bool fromHereValid = false;
// 				switch (newObj) {
// 					case DOTweenAnimation anim when anim.isRelative: {
// 						if (!(anim.animationType == DOTweenAnimation.AnimationType.None ||
// 								anim.animationType > DOTweenAnimation.AnimationType.LocalMove &&
// 								anim.animationType <= DOTweenAnimation.AnimationType.UIWidthHeight)) {
// 							fromHereValid = true;
// 						}
// 						break;
// 					}
// 					case DOTweenPath path when path.relative && !path.isLocal:
// 						fromHereValid = true;
// 						break;
// 				}
//
// 				bool prevEnabled = GUI.enabled;
// 				GUI.enabled = fromHereValid && prevEnabled;
// 				bool oldFArgument = fArgument > 0;
// 				bool newArgument = DrawToggle(oldFArgument, "From Here", BTN_WIDTH_OPTION);
// 				if (oldFArgument != newArgument) {
// 					property.RecordForUndo("FArgument");
// 					fArgument = newArgument ? 1 : 0;
// 				}
// 				GUI.enabled = prevEnabled;
// 				
// 				EditorGUILayout.EndHorizontal();
// 			}
// 		}
//
// 		private void DrawDOTweenGoto(InspectorProperty property) {
// 			ABSAnimationComponent newObj = DrawCompField<ABSAnimationComponent>("缓动器", obj);
// 			if (newObj != obj) {
// 				property.RecordForUndo("Obj");
// 				obj = newObj;
// 			}
// 			if (newObj != null) {
// 				EditorGUILayout.BeginHorizontal();
// 				bool loop = false;
// 				switch (newObj) {
// 					case DOTweenAnimation doTweenAnim:
// 						loop = doTweenAnim.loops == -1;
// 						break;
// 					case DOTweenPath doTweenPath:
// 						loop = doTweenPath.loops == -1;
// 						break;
// 				}
// 				
// 				bool isPercent = sArgument == "1";
// 				float newFArgument = !loop && isPercent ? EditorGUILayout.Slider("进度", fArgument, 0, 1) :
// 						Mathf.Max(EditorGUILayout.FloatField("进度", fArgument), 0);
// 				if (Math.Abs(newFArgument - fArgument) > Mathf.Epsilon) {
// 					property.RecordForUndo("FArgument");
// 					fArgument = newFArgument;
// 				}
//
// 				bool prevEnabled = GUI.enabled;
// 				GUI.enabled = !loop && prevEnabled;
// 				bool newIsPercent = DrawToggle(isPercent, "%", BTN_WIDTH_OPTION);
// 				if (isPercent != newIsPercent) {
// 					property.RecordForUndo("SArgument");
// 					sArgument = newIsPercent ? "1" : "0";
// 				}
// 				GUI.enabled = prevEnabled;
// 				
// 				GUILayoutOption width = GUILayout.Width(s_ContextWidth * 0.3F - 80F - 3F);
// 				EditorGUILayout.LabelField("", width);
// 				EditorGUILayout.EndHorizontal();
// 			}
// 		}
//
// 		private void DrawDOTweenPause(InspectorProperty property) {
// 			ABSAnimationComponent newObj = DrawCompField<ABSAnimationComponent>("缓动器", obj);
// 			if (newObj != obj) {
// 				property.RecordForUndo("Obj");
// 				obj = newObj;
// 			}
// 		}
//
// 		private void DrawDOTweenResume(InspectorProperty property) {
// 			ABSAnimationComponent newObj = DrawCompField<ABSAnimationComponent>("缓动器", obj);
// 			if (newObj != obj) {
// 				property.RecordForUndo("Obj");
// 				obj = newObj;
// 			}
// 		}
//
// 		private void DrawDOTweenKill(InspectorProperty property) {
// 			ABSAnimationComponent newObj = DrawCompField<ABSAnimationComponent>("缓动器", obj);
// 			if (newObj != obj) {
// 				property.RecordForUndo("Obj");
// 				obj = newObj;
// 			}
// 		}
//
// 		private void DrawAnimator(InspectorProperty property) {
// 			Animator newObj = DrawCompField<Animator>("动画器", obj);
// 			if (newObj != obj) {
// 				property.RecordForUndo("Obj");
// 				obj = newObj;
// 			}
// 			if (newObj != null) {
// 				EditorGUILayout.BeginHorizontal();
// 				EditorGUILayout.LabelField("参数", CustomEditorGUI.LabelWidthOption);
//
// 				if (newObj.gameObject.activeInHierarchy && newObj.runtimeAnimatorController is AnimatorController controller) {
// 					// if (!newObj.isInitialized) {
// 					// 	// 随便找个字段改一下
// 					// 	SerializedObject serializedObject = new SerializedObject(newObj);
// 					// 	SerializedProperty serializedProperty = serializedObject.FindProperty("m_ApplyRootMotion");
// 					// 	bool willRevert = serializedProperty.isInstantiatedPrefab && !serializedProperty.prefabOverride;
// 					// 	serializedProperty.boolValue = !serializedProperty.boolValue;
// 					// 	serializedProperty.boolValue = !serializedProperty.boolValue;
// 					// 	if (willRevert && serializedProperty.prefabOverride) {
// 					// 		PrefabUtility.RevertPropertyOverride(serializedProperty, InteractionMode.AutomatedAction);
// 					// 	} else {
// 					// 		serializedObject.ApplyModifiedProperties();
// 					// 	}
// 					// }
// 					List<AnimatorControllerParameter> parameters = new List<AnimatorControllerParameter>(controller.parameters);
// 					if (parameters.Count > 0) {
// 						List<string> paramNames = parameters.ConvertAll(parameter => parameter.name);
// 						int index = paramNames.IndexOf(sArgument);
// 						int newIndex = EditorGUILayout.Popup(index, paramNames.ToArray());
// 						if (newIndex != index) {
// 							property.RecordForUndo("SArgument");
// 							sArgument = paramNames[newIndex];
// 						}
//
// 						if (newIndex != -1) {
// 							switch (parameters[newIndex].type) {
// 								case AnimatorControllerParameterType.Float: {
// 									GUILayoutOption fieldWidth = GUILayout.Width(s_ContextWidth * 0.3F);
// 									float newArgument = EditorGUILayout.FloatField(fArgument, fieldWidth);
// 									if (Math.Abs(newArgument - fArgument) > Mathf.Epsilon) {
// 										property.RecordForUndo("FArgument");
// 										fArgument = newArgument;
// 									}
// 									break;
// 								}
// 								case AnimatorControllerParameterType.Int: {
// 									GUILayoutOption fieldWidth = GUILayout.Width(s_ContextWidth * 0.3F);
// 									int oldArgument = (int) fArgument;
// 									int newArgument = EditorGUILayout.IntField(oldArgument, fieldWidth);
// 									if (newArgument != oldArgument) {
// 										property.RecordForUndo("FArgument");
// 										fArgument = newArgument;
// 									}
// 									break;
// 								}
// 								case AnimatorControllerParameterType.Bool: {
// 									bool oldArgument = fArgument > 0;
// 									if (DrawButton(oldArgument ? "√" : "X", BTN_WIDTH_OPTION)) {
// 										property.RecordForUndo("FArgument");
// 										fArgument = oldArgument ? 0 : 1;
// 									}
// 									GUILayoutOption blankWidth = GUILayout.Width(s_ContextWidth * 0.3F - 80F - 3F);
// 									EditorGUILayout.LabelField("", blankWidth);
// 									break;
// 								}
// 								case AnimatorControllerParameterType.Trigger: {
// 									bool oldArgument = fArgument > 0;
// 									if (DrawButton(oldArgument ? "触发" : "取消", BTN_WIDTH_OPTION)) {
// 										property.RecordForUndo("FArgument");
// 										fArgument = oldArgument ? 0 : 1;
// 									}
// 									GUILayoutOption blankWidth = GUILayout.Width(s_ContextWidth * 0.3F - 80F - 3F);
// 									EditorGUILayout.LabelField("", blankWidth);
// 									break;
// 								}
// 							}
// 						} else {
// 							GUILayoutOption blankWidth = GUILayout.Width(s_ContextWidth * 0.3F);
// 							EditorGUILayout.LabelField("", blankWidth);
// 						}
// 						EditorGUILayout.EndHorizontal();
// 						return;
// 					}
// 				}
// 				bool prevEnabled = GUI.enabled;
// 				GUI.enabled = false;
// 				EditorGUILayout.Popup(0, new []{sArgument});
// 				GUI.enabled = prevEnabled;
// 				
// 				GUILayoutOption width = GUILayout.Width(s_ContextWidth * 0.3F);
// 				EditorGUILayout.LabelField("", width);
// 				EditorGUILayout.EndHorizontal();
// 			}
// 		}
//
// 		private void DrawAnimatorController(InspectorProperty property) {
// 			Animator newObj = DrawCompField<Animator>("动画器", obj);
// 			if (newObj != obj) {
// 				property.RecordForUndo("Obj");
// 				obj = newObj;
// 			}
//
// 			if (newObj != null) {
// 				EditorGUILayout.BeginHorizontal();
// 				RuntimeAnimatorController newObjArgument = DrawObjectField<RuntimeAnimatorController>("状态机", objArgument);
// 				if (newObjArgument != objArgument) {
// 					property.RecordForUndo("ObjArgument");
// 					objArgument = newObjArgument;
// 				}
//
// 				GUILayoutOption width = GUILayout.Width(s_ContextWidth * 0.3F);
// 				EditorGUILayout.LabelField("", width);
// 				EditorGUILayout.EndHorizontal();
// 			}
// 		}
//
// 		private void DrawAnimatorApplyRootMotion(InspectorProperty property) {
// 			Animator newObj = DrawCompField<Animator>("动画器", obj);
// 			if (newObj != obj) {
// 				property.RecordForUndo("Obj");
// 				obj = newObj;
// 			}
//
// 			if (newObj != null) {
// 				EditorGUILayout.BeginHorizontal();
// 				EditorGUILayout.LabelField("根移动", CustomEditorGUI.LabelWidthOption);
//
// 				bool oldApplyRootMotion = fArgument > 0;
// 				bool newApplyRootMotion = DrawToggle(oldApplyRootMotion, oldApplyRootMotion ? "√" : "X", BTN_WIDTH_OPTION);
// 				if (newApplyRootMotion != oldApplyRootMotion) {
// 					property.RecordForUndo("SArgument");
// 					fArgument = newApplyRootMotion ? 1 : 0;
// 				}
// 				
// 				GUILayoutOption width = GUILayout.Width(s_ContextWidth * 0.3F);
// 				EditorGUILayout.LabelField("", width);
// 				EditorGUILayout.EndHorizontal();
// 			}
// 		}
//
// 		private void DrawAudioOneShot(InspectorProperty property) {
// 			AudioClip newObj = DrawObjectField<AudioClip>("音频", obj);
// 			if (newObj != obj) {
// 				property.RecordForUndo("Obj");
// 				obj = newObj;
// 			}
// 			if (newObj != null) {
// 				EditorGUILayout.BeginHorizontal();
// 				float newArgument = EditorGUILayout.FloatField("音量", fArgument);
// 				if (Math.Abs(newArgument - fArgument) > Mathf.Epsilon) {
// 					property.RecordForUndo("FArgument");
// 					fArgument = newArgument;
// 				}
// 				
// 				GUILayoutOption width = GUILayout.Width(s_ContextWidth * 0.3F);
// 				EditorGUILayout.LabelField("", width);
// 				EditorGUILayout.EndHorizontal();
// 			}
// 		}
//
// 		private void DrawAudioSource(InspectorProperty property) {
// 			AudioSource newObj = DrawCompField<AudioSource>("音源", obj);
// 			if (newObj != obj) {
// 				property.RecordForUndo("Obj");
// 				obj = newObj;
// 			}
// 			if (newObj != null) {
// 				EditorGUILayout.BeginHorizontal();
// 				float newArgument = EditorGUILayout.FloatField("渐变", fArgument);
// 				if (Math.Abs(newArgument - fArgument) > Mathf.Epsilon) {
// 					property.RecordForUndo("FArgument");
// 					fArgument = newArgument;
// 				}
// 				
// 				GUILayoutOption width = GUILayout.Width(s_ContextWidth * 0.3F);
// 				EditorGUILayout.LabelField("", width);
// 				EditorGUILayout.EndHorizontal();
// 			}
// 		}
//
// 		private void DrawActive(InspectorProperty property) {
// 			GameObject newObj = DrawObjectField<GameObject>("游戏对象", obj);
// 			if (newObj != obj) {
// 				property.RecordForUndo("Obj");
// 				obj = newObj;
// 			}
// 			if (newObj != null) {
// 				EditorGUILayout.BeginHorizontal();
// 				EditorGUILayout.LabelField("活跃", CustomEditorGUI.LabelWidthOption);
// 				
// 				bool oldArgument = fArgument > 0;
// 				bool newArgument = DrawToggle(oldArgument, oldArgument ? "是" : "否", BTN_WIDTH_OPTION);
// 				if (newArgument != oldArgument) {
// 					property.RecordForUndo("FArgument");
// 					fArgument = newArgument ? 1 : 0;
// 				}
// 				
// 				GUILayoutOption width = GUILayout.Width(s_ContextWidth * 0.3F);
// 				EditorGUILayout.LabelField("", width);
// 				EditorGUILayout.EndHorizontal();
// 			}
// 		}
//
// 		private void DrawEnabled(InspectorProperty property) {
// 			Component newObj = DrawCompField<Component>("组件", obj, typeof(Behaviour), typeof(Renderer), typeof(Collider), typeof(LODGroup), typeof(Cloth));
// 			if (newObj != obj) {
// 				property.RecordForUndo("Obj");
// 				obj = newObj;
// 			}
// 			if (newObj != null) {
// 				EditorGUILayout.BeginHorizontal();
// 				EditorGUILayout.LabelField("启用", CustomEditorGUI.LabelWidthOption);
// 				
// 				bool oldArgument = fArgument > 0;
// 				bool newArgument = DrawToggle(oldArgument, oldArgument ? "是" : "否", BTN_WIDTH_OPTION);
// 				if (newArgument != oldArgument) {
// 					property.RecordForUndo("FArgument");
// 					fArgument = newArgument ? 1 : 0;
// 				}
// 				
// 				GUILayoutOption width = GUILayout.Width(s_ContextWidth * 0.3F);
// 				EditorGUILayout.LabelField("", width);
// 				EditorGUILayout.EndHorizontal();
// 			}
// 		}
//
// 		private void DrawInstantiate(InspectorProperty property) {
// 			Transform newObj = DrawObjectField<Transform>("预制体", obj);
// 			if (newObj != obj) {
// 				property.RecordForUndo("Obj");
// 				obj = newObj;
// 			}
// 			if (newObj != null) {
// 				bool pivotIsParent = sArgument == "1" || sArgument == "3";
// 				bool resetToPivot = sArgument == "2" || sArgument == "3";
// 				
// 				EditorGUILayout.BeginHorizontal();
// 				EditorGUILayout.LabelField("父节点", CustomEditorGUI.LabelWidthOption);
// 				
// 				if (DrawButton(pivotIsParent ? "指定节点" : "当前节点", BTN_WIDTH_OPTION)) {
// 					property.RecordForUndo("FArgument");
// 					pivotIsParent = !pivotIsParent;
// 					sArgument = (resetToPivot ? 2 : 0) + (pivotIsParent ? 1 : 0) + "";
// 				}
// 				EditorGUILayout.EndHorizontal();
// 				
// 				EditorGUILayout.BeginHorizontal();
// 				EditorGUILayout.LabelField("位置&旋转", CustomEditorGUI.LabelWidthOption);
// 				
// 				bool oldArgument = fArgument > 0;
// 				bool newArgument = DrawToggle(oldArgument, "重置", BTN_WIDTH_OPTION);
// 				if (newArgument != oldArgument) {
// 					property.RecordForUndo("FArgument");
// 					fArgument = newArgument ? 1 : 0;
// 				}
// 				EditorGUILayout.EndHorizontal();
// 				
// 				if (newArgument) {
// 					EditorGUILayout.BeginHorizontal();
// 					EditorGUILayout.LabelField("重置到", CustomEditorGUI.LabelWidthOption);
// 					if (DrawButton(resetToPivot ? "指定节点" : "当前节点", BTN_WIDTH_OPTION)) {
// 						property.RecordForUndo("FArgument");
// 						resetToPivot = !resetToPivot;
// 						sArgument = (resetToPivot ? 2 : 0) + (pivotIsParent ? 1 : 0) + "";
// 					}
// 					EditorGUILayout.EndHorizontal();
// 				}
// 				
// 				if (pivotIsParent || resetToPivot && newArgument) {
// 					Transform newObjArgument = DrawObjectField<Transform>("指定节点", objArgument);
// 					if (newObjArgument != objArgument) {
// 						property.RecordForUndo("ObjArgument");
// 						objArgument = newObjArgument;
// 					}
// 				}
// 			}
// 		}
//
// 		private void DrawDestroy(InspectorProperty property) {
// 			GameObject newObj = DrawObjectField<GameObject>("游戏对象", obj);
// 			if (newObj != obj) {
// 				property.RecordForUndo("Obj");
// 				obj = newObj;
// 			}
// 			if (newObj != null) {
// 				EditorGUILayout.BeginHorizontal();
// 				EditorGUILayout.LabelField("销毁目标", CustomEditorGUI.LabelWidthOption);
// 				
// 				bool oldArgument = fArgument > 0;
// 				bool newArgument = DrawToggle(oldArgument, "仅子节点", BTN_WIDTH_OPTION);
// 				if (newArgument != oldArgument) {
// 					property.RecordForUndo("FArgument");
// 					fArgument = newArgument ? 1 : 0;
// 				}
// 				
// 				GUILayoutOption width = GUILayout.Width(s_ContextWidth * 0.3F);
// 				EditorGUILayout.LabelField("", width);
// 				EditorGUILayout.EndHorizontal();
// 			}
// 		}
//
// 		private void DrawParent(InspectorProperty property) {
// 			Transform newObj = DrawObjectField<Transform>("游戏对象", obj);
// 			if (newObj != obj) {
// 				property.RecordForUndo("Obj");
// 				obj = newObj;
// 			}
// 			if (newObj != null) {
// 				Transform newObjArgument = DrawObjectField<Transform>("父节点", objArgument);
// 				if (newObjArgument != objArgument) {
// 					property.RecordForUndo("ObjArgument");
// 					objArgument = newObjArgument;
// 				}
// 				//
// 				// EditorGUILayout.BeginHorizontal();
// 				// EditorGUILayout.LabelField("位置&旋转", CustomEditorGUI.LabelWidthOption);
// 				//
// 				// bool oldArgument = fArgument > 0;
// 				// bool newArgument = DrawToggle(oldArgument, "重置", BTN_WIDTH_OPTION);
// 				// if (newArgument != oldArgument) {
// 				// 	property.RecordForUndo("FArgument");
// 				// 	fArgument = newArgument ? 1 : 0;
// 				// }
// 				//
// 				// GUILayoutOption width = GUILayout.Width(s_ContextWidth * 0.3F);
// 				// EditorGUILayout.LabelField("", width);
// 				// EditorGUILayout.EndHorizontal();
// 			}
// 		}
//
// 		private void DrawPlayable(InspectorProperty property) {
// 			PlayableDirector newObj = DrawCompField<PlayableDirector>("导演", obj);
// 			if (newObj != obj) {
// 				property.RecordForUndo("Obj");
// 				obj = newObj;
// 			}
// 		}
//
// 		private void DrawPlayableGoto(InspectorProperty property) {
// 			PlayableDirector newObj = DrawCompField<PlayableDirector>("导演", obj);
// 			if (newObj != obj) {
// 				property.RecordForUndo("Obj");
// 				obj = newObj;
// 			}
// 			if (newObj != null) {
// 				EditorGUILayout.BeginHorizontal();
// 				
// 				// bool isPercent = sArgument == "1";
// 				const bool isPercent = false;
// 				float newFArgument = isPercent ? EditorGUILayout.Slider("进度", fArgument, 0, 1) :
// 						Mathf.Max(EditorGUILayout.FloatField("进度", fArgument), 0);
// 				if (Math.Abs(newFArgument - fArgument) > Mathf.Epsilon) {
// 					property.RecordForUndo("FArgument");
// 					fArgument = newFArgument;
// 				}
//
// 				bool prevEnabled = GUI.enabled;
// 				GUI.enabled = false;
// 				bool newIsPercent = DrawToggle(isPercent, "%", BTN_WIDTH_OPTION);
// 				if (isPercent != newIsPercent) {
// 					property.RecordForUndo("SArgument");
// 					sArgument = newIsPercent ? "1" : "0";
// 				}
// 				GUI.enabled = prevEnabled;
// 				
// 				GUILayoutOption width = GUILayout.Width(s_ContextWidth * 0.3F - 80F - 3F);
// 				EditorGUILayout.LabelField("", width);
// 				EditorGUILayout.EndHorizontal();
// 			}
// 		}
//
// 		private void DrawCustomEvent(InspectorProperty property) {
// 			EditorGUILayout.BeginHorizontal();
// 			string newEventName = EditorGUILayout.TextField("事件", sArgument);
// 			if (newEventName != sArgument) {
// 				property.RecordForUndo("SArgument");
// 				sArgument = newEventName;
// 			}
// 			
// 			bool oldArgument = fArgument > 0;
// 			bool newArgument = DrawToggle(oldArgument, "广播", BTN_WIDTH_OPTION);
// 			if (newArgument != oldArgument) {
// 				property.RecordForUndo("FArgument");
// 				fArgument = newArgument ? 1 : 0;
// 			}
// 			
// 			GUILayoutOption width = GUILayout.Width(s_ContextWidth * 0.3F - 80 - 3F);
// 			EditorGUILayout.LabelField("", width);
// 			EditorGUILayout.EndHorizontal();
// 			
// 			if (!newArgument) {
// 				GameObject newObj = DrawObjectField<GameObject>("目标", obj);
// 				if (newObj != obj) {
// 					property.RecordForUndo("Obj");
// 					obj = newObj;
// 				}
// 			}
// 		}
//
// 		private void DrawUnityEvent(InspectorProperty property) {
// 			int valueCount = property.SerializationRoot.ValueEntry.ValueCount;
// 			TriggerCtrlProcess trigger = null;
// 			for (int i = 0; i < valueCount; i++) {
// 				trigger = property.SerializationRoot.ValueEntry.WeakValues[i] as TriggerCtrlProcess;
// 				if (trigger != null) {
// 					break;
// 				}
// 			}
// 			if (trigger != null) {
// 				EditorGUILayout.BeginHorizontal();
// 				EditorGUILayout.LabelField("事件", CustomEditorGUI.LabelWidthOption);
// 				
// 				int index = trigger.steps.IndexOf(this);
// 				if (index != -1) {
// 					SerializedObject serializedObject = new SerializedObject(trigger);
// 					SerializedProperty serializedProperty = serializedObject.FindProperty("steps")
// 							.GetArrayElementAtIndex(index)
// 							.FindPropertyRelative("unityEvent");
// 					EditorGUILayout.PropertyField(serializedProperty);
// 					serializedObject.ApplyModifiedProperties();
// 				}
// 				EditorGUILayout.EndHorizontal();
// 			}
// 		}
//
// 		private static readonly Color BTN_CHECKED_COLOR = new Color(0, 0.8F, 0.8F, 1);
// 		private static readonly GUILayoutOption BTN_WIDTH_OPTION = GUILayout.Width(80F);
// 		
// 		private static bool DrawToggle(bool value, string text, params GUILayoutOption[] options) {
// 			return CustomEditorGUI.Toggle(value, text, BTN_CHECKED_COLOR, options);
// 		}
// 		
// 		private static bool DrawButton(string text, params GUILayoutOption[] options) {
// 			return CustomEditorGUI.Button(text, BTN_CHECKED_COLOR * 1.2F, options);
// 		}
// 		
// 		private static T DrawObjectField<T>(string text, UObject obj, params Type[] types) where T : UObject {
// 			EditorGUILayout.BeginHorizontal();
// 			Type _Type = typeof(T);
// 			T newObj = string.IsNullOrEmpty(text) ?
// 					EditorGUILayout.ObjectField(obj, _Type) as T :
// 					EditorGUILayout.ObjectField(text, obj, _Type) as T;
// 			if (newObj && newObj is Component comp && !typeof(Transform).IsAssignableFrom(typeof(T))) {
// 				List<Component> comps = new List<Component>(comp.GetComponents(_Type));
// 				if (types.Length > 0) {
// 					comps = comps.FindAll(_comp => Array.Exists(types, _type => _type.IsInstanceOfType(_comp)));
// 				}
// 				int compCount = comps.Count;
// 				string[] compNames = new string[compCount + 1];
// 				int[] compIndexes = new int[compCount + 1];
// 				compNames[0] = "0.None";
// 				compIndexes[0] = -1;
// 				for (int index = 0; index < compCount; ++index) {
// 					string customLabel = CustomEditorGUI.GetCustomLabel(comps[index]);
// 					string name = index + 1 + "." + comps[index].GetType().Name;
// 					if (customLabel != null) {
// 						name += " - " + customLabel;
// 					}
// 					compNames[index + 1] = name;
// 					compIndexes[index + 1] = index;
// 				}
// 				int currentIndex = comps.IndexOf(comp);
// 				if (currentIndex == -1) {
// 					newObj = compCount > 0 ? comps[0] as T : null;
// 				}
// 				GUILayoutOption width = GUILayout.Width(s_ContextWidth * 0.3F);
// 				int dataIndex = EditorGUILayout.IntPopup(currentIndex, compNames, compIndexes, width);
// 				if (dataIndex != currentIndex) {
// 					newObj = dataIndex == -1 ? null : comps[dataIndex] as T;
// 				}
// 			}
// 			EditorGUILayout.EndHorizontal();
// 			return newObj;
// 		}
//
// 		private void SetDirty(InspectorProperty property) {
// 			int valueCount = property.SerializationRoot.ValueEntry.ValueCount;
// 			TriggerCtrlProcess trigger = null;
// 			for (int i = 0; i < valueCount; i++) {
// 				trigger = property.SerializationRoot.ValueEntry.WeakValues[i] as TriggerCtrlProcess;
// 				if (trigger != null) {
// 					break;
// 				}
// 			}
// 			if (trigger != null) {
// 				EditorUtility.SetDirty(trigger);
// 			}
// 		}
// 		
// 		private static float GetContextWidth() {
// 			// return EditorGUIUtility.contextWidth;
// 			PropertyInfo pi = typeof(EditorGUIUtility).GetProperty("contextWidth", BindingFlags.Static | BindingFlags.NonPublic);
// 			return (float) (pi?.GetValue(null) ?? EditorGUIUtility.currentViewWidth);
// 		}
// 		
// 		private static Dictionary<ProcessStepType, string> s_TypeNameDict = new Dictionary<ProcessStepType, string>() {
// 			{ProcessStepType.TRIGGER, "触发"},
// 			{ProcessStepType.STATE_CONTROLLER, "状态控制"},
// 			{ProcessStepType.PROGRESS_CONTROLLER, "进度控制"},
// 			
// 			{ProcessStepType.DO_TWEEN_RESTART, "缓动 - 从头开始"},
// 			{ProcessStepType.DO_TWEEN_GOTO, "缓动 - 跳转进度"},
// 			{ProcessStepType.DO_TWEEN_PAUSE, "缓动 - 暂停"},
// 			{ProcessStepType.DO_TWEEN_RESUME, "缓动 - 继续"},
// 			{ProcessStepType.DO_TWEEN_KILL, "缓动 - 终止"},
// 			
// 			{ProcessStepType.ANIMATOR, "动画 - 参数控制"},
// 			{ProcessStepType.ANIMATOR_CONTROLLER, "动画 - 换状态机"},
// 			{ProcessStepType.ANIMATOR_APPLY_ROOT_MOTION, "动画 - 根移动"},
// 			
// 			{ProcessStepType.AUDIO_ONE_SHOT, "音频 - 单次播放"},
// 			{ProcessStepType.AUDIO_PLAY, "音频 - 音源播放"},
// 			{ProcessStepType.AUDIO_STOP, "音频 - 音源停止"},
// 			
// 			{ProcessStepType.ACTIVE, "开关 - 活跃状态"},
// 			{ProcessStepType.ENABLED, "开关 - 启用状态"},
// 			
// 			{ProcessStepType.INSTANTIATE, "组织 - 实例化"},
// 			{ProcessStepType.DESTROY, "组织 - 销毁"},
// 			{ProcessStepType.PARENT, "组织 - 换父节点"},
// 			
// 			{ProcessStepType.PLAYABLE_PLAY, "时间轴 - 播放"},
// 			{ProcessStepType.PLAYABLE_STOP, "时间轴 - 停止"},
// 			{ProcessStepType.PLAYABLE_PAUSE, "时间轴 - 暂停"},
// 			{ProcessStepType.PLAYABLE_RESUME, "时间轴 - 继续"},
// 			{ProcessStepType.PLAYABLE_GOTO, "时间轴 - 跳转进度"},
// 			
// 			{ProcessStepType.UNITY_EVENT, "事件"},
// 		};
// 	}
// 	
// 	public partial class TriggerCtrlProcess {
// 		protected override bool IsTriggered {
// 			get => base.IsTriggered;
// 			set {
// 				base.IsTriggered = value;
// 				if (!value) {
// 					foreach (var step in steps) {
// 						step.IsTriggered = false;
// 					}
// 				}
// 			}
// 		}
//
// 		[OnInspectorGUI]
// 		private void OnInspectorGUI(InspectorProperty property) {
// 			EditorGUILayout.BeginHorizontal();
// 			if (GUILayout.Button("克隆最后一步")) {
// 				property.RecordForUndo("Add");
// 				int stepCount = steps.Count;
// 				if (stepCount > 0) {
// 					FieldInfo copyModeFi = typeof(Clipboard).GetField("copyMode", BindingFlags.Static | BindingFlags.NonPublic);
// 					FieldInfo objFi = typeof(Clipboard).GetField("obj", BindingFlags.Static | BindingFlags.NonPublic);
// 					CopyModes copyMode = (CopyModes) (copyModeFi?.GetValue(null) ?? CopyModes.DeepCopy);
// 					string systemCopyBuffer = GUIUtility.systemCopyBuffer;
// 					object obj = objFi?.GetValue(null);
// 					Clipboard.Copy(steps[stepCount - 1]);
// 					steps.Add(Clipboard.Paste<ProcessStep>());
// 					copyModeFi?.SetValue(null, copyMode);
// 					GUIUtility.systemCopyBuffer = systemCopyBuffer;
// 					objFi?.SetValue(null, obj);
// 				} else {
// 					steps.Add(new ProcessStep());
// 				}
// 			}
//
// 			bool prevEnabled = GUI.enabled;
// 			GUI.enabled = Clipboard.CanPaste<ProcessStep>();
// 			if (GUILayout.Button("粘贴到最后")) {
// 				property.RecordForUndo("Add");
// 				steps.Add(Clipboard.Paste<ProcessStep>());
// 			}
// 			GUI.enabled = prevEnabled;
// 			// if (GUILayout.Button("排序")) {
// 			// 	property.RecordForUndo("Sort");
// 			// 	BubbleSort(steps, (step1, step2) => step1.time - step2.time);
// 			// }
// 			EditorGUILayout.EndHorizontal();
// 		}
// 	}
// }
// #endif