/*
 * @Author: wangyun
 * @CreateTime: 2022-05-28 20:03:04 748
 * @LastEditor: wangyun
 * @EditTime: 2022-05-28 20:03:04 752
 */

using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;

namespace Control {
	public enum TriggerCtrlDOTweenStatusType {
		PAUSE = 0,
		RESUME = 1,
		TOGGLE = 2,
		KILL = 3,
	}
	
	public class TriggerCtrlDOTweenExt : TriggerCtrlTrigger {
		public TriggerCtrlDOTweenStatusType type = TriggerCtrlDOTweenStatusType.PAUSE;
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
						case TriggerCtrlDOTweenStatusType.PAUSE:
							tween.Pause();
							break;
						case TriggerCtrlDOTweenStatusType.RESUME:
							tween.Play();
							break;
						case TriggerCtrlDOTweenStatusType.TOGGLE:
							if (tween.IsPlaying()) {
								tween.Pause();
							} else {
								tween.Play();
							}
							break;
						case TriggerCtrlDOTweenStatusType.KILL:
							tween.Kill();
							break;
					}
				}
			}
		}
	}
}