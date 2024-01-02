/*
 * @Author: wangyun
 * @CreateTime: 2023-01-20 04:45:21 359
 * @LastEditor: wangyun
 * @EditTime: 2023-01-20 04:45:21 366
 */

using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;

namespace Control {
	public enum ProgressCtrlParticleEmissionRateType {
		RATE_OVER_TIME,
		RATE_OVER_DISTANCE = 1,
	}
	public class ProgressCtrlParticleEmissionRate : BaseProgressCtrlMinMaxCurve {
		public ProgressCtrlParticleEmissionRateType type = ProgressCtrlParticleEmissionRateType.RATE_OVER_TIME;
		
		public bool tween;
		[HideIf("@!this.tween")]
		public float tweenDelay;
		[HideIf("@!this.tween")]
		public float tweenDuration = 0.3F;
		[HideIf("@!this.tween")]
		public Ease tweenEase = Ease.OutQuad;
		[HideIf("@!tween || tweenEase != Ease.INTERNAL_Custom")]
		public AnimationCurve tweenEaseCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));

		private Tween m_Tweener;
		
		private ParticleSystem m_Particle;
		private ParticleSystem Particle => m_Particle ? m_Particle : m_Particle = GetComponent<ParticleSystem>();

		protected override ParticleSystem.MinMaxCurve TargetValue {
			get => Rate;
			set {
				m_Tweener.Kill();
				m_Tweener = null;
				
#if UNITY_EDITOR
				if (tween && !controller.InvalidateTween && Application.isPlaying) {
#else
				if (tween && !controller.InvalidateTween) {
#endif
					ParticleSystem.MinMaxCurve startValue = Rate;
					float time = 0;
					m_Tweener = DOTween.To(
							() => time,
							v => Rate = CurveLerpUtils.MinMaxCurveLerpUnclamped(startValue, value, time = v),
							1,
							tweenDuration
					);
					if (tweenEase == Ease.INTERNAL_Custom) {
						m_Tweener.SetEase(tweenEaseCurve);
					} else {
						m_Tweener.SetEase(tweenEase);
					}
					m_Tweener.SetDelay(tweenDelay).OnComplete(() => m_Tweener = null);
				} else {
					Rate = value;
				}
			}
		}

		protected ParticleSystem.MinMaxCurve Rate {
			get {
				if (Particle) {
					switch (type) {
						case ProgressCtrlParticleEmissionRateType.RATE_OVER_TIME:
							return Particle.emission.rateOverTime;
						case ProgressCtrlParticleEmissionRateType.RATE_OVER_DISTANCE:
							return Particle.emission.rateOverDistance;
					}
				}
				return 0;
			}
			set {
				if (Particle) {
					ParticleSystem.EmissionModule emission = Particle.emission;
					switch (type) {
						case ProgressCtrlParticleEmissionRateType.RATE_OVER_TIME:
							emission.rateOverTime = value;
							break;
						case ProgressCtrlParticleEmissionRateType.RATE_OVER_DISTANCE:
							emission.rateOverDistance = value;
							break;
					}
				}
			}
		}
	}
}