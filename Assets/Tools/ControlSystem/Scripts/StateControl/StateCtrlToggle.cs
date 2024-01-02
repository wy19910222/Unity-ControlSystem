/*
 * @Author: wangyun
 * @CreateTime: 2023-08-13 07:30:25 958
 * @LastEditor: wangyun
 * @EditTime: 2023-08-13 07:30:25 963
 */

using UnityEngine;
using UnityEngine.UI;

namespace Control {
	[RequireComponent(typeof(Toggle))]
	public class StateCtrlToggle : BaseStateCtrl<bool> {
		protected override bool TargetValue {
			get => GetComponent<Toggle>().isOn;
			set => GetComponent<Toggle>().isOn = value;
		}
	}
}