/*
 * @Author: wangyun
 * @CreateTime: 2023-01-20 05:11:22 482
 * @LastEditor: wangyun
 * @EditTime: 2023-01-20 05:11:22 488
 */

using UnityEngine;

namespace Control {
	public enum StateCtrlParticleEnableType {
		EMISSION
	}
	public class StateCtrlParticleEnable : BaseStateCtrl<bool> {
		public StateCtrlParticleEnableType type = StateCtrlParticleEnableType.EMISSION;
		
		private ParticleSystem m_Particle;
		private ParticleSystem Particle => m_Particle ? m_Particle : m_Particle = GetComponent<ParticleSystem>();

		protected override bool TargetValue {
			get {
				if (Particle) {
					switch (type) {
						case StateCtrlParticleEnableType.EMISSION:
							return Particle.emission.enabled;
					}
				}
				return false;
			}
			set {
				ParticleSystem.EmissionModule emission = Particle.emission;
				switch (type) {
					case StateCtrlParticleEnableType.EMISSION:
						emission.enabled = value;
						break;
				}
			}
		}
	}
}