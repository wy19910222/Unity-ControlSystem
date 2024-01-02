/*
 * @Author: wangyun
 * @CreateTime: 2023-01-28 16:15:35 643
 * @LastEditor: wangyun
 * @EditTime: 2023-01-28 16:15:35 650
 */

using UnityEngine;
using UnityEngine.UI;

namespace Control {
	[RequireComponent(typeof(Graphic))]
	public class ProgressCtrlTouchable : BaseProgressCtrlConst<bool> {
		protected override bool TargetValue {
			get {
				var group = GetComponent<CanvasGroup>();
				if (group) {
					return group.blocksRaycasts;
				}
				var graphic = GetComponent<Graphic>();
				if (graphic) {
					return graphic.raycastTarget;
				}
				return false;
			}
			set {
				var group = GetComponent<CanvasGroup>();
				if (group) {
					group.blocksRaycasts = value;
				} else {
					var graphic = GetComponent<Graphic>();
					if (graphic) {
						graphic.raycastTarget = value;
					}
				}
			}
		}
	}
}