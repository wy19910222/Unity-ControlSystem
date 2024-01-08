/*
 * @Author: wangyun
 * @CreateTime: 2024-01-09 00:56:01 952
 * @LastEditor: wangyun
 * @EditTime: 2024-01-09 00:56:01 956
 */

using UnityEngine;

namespace Control {
	public class ProgressCtrlProgressChange : BaseProgressCtrlFloat {
		[ComponentSelect]
		public ProgressController target;
		public float speed;
		public bool ignoreFramerate;

		protected override float TargetValue { get; set; }

		protected override void Reset() {
			base.Reset();
			target = GetComponent<ProgressController>();
		}

		private void Update() {
			float value = TargetValue * 2 - 1;
			if (!ignoreFramerate) {
				value *= Time.deltaTime;
			}
			target.Progress += speed * value;
		}
	}
}