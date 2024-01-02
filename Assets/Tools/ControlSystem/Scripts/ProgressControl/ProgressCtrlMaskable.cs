/*
 * @Author: wangyun
 * @CreateTime: 2023-01-28 16:16:20 178
 * @LastEditor: wangyun
 * @EditTime: 2023-01-28 16:16:20 181
 */

using UnityEngine;
using UnityEngine.UI;

namespace Control {
	[RequireComponent(typeof(MaskableGraphic))]
	public class ProgressCtrlMaskable : BaseProgressCtrlConst<bool> {
		protected override bool TargetValue {
			get => GetComponent<MaskableGraphic>().maskable;
			set => GetComponent<MaskableGraphic>().maskable = value;
		}
	}
}