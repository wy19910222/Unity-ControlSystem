/*
 * @Author: wangyun
 * @CreateTime: 2022-03-30 16:21:41 627
 * @LastEditor: wangyun
 * @EditTime: 2022-09-16 14:16:07 721
 */

using UnityEngine;

namespace Control {
	public class StateCtrlEnable : BaseStateCtrl<bool> {
		[ComponentSelect(true, typeof(Behaviour), typeof(Renderer), typeof(Collider), typeof(LODGroup), typeof(Cloth))]
		public Component target;

		protected override bool TargetValue {
			get {
				switch (target) {
					case Behaviour bhv:
						return bhv.enabled;
					case Renderer rdr:
						return rdr.enabled;
					case Collider cld:
						return cld.enabled;
					case LODGroup lodG:
						return lodG.enabled;
					case Cloth cloth:
						return cloth.enabled;
				}
				return false;
			}
			set {
				switch (target) {
					case Behaviour bhv:
						bhv.enabled = value;
						break;
					case Renderer rdr:
						rdr.enabled = value;
						break;
					case Collider cld:
						cld.enabled = value;
						break;
					case LODGroup lodG:
						lodG.enabled = value;
						break;
					case Cloth cloth:
						cloth.enabled = value;
						break;
				}
			}
		}
	}
}