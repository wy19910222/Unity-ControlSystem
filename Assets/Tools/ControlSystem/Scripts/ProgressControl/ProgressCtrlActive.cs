/*
 * @Author: wangyun
 * @CreateTime: 2022-04-18 17:40:11 981
 * @LastEditor: wangyun
 * @EditTime: 2022-04-18 17:40:11 976
 */

namespace Control {
	public class ProgressCtrlActive : BaseProgressCtrlConst<bool> {
		protected override bool TargetValue {
			get => gameObject.activeSelf;
			set => gameObject.SetActive(value);
		}
	}
}