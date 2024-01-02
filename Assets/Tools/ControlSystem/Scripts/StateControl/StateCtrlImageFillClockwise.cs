/*
 * @Author: wangyun
 * @CreateTime: 2023-02-26 23:03:27 005
 * @LastEditor: wangyun
 * @EditTime: 2023-02-26 23:03:27 010
 */

using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

namespace Control {
	[RequireComponent(typeof(Image))]
	public class StateCtrlImageFillClockwise : BaseStateCtrl<bool> {
		protected override bool TargetValue {
			get => GetComponent<Image>().fillClockwise;
			set => GetComponent<Image>().fillClockwise = value;
		}
	}
}