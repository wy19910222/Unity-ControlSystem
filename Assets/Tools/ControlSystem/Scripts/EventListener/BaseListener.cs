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

		protected virtual bool StateControllerEnabled => true;
		protected virtual bool ProgressControllerEnabled => true;
		protected virtual bool ExecutorEnabled => true;

		protected virtual void SetState(int stateIndex) {
			foreach (var stateController in stateControllers) {
				stateController.Index = stateIndex;
			}
		}

		protected virtual void SetState(string stateName) {
			foreach (var stateController in stateControllers) {
				stateController.State = stateName;
			}
		}

		protected virtual void SetProgress(float progress) {
			foreach (var progressController in progressControllers) {
				progressController.Progress = progress;
			}
		}

		protected virtual void Execute() {
			foreach (var executor in executors) {
				executor.Execute();
			}
		}
	}
}