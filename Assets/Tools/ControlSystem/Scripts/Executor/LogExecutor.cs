/*
 * @Author: wangyun
 * @CreateTime: 2022-10-12 18:19:03 737
 * @LastEditor: wangyun
 * @EditTime: 2022-10-12 18:19:03 747
 */

using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Control {
	public enum ExecutorLogLevel {
		NONE,
		ERROR,
		WARNING,
		LOG,
	}
	
	public class LogExecutor : BaseExecutor {
		public ExecutorLogLevel level = ExecutorLogLevel.LOG;
		public string message;
		public Object context;
		public bool showTime;
		
		protected override void DoExecute() {
			string msg = showTime ? $"[{DateTime.Now:HH:mm:ss.fff} {Time.frameCount}]{message}" : message;
			switch (level) {
				case ExecutorLogLevel.NONE:
					break;
				case ExecutorLogLevel.ERROR:
					Debug.LogError(msg, context);
					break;
				case ExecutorLogLevel.WARNING:
					Debug.LogWarning(msg, context);
					break;
				case ExecutorLogLevel.LOG:
					Debug.Log(msg, context);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}