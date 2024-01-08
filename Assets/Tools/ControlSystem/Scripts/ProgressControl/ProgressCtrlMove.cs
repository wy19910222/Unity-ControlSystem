/*
 * @Author: wangyun
 * @CreateTime: 2024-01-08 03:35:43 388
 * @LastEditor: wangyun
 * @EditTime: 2024-01-08 03:35:43 392
 */

using UnityEngine;

namespace Control {
	public class ProgressCtrlMove : BaseProgressCtrlFloat {
		[ComponentSelect]
		public Transform target;
		public Vector3 velocity;
		public Space space = Space.Self;

		protected override void Reset() {
			base.Reset();
			target = transform;
		}

		private void Update() {
			target.Translate(velocity * (TargetValue * Time.deltaTime), space);
		}
	}
}