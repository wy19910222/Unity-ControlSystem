/*
 * @Author: wangyun
 * @CreateTime: 2023-02-26 22:29:59 312
 * @LastEditor: wangyun
 * @EditTime: 2023-02-26 22:29:59 322
 */

using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

namespace Control {
	[RequireComponent(typeof(Slider))]
	public class StateCtrlSliderWholeNumbers : BaseStateCtrl<bool> {
		protected override bool TargetValue {
			get => GetComponent<Slider>().wholeNumbers;
			set => GetComponent<Slider>().wholeNumbers = value;
		}
	}
}