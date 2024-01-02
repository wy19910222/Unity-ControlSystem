/*
 * @Author: wangyun
 * @CreateTime: 2023-01-28 19:37:12 050
 * @LastEditor: wangyun
 * @EditTime: 2023-01-28 19:37:12 055
 */

using UnityEngine;
using TMPro;

namespace Control {
	[RequireComponent(typeof(TMP_Text))]
	public class StateCtrlTextAlignment_TMP : BaseStateCtrl<TextAlignmentOptions> {
		protected override TextAlignmentOptions TargetValue {
			get => GetComponent<TMP_Text>().alignment;
			set => GetComponent<TMP_Text>().alignment = value;
		}
	}
}