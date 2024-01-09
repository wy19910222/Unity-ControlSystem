/*
 * @Author: wangyun
 * @CreateTime: 2024-01-09 16:00:16 863
 * @LastEditor: wangyun
 * @EditTime: 2024-01-09 16:00:16 867
 */

using UnityEngine;
using UnityEngine.Events;

namespace Control {
	public class StateCtrlObjectColor : BaseStateCtrl<Color> {
		public Color valueTemp;
		public UnityEvent<Color> unityEvent;

		protected override Color TargetValue {
			get => valueTemp;
			set {
				valueTemp = value;
				unityEvent.Invoke(value);
			}
		}
	}
}