/*
 * @Author: wangyun
 * @CreateTime: 2022-05-02 19:01:25 097
 * @LastEditor: wangyun
 * @EditTime: 2022-07-24 19:27:39 073
 */

using UnityEngine;
using Sirenix.OdinInspector;

namespace Control {
	public enum TriggerCtrlInstantiateType {
		SELF,
		PIVOT,
	}
	public class TriggerCtrlInstantiate : TriggerCtrlTrigger {
		public TriggerCtrlInstantiateType parentType = TriggerCtrlInstantiateType.PIVOT;
		[ShowIf("@resetPos || resetRot")]
		public TriggerCtrlInstantiateType resetType = TriggerCtrlInstantiateType.PIVOT;
		public Transform prefab;
		[ShowIf("@parentType == TriggerCtrlInstantiateType.PIVOT || (resetPos || resetRot) && resetType == TriggerCtrlInstantiateType.PIVOT")]
		public Transform pivot;
		public bool resetPos = true;
		public bool resetRot = true;
		public bool activeAtOnce = true;
		
		protected override void DoTrigger() {
			Transform parent = parentType == TriggerCtrlInstantiateType.PIVOT ? pivot : transform;
			if (parent) {
				Vector3 pos;
				if (resetPos) {
					// 如果pivot不存在，则parent就是self
					if (resetType == TriggerCtrlInstantiateType.PIVOT && pivot) {
						pos = pivot.position;
					} else {
						pos = transform.position;
					}
				} else {
					pos = parent.TransformPoint(prefab.localPosition);
				}
				Quaternion rot;
				if (resetRot) {
					// 如果pivot不存在，则parent就是self
					if (resetType == TriggerCtrlInstantiateType.PIVOT && pivot) {
						rot = pivot.rotation;
					} else {
						rot = transform.rotation;
					}
				} else {
					rot = parent.rotation * prefab.localRotation;
				}
				Transform trans = Instantiate(prefab, pos, rot, parent);
				if (activeAtOnce) {
					trans.gameObject.SetActive(true);
				}
			} else {
				// 因为self不可能不存在，所以pivot必定不存在
				Vector3 pos = resetPos ? resetType == TriggerCtrlInstantiateType.PIVOT ? Vector3.zero : transform.position : prefab.position;
				Quaternion rot = resetRot ? resetType == TriggerCtrlInstantiateType.PIVOT ? Quaternion.identity : transform.rotation : prefab.rotation;
				Transform trans = Instantiate(prefab, pos, rot);
				if (activeAtOnce) {
					trans.gameObject.SetActive(true);
				}
			}
		}
	}
}