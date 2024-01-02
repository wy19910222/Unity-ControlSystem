/*
 * @Author: wangyun
 * @CreateTime: 2022-07-25 12:42:57 839
 * @LastEditor: wangyun
 * @EditTime: 2022-07-25 12:42:57 844
 */

using UnityEngine.Playables;

namespace Control {
	public class TriggerCtrlPlayableTime : TriggerCtrlTrigger {
		public PlayableDirector director;
		public double time;
		public bool evaluate;

		protected override void DoTrigger() {
			if (director) {
				director.time = time;
				if (evaluate) {
					director.Evaluate();
				}
			}
		}
	}
}