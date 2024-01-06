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
	public class BaseListener : MonoBehaviour {
		public string title;
		[ComponentSelect, ShowIf("@StateControllerEnabled")]
		public List<StateController> stateControllers = new List<StateController>();
		[ComponentSelect, ShowIf("@ProgressControllerEnabled")]
		public List<ProgressController> progressControllers = new List<ProgressController>();
		[ComponentSelect, ShowIf("@ExecutorEnabled")]
		public List<BaseExecutor> executors = new List<BaseExecutor>();
		
		private Action<int> m_OnStateChange;
		private Action<float> m_OnProgressChange;
		private Action m_OnExecute;

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

		protected virtual void Execute() {
			foreach (var executor in executors) {
				executor.Execute();
			}
			m_OnExecute?.Invoke();
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
		
		protected virtual bool ExecutorEnabled => true;
		public void OnExecute(Action action) {
			m_OnExecute += action;
		}
		public void OffExecute(Action action) {
			m_OnExecute -= action;
		}
	}
}