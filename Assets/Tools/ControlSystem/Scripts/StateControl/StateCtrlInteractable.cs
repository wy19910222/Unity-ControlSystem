/*
 * @Author: wangyun
 * @CreateTime: 2023-08-13 07:38:24 695
 * @LastEditor: wangyun
 * @EditTime: 2023-08-13 07:38:24 699
 */

using UnityEngine;
using UnityEngine.UI;

namespace Control {
	[RequireComponent(typeof(Selectable))]
	public class StateCtrlInteractable : BaseStateCtrl<bool> {
		protected override bool TargetValue {
			get => GetComponent<Selectable>().interactable;
			set => GetComponent<Selectable>().interactable = value;
		}
	}
}