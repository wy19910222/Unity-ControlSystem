/*
 * @Author: wangyun
 * @CreateTime: 2023-02-26 22:57:03 517
 * @LastEditor: wangyun
 * @EditTime: 2023-02-26 22:57:03 523
 */

using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

namespace Control {
	[RequireComponent(typeof(Image))]
	public class StateCtrlImageFillOrigin : BaseStateCtrl<int> {
		protected override int TargetValue {
			get => GetComponent<Image>().fillOrigin;
			set => GetComponent<Image>().fillOrigin = value;
		}
	}
}