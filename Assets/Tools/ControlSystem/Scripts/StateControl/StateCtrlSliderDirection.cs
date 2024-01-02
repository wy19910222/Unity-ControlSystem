/*
 * @Author: wangyun
 * @CreateTime: 2023-02-26 22:27:46 093
 * @LastEditor: wangyun
 * @EditTime: 2023-02-26 22:27:46 100
 */

using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

namespace Control {
	[RequireComponent(typeof(Slider))]
	public class StateCtrlSliderDirection : BaseStateCtrl<Slider.Direction> {
		protected override Slider.Direction TargetValue {
			get => GetComponent<Slider>().direction;
			set => GetComponent<Slider>().direction = value;
		}
	}
}