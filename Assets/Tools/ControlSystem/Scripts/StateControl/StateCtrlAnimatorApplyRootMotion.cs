/*
 * @Author: wangyun
 * @CreateTime: 2022-08-31 01:51:16 904
 * @LastEditor: wangyun
 * @EditTime: 2022-09-16 14:15:35 799
 */

using UnityEngine;

namespace Control {
	[RequireComponent(typeof(Animator))]
	public class StateCtrlAnimatorApplyRootMotion : BaseStateCtrl<bool> {
		protected override bool TargetValue {
			get => GetComponent<Animator>().applyRootMotion;
			set => GetComponent<Animator>().applyRootMotion = value;
		}
	}
}