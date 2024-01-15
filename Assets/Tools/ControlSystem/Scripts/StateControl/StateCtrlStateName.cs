/*
 * @Author: wangyun
 * @CreateTime: 2022-08-28 01:46:03 963
 * @LastEditor: wangyun
 * @EditTime: 2022-08-28 01:46:03 967
 */

namespace Control {
	public class StateCtrlStateName : BaseStateCtrl<string> {
		[ComponentSelect]
		public StateController target;

		protected override string TargetValue {
			get => target ? target.State : null;
			set {
				if (target) {
					target.State = value;
				}
			}
		}
	}
}