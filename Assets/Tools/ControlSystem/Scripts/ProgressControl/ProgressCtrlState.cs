/*
 * @Author: wangyun
 * @CreateTime: 2022-08-28 01:48:21 963
 * @LastEditor: wangyun
 * @EditTime: 2022-08-28 01:48:21 968
 */

using UnityEngine;

namespace Control {
	public enum ProgressCtrlStateType {
		Round,
		Floor = 1,
		Ceil = 2
	}
	
	public class ProgressCtrlState : BaseProgressCtrlFloat {
		[ComponentSelect]
		public StateController target;
		public ProgressCtrlStateType type = ProgressCtrlStateType.Round;

		protected override float TargetValue {
			get => target ? target.Index : 0;
			set {
				if (target) {
					switch (type) {
						case ProgressCtrlStateType.Round:
							target.Index = Mathf.RoundToInt(value);
							break;
						case ProgressCtrlStateType.Floor:
							target.Index = Mathf.FloorToInt(value);
							break;
						case ProgressCtrlStateType.Ceil:
							target.Index = Mathf.CeilToInt(value);
							break;
					}
				}
			}
		}
	}
}