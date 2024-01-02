/*
 * @Author: wangyun
 * @CreateTime: 2022-07-28 21:19:11 853
 * @LastEditor: wangyun
 * @EditTime: 2022-07-28 21:19:11 857
 */

using UnityEngine;

namespace Control {
	public enum ProgressCtrlAudioSourceBoolType {
		MUTE = 0,
		LOOP = 1
	}

	[RequireComponent(typeof(AudioSource))]
	public class ProgressCtrlAudioSourceBool : BaseProgressCtrlConst<bool> {
		public ProgressCtrlAudioSourceBoolType type;

		protected override bool TargetValue {
			get {
				AudioSource source = GetComponent<AudioSource>();
				switch (type) {
					case ProgressCtrlAudioSourceBoolType.MUTE:
						return source.mute;
					case ProgressCtrlAudioSourceBoolType.LOOP:
						return source.loop;
				}
				return false;
			}
			set {
				AudioSource source = GetComponent<AudioSource>();
				switch (type) {
					case ProgressCtrlAudioSourceBoolType.MUTE:
						source.mute = value;
						break;
					case ProgressCtrlAudioSourceBoolType.LOOP:
						source.loop = value;
						break;
				}
			}
		}
	}
}