/*
 * @Author: wangyun
 * @CreateTime: 2022-07-28 21:23:07 764
 * @LastEditor: wangyun
 * @EditTime: 2022-07-28 21:23:07 784
 */

using UnityEngine;

namespace Control {
	[RequireComponent(typeof(AudioSource))]
	public class StateCtrlAudioSourceClip : BaseStateCtrl<AudioClip> {
		protected override AudioClip TargetValue {
			get => GetComponent<AudioSource>().clip;
			set {
				AudioSource source = GetComponent<AudioSource>();
				if (source.clip != value) {
					bool isPlaying = source.isPlaying;
					source.clip = value;
					if (isPlaying) {
						source.Play();
					}
				}
			}
		}
	}
}