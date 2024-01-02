/*
 * @Author: wangyun
 * @CreateTime: 2022-07-22 00:23:33 315
 * @LastEditor: wangyun
 * @EditTime: 2022-07-22 00:23:33 319
 */

using System;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

using URandom = UnityEngine.Random;

namespace Control {
	public enum TriggerCtrlTriggerOneType {
		ONE_BY_ONE = 0,
		RANDOM = 1,
	}
	
	[Obsolete("TriggerCtrlTriggerOne has been deprecated. Use TriggerCtrlProcess instead")]
	[AddComponentMenu("")]
	public class TriggerCtrlTriggerOne : TriggerCtrlTrigger {
		public TriggerCtrlTriggerOneType type = TriggerCtrlTriggerOneType.ONE_BY_ONE;
		[ShowIf("@type == TriggerCtrlTriggerOneType.ONE_BY_ONE")]
		public bool shuffleOnAwake;
		[ShowIf("@type == TriggerCtrlTriggerOneType.RANDOM")]
		public TriggerCtrlTriggerSeveralRandomType randomType;
		[ComponentSelect(true)]
		public List<BaseTriggerCtrl> triggers = new List<BaseTriggerCtrl>();

		private int m_PrevIndex = -1;

		private void Awake() {
			if (type == TriggerCtrlTriggerOneType.ONE_BY_ONE && shuffleOnAwake) {
				Shuffle();
			}
		}
		
		protected override void DoTrigger() {
			switch (type) {
				case TriggerCtrlTriggerOneType.ONE_BY_ONE:
					m_PrevIndex = (m_PrevIndex + 1) % triggers.Count;
					break;
				case TriggerCtrlTriggerOneType.RANDOM:
					if (randomType == TriggerCtrlTriggerSeveralRandomType.NO_REPEAT) {
						int index = URandom.Range(0, triggers.Count - 1);
						m_PrevIndex = index < m_PrevIndex ? index : index + 1;
					} else {
						m_PrevIndex = URandom.Range(0, triggers.Count);
					}
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(type));
			}
			triggers[m_PrevIndex]?.Trigger();
		}

		public void Shuffle() {
			for (int i = triggers.Count - 1; i > 0; --i) {
				int j = URandom.Range(0, i + 1);
				if (j != i) {
					(triggers[i], triggers[j]) = (triggers[j], triggers[i]);
				}
			}
		}
	}
}