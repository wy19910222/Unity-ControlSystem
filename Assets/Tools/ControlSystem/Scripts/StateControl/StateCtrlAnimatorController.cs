/*
 * @Author: wangyun
 * @CreateTime: 2022-04-17 09:54:56 864
 * @LastEditor: wangyun
 * @EditTime: 2022-06-10 10:56:56 259
 */

using UnityEngine;

namespace Control {
	[RequireComponent(typeof(Animator))]
	public class StateCtrlAnimatorController : BaseStateCtrl<RuntimeAnimatorController> {
		protected override RuntimeAnimatorController TargetValue {
			get => GetComponent<Animator>().runtimeAnimatorController;
			set => GetComponent<Animator>().runtimeAnimatorController = value;
		}
	}
}