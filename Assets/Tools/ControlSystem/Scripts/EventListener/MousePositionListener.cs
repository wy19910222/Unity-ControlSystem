/*
 * @Author: wangyun
 * @CreateTime: 2024-01-08 01:41:08 584
 * @LastEditor: wangyun
 * @EditTime: 2024-01-08 01:41:08 589
 */

using System.Collections.Generic;
using UnityEngine;

namespace Control {
	public class MousePositionListener : MonoBehaviour {
		public string title;
		[ComponentSelect]
		public List<ProgressController> xProgressControllers = new List<ProgressController>() { null };
		[ComponentSelect]
		public List<ProgressController> yProgressControllers = new List<ProgressController>() { null };
		
		private void Update() {
			Vector3 mousePos = Input.mousePosition;
			float xValue = mousePos.x / Screen.width;
			foreach (var xProgressController in xProgressControllers) {
				if (xProgressController) {
					xProgressController.Progress = xValue;
				}
			}
			float yValue = mousePos.y / Screen.height;
			foreach (var xProgressController in yProgressControllers) {
				if (xProgressController) {
					xProgressController.Progress = yValue;
				}
			}
		}
	}
}