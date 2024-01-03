/*
 * @Author: wangyun
 * @CreateTime: 2022-10-12 18:19:03 737
 * @LastEditor: wangyun
 * @EditTime: 2022-10-12 18:19:03 747
 */

using System;
using Sirenix.OdinInspector;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Control {
	public enum TriggerCtrlLogLevel {
		NONE,
		ERROR,
		WARNING,
		LOG,
	}
	
	public class TriggerCtrlLog : BaseTriggerCtrl {
		public TriggerCtrlLogLevel level = TriggerCtrlLogLevel.LOG;
		public string message;
		public Object context;
		public bool showTime;
		
		protected override void DoTrigger() {
			string msg = showTime ? $"[{DateTime.Now:HH:mm:ss.fff} {Time.frameCount}]{message}" : message;
			switch (level) {
				case TriggerCtrlLogLevel.NONE:
					break;
				case TriggerCtrlLogLevel.ERROR:
					Debug.LogError(msg, context);
					break;
				case TriggerCtrlLogLevel.WARNING:
					Debug.LogWarning(msg, context);
					break;
				case TriggerCtrlLogLevel.LOG:
					Debug.Log(msg, context);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}