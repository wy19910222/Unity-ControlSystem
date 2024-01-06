/*
 * @Author: wangyun
 * @CreateTime: 2022-12-15 13:03:59 091
 * @LastEditor: wangyun
 * @EditTime: 2022-12-15 13:03:59 096
 */

using UnityEngine;
using DG.Tweening;

namespace Control {
	public partial class BaseProcessStep {
		[Sirenix.OdinInspector.InfoBox("每帧会调用反射，少用")]
		public bool tweenUseRotationOrder;
		
		private Tweener m_TransformTweener;
		private void DoStepTransform() {
			if (obj is Transform trans) {
				int transType = GetIArgument(0);

				Vector3 value = Vector3.zero;
				bool targetAsValue = GetBArgument(0);
				bool random = GetBArgument(1);
				if (targetAsValue) {
					if (random) {
						float randomX, randomY, randomZ;
						bool uniform = GetBArgument(3);
						if (uniform) {
							randomX = randomY = randomZ = Random.value;
						} else {
							randomX = Random.value;
							randomY = Random.value;
							randomZ = Random.value;
						}
						Transform minTarget = GetObjArgument<Transform>(0);
						Transform maxTarget = GetObjArgument<Transform>(1);
						Vector3 min = Vector3.zero;
						Vector3 max = Vector3.zero;
						switch (transType) {
							case 0:	// localPosition
								min = minTarget.localPosition;
								max = maxTarget.localPosition;
								break;
							case 1:	// localEulerAngles
								min = minTarget.localEulerAngles;
								max = maxTarget.localEulerAngles;
								break;
							case 2:	// localScale
								min = minTarget.localScale;
								max = maxTarget.localScale;
								break;
							case 3:	// position
								min = minTarget.position;
								max = maxTarget.position;
								break;
							case 4:	// eulerAngles
								min = minTarget.eulerAngles;
								max = maxTarget.eulerAngles;
								break;
						}
						value.x = Mathf.Lerp(min.x, max.x, randomX);
						value.y = Mathf.Lerp(min.y, max.y, randomY);
						value.z = Mathf.Lerp(min.z, max.z, randomZ);
					} else {
						Transform target = GetObjArgument<Transform>(0);
						switch (transType) {
							case 0:	// localPosition
								value = target.localPosition;
								break;
							case 1:	// localEulerAngles
								value = target.localEulerAngles;
								break;
							case 2:	// localScale
								value = target.localScale;
								break;
							case 3:	// position
								value = target.position;
								break;
							case 4:	// eulerAngles
								value = target.eulerAngles;
								break;
						}
					}
				} else {
					if (random) {
						float randomX, randomY, randomZ;
						bool uniform = GetBArgument(3);
						if (uniform) {
							randomX = randomY = randomZ = Random.value;
						} else {
							randomX = Random.value;
							randomY = Random.value;
							randomZ = Random.value;
						}
						float minX = GetFArgument(0);
						float minY = GetFArgument(1);
						float minZ = GetFArgument(2);
						float maxX = GetFArgument(3);
						float maxY = GetFArgument(4);
						float maxZ = GetFArgument(5);
						value.x = Mathf.Lerp(minX, maxX, randomX);
						value.y = Mathf.Lerp(minY, maxY, randomY);
						value.z = Mathf.Lerp(minZ, maxZ, randomZ);
					} else {
						value.x = GetFArgument(0);
						value.y = GetFArgument(1);
						value.z = GetFArgument(2);
					}
				}
				
				int part = GetIArgument(1);
				Vector3 SetValue(Vector3 origin, Vector3 cover) {
					if ((part & (int) Vector3Part.X) != 0) {
						origin.x = cover.x;
					}
					if ((part & (int) Vector3Part.Y) != 0) {
						origin.y = cover.y;
					}
					if ((part & (int) Vector3Part.Z) != 0) {
						origin.z = cover.z;
					}
					return origin;
				}
				if (m_TransformTweener != null) {
					m_TransformTweener.Kill();
					m_TransformTweener = null;
				}
#if UNITY_EDITOR
				if (tween && Application.isPlaying) {
#else
				if (tween) {
#endif
					bool relative = GetBArgument(2);
					switch (transType) {
						// localPosition
						case 0: {
							Vector3 endValue = relative ? trans.localPosition + value : value;
							m_TransformTweener = DOTween.To(
									() => trans.localPosition,
									v => trans.localPosition = SetValue(trans.localPosition, v),
									endValue,
									tweenDuration
							);
							break;
						}
						// localEulerAngles
						case 1: {
							if (part == (int) Vector3Part.XYZ) {
								Quaternion rotationFrom = trans.localRotation;
								Quaternion rotationTo = Quaternion.Euler(relative ? trans.localEulerAngles + value : value);
								float temp = 0;
								m_TransformTweener = DOTween.To(
										() => temp,
										v => {
											trans.localRotation = Quaternion.LerpUnclamped(rotationFrom, rotationTo, v);
											temp = v;
										},
										1,
										tweenDuration
								);
							} else {
								Vector3 endValue = relative ? trans.localEulerAngles + value : value;
								m_TransformTweener = DOTween.To(
										() => trans.localRotation,
										v => trans.localEulerAngles = SetValue(trans.localEulerAngles, v.eulerAngles),
										endValue,
										tweenDuration
								);
							}
							break;
						}
						// localScale
						case 2: {
							Vector3 endValue = relative ? trans.localScale + value : value;
							m_TransformTweener = DOTween.To(
									() => trans.localScale,
									v => trans.localScale = SetValue(trans.localScale, v),
									endValue,
									tweenDuration
							);
							break;
						}
						// position
						case 3: {
							Vector3 endValue = relative ? trans.position + value : value;
							m_TransformTweener = DOTween.To(
									() => trans.position,
									v => trans.position = SetValue(trans.position, v),
									endValue,
									tweenDuration);
							break;
						}
						// eulerAngles
						case 4: {
							if (part == (int) Vector3Part.XYZ) {
								Quaternion rotationFrom = trans.rotation;
								Quaternion rotationTo = Quaternion.Euler(relative ? trans.eulerAngles + value : value);
								float temp = 0;
								m_TransformTweener = DOTween.To(
										() => temp,
										v => {
											trans.rotation = Quaternion.LerpUnclamped(rotationFrom, rotationTo, v);
											temp = v;
										},
										1,
										tweenDuration
								);
							} else {
								Vector3 endValue = relative ? trans.eulerAngles + value : value;
								m_TransformTweener = DOTween.To(
										() => trans.rotation,
										v => trans.eulerAngles = SetValue(trans.eulerAngles, v.eulerAngles),
										endValue,
										tweenDuration
								);
							}
							break;
						}
					}
					if (m_TransformTweener != null) {
						if (tweenEase == Ease.INTERNAL_Custom) {
							m_TransformTweener.SetEase(tweenEaseCurve);
						} else {
							m_TransformTweener.SetEase(tweenEase);
						}
						m_TransformTweener.SetDelay(tweenDelay).OnComplete(() => m_TransformTweener = null);
					}
				} else {
					bool relative = GetBArgument(2);
					switch (transType) {
						case 0:	// localPosition
							trans.localPosition = SetValue(trans.localPosition, relative ? trans.localPosition + value : value);
							break;
						case 1:	// localEulerAngles
							trans.localEulerAngles = SetValue(trans.localEulerAngles, relative ? trans.localEulerAngles + value : value);
							break;
						case 2:	// localScale
							trans.localScale = SetValue(trans.localScale, relative ? trans.localScale + value : value);
							break;
						case 3:	// position
							trans.position = SetValue(trans.position, relative ? trans.position + value : value);
							break;
						case 4:	// eulerAngles
							trans.eulerAngles = SetValue(trans.eulerAngles, relative ? trans.eulerAngles + value : value);
							break;
					}
				}
			}
		}
		
		private Tweener m_LookAtTweener;
		private void DoStepLookAt() {
			if (obj is Transform trans) {
				int lookPart = GetIArgument(0);
				int upPart = GetIArgument(1);
				Transform target = GetObjArgument<Transform>(0);
				if (target) {
					Vector3 direction = target.position - trans.position;
					if (direction != Vector3.zero) {
						Quaternion fromRot = trans.rotation;
						Vector3 GetAxis(int part) {
							switch (part) {
								case 0:
									return Vector3.right;
								case 1:
									return Vector3.up;
								case 2:
									return Vector3.forward;
								case 3:
									return Vector3.left;
								case 4:
									return Vector3.down;
								case 5:
									return Vector3.back;
								default:
									return Vector3.zero;
							}
						}
					
						switch (lookPart) {
							case 0:
								trans.right = direction;
								break;
							case 1:
								trans.up = direction;
								break;
							case 2:
								trans.forward = direction;
								break;
							case 3:
								trans.right = -direction;
								break;
							case 4:
								trans.up = -direction;
								break;
							case 5:
								trans.forward = -direction;
								break;
						}
						Vector3 expectUp = Vector3.Cross(direction, Vector3.Cross(Vector3.up, direction));
						Vector3 currentUp = trans.TransformDirection(GetAxis(upPart));
						if (currentUp != Vector3.zero) {
							float angle = Vector3.SignedAngle(currentUp, expectUp, direction);
							trans.rotation *= Quaternion.AngleAxis(angle, GetAxis(lookPart));
						}
					
						if (m_LookAtTweener != null) {
							m_LookAtTweener.Kill();
							m_LookAtTweener = null;
						}
#if UNITY_EDITOR
						if (tween && Application.isPlaying) {
#else
						if (tween) {
#endif
							Quaternion toRot = trans.rotation;
							trans.rotation = fromRot;
							m_LookAtTweener = trans.DORotateQuaternion(toRot, tweenDuration);
							if (tweenEase == Ease.INTERNAL_Custom) {
								m_LookAtTweener.SetEase(tweenEaseCurve);
							} else {
								m_LookAtTweener.SetEase(tweenEase);
							}
							m_LookAtTweener.SetDelay(tweenDelay).OnComplete(() => m_LookAtTweener = null);
						}
					}
				}
			}
		}
		
		private Tweener m_CameraAnchorTweener;
		private void DoStepCameraAnchor() {
			if (obj is Transform trans) {
				Behaviour camera = GetObjArgument<Behaviour>(0) ?? Camera.main;
				Camera anchorCamera;
#if CINEMACHINE_EXIST
				if (camera is Cinemachine.CinemachineVirtualCamera virtualCamera) {
					GameObject go = new GameObject("Temp Camera");
					anchorCamera = go.AddComponent<Camera>();
					Transform anchorCameraTrans = anchorCamera.transform;
					Transform virtualCameraTrans = virtualCamera.transform;
					anchorCameraTrans.position = virtualCameraTrans.position;
					anchorCameraTrans.rotation = virtualCameraTrans.rotation * Quaternion.AngleAxis(virtualCamera.m_Lens.Dutch, Vector3.forward);
					anchorCamera.orthographic = virtualCamera.m_Lens.Orthographic;
					anchorCamera.fieldOfView = virtualCamera.m_Lens.FieldOfView;
					anchorCamera.orthographicSize = virtualCamera.m_Lens.OrthographicSize;
				} else
#endif
				{
					anchorCamera = camera as Camera;
				}
				if (anchorCamera) {
					int part = GetIArgument(0);
					Vector3 Anchor2Position(Vector3 worldPosition) {
						Vector3 viewPortPos = anchorCamera.WorldToViewportPoint(worldPosition);
						if ((part & 1 << 0) != 0) {
							float xAnchor = GetFArgument(0);
							viewPortPos.x = xAnchor;
						}
						if ((part & 1 << 1) != 0) {
							float yAnchor = GetFArgument(1);
							viewPortPos.y = yAnchor;
						}
						if ((part & 1 << 2) != 0) {
							float zAnchor = GetFArgument(2);
							viewPortPos.z = zAnchor;
						}
						return anchorCamera.ViewportToWorldPoint(viewPortPos);
					}
					
					
					if (m_CameraAnchorTweener != null) {
						m_CameraAnchorTweener.Kill();
						m_CameraAnchorTweener = null;
					}
#if UNITY_EDITOR
					if (tween && Application.isPlaying) {
#else
					if (tween) {
#endif
						Vector3 position = trans.position;
						Vector3 endPos = Anchor2Position(position);
						Vector3 deltaPos = endPos - position;
						Vector3 temp = Vector3.zero;
						m_CameraAnchorTweener = DOTween.To(
							() => temp,
							v => {
								trans.position += v - temp;
								temp = v;
							},
							deltaPos,
							tweenDuration
						);
						if (tweenEase == Ease.INTERNAL_Custom) {
							m_LookAtTweener.SetEase(tweenEaseCurve);
						} else {
							m_LookAtTweener.SetEase(tweenEase);
						}
						m_LookAtTweener.SetDelay(tweenDelay).OnComplete(() => m_LookAtTweener = null);
					} else {
						trans.position = Anchor2Position(trans.position);
					}
					if (anchorCamera != camera) {
#if UNITY_EDITOR
						if (Application.isPlaying) {
							Object.Destroy(anchorCamera.gameObject);
						} else {
							Object.DestroyImmediate(anchorCamera.gameObject);
						}
#else
						Object.Destroy(anchorCamera.gameObject);
#endif
					}
				}
			}
		}
	}
}