/*
 * @Author: wangyun
 * @CreateTime: 2023-01-28 19:32:05 403
 * @LastEditor: wangyun
 * @EditTime: 2023-01-28 19:32:05 410
 */

using UnityEngine;
using UnityEngine.UI;

namespace Control {
	[RequireComponent(typeof(Text))]
	public class StateCtrlTextAlignByGeometry : BaseStateCtrl<bool> {
		protected override bool TargetValue {
			get => GetComponent<Text>().alignByGeometry;
			set => GetComponent<Text>().alignByGeometry = value;
		}
	}
}