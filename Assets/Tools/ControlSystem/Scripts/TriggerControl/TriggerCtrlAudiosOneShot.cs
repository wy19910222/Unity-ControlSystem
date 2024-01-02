/*
 * @Author: wangyun
 * @CreateTime: 2022-04-26 12:17:14 806
 * @LastEditor: wangyun
 * @EditTime: 2022-04-26 12:17:14 801
 */

using System.Collections.Generic;
using UnityEngine;

namespace Control {
	public class TriggerCtrlAudiosOneShot : TriggerCtrlTrigger {
		public List<AudioClip> clips = new List<AudioClip>();
		public float volumeScale = 1;

		protected override void DoTrigger() {
			foreach (var clip in clips) {
				AudioManager.Instance.Play(clip, volumeScale);
			}
		}
	}
}