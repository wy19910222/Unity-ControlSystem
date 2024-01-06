/*
 * @Author: wangyun
 * @CreateTime: 2022-12-15 13:03:54 805
 * @LastEditor: wangyun
 * @EditTime: 2022-12-15 13:03:54 812
 */

using UnityEngine;

namespace Control {
	public partial class BaseProcessStep {
		private void DoStepInstantiate(Component executor) {
			if (obj is Transform prefab) {
				bool specifyParent = GetBArgument(0);
				Transform parent = specifyParent ? GetObjArgument<Transform>(0) : executor.transform;
				bool resetPos = GetBArgument(1);
				Transform posBased = GetObjArgument<Transform>(1);
				bool resetRot = GetBArgument(2);
				Transform rotBased = GetObjArgument<Transform>(2);
				bool resetScale = GetBArgument(3);
				Transform scaleBased = GetObjArgument<Transform>(3);
				if (parent) {
					Vector3 pos = resetPos ? posBased ? posBased.position : parent.position : parent.TransformPoint(prefab.localPosition);
					Quaternion rot = resetRot ? rotBased ? rotBased.rotation : parent.rotation : parent.rotation * prefab.localRotation;
					Transform trans = Object.Instantiate(prefab, pos, rot, parent);
#if UNITY_EDITOR
					UnityEditor.Undo.RegisterCreatedObjectUndo(trans.gameObject, "Instantiate " + trans.name);
#endif
					if (resetScale) {
						trans.localScale = scaleBased ? scaleBased.localScale : Vector3.one;
					}
					bool activeAtOnce = GetBArgument(4);
					if (activeAtOnce) {
						trans.gameObject.SetActive(true);
					}
				} else {
					Vector3 pos = resetPos ? posBased ? posBased.position : Vector3.zero : prefab.position;
					Quaternion rot = resetRot ? rotBased ? rotBased.rotation : Quaternion.identity : prefab.rotation;
					Transform trans = Object.Instantiate(prefab, pos, rot);
#if UNITY_EDITOR
					UnityEditor.Undo.RegisterCreatedObjectUndo(trans.gameObject, "Instantiate " + trans.name);
#endif
					if (resetScale) {
						trans.localScale = scaleBased ? scaleBased.localScale : Vector3.one;
					}
					bool activeAtOnce = GetBArgument(4);
					if (activeAtOnce) {
						trans.gameObject.SetActive(true);
					}
				}
			}
		}

		private void DoStepDestroy() {
			if (obj is GameObject goDestroy) {
				bool onlyDestroyChildren = GetBArgument(0);
				if (onlyDestroyChildren) {
					Transform trans = goDestroy.transform;
					for (int index = trans.childCount - 1; index >= 0; --index) {
						Transform child = trans.GetChild(index);
#if UNITY_EDITOR
						if (Application.isPlaying) {
							Object.Destroy(child.gameObject);
						} else {
							UnityEditor.Undo.DestroyObjectImmediate(child.gameObject);
						}
#else
						Object.Destroy(child.gameObject);
#endif
					}
				} else {
#if UNITY_EDITOR
					if (Application.isPlaying) {
						Object.Destroy(goDestroy);
					} else {
						UnityEditor.Undo.DestroyObjectImmediate(goDestroy);
					}
#else
					Object.Destroy(goDestroy);
#endif
				}
			}
		}
		
		private void DoStepParent() {
			if (obj is Transform trans) {
				Transform parent = GetObjArgument<Transform>(0);
				bool worldPositionStays = GetBArgument(0);
				trans.SetParent(parent, worldPositionStays);
				int siblingIndex = GetIArgument(0);
				if (siblingIndex != -1) {	// 刚加进去本来就在最后一位
					if (siblingIndex < 0) {
						trans.SetSiblingIndex(siblingIndex + parent.childCount);
					} else {
						trans.SetSiblingIndex(siblingIndex);
					}
				}
			}
		}
		
		private void DoStepActive() {
			if (obj is GameObject go) {
				bool active = GetBArgument(0);
				go.SetActive(active);
			}
		}
		
		private void DoStepEnabled() {
			bool enabled = GetBArgument(0);
			switch (obj) {
				case Behaviour bhv:
					bhv.enabled = enabled;
					break;
				case Renderer rdr:
					rdr.enabled = enabled;
					break;
				case Collider cld:
					cld.enabled = enabled;
					break;
				case LODGroup lodG:
					lodG.enabled = enabled;
					break;
				case Cloth cloth:
					cloth.enabled = enabled;
					break;
			}
		}
	}
}