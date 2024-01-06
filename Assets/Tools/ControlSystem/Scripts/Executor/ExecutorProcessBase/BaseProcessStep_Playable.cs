/*
 * @Author: wangyun
 * @CreateTime: 2022-12-15 13:04:32 734
 * @LastEditor: wangyun
 * @EditTime: 2022-12-15 13:04:32 737
 */

using UnityEngine;
using UnityEngine.Playables;

namespace Control {
	public partial class BaseProcessStep {
		private void DoStepPlayableCtrl() {
			if (obj is PlayableDirector director) {
				int ctrlType = GetIArgument(0);
				switch (ctrlType) {
					case 0:
						director.Play();
						break;
					case 1:
						director.Stop();
						break;
					case 2:
						director.Pause();
						break;
					case 3:
						director.Resume();
						break;
					case 4:
						switch (director.state) {
							case PlayState.Paused:
								director.Resume();
								break;
							case PlayState.Playing:
								director.Pause();
								break;
						}
						break;
				}
			}
		}
		
		private void DoStepPlayableGoto() {
			if (obj is PlayableDirector director) {
				bool isPercent = GetBArgument(0);
				float value = GetFArgument(0);
				director.time = isPercent ? director.duration * Mathf.Clamp01(value) : Mathf.Max(value, 0);
				bool evaluate = GetBArgument(1);
				if (evaluate) {
					director.Evaluate();
				}
			}
		}
	}
}