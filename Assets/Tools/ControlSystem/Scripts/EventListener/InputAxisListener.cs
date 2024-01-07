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
		[Tooltip("扳机键等轴为单向轴，取值为0-1")]
		public bool singleDirection;
		[ComponentSelect]
		public List<ProgressController> progressControllers = new List<ProgressController>() { null };
		
		private void Update() {
			float axisValue = Input.GetAxis(axisName);
			if (!singleDirection) {
				axisValue = axisValue * 0.5F + 0.5F;
			}
			foreach (var progressController in progressControllers) {
				if (progressController) {
					progressController.Progress = axisValue;
				}
			}
		}
	}
}