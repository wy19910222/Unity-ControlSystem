/*
 * @Author: wangyun
 * @CreateTime: 2022-05-07 14:45:27 552
 * @LastEditor: wangyun
 * @EditTime: 2022-05-07 14:45:27 546
 */

using UnityEngine;

namespace Control {
	[RequireComponent(typeof(Renderer))]
	public class StateCtrlSortingOrder : BaseStateCtrl<int> {
		protected override int TargetValue {
			get => GetComponent<Renderer>().sortingOrder;
			set => GetComponent<Renderer>().sortingOrder = value;
		}
	}
}