/*
 * @Author: wangyun
 * @CreateTime: 2022-08-29 17:32:10 717
 * @LastEditor: wangyun
 * @EditTime: 2022-08-29 17:32:10 721
 */

using System.Reflection;
using UnityEngine;
using DG.Tweening;
using DG.Tweening.Core;

namespace Control {
	public enum ProgressCtrlDOTweenFloatType {
		DURATION,
		DELAY,
	}
	
	public class ProgressCtrlDOTweenFloat : BaseProgressCtrlFloat {
		[ComponentSelect]
		public ABSAnimationComponent target;
		public ProgressCtrlDOTweenFloatType type = ProgressCtrlDOTweenFloatType.DURATION;

		protected override float TargetValue {
			get {
				switch (target) {
					case DOTweenAnimation anim:
						switch (type) {
							case ProgressCtrlDOTweenFloatType.DURATION:
								return anim.duration;
							case ProgressCtrlDOTweenFloatType.DELAY:
								return anim.delay;
						}
						break;
					case DOTweenPath path:
						switch (type) {
							case ProgressCtrlDOTweenFloatType.DURATION:
								return path.duration;
							case ProgressCtrlDOTweenFloatType.DELAY:
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
							case ProgressCtrlDOTweenFloatType.DURATION: {
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
							case ProgressCtrlDOTweenFloatType.DELAY: {
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
							case ProgressCtrlDOTweenFloatType.DURATION: {
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
							case ProgressCtrlDOTweenFloatType.DELAY: {
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