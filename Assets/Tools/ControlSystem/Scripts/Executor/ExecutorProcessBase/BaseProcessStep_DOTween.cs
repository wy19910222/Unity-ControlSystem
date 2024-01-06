/*
 * @Author: wangyun
 * @CreateTime: 2022-12-15 13:04:42 503
 * @LastEditor: wangyun
 * @EditTime: 2022-12-15 13:04:42 512
 */

using System;
using System.Reflection;
using UnityEngine;
using DG.Tweening;
using DG.Tweening.Core;

namespace Control {
	public partial class BaseProcessStep {
		private void DoStepDOTweenRestart() {
			if (obj is ABSAnimationComponent doTween) {
				Tween tweener = doTween.tween;
				if (tweener != null) {
					bool includeDelay = GetBArgument(0);
					bool fromHere = GetBArgument(1);
					if (fromHere) {
						switch (doTween) {
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
								doTween.DORestart(true);
								break;
						}
					}
					tweener.Restart(includeDelay);
				}
			}
		}
		
		private void DoStepDOTweenCtrl() {
			if (obj is ABSAnimationComponent doTween) {
				Tween tweener = doTween.tween;
				if (tweener != null) {
					int ctrlType = GetIArgument(0);
					switch (ctrlType) {
						case 0:
							tweener.PlayForward();
							break;
						case 1:
							tweener.PlayBackwards();
							break;
						case 2:
							tweener.Pause();
							break;
						case 3:
							tweener.Play();
							break;
						case 4:
							if (tweener.IsPlaying()) {
								tweener.Pause();
							} else {
								tweener.Play();
							}
							break;
					}
				}
			}
		}
		
		private void DoStepDOTweenGoto() {
			if (obj is ABSAnimationComponent doTween) {
				Tween tweener = doTween.tween;
				if (tweener != null) {
					bool isPercent = GetBArgument(0) && tweener.Loops() != -1;
					float value = GetFArgument(0);
					value = isPercent ? tweener.Duration() * Mathf.Clamp01(value) : Mathf.Max(value, 0);
					bool pauseIfPlaying = GetBArgument(1);
					bool includeDelay = GetBArgument(2);
					if (value > 0) {
						tweener.Goto(value, !pauseIfPlaying && tweener.IsPlaying());
					} else {
						if (!pauseIfPlaying && tweener.IsPlaying()) {
							if (tweener.position != 0) {
								tweener.Goto(value, true);
							}
						} else {
							tweener.Rewind(includeDelay);
						}
					}
				}
			}
		}
		
		private void DoStepDOTweenLife() {
			if (obj is ABSAnimationComponent doTween) {
				int lifeType = GetIArgument(0);
				switch (lifeType) {
					case 0:
						Tween tweener = doTween.tween;
						if (tweener != null) {
							bool complete = GetBArgument(0);
							tweener.Kill(complete);
						}
						break;
					case 1:
						if (doTween.tween == null && doTween is DOTweenAnimation anim) {
							anim.CreateTween();
						}
						break;
				}
			}
		}
	}
}