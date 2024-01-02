/*
 * @Author: wangyun
 * @CreateTime: 2023-01-28 16:48:45 598
 * @LastEditor: wangyun
 * @EditTime: 2023-01-28 16:48:45 602
 */

using UnityEngine;
using TMPro;

namespace Control {
	[RequireComponent(typeof(TMP_Text))]
	public class StateCtrlTextFont_TMP : BaseStateCtrl<TMP_FontAsset> {
		protected override TMP_FontAsset TargetValue {
			get => GetComponent<TMP_Text>().font;
			set => GetComponent<TMP_Text>().font = value;
		}
	}
}