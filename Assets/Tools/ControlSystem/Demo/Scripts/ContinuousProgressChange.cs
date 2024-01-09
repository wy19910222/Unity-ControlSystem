/*
 * @Author: wangyun
 * @CreateTime: 2024-01-08 03:35:43 388
 * @LastEditor: wangyun
 * @EditTime: 2024-01-08 03:35:43 392
 */

using System;
using UnityEngine;

namespace Control {
	public class ContinuousProgressChange : MonoBehaviour {
		[ComponentSelect]
		public ProgressController target;
		public float speed;
		public bool ignoreFramerate;
		
		public float ChangeSpeed { get; set; }

		private ProgressController m_ProgressController;
		
		private void Awake() {
			m_ProgressController = GetComponent<ProgressController>();
		}

		private void Update() {
			float value = ChangeSpeed;
			if (!ignoreFramerate) {
				value *= Time.deltaTime;
			}
			target.Progress += speed * value;
		}
	}
}