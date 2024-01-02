/*
 * @Author: wangyun
 * @CreateTime: 2022-05-02 19:32:23 886
 * @LastEditor: wangyun
 * @EditTime: 2022-05-02 19:32:23 890
 */

using System;
using UnityEditor;
using UnityEngine;

namespace Control {
	public class StateCtrlChildPath : BaseStateCtrl<string> {
		public string prefabPath;
		public Action InstantiateCall { get; set; }
		public Action destroyCall { get; set; }
		
		protected override string TargetValue {
			get => prefabPath;
			set {
				if (value != prefabPath) {
					prefabPath = value;
					destroyCall?.Invoke();
					InstantiateCall?.Invoke();
#if UNITY_EDITOR
					if (!Application.isPlaying) {
						Transform trans = transform;
						int childCount = trans.childCount;
						for (int i = childCount - 1; i >= 0; --i) {
							try {
								DestroyImmediate(trans.GetChild(i).gameObject);
							} catch (Exception e) {
								Debug.LogError(e);
							}
						}
						Transform prefab = AssetDatabase.LoadAssetAtPath<Transform>(prefabPath);
						if (prefab) {
							Instantiate(prefab, trans.position, trans.rotation, trans);
						}
					}
#endif
				}
			}
		}
	}
}