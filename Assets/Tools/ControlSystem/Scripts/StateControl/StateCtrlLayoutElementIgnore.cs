/*
 * @Author: wangyun
 * @CreateTime: 2023-04-15 22:09:44 605
 * @LastEditor: wangyun
 * @EditTime: 2023-04-15 22:09:44 610
 */

using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

namespace Control {
	[RequireComponent(typeof(LayoutElement))]
	public class StateCtrlLayoutElementIgnore : BaseStateCtrl<bool> {
		protected override bool TargetValue {
			get => GetComponent<LayoutElement>().ignoreLayout;
			set => GetComponent<LayoutElement>().ignoreLayout = value;
		}
	}
}