/*
 * @Author: wangyun
 * @CreateTime: 2022-05-28 22:37:12 250
 * @LastEditor: wangyun
 * @EditTime: 2022-05-28 22:37:12 254
 */

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Control {
	public class BaseEventListener : MonoBehaviour {
		public string title;
		[ComponentSelect]
		public List<BaseExecutor> executors = new List<BaseExecutor>() { null };

		private Action m_OnExecute;

		protected virtual void Execute() {
			foreach (var executor in executors) {
				if (executor) {
					executor.Execute();
				}
			}
			m_OnExecute?.Invoke();
		}

		public void OnExecute(Action onExecute) {
			m_OnExecute += onExecute;
		}

		public void OffExecute(Action onExecute) {
			m_OnExecute -= onExecute;
		}
	}
}