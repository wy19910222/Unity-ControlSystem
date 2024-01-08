/*
 * @Author: wangyun
 * @CreateTime: 2024-01-08 01:41:01 906
 * @LastEditor: wangyun
 * @EditTime: 2024-01-08 01:41:01 910
 */

using System.Collections.Generic;
using UnityEngine;

namespace Control {
	public class InputAxisListener : MonoBehaviour {
		public string title;
		[InputAxesSelect]
		public string axisName;
		[Tooltip("鼠标移动取值范围不是[-1, 1]，此时建议填0.01")]
		public float valueRate = 1;

		[ComponentSelect]
		public List<ProgressController> progressControllers = new List<ProgressController>() { null };
		
		private void Update() {
			float axisValue = Input.GetAxis(axisName) * valueRate;
			axisValue = axisValue * 0.5F + 0.5F;
			foreach (var progressController in progressControllers) {
				if (progressController) {
					progressController.Progress = axisValue;
				}
			}
		}
	}
}