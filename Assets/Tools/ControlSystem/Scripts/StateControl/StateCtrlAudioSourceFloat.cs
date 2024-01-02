/*
 * @Author: wangyun
 * @CreateTime: 2022-07-28 21:22:07 921
 * @LastEditor: wangyun
 * @EditTime: 2022-07-28 21:22:07 929
 */

using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;

namespace Control {
	public enum StateCtrlAudioSourceFloatType {
		VOLUME = 0,
		PITCH = 1,
		TIME = 2
	}

	[RequireComponent(typeof(AudioSource))]
	public class StateCtrlAudioSourceFloat : BaseStateCtrl<float> {
		public StateCtrlAudioSourceFloatType type;
		
		public bool tween;
		[HideIf("@!this.tween")]
		public float tweenDelay;
		[HideIf("@!this.tween")]
		public float tweenDuration = 0.3F;
		[HideIf("@!this.tween")]
		public Ease tweenEase = Ease.OutQuad;
		[HideIf("@!tween || tweenEase != Ease.INTERNAL_Custom")]
		public AnimationCurve tweenEaseCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));

		private Tweener m_Tweener;

		protected override float TargetValue {
			get {
				AudioSource source = GetComponent<AudioSource>();
				switch (type) {
					case StateCtrlAudioSourceFloatType.VOLUME:
						return source.volume;
					case StateCtrlAudioSourceFloatType.PITCH:
						return source.pitch;
					case StateCtrlAudioSourceFloatType.TIME:
						return source.time;
				}
				return 0;
			}
			set {
				if (m_Tweener != null) {
					m_Tweener.Kill();
					m_Tweener = null;
				}
#if UNITY_EDITOR
				if (tween && !controller.InvalidateTween && Application.isPlaying) {
#else
				if (tween && !controller.InvalidateTween) {
#endif
					AudioSource source = GetComponent<AudioSource>();
					switch (type) {
						case StateCtrlAudioSourceFloatType.VOLUME:
							m_Tweener = DOTween.To(
								() => source.volume,
								v => source.volume = v,
								value,
								tweenDuration
							);
							break;
						case StateCtrlAudioSourceFloatType.PITCH:
							m_Tweener = DOTween.To(
								() => source.pitch,
								v => source.pitch = v,
								value,
								tweenDuration
							);
							break;
						case StateCtrlAudioSourceFloatType.TIME:
							m_Tweener = DOTween.To(
								() => source.time,
								v => source.time = v,
								value,
								tweenDuration
							);
							break;
					}
					if (m_Tweener != null) {
						if (tweenEase == Ease.INTERNAL_Custom) {
							m_Tweener.SetEase(tweenEaseCurve);
						} else {
							m_Tweener.SetEase(tweenEase);
						}
						m_Tweener.SetDelay(tweenDelay).OnComplete(() => m_Tweener = null);
					}
				} else {
					AudioSource source = GetComponent<AudioSource>();
					switch (type) {
						case StateCtrlAudioSourceFloatType.VOLUME:
							source.volume = value;
							break;
						case StateCtrlAudioSourceFloatType.PITCH:
							source.pitch = value;
							break;
						case StateCtrlAudioSourceFloatType.TIME:
							source.time = value;
							break;
					}
				}
			}
		}
	}
}