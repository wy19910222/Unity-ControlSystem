/*
 * @Author: wangyun
 * @CreateTime: 2024-01-09 15:24:58 240
 * @LastEditor: wangyun
 * @EditTime: 2024-01-09 15:24:58 245
 */

using UnityEngine;
using UnityEngine.Events;

namespace Control {
	public class ProgressCtrlObjectColor : BaseProgressCtrlColor {
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