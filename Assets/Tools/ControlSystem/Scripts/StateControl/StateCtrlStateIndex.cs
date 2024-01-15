/*
 * @Author: wangyun
 * @CreateTime: 2022-08-28 01:46:03 963
 * @LastEditor: wangyun
 * @EditTime: 2022-08-28 01:46:03 967
 */

namespace Control {
	public class StateCtrlStateIndex : BaseStateCtrl<int> {
		[ComponentSelect]
		public StateController target;

		protected override int TargetValue {
			get => target ? target.Index : 0;
			set {
				if (target) {
					target.Index = value;
				}
			}
		}
	}
}