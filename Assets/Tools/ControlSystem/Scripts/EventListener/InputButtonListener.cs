/*
 * @Author: wangyun
 * @CreateTime: 2024-01-08 01:34:38 562
 * @LastEditor: wangyun
 * @EditTime: 2024-01-08 01:34:38 572
 */

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Control {
	public class InputButtonListener : MonoBehaviour {
		public string title;
		[InputAxesSelect]
		public string buttonName;
		[ComponentSelect]
		public List<StateController> stateControllers = new List<StateController>() { null };
		[ComponentSelect]
		public List<BaseExecutor> downExecutors = new List<BaseExecutor>() { null };
		[ComponentSelect]
		public List<BaseExecutor> upExecutors = new List<BaseExecutor>() { null };
		
		private bool m_IsHolding;
		private Action m_OnDown;
		private Action m_OnUp;
		
		private void Update() {
			bool isHolding = Input.GetButton(buttonName);
			int stateIndex = isHolding ? 1 : 0;
			foreach (var stateController in stateControllers) {
				if (stateController) {
					stateController.Index = stateIndex;
				}
			}
			if (isHolding != m_IsHolding) {
				if (isHolding) {
					foreach (var executor in downExecutors) {
						if (executor) {
							executor.Execute();
						}
					}
					m_OnDown?.Invoke();
				} else {
					foreach (var executor in upExecutors) {
						if (executor) {
							executor.Execute();
						}
					}
					m_OnUp?.Invoke();
				}
			}
		}

		public void OnDown(Action onDown) {
			m_OnDown += onDown;
		}

		public void OffDown(Action onDown) {
			m_OnDown -= onDown;
		}

		public void OnUp(Action onUp) {
			m_OnUp += onUp;
		}

		public void OffUp(Action onUp) {
			m_OnUp -= onUp;
		}
	}
}