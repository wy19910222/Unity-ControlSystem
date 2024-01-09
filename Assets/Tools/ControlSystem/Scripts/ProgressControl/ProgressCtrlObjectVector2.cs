/*
 * @Author: wangyun
 * @CreateTime: 2024-01-09 15:24:16 259
 * @LastEditor: wangyun
 * @EditTime: 2024-01-09 15:24:16 263
 */

using UnityEngine;
using UnityEngine.Events;

namespace Control {
	public class ProgressCtrlObjectVector2 : BaseProgressCtrlVector2 {
		public Vector2 valueTemp;
		public UnityEvent<Vector2> unityEvent;

		protected override Vector2 TargetValue {
			get => valueTemp;
			set {
				valueTemp = value;
				unityEvent.Invoke(value);
			}
		}
	}
}