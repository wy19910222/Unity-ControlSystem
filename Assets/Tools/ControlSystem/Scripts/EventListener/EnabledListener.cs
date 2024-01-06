/*
 * @Author: wangyun
 * @CreateTime: 2022-06-28 17:24:50 031
 * @LastEditor: wangyun
 * @EditTime: 2022-06-28 17:24:50 026
 */

namespace Control {
	public enum EnabledType {
		ON_DISABLE = 0,
		ON_ENABLE = 1
	}
	
	public class EnabledListener : BaseListener {
		public EnabledType type = EnabledType.ON_ENABLE;
		
		protected override bool ExecutorEnabled => true;

		private void OnEnable() {
			if (type == EnabledType.ON_ENABLE) {
				Execute();
			}
		}

		private void OnDisable() {
			if (type == EnabledType.ON_DISABLE) {
				Execute();
			}
		}
	}
}