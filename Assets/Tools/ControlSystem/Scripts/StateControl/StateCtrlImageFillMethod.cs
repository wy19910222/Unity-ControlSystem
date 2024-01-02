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
	[RequireComponent(typeof(Image))]
	public class StateCtrlImageFillMethod : BaseStateCtrl<Image.FillMethod> {
		protected override Image.FillMethod TargetValue {
			get => GetComponent<Image>().fillMethod;
			set => GetComponent<Image>().fillMethod = value;
		}
	}
}