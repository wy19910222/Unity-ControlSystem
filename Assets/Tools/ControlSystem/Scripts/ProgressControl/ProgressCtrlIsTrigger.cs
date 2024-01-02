/*
 * @Author: wangyun
 * @CreateTime: 2022-11-23 22:00:32 129
 * @LastEditor: wangyun
 * @EditTime: 2022-11-23 22:00:32 132
 */

using UnityEngine;

namespace Control {
	public class ProgressCtrlIsTrigger : BaseProgressCtrlConst<bool> {
		[ComponentSelect(true, typeof(Collider), typeof(Collider2D))]
		public Component target;

		protected override bool TargetValue {
			get {
				switch (target) {
					case Collider col:
						return col.isTrigger;
					case Collider2D col2D:
						return col2D.isTrigger;
				}
				return false;
			}
			set {
				switch (target) {
					case Collider col:
						col.isTrigger = value;
						break;
					case Collider2D col2D:
						col2D.isTrigger = value;
						break;
				}
			}
		}
	}
}