/*
 * @Author: wangyun
 * @CreateTime: 2022-07-28 21:19:20 843
 * @LastEditor: wangyun
 * @EditTime: 2022-07-28 21:19:20 850
 */

using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;

namespace Control {
	public enum ProgressCtrlAudioSourceFloatType {
		VOLUME = 0,
		PITCH = 1,
		TIME = 2
	}

	[RequireComponent(typeof(AudioSource))]
	public class ProgressCtrlAudioSourceFloat : BaseProgressCtrlFloat {
		public ProgressCtrlAudioSourceFloatType type;
		
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
					case ProgressCtrlAudioSourceFloatType.VOLUME:
						return source.volume;
					case ProgressCtrlAudioSourceFloatType.PITCH:
						return source.pitch;
					case ProgressCtrlAudioSourceFloatType.TIME:
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
						case ProgressCtrlAudioSourceFloatType.VOLUME:
							m_Tweener = DOTween.To(() => source.volume, v => source.volume = v, value, tweenDuration);
							break;
						case ProgressCtrlAudioSourceFloatType.PITCH:
							m_Tweener = DOTween.To(() => source.pitch, v => source.pitch = v, value, tweenDuration);
							break;
						case ProgressCtrlAudioSourceFloatType.TIME:
							m_Tweener = DOTween.To(() => source.time, v => source.time = v, value, tweenDuration);
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
						case ProgressCtrlAudioSourceFloatType.VOLUME:
							source.volume = value;
							break;
						case ProgressCtrlAudioSourceFloatType.PITCH:
							source.pitch = value;
							break;
						case ProgressCtrlAudioSourceFloatType.TIME:
							source.time = value;
							break;
					}
				}
			}
		}
	}
}