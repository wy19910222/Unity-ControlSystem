/*
 * @Author: wangyun
 * @CreateTime: 2022-05-02 19:32:23 886
 * @LastEditor: wangyun
 * @EditTime: 2022-05-02 19:32:23 890
 */

using UnityEngine;

namespace Control {
	public class StateCtrlChild : BaseStateCtrl<Transform> {
		public Transform prefab;
		
		protected override Transform TargetValue {
			get => prefab;
			set {
				if (value != prefab) {
					prefab = value;
					Transform trans = transform;
					int childCount = trans.childCount;
					if (childCount > 0) {
#if UNITY_EDITOR
						if (Application.isPlaying) {
							Destroy(trans.GetChild(childCount - 1).gameObject);
						} else {
							DestroyImmediate(trans.GetChild(childCount - 1).gameObject);
						}
#else
						Destroy(trans.GetChild(childCount - 1).gameObject);
#endif
					}
					if (prefab) {
						Instantiate(prefab, trans);
					}
				}
			}
		}
	}
}