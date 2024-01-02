/*
 * @Author: wangyun
 * @CreateTime: 2023-01-28 16:48:45 598
 * @LastEditor: wangyun
 * @EditTime: 2023-01-28 16:48:45 602
 */

using UnityEngine;
using UnityEngine.UI;

namespace Control {
	[RequireComponent(typeof(Text))]
	public class StateCtrlTextFont : BaseStateCtrl<Font> {
		protected override Font TargetValue {
			get => GetComponent<Text>().font;
			set => GetComponent<Text>().font = value;
		}
	}
}