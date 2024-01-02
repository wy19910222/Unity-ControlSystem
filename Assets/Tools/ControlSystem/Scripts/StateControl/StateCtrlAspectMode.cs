/*
 * @Author: wangyun
 * @CreateTime: 2023-02-16 19:32:48 803
 * @LastEditor: wangyun
 * @EditTime: 2023-02-16 19:32:48 809
 */

using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

namespace Control {
	[RequireComponent(typeof(AspectRatioFitter))]
	public class StateCtrlAspectMode : BaseStateCtrl<AspectRatioFitter.AspectMode> {
		protected override AspectRatioFitter.AspectMode TargetValue {
			get => GetComponent<AspectRatioFitter>().aspectMode;
			set => GetComponent<AspectRatioFitter>().aspectMode = value;
		}
	}
}