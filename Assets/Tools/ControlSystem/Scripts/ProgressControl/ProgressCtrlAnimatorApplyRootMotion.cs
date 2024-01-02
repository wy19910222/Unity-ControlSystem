/*
 * @Author: wangyun
 * @CreateTime: 2022-08-31 01:53:24 845
 * @LastEditor: wangyun
 * @EditTime: 2022-08-31 01:53:24 849
 */

using UnityEngine;

namespace Control {
	[RequireComponent(typeof(Animator))]
	public class ProgressCtrlAnimatorApplyRootMotion : BaseProgressCtrlConst<bool> {
		protected override bool TargetValue {
			get => GetComponent<Animator>().applyRootMotion;
			set => GetComponent<Animator>().applyRootMotion = value;
		}
	}
}