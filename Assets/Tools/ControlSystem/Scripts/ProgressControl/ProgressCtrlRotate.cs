/*
 * @Author: wangyun
 * @CreateTime: 2024-01-08 03:35:43 388
 * @LastEditor: wangyun
 * @EditTime: 2024-01-08 03:35:43 392
 */

using System;
using UnityEngine;

namespace Control {
	public class ProgressCtrlRotate : BaseProgressCtrlFloat {
		[ComponentSelect]
		public Transform target;
		public Vector3 angles;
		public Space relativeTo = Space.Self;
		public bool ignoreFramerate;

		protected override void Reset() {
			base.Reset();
			target = transform;
		}

		private void Update() {
			target.Rotate(angles * (ignoreFramerate ? TargetValue : TargetValue * Time.deltaTime), relativeTo);
		}
	}
}