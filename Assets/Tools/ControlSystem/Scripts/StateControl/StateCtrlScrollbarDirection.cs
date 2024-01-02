/*
 * @Author: wangyun
 * @CreateTime: 2023-02-26 22:27:10 242
 * @LastEditor: wangyun
 * @EditTime: 2023-02-26 22:27:10 248
 */

using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

namespace Control {
	[RequireComponent(typeof(Scrollbar))]
	public class StateCtrlScrollbarDirection : BaseStateCtrl<Scrollbar.Direction> {
		protected override Scrollbar.Direction TargetValue {
			get => GetComponent<Scrollbar>().direction;
			set => GetComponent<Scrollbar>().direction = value;
		}
	}
}