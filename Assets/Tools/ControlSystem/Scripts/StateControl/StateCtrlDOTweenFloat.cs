/*
 * @Author: wangyun
 * @CreateTime: 2022-08-29 17:17:30 780
 * @LastEditor: wangyun
 * @EditTime: 2022-08-29 17:17:30 784
 */

using System.Reflection;
using UnityEngine;
using DG.Tweening;
using DG.Tweening.Core;

namespace Control {
	public enum StateCtrlDOTweenFloatType {
		DURATION,
		DELAY,
	}
	
	public class StateCtrlDOTweenFloat : BaseStateCtrl<float> {
		[ComponentSelect]
		public ABSAnimationComponent target;
		public StateCtrlDOTweenFloatType type = StateCtrlDOTweenFloatType.DURATION;

		protected override float TargetValue {
			get {
				switch (target) {
					case DOTweenAnimation anim:
						switch (type) {
							case StateCtrlDOTweenFloatType.DURATION:
								return anim.duration;
							case StateCtrlDOTweenFloatType.DELAY:
								return anim.delay;
						}
						break;
					case DOTweenPath path:
						switch (type) {
							case StateCtrlDOTweenFloatType.DURATION:
								return path.duration;
							case StateCtrlDOTweenFloatType.DELAY:
								return path.delay;
						}
						break;
				}
				return 0;
			}
			set {
				switch (target) {
					case DOTweenAnimation anim:
						switch (type) {
							case StateCtrlDOTweenFloatType.DURATION: {
								anim.duration = value;
								Tween tween = anim.tween;
								if (tween != null) {
									float duration = tween.Duration(false);
									if (Mathf.Abs(duration - value) > Mathf.Epsilon) {
										float newPosition = duration == 0 ? 0 : tween.position / duration * value;
										FieldInfo fi = typeof(Tween).GetField("duration", BindingFlags.Instance | BindingFlags.NonPublic);
										fi?.SetValue(tween, value);
										tween.Goto(newPosition, tween.IsPlaying());
									}
								}
								break;
							}
							case StateCtrlDOTweenFloatType.DELAY: {
								anim.delay = value;
								Tween tween = anim.tween;
								if (tween != null) {
									float delay = tween.Delay();
									if (Mathf.Abs(delay - value) > Mathf.Epsilon) {
										FieldInfo fi1 = typeof(Tween).GetField("delay", BindingFlags.Instance | BindingFlags.NonPublic);
										fi1?.SetValue(tween, value);
										FieldInfo fi2 = typeof(Tween).GetField("delayComplete", BindingFlags.Instance | BindingFlags.NonPublic);
										fi2?.SetValue(tween, delay <= 0);
									}
								}
								break;
							}
						}
						break;
					case DOTweenPath path:
						switch (type) {
							case StateCtrlDOTweenFloatType.DURATION: {
								path.duration = value;
								Tween tween = path.tween;
								if (tween != null) {
									float duration = tween.Duration(false);
									if (Mathf.Abs(duration - value) > Mathf.Epsilon) {
										float newPosition = duration == 0 ? 0 : tween.position / duration * value;
										FieldInfo fi = typeof(Tween).GetField("duration", BindingFlags.Instance | BindingFlags.NonPublic);
										fi?.SetValue(tween, value);
										tween.Goto(newPosition, tween.IsPlaying());
									}
								}
								break;
							}
							case StateCtrlDOTweenFloatType.DELAY: {
								path.delay = value;
								Tween tween = path.tween;
								if (tween != null) {
									float delay = tween.Delay();
									if (Mathf.Abs(delay - value) > Mathf.Epsilon) {
										FieldInfo fi1 = typeof(Tween).GetField("delay", BindingFlags.Instance | BindingFlags.NonPublic);
										fi1?.SetValue(tween, value);
										FieldInfo fi2 = typeof(Tween).GetField("delayComplete", BindingFlags.Instance | BindingFlags.NonPublic);
										fi2?.SetValue(tween, delay <= 0);
									}
								}
								break;
							}
						}
						break;
				}
			}
		}
	}
}