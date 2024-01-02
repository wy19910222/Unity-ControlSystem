/*
 * @Author: wangyun
 * @CreateTime: 2023-02-09 17:48:34 634
 * @LastEditor: wangyun
 * @EditTime: 2023-02-09 17:48:34 639
 */

using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

namespace Control {
	[RequireComponent(typeof(LayoutGroup))]
	public class StateCtrlLayoutAlignment : BaseStateCtrl<TextAnchor> {
		protected override TextAnchor TargetValue {
			get => GetComponent<LayoutGroup>().childAlignment;
			set => GetComponent<LayoutGroup>().childAlignment = value;
		}
	}
}