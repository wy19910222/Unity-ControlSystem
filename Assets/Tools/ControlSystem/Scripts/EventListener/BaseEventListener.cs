/*
 * @Author: wangyun
 * @CreateTime: 2022-05-28 22:37:12 250
 * @LastEditor: wangyun
 * @EditTime: 2022-05-28 22:37:12 254
 */

using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Control {
	public class BaseEventListener : MonoBehaviour {
		public string title;
		[ComponentSelect, ShowIf("@StateControllerEnabled")]
		public List<StateController> stateControllers = new List<StateController>();
		[ComponentSelect, ShowIf("@ProgressControllerEnabled")]
		public List<ProgressController> progressControllers = new List<ProgressController>();
		[ComponentSelect, ShowIf("@TriggerEnabled")]
		public List<BaseTrigger> triggers = new List<BaseTrigger>();
		
		private Action<int> m_OnStateChange;
		private Action<float> m_OnProgressChange;
		private Action m_OnTrigger;

		protected virtual void SetState(int index) {
			foreach (var stateController in stateControllers) {
				stateController.Index = index;
			}
			m_OnStateChange?.Invoke(index);
		}

		protected virtual void SetProgress(float progress) {
			foreach (var progressController in progressControllers) {
				progressController.Progress = progress;
			}
			m_OnProgressChange?.Invoke(progress);
		}

		protected virtual void Trigger() {
			foreach (var trigger in triggers) {
				trigger.Trigger();
			}
			m_OnTrigger?.Invoke();
		}

		protected virtual bool StateControllerEnabled => true;
		public void OnStateChange(Action<int> action) {
			m_OnStateChange += action;
		}
		public void OffStateChange(Action<int> action) {
			m_OnStateChange -= action;
		}
		
		protected virtual bool ProgressControllerEnabled => true;
		public void OnProgressChange(Action<float> action) {
			m_OnProgressChange += action;
		}
		public void OffProgressChange(Action<float> action) {
			m_OnProgressChange -= action;
		}
		
		protected virtual bool TriggerEnabled => true;
		public void OnTrigger(Action action) {
			m_OnTrigger += action;
		}
		public void OffTrigger(Action action) {
			m_OnTrigger -= action;
		}
	}
}