/*
 * @Author: wangyun
 * @LastEditors: wangyun
 * @Date: 2022-03-30 16:21:41 627
 */

using UnityEngine;

namespace Control {
	[RequireComponent(typeof(Renderer))]
	public class ProgressCtrlSortingOrder : BaseProgressCtrlInt {
		protected override int TargetValue {
			get => GetComponent<Renderer>().sortingOrder;
			set => GetComponent<Renderer>().sortingOrder = value;
		}
	}
}