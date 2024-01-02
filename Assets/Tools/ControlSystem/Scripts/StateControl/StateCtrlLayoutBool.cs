/*
 * @Author: wangyun
 * @CreateTime: 2023-02-09 18:43:41 408
 * @LastEditor: wangyun
 * @EditTime: 2023-02-09 18:43:41 414
 */

using System;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

namespace Control {
	public enum StateCtrlLayoutBoolType {
		CONTROL_CHILD_SIZE,
		USE_CHILD_SCALE,
		CHILD_FORCE_EXPAND,
	}
	
	[Serializable]
	public struct LayoutBool {
		public bool width;
		public bool height;
		public LayoutBool(bool width, bool height) {
			this.width = width;
			this.height = height;
		}
	}
	
	[RequireComponent(typeof(HorizontalOrVerticalLayoutGroup))]
	public class StateCtrlLayoutBool : BaseStateCtrl<LayoutBool> {
		public StateCtrlLayoutBoolType type;
		public SizePart part;
		protected override LayoutBool TargetValue {
			get {
				HorizontalOrVerticalLayoutGroup layout = GetComponent<HorizontalOrVerticalLayoutGroup>();
				switch (type) {
					case StateCtrlLayoutBoolType.CONTROL_CHILD_SIZE:
						return new LayoutBool(layout.childControlWidth, layout.childControlHeight);
					case StateCtrlLayoutBoolType.USE_CHILD_SCALE:
						return new LayoutBool(layout.childScaleWidth, layout.childScaleHeight);
					case StateCtrlLayoutBoolType.CHILD_FORCE_EXPAND:
						return new LayoutBool(layout.childForceExpandWidth, layout.childForceExpandHeight);
					default:
						return new LayoutBool(false, false);
				}
			}
			set {
				HorizontalOrVerticalLayoutGroup layout = GetComponent<HorizontalOrVerticalLayoutGroup>();
				switch (type) {
					case StateCtrlLayoutBoolType.CONTROL_CHILD_SIZE:
						if ((part & SizePart.WIDTH) != 0) {
							layout.childControlWidth = value.width;
						}
						if ((part & SizePart.HEIGHT) != 0) {
							layout.childControlHeight = value.height;
						}
						break;
					case StateCtrlLayoutBoolType.USE_CHILD_SCALE:
						if ((part & SizePart.WIDTH) != 0) {
							layout.childScaleWidth = value.width;
						}
						if ((part & SizePart.HEIGHT) != 0) {
							layout.childScaleHeight = value.height;
						}
						break;
					case StateCtrlLayoutBoolType.CHILD_FORCE_EXPAND:
						if ((part & SizePart.WIDTH) != 0) {
							layout.childForceExpandWidth = value.width;
						}
						if ((part & SizePart.HEIGHT) != 0) {
							layout.childForceExpandHeight = value.height;
						}
						break;
				}
			}
		}
	}
}