/*
 * @Author: wangyun
 * @CreateTime: 2022-12-07 16:00:55 674
 * @LastEditor: wangyun
 * @EditTime: 2022-12-07 16:00:55 678
 */

using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

using URandom = UnityEngine.Random;

namespace Control {
	public enum TriggerCtrlTriggerSeveralType {
		ONE_BY_ONE = 0,
		RANDOM = 1,
	}
	public enum TriggerCtrlTriggerSeveralShuffleType {
		NONE = 0,
		ONCE = 1,
		EACH_LOOP = 2,
	}
	public enum TriggerCtrlTriggerSeveralRandomType {
		NONE = 0,
		NO_REPEAT = 1,
	}
	
	[Obsolete("TriggerCtrlTriggerSeveral has been deprecated. Use TriggerCtrlProcess instead")]
	[AddComponentMenu("")]
	public class TriggerCtrlTriggerSeveral : TriggerCtrlTrigger {
		public int triggerCount = 1;
		public TriggerCtrlTriggerSeveralType type = TriggerCtrlTriggerSeveralType.ONE_BY_ONE;
		[ShowIf("@type == TriggerCtrlTriggerSeveralType.ONE_BY_ONE")]
		public TriggerCtrlTriggerSeveralShuffleType shuffleType;
		[ShowIf("@type == TriggerCtrlTriggerSeveralType.RANDOM && triggerCount <= triggers.Count / 2")]
		public TriggerCtrlTriggerSeveralRandomType randomType;
		[ComponentSelect(true)]
		public List<BaseTriggerCtrl> triggers = new List<BaseTriggerCtrl>();

		private readonly List<int> m_PrevIndexList = new List<int>();
		private bool m_Shuffled;
		
		protected override void DoTrigger() {
			List<BaseTriggerCtrl> triggerList = new List<BaseTriggerCtrl>();
			switch (type) {
				case TriggerCtrlTriggerSeveralType.ONE_BY_ONE:
					int nextIndex = m_PrevIndexList.Count > 0 ? m_PrevIndexList[m_PrevIndexList.Count - 1] + 1 : 0;
					switch (shuffleType) {
						case TriggerCtrlTriggerSeveralShuffleType.ONCE:
							if (!m_Shuffled) {
								m_Shuffled = true;
								Shuffle();
							}
							break;
						case TriggerCtrlTriggerSeveralShuffleType.EACH_LOOP:
							if (nextIndex % triggers.Count == 0) {
								m_Shuffled = true;
								Shuffle();
							}
							break;
					}
					m_PrevIndexList.Clear();
					for (int i = 0, totalCount = triggers.Count, count = Mathf.Min(triggerCount, totalCount); i < count; ++i) {
						int index = nextIndex + i;
						if (index >= totalCount) {
							index -= totalCount;
						}
						m_PrevIndexList.Add(index);
						triggerList.Add(triggers[index]);
					}
					break;
				case TriggerCtrlTriggerSeveralType.RANDOM:
					List<int> indexList = new List<int>();
					for (int i = 0, prevIndexCount = m_PrevIndexList.Count, totalCount = triggers.Count; i < totalCount; ++i) {
						if (randomType == TriggerCtrlTriggerSeveralRandomType.NONE || prevIndexCount + triggerCount > totalCount || !m_PrevIndexList.Contains(i)) {
							indexList.Add(i);
						}
					}
					m_PrevIndexList.Clear();
					for (int i = 0, count = Mathf.Min(triggerCount, indexList.Count); i < count; ++i) {
						int indexIndex = URandom.Range(0, indexList.Count);
						int index = indexList[indexIndex];
						indexList.RemoveAt(indexIndex);
						m_PrevIndexList.Add(index);
						triggerList.Add(triggers[index]);
					}
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(type));
			}
			foreach (var trigger in triggerList) {
				if (trigger) {
					trigger.Trigger();
				}
			}
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