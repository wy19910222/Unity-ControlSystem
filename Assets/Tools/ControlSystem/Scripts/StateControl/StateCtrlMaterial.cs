/*
 * @Author: wangyun
 * @CreateTime: 2022-06-28 14:49:32 527
 * @LastEditor: wangyun
 * @EditTime: 2022-06-28 14:49:32 517
 */

using UnityEngine;

namespace Control {
	[RequireComponent(typeof(Renderer))]
	public class StateCtrlMaterial : BaseStateCtrl<Material> {
		protected override Material TargetValue {
			get => GetComponent<Renderer>()?.sharedMaterial;
			set {
				var rdr = GetComponent<Renderer>();
				if (rdr) rdr.sharedMaterial = value;
			}
		}
	}
}