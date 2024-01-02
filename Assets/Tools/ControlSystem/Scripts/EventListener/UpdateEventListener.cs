/*
 * @Author: wangyun
 * @CreateTime: 2022-06-28 17:46:08 579
 * @LastEditor: wangyun
 * @EditTime: 2022-07-07 14:56:39 268
 */

using System;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Control {
	public enum UpdateEventType {
		FRAMES_INTERVAL = 0,
		SECONDS_INTERVAL = 1
	}
	
	public class UpdateEventListener : BaseEventListener {
		public UpdateEventType type = UpdateEventType.FRAMES_INTERVAL;
		public bool triggerOnce;
		[ShowIf("@type == UpdateEventType.FRAMES_INTERVAL")]
		[LabelText("Delay")]
		public int framesDelay;
		[ShowIf("@type == UpdateEventType.SECONDS_INTERVAL")]
		[LabelText("Delay")]
		public float secondsDelay;
		[ShowIf("@type == UpdateEventType.FRAMES_INTERVAL && !triggerOnce")]
		[LabelText("Interval")]
		public int framesInterval = 1;
		[ShowIf("@type == UpdateEventType.SECONDS_INTERVAL && !triggerOnce")]
		[LabelText("Interval")]
		public float secondsInterval = 1;

		private int m_Frames;
		private float m_Seconds;
		private bool m_IsTriggered;
		
		private void OnEnable() {
			m_Frames = 0;
			m_Seconds = 0;
			m_IsTriggered = false;
			DoUpdate();
		}

		private void Update() {
			m_Frames++;
			m_Seconds += Time.deltaTime;
			DoUpdate();
		}

		private void DoUpdate() {
			switch (type) {
				case UpdateEventType.FRAMES_INTERVAL: {
					if (!m_IsTriggered && m_Frames >= framesDelay) {
						m_Frames -= Mathf.Max(framesDelay, 1);
						Trigger();
						m_IsTriggered = true;
						if (triggerOnce) {
							enabled = false;
						}
					}
					if (m_IsTriggered) {
						while (m_Frames >= framesInterval) {
							m_Frames -= Mathf.Max(framesInterval, 1);
							Trigger();
							if (triggerOnce) {
								enabled = false;
								break;
							}
						}
					}
					break;
				}
				case UpdateEventType.SECONDS_INTERVAL: {
					if (!m_IsTriggered && m_Seconds >= secondsDelay) {
						m_Seconds -= Mathf.Max(secondsDelay, Time.deltaTime);
						Trigger();
						m_IsTriggered = true;
						if (triggerOnce) {
							enabled = false;
						}
					}
					if (m_IsTriggered) {
						while (m_Seconds >= secondsInterval) {
							m_Seconds -= Mathf.Max(secondsInterval, Time.deltaTime);
							Trigger();
							if (triggerOnce) {
								enabled = false;
								break;
							}
						}
					}
					break;
				}
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}