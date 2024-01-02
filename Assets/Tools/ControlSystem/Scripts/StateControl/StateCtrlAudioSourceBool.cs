/*
 * @Author: wangyun
 * @CreateTime: 2022-07-28 21:20:46 252
 * @LastEditor: wangyun
 * @EditTime: 2022-09-16 14:16:00 283
 */

using UnityEngine;

namespace Control {
	public enum StateCtrlAudioSourceBoolType {
		MUTE = 0,
		LOOP = 1
	}

	[RequireComponent(typeof(AudioSource))]
	public class StateCtrlAudioSourceBool : BaseStateCtrl<bool> {
		public StateCtrlAudioSourceBoolType type;

		protected override bool TargetValue {
			get {
				AudioSource source = GetComponent<AudioSource>();
				switch (type) {
					case StateCtrlAudioSourceBoolType.MUTE:
						return source.mute;
					case StateCtrlAudioSourceBoolType.LOOP:
						return source.loop;
				}
				return false;
			}
			set {
				AudioSource source = GetComponent<AudioSource>();
				switch (type) {
					case StateCtrlAudioSourceBoolType.MUTE:
						source.mute = value;
						break;
					case StateCtrlAudioSourceBoolType.LOOP:
						source.loop = value;
						break;
				}
			}
		}
	}
}