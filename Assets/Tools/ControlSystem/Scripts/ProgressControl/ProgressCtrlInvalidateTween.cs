/*
 * @Author: wangyun
 * @CreateTime: 2022-06-26 03:33:50 211
 * @LastEditor: wangyun
 * @EditTime: 2022-09-16 14:08:22 886
 */

using UnityEngine;

namespace Control {
	public class ProgressCtrlInvalidateTween : BaseStateCtrl<bool> {
		[ComponentSelect(true, typeof(StateController), typeof(ProgressController))]
		public Behaviour target;

		protected override bool TargetValue {
			get {
				switch (target) {
					case StateController state:
						return state.InvalidateTween;
					case ProgressController progress:
						return progress.InvalidateTween;
				}
				return false;
			}
			set {
				switch (target) {
					case StateController state:
						state.InvalidateTween = value;
						break;
					case ProgressController progress:
						progress.InvalidateTween = value;
						break;
				}
			}
		}
	}
}