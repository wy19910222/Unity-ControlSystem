/*
 * @Author: wangyun
 * @CreateTime: 2023-02-16 19:59:22 269
 * @LastEditor: wangyun
 * @EditTime: 2023-02-16 19:59:22 273
 */

using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

namespace Control {
	public enum StateCtrlSizeFitModeDirection {
		HORIZONTAL = 0,
		VERTICAL = 1,
	}
	
	[RequireComponent(typeof(ContentSizeFitter))]
	public class StateCtrlSizeFitMode : BaseStateCtrl<ContentSizeFitter.FitMode> {
		public StateCtrlSizeFitModeDirection direction;
		
		protected override ContentSizeFitter.FitMode TargetValue {
			get {
				switch (direction) {
					case StateCtrlSizeFitModeDirection.HORIZONTAL:
						return GetComponent<ContentSizeFitter>().horizontalFit;
					case StateCtrlSizeFitModeDirection.VERTICAL:
						return GetComponent<ContentSizeFitter>().verticalFit;
				}
				return ContentSizeFitter.FitMode.Unconstrained;
			}
			set {
				switch (direction) {
					case StateCtrlSizeFitModeDirection.HORIZONTAL:
						GetComponent<ContentSizeFitter>().horizontalFit = value;
						break;
					case StateCtrlSizeFitModeDirection.VERTICAL:
						GetComponent<ContentSizeFitter>().verticalFit = value;
						break;
				}
			}
		}
	}
}