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
		public float axisValueMin = -1;
		public float axisValueMax = 1;
		
		[ComponentSelect]
		public List<ProgressController> progressControllers = new List<ProgressController>() { null };
		
		private void Update() {
			float axisValue = Input.GetAxis(axisName);
			axisValue = (axisValue - axisValueMin) / (axisValueMax - axisValueMin);
			foreach (var progressController in progressControllers) {
				if (progressController) {
					progressController.Progress = axisValue;
				}
			}
		}
	}
}