/*
 * @Author: wangyun
 * @CreateTime: 2022-06-28 17:24:50 031
 * @LastEditor: wangyun
 * @EditTime: 2022-06-28 17:24:50 026
 */

namespace Control {
	public enum EnabledEventType {
		ON_DISABLE = 0,
		ON_ENABLE = 1
	}
	
	public class EnabledEventListener : BaseEventListener {
		public EnabledEventType type = EnabledEventType.ON_ENABLE;
		
		private void OnEnable() {
			if (type == EnabledEventType.ON_ENABLE) {
				Execute();
			}
		}

		private void OnDisable() {
			if (type == EnabledEventType.ON_DISABLE) {
				Execute();
			}
		}
	}
}