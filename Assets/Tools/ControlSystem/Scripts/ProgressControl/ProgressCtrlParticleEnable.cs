/*
 * @Author: wangyun
 * @CreateTime: 2023-01-20 05:11:22 482
 * @LastEditor: wangyun
 * @EditTime: 2023-01-20 05:11:22 488
 */

using UnityEngine;

namespace Control {
	public enum ProgressCtrlParticleEnableType {
		EMISSION
	}
	public class ProgressCtrlParticleEnable : BaseProgressCtrlConst<bool> {
		public ProgressCtrlParticleEnableType type = ProgressCtrlParticleEnableType.EMISSION;
		
		private ParticleSystem m_Particle;
		private ParticleSystem Particle => m_Particle ? m_Particle : m_Particle = GetComponent<ParticleSystem>();

		protected override bool TargetValue {
			get {
				if (Particle) {
					switch (type) {
						case ProgressCtrlParticleEnableType.EMISSION:
							return Particle.emission.enabled;
					}
				}
				return false;
			}
			set {
				ParticleSystem.EmissionModule emission = Particle.emission;
				switch (type) {
					case ProgressCtrlParticleEnableType.EMISSION:
						emission.enabled = value;
						break;
				}
			}
		}
	}
}