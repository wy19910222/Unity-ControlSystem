/*
 * @Author: wangyun
 * @CreateTime: 2022-05-28 23:34:06 199
 * @LastEditor: wangyun
 * @EditTime: 2022-05-28 23:34:06 203
 */

namespace Control {
	public enum VisibleEventType {
		ON_BECAME_INVISIBLE = 0,
		ON_BECAME_VISIBLE = 1
	}
	
	public class VisibleEventListener : BaseEventListener {
		public VisibleEventType type = VisibleEventType.ON_BECAME_INVISIBLE;
		
		private void OnBecameVisible() {
			if (enabled && type == VisibleEventType.ON_BECAME_INVISIBLE) {
				Execute();
			}
		}

		private void OnBecameInvisible() {
			if (enabled && type == VisibleEventType.ON_BECAME_VISIBLE) {
				Execute();
			}
		}
	}
}