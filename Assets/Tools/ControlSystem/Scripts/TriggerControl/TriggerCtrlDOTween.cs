/*
 * @Author: wangyun
 * @CreateTime: 2022-04-20 14:45:01 688
 * @LastEditor: wangyun
 * @EditTime: 2022-04-20 14:45:01 683
 */

using System;
using System.Reflection;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using DG.Tweening;
using DG.Tweening.Core;
using Sirenix.OdinInspector;

namespace Control {
	public enum TriggerCtrlDOTweenType {
		RESTART = 0,
		BACKWARDS = 1,
		GOTO = 2,
		FORWARDS = 3
	}
	
	public class TriggerCtrlDOTween : TriggerCtrlTrigger {
		public TriggerCtrlDOTweenType type = TriggerCtrlDOTweenType.RESTART;
		[Tooltip("Enabled while percent >= 0")]
		public float percent = -1;
		[ShowIf("@type != TriggerCtrlDOTweenType.BACKWARDS && percent <= 0")]
		public bool includeDelay = true;
		[ShowIf("@type == TriggerCtrlDOTweenType.RESTART")]
		public bool fromHere;
		[ShowIf("@type == TriggerCtrlDOTweenType.GOTO")]
		public bool pauseIfPlaying = true;
		[ComponentSelect]
		public List<ABSAnimationComponent> tweenAnims = new List<ABSAnimationComponent>();

		private void Reset() {
			tweenAnims.Clear();
			tweenAnims.AddRange(GetComponents<ABSAnimationComponent>());
		}
		
		protected override void DoTrigger() {
			foreach (var tweenAnim in tweenAnims) {
				Tween tween = tweenAnim.tween;
				if (tween != null) {
					switch (type) {
						case TriggerCtrlDOTweenType.FORWARDS:
							PlayForwards(tween);
							break;
						case TriggerCtrlDOTweenType.BACKWARDS:
							PlayBackwards(tween);
							break;
						case TriggerCtrlDOTweenType.GOTO:
							Goto(tween);
							break;
						case TriggerCtrlDOTweenType.RESTART:
							Restart(tween, tweenAnim);
							break;
					}
				}
			}
		}

		private void Restart([NotNull]Tween tween, ABSAnimationComponent tweenAnim) {
			var duration = tween.Duration();
			var value = float.IsPositiveInfinity(duration) ? percent : duration * percent;
			if (fromHere) {
				switch (tweenAnim) {
					case DOTweenAnimation anim:
						if (anim.isRelative) {
							MethodInfo mi = typeof(DOTweenAnimation).GetMethod("ReEvaluateRelativeTween",
									BindingFlags.Instance | BindingFlags.NonPublic);
							mi?.Invoke(anim, Array.Empty<object>());
						}
						break;
					case DOTweenPath path:
						if (path.relative && !path.isLocal) {
							MethodInfo mi = typeof(DOTweenPath).GetMethod("ReEvaluateRelativeTween",
									BindingFlags.Instance | BindingFlags.NonPublic);
							mi?.Invoke(path, Array.Empty<object>());
						}
						break;
					default:
						// 只是借这个接口实现fromHere，目前不会走到这里
						tweenAnim.DORestart(true);
						break;
				}
			}
			if (percent > 0) {
				tween.Goto(value);
				tween.PlayForward();
			} else {
				tween.Restart(includeDelay);
			}
		}

		private void PlayForwards([NotNull]Tween tween) {
			var duration = tween.Duration();
			var value = float.IsPositiveInfinity(duration) ? percent : duration * percent;
			if (percent > 0) {
				tween.Goto(value);
			} else {
				tween.Rewind(includeDelay);
			}
			tween.PlayForward();
		}

		private void PlayBackwards([NotNull]Tween tween) {
			var duration = tween.Duration();
			var value = float.IsPositiveInfinity(duration) ? percent : duration * percent;
			if (percent >= 0) {
				tween.Goto(value);
			}
			tween.PlayBackwards();
		}

		private void Goto([NotNull]Tween tween) {
			var duration = tween.Duration();
			var value = float.IsPositiveInfinity(duration) ? percent : duration * percent;
			if (percent > 0) {
				tween.Goto(value, !pauseIfPlaying && tween.IsPlaying());
			} else {
				if (!pauseIfPlaying && tween.IsPlaying()) {
					if (tween.position != 0) {
						tween.Goto(value, true);
					}
				} else {
					tween.Rewind(includeDelay);
				}
			}
		}
	}
}