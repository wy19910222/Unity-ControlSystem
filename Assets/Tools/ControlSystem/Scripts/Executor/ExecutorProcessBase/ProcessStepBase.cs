/*
 * @Author: wangyun
 * @CreateTime: 2022-12-09 14:13:20 968
 * @LastEditor: wangyun
 * @EditTime: 2023-03-05 16:55:13 023
 */

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

using UObject = UnityEngine.Object;

namespace Control {
	public enum ProcessStepTypeBase {
		// executeType说明：0-依次, 1-随机
		// shuffleType说明：0-不洗牌, 1-仅洗一次, 2-循环时洗牌
		// randomType说明：0-无限制, 1-不重复
		// BaseExecutor[] executors, int[0] executorCount, int[1] executeType, int[2] shuffleType
		// BaseExecutor[] executors, int[0] executorCount, int[1] executeType, int[2] randomType
		EXECUTOR = 0,
		// StateController cState, bool[0] isRelative, int[0] offset, bool[1] loop
		// StateController cState, bool[0] isRelative, bool[1] recordIndex, bool[2] random, int[0] uid
		// StateController cState, bool[0] isRelative, bool[1] recordIndex, bool[2] random, int[] uids, bool[3] noRepeat
		// StateController cState, bool[0] isRelative, bool[1] recordIndex, bool[2] random, int[0] index
		// StateController cState, bool[0] isRelative, bool[1] recordIndex, bool[2] random, int[] indexes, bool[3] noRepeat
		STATE_CONTROLLER = 1,
		// ProgressController cProgress, bool[0] isRelative, float[0] offset, bool[1] loop
		// ProgressController cProgress, bool[0] isRelative, bool[1] random, float[0] progress
		// ProgressController cProgress, bool[0] isRelative, bool[1] random, float[0] min, float[1] max
		PROGRESS_CONTROLLER = 2,	// tween
		// string[0] eventName, bool[0] broadcast, GameObject[0] target
		CUSTOM_EVENT = 3,
		
		// Transform trans, bool[0] specifyParent, Transform[0] parent,
		// 		bool[1] resetPos, Transform[1] posBased,
		// 		bool[2] resetRot, Transform[2] rotBased,
		// 		bool[3] resetScale, Transform[3] scaleBased
		// 		bool[4] activeAtOnce
		INSTANTIATE = 11,
		// GameObject go, bool[0] onlyDestroyChildren
		DESTROY = 12,
		// Transform trans, Transform[0] parent, bool[0] worldPositionStays, int[0] siblingIndex
		PARENT = 13,
		
		// GameObject go, bool[0] active
		ACTIVE = 21,
		// Component comp, bool[0] enabled
		ENABLED = 22,
		
		// transformType说明：0-localPosition, 1-localEulerAngles, 2-localScale, 3-position, 4-eulerAngles
		// Transform trans, int[0] transformType, int[1] part, bool[0] targetAsValue, bool[1] random, bool[2] relative, Vector3[012] value
		// Transform trans, int[0] transformType, int[1] part, bool[0] targetAsValue, bool[1] random, bool[2] relative, bool[3] uniform, Vector3[012] min, Vector3[345] max
		// Transform trans, int[0] transformType, int[1] part, bool[0] targetAsValue, bool[1] random, bool[2] relative, Transform[0] target
		// Transform trans, int[0] transformType, int[1] part, bool[0] targetAsValue, bool[1] random, bool[2] relative, bool[3] uniform, Transform[0] min, Transform[1] max
		TRANSFORM = 31,	// tween
		// lookPart/upPart说明：0-x, 1-y, 2-z, 3--x, 4--y, 5--z
		// Transform trans, int[0] lookPart, int[1] upPart, Transform[0] target
		LOOK_AT = 32,	// tween
		// Transform trans, Camera/CinemachineVirtualCamera[0] camera, int[0] part, float[0] xAnchor, float[1] yAnchor, float[2] zAnchor
		CAMERA_ANCHOR = 33,	// tween
		
		// AudioClip clip, float[0] volumeScale
		AUDIO_ONE_SHOT = 41,
		// ctrlType说明：0-播放, 1-停止, 2-暂停, 3-继续, 4-暂停或继续
		// AudioSource source, int[0] ctrlType, float[0] fadeDuration
		AUDIO_SOURCE_CTRL = 42,
		// playType说明：0-依次, 1-随机
		// shuffleType说明：0-不洗牌, 1-仅洗一次, 2-循环时洗牌
		// randomType说明：0-无限制, 1-不重复
		// AudioClip[] clips, int[0] playCount, int[1] playType, int[2] shuffleType
		// AudioClip[] clips, int[0] playCount, int[1] playType, int[2] randomType
		AUDIOS_PLAY = 43,
		
		// toIntType说明：-1-向下取整, 0-四舍六入五成双, 1-向上取整，动画器参数为整型且开启缓动时才有该参数，用以选择插值转整型的方式
		// Animator animator, string[0] paramName, bool[0] isSet/bValue
		// Animator animator, string[0] paramName, float[0] fValue, bool[0] isRelative
		// Animator animator, string[0] paramName, int[0] iValue, bool[0] isRelative, int[1] toIntType
		ANIMATOR_PARAMETERS = 51,	// tween
		// Animator animator, RuntimeAnimatorController[0] controller
		ANIMATOR_CONTROLLER = 52,
		// Animator animator, Avatar[0] avatar
		ANIMATOR_AVATAR = 53,
		// Animator animator, bool[0] applyRootMotion
		ANIMATOR_APPLY_ROOT_MOTION = 54,

		// ctrlType说明：0-播放, 1-停止, 2-暂停, 3-继续, 4-暂停或继续
		// PlayableDirector director, int[0] ctrlType
		PLAYABLE_CTRL = 61,
		// PlayableDirector director, bool[0] isPercent, float[0] time, bool[1] evaluate
		// PlayableDirector director, bool[0] isPercent, float[0] percent, bool[1] evaluate
		PLAYABLE_GOTO = 62,
		// PLAYABLE_EVALUATE = 63,
		// PLAYABLE_REBUILD_GRAPH = 64,
		
		// ABSAnimationComponent doTween, bool[0] includeDelay, bool[1] formHere
		DO_TWEEN_RESTART = 71,
		// ctrlType说明：0-正播, 1-倒播, 2-暂停, 3-继续, 4-暂停或继续
		// ABSAnimationComponent doTween, int[0] ctrlType
		DO_TWEEN_CTRL = 72,
		// ABSAnimationComponent doTween, bool[0] isPercent, float[0] time, bool[1] pauseIfPlaying, bool[2] includeDelay
		// ABSAnimationComponent doTween, bool[0] isPercent, float[0] percent, bool[1] pauseIfPlaying, bool[2] includeDelay
		DO_TWEEN_GOTO = 73,
		// lifeType说明：0-杀掉, 1-重生
		// ABSAnimationComponent doTween, int[0] lifeType, bool[0] complete
		DO_TWEEN_LIFE = 74,
		
#if SPINE_EXIST
		// fieldType说明：0-动画名称, 1-播放速度, 2-是否循环
		// SkeletonAnimation/SkeletonGraphic obj, int[0] fieldType, string[0] animName
		// SkeletonAnimation/SkeletonGraphic obj, int[0] fieldType, float[0] timeScale
		// SkeletonAnimation/SkeletonGraphic obj, int[0] fieldType, bool[0] loop, bool[1] stop/resume
		SPINE_ANIMATION_SET_VALUE = 81,
		// SkeletonAnimation/SkeletonGraphic obj, bool[0] whenIsNotStarted, bool[1] whenIsNotComplete
		SPINE_ANIMATION_RESTART = 82,
		// SkeletonAnimation/SkeletonGraphic obj
		SPINE_ANIMATION_STOP = 83,
#endif
		
#if LUA_BEHAVIOUR_EXIST
		// LuaBehaviour self, string[0] luaCode, List<Injection7> parameters
		LUA_CODE_EXECUTE = 901,
		// LuaBehaviour luaBehaviour, string[0] fieldName, string[1] fieldPath, bool[0] isInstanceInvoke, List<Injection7>[0] value
		LUA_SET_VALUE = 902,
		// LuaBehaviour luaBehaviour, string[0] funcName, string[1] funcPath, bool[0] isInstanceInvoke, List<Injection7> parameters
		LUA_FUNCTION_INVOKE = 903,
#endif
		
		// UnityEvent unityEvent
		UNITY_EVENT = 999,
	}
	
	[Serializable]
	public partial class ProcessStepBase {
		public string title;
		public float time;
		public float delayFrames;
		public ProcessStepTypeBase type;
		public UObject obj;
		public List<string> sArguments = new List<string>(0);
		public List<bool> bArguments = new List<bool>(0);
		public List<int> iArguments = new List<int>(0);
		public List<long> lArguments = new List<long>(0);
		public List<float> fArguments = new List<float>(0);
		public List<UObject> objArguments = new List<UObject>(0);
		public UnityEvent unityEvent;
		
		public bool tween;
		public float tweenDelay;
		public float tweenDuration = 0.3F;
		public Ease tweenEase = Ease.OutQuad;
		public AnimationCurve tweenEaseCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));
		
#if UNITY_EDITOR
		protected bool IsExecuted { get; set; }
#endif

		public ProcessStepTypeBase Type {
			get => type;
			set {
				if (value != type) {
					type = value;
					ResetByType();
				}
			}
		}

		public ProcessStepBase() {
			// Reset by ProcessStepTypeBase.EXECUTOR
			objArguments.Add(null);	// executors[0]
			iArguments.Add(1);	// executorCount
			iArguments.Add(0);	// executeType
			iArguments.Add(0);	// shuffleType/randomType
		}
		
		public virtual void DoStep(BaseExecutor executor) {
#if UNITY_EDITOR
			IsExecuted = true;
#endif
			switch (Type) {
				case ProcessStepTypeBase.EXECUTOR:
					DoStepExecutor();
					break;
				case ProcessStepTypeBase.STATE_CONTROLLER:
					DoStepStateController();
					break;
				case ProcessStepTypeBase.PROGRESS_CONTROLLER:
					DoStepProgressController();
					break;
				case ProcessStepTypeBase.CUSTOM_EVENT:
					DoStepCustomEvent();
					break;
				
				case ProcessStepTypeBase.INSTANTIATE:
					DoStepInstantiate(executor);
					break;
				case ProcessStepTypeBase.DESTROY:
					DoStepDestroy();
					break;
				case ProcessStepTypeBase.PARENT:
					DoStepParent();
					break;
				case ProcessStepTypeBase.ACTIVE:
					DoStepActive();
					break;
				case ProcessStepTypeBase.ENABLED:
					DoStepEnabled();
					break;
				case ProcessStepTypeBase.TRANSFORM:
					DoStepTransform();
					break;
				case ProcessStepTypeBase.LOOK_AT:
					DoStepLookAt();
					break;
				case ProcessStepTypeBase.CAMERA_ANCHOR:
					DoStepCameraAnchor();
					break;
				
				case ProcessStepTypeBase.AUDIO_ONE_SHOT:
					DoStepAudioOneShot();
					break;
				case ProcessStepTypeBase.AUDIO_SOURCE_CTRL:
					DoStepAudioSourceCtrl(executor);
					break;
				case ProcessStepTypeBase.AUDIOS_PLAY:
					DoStepAudiosPlay();
					break;
				
				case ProcessStepTypeBase.ANIMATOR_PARAMETERS:
					DoStepAnimatorParameters();
					break;
				case ProcessStepTypeBase.ANIMATOR_CONTROLLER:
					DoStepAnimatorController();
					break;
				case ProcessStepTypeBase.ANIMATOR_AVATAR:
					DoStepAnimatorAvatar();
					break;
				case ProcessStepTypeBase.ANIMATOR_APPLY_ROOT_MOTION:
					DoStepAnimatorApplyRootMotion();
					break;
				
				case ProcessStepTypeBase.PLAYABLE_CTRL:
					DoStepPlayableCtrl();
					break;
				case ProcessStepTypeBase.PLAYABLE_GOTO:
					DoStepPlayableGoto();
					break;
				
				case ProcessStepTypeBase.DO_TWEEN_RESTART:
					DoStepDOTweenRestart();
					break;
				case ProcessStepTypeBase.DO_TWEEN_CTRL:
					DoStepDOTweenCtrl();
					break;
				case ProcessStepTypeBase.DO_TWEEN_GOTO:
					DoStepDOTweenGoto();
					break;
				case ProcessStepTypeBase.DO_TWEEN_LIFE:
					DoStepDOTweenLife();
					break;
				
#if SPINE_EXIST
				case ProcessStepTypeBase.SPINE_ANIMATION_SET_VALUE:
					DoStepSpineSetValue();
					break;
				case ProcessStepTypeBase.SPINE_ANIMATION_RESTART:
					DoStepSpineRestart();
					break;
				case ProcessStepTypeBase.SPINE_ANIMATION_STOP:
					DoStepSpineStop();
					break;
#endif
				
#if LUA_BEHAVIOUR_EXIST
				case ProcessStepTypeBase.LUA_CODE_EXECUTE:
					DoStepLuaCodeExecute();
					break;
				case ProcessStepTypeBase.LUA_SET_VALUE:
					DoStepLuaSetValue();
					break;
				case ProcessStepTypeBase.LUA_FUNCTION_INVOKE:
					DoStepLuaFunctionInvoke();
					break;
#endif
				
				case ProcessStepTypeBase.UNITY_EVENT:
					DoStepUnityEvent();
					break;
			}
		}
		
		private void DoStepUnityEvent() {
			unityEvent?.Invoke();
		}

		protected virtual void ResetByType() {
			obj = null;
			sArguments.Clear();
			bArguments.Clear();
			iArguments.Clear();
			lArguments.Clear();
			fArguments.Clear();
			objArguments.Clear();
			unityEvent = null;
			tween = false;
#if LUA_BEHAVIOUR_EXIST
			luaInjectionList.Clear();
#endif
			
			switch (type) {
				case ProcessStepTypeBase.EXECUTOR:
					objArguments.Add(null);	// executors[0]
					iArguments.Add(1);	// executorCount
					iArguments.Add(0);	// executeType
					iArguments.Add(0);	// shuffleType/randomType
					break;
				case ProcessStepTypeBase.STATE_CONTROLLER:
					bArguments.Add(false);	// isRelative
					bArguments.Add(false);	// recordIndexes/loop
					bArguments.Add(false);	// random
					iArguments.Add(1);	// offset/uid/index/uids/indexes
					break;
				case ProcessStepTypeBase.PROGRESS_CONTROLLER:
					bArguments.Add(false);	// additive
					bArguments.Add(false);	// random/loop
					fArguments.Add(0);	// progress/offset/min
					fArguments.Add(1);	// max
					break;
				case ProcessStepTypeBase.CUSTOM_EVENT:
					sArguments.Add(string.Empty);	// paramName
					bArguments.Add(true);	// broadcast
					objArguments.Add(null);	// target
					break;
				
				case ProcessStepTypeBase.INSTANTIATE:
					bArguments.Add(false);	// specifyParent
					bArguments.Add(false);	// resetPos
					bArguments.Add(false);	// resetRot
					bArguments.Add(false);	// resetScale
					bArguments.Add(true);	// activeAtOnce
					objArguments.Add(null);	// parent
					objArguments.Add(null);	// posBased
					objArguments.Add(null);	// rotBased
					objArguments.Add(null);	// scaleBased
					break;
				case ProcessStepTypeBase.DESTROY:
					bArguments.Add(false);	// onlyDestroyChildren
					break;
				case ProcessStepTypeBase.PARENT:
					objArguments.Add(null);	// parent
					bArguments.Add(true);	// worldPositionStays
					iArguments.Add(-1);	// siblingIndex
					break;
				case ProcessStepTypeBase.ACTIVE:
					bArguments.Add(true);	// active
					break;
				case ProcessStepTypeBase.ENABLED:
					bArguments.Add(true);	// enabled
					break;
				
				case ProcessStepTypeBase.TRANSFORM:
					iArguments.Add(0);	// transformType
					iArguments.Add(7);	// part
					bArguments.Add(false);	// targetAsValue
					bArguments.Add(false);	// random
					bArguments.Add(false);	// relative
					bArguments.Add(false);	// uniform
					fArguments.Add(0);	// minX
					fArguments.Add(0);	// minY
					fArguments.Add(0);	// minZ
					fArguments.Add(0);	// maxX
					fArguments.Add(0);	// maxY
					fArguments.Add(0);	// maxZ
					objArguments.Add(null);	// min
					objArguments.Add(null);	// max
					break;
				case ProcessStepTypeBase.LOOK_AT:
					iArguments.Add(2);	// lookPart
					iArguments.Add(1);	// upPart
					objArguments.Add(null);	// target
					break;
				case ProcessStepTypeBase.CAMERA_ANCHOR:
					objArguments.Add(null);	// camera
					iArguments.Add(3);	// part
					fArguments.Add(0);	// xAnchor
					fArguments.Add(0);	// yAnchor
					fArguments.Add(1);	// zAnchor
					break;
				
				case ProcessStepTypeBase.AUDIO_ONE_SHOT:
					fArguments.Add(1);	// volumeScale
					break;
				case ProcessStepTypeBase.AUDIO_SOURCE_CTRL:
					iArguments.Add(0);	// ctrlType
					fArguments.Add(0.5F);	// fadeDuration
					break;
				case ProcessStepTypeBase.AUDIOS_PLAY:
					objArguments.Add(null);	// clips[0]
					iArguments.Add(1);	// clipCount
					iArguments.Add(0);	// playType
					iArguments.Add(0);	// shuffleType/randomType
					break;
				
				case ProcessStepTypeBase.ANIMATOR_PARAMETERS:
					sArguments.Add(string.Empty);	// paramName
					bArguments.Add(true);	// value/isSet
					fArguments.Add(0);	// value
					iArguments.Add(0);	// value
					iArguments.Add(0);	// toIntType
					break;
				case ProcessStepTypeBase.ANIMATOR_CONTROLLER:
					objArguments.Add(null);	// controller
					break;
				case ProcessStepTypeBase.ANIMATOR_AVATAR:
					objArguments.Add(null);	// avatar
					break;
				case ProcessStepTypeBase.ANIMATOR_APPLY_ROOT_MOTION:
					bArguments.Add(false);	// applyRootMotion
					break;
				
				case ProcessStepTypeBase.PLAYABLE_CTRL:
					iArguments.Add(0);	// ctrlType
					break;
				case ProcessStepTypeBase.PLAYABLE_GOTO:
					bArguments.Add(false);	// isPercent
					fArguments.Add(0);	// time/percent
					bArguments.Add(false);	// evaluate
					break;
				
				case ProcessStepTypeBase.DO_TWEEN_RESTART:
					bArguments.Add(true);	// includeDelay
					bArguments.Add(true);	// formHere
					break;
				case ProcessStepTypeBase.DO_TWEEN_CTRL:
					iArguments.Add(0);	// ctrlType
					break;
				case ProcessStepTypeBase.DO_TWEEN_GOTO:
					bArguments.Add(true);	// isPercent
					fArguments.Add(0);	// time/percent
					bArguments.Add(true);	// pauseIfPlaying
					bArguments.Add(true);	// includeDelay
					break;
				case ProcessStepTypeBase.DO_TWEEN_LIFE:
					iArguments.Add(0);	// lifeType
					bArguments.Add(false);	// complete
					break;
				
#if SPINE_EXIST
				case ProcessStepTypeBase.SPINE_ANIMATION_SET_VALUE:
					iArguments.Add(0);	// fieldType
					sArguments.Add(string.Empty);	// animName
					fArguments.Add(1);	// timeScale
					bArguments.Add(false);	// loop
					bArguments.Add(false);	// stop/resume
					break;
				case ProcessStepTypeBase.SPINE_ANIMATION_RESTART:
					bArguments.Add(false);	// whenIsNotStarted
					bArguments.Add(false);	// whenIsNotComplete
					break;
				case ProcessStepTypeBase.SPINE_ANIMATION_STOP:
					break;
#endif
				
#if LUA_BEHAVIOUR_EXIST
				case ProcessStepTypeBase.LUA_CODE_EXECUTE:
					sArguments.Add(string.Empty);	// luaCode
					break;
				case ProcessStepTypeBase.LUA_SET_VALUE:
					sArguments.Add(string.Empty);	// fieldName
					sArguments.Add(string.Empty);	// fieldPath
					luaInjectionList.Add(new LuaApp.Injection7());	// value
					break;
				case ProcessStepTypeBase.LUA_FUNCTION_INVOKE:
					sArguments.Add(string.Empty);	// funcName
					sArguments.Add(string.Empty);	// funcPath
					bArguments.Add(true);	// isInstanceInvoke
					break;
#endif
				
				case ProcessStepTypeBase.UNITY_EVENT:
				default:
					break;
			}
		}

		protected string GetSArgument(int index) {
			return sArguments.Count > index ? sArguments[index] : default;
		}
		protected bool GetBArgument(int index) {
			return bArguments.Count > index ? bArguments[index] : default;
		}
		protected int GetIArgument(int index) {
			return iArguments.Count > index ? iArguments[index] : default;
		}
		protected long GetLArgument(int index) {
			return lArguments.Count > index ? lArguments[index] : default;
		}
		protected float GetFArgument(int index) {
			return fArguments.Count > index ? fArguments[index] : default;
		}
		protected T GetObjArgument<T>(int index) where T : UObject {
			return objArguments.Count > index ? objArguments[index] as T : default;
		}
	}
}