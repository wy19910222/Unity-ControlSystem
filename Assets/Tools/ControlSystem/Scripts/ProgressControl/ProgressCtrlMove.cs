/*
 * @Author: wangyun
 * @CreateTime: 2024-01-08 03:35:43 388
 * @LastEditor: wangyun
 * @EditTime: 2024-01-08 03:35:43 392
 */

using UnityEngine;

namespace Control {
	public class ProgressCtrlMove : BaseProgressCtrlFloat {
		[ComponentSelect(false, typeof(Transform), typeof(CharacterController))]
		public Component target;
		public Vector3 velocity;
		public Space space = Space.Self;

		protected override void Reset() {
			base.Reset();
			target = transform;
		}

		private void Update() {
			switch (target) {
				case Transform trans:
					trans.Translate(velocity * (TargetValue * Time.deltaTime), space);
					break;
				case CharacterController characterController:
					Vector3 motion = velocity * (TargetValue * Time.deltaTime);
					if (space == Space.Self) {
						motion = characterController.transform.TransformVector(motion);
					}
					characterController.Move(motion);
					break;
			}
		}
	}
}