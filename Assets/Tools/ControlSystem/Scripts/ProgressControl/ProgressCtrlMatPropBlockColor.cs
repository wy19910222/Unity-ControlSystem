/*
 * @Author: wangyun
 * @CreateTime: 2022-09-26 03:53:43 088
 * @LastEditor: wangyun
 * @EditTime: 2022-12-06 17:15:08 797
 */

using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;

namespace Control {
	public class ProgressCtrlMatPropBlockColor : BaseProgressCtrlColor {
		public Color color = Color.white;
		public string propertyName = "_BaseColor";
		[ComponentSelect]
		public List<Renderer> renderers = new List<Renderer>();
		
		public bool tween;
		[HideIf("@!this.tween")]
		public float tweenDelay;
		[HideIf("@!this.tween")]
		public float tweenDuration = 0.3F;
		[HideIf("@!this.tween")]
		public Ease tweenEase = Ease.OutQuad;
		[HideIf("@!tween || tweenEase != Ease.INTERNAL_Custom")]
		public AnimationCurve tweenEaseCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));

		private MaterialPropertyBlock m_TempBlock;
		private MaterialPropertyBlock TempBlock => m_TempBlock ?? (m_TempBlock = new MaterialPropertyBlock());

		private readonly HashSet<Tween> m_TweenerSet = new HashSet<Tween>();

		private void Reset() {
			renderers.Clear();
			renderers.AddRange(GetComponents<Renderer>());
		}

		[ContextMenu("GetRenderersInChildren")]
		private void GetRenderersInChildren() {
			renderers.Clear();
			renderers.AddRange(GetComponentsInChildren<Renderer>(true));
		}
		
		protected override Color TargetValue {
			get => color;
			set {
				// 如果材质中的颜色和color不同步，则有可能出现意外return，所以注释掉
				// if (value == color) return;
				color = value;
				
				foreach (var tweener in m_TweenerSet) {
					tweener?.Kill();
				}
				m_TweenerSet.Clear();
#if UNITY_EDITOR
				if (tween && !controller.InvalidateTween && Application.isPlaying) {
#else
				if (tween && !controller.InvalidateTween) {
#endif
					foreach (var rdr in renderers) {
						Tweener tweener = DOTween.To(
								() => {
									rdr.GetPropertyBlock(TempBlock);
									Color oldColor = Color.white;
									if (TempBlock.isEmpty) {
										Material[] mats = rdr.sharedMaterials;
										foreach (var mat in mats) {
											if (mat.HasProperty(propertyName)) {
												oldColor = mat.GetColor(propertyName);
												break;
											}
										}
									} else {
										oldColor = TempBlock.GetColor(propertyName);
									}
									return oldColor;
								},
								v => {
									rdr.GetPropertyBlock(TempBlock);
									Color oldColor = Color.white;
									if (TempBlock.isEmpty) {
										Material[] mats = rdr.sharedMaterials;
										foreach (var mat in mats) {
											if (mat.HasProperty(propertyName)) {
												oldColor = mat.GetColor(propertyName);
												break;
											}
										}
									} else {
										oldColor = TempBlock.GetColor(propertyName);
									}
									TempBlock.SetColor(propertyName, SetValue(oldColor, v));
									rdr.SetPropertyBlock(TempBlock);
								},
								color,
								tweenDuration
						);
						m_TweenerSet.Add(tweener);
						if (tweenEase == Ease.INTERNAL_Custom) {
							tweener.SetEase(tweenEaseCurve);
						} else {
							tweener.SetEase(tweenEase);
						}
						tweener.SetDelay(tweenDelay).OnComplete(() => m_TweenerSet.Remove(tweener));
					}
				} else {
					foreach (var rdr in renderers) {
						rdr.GetPropertyBlock(TempBlock);
						Color oldColor = Color.white;
						if (TempBlock.isEmpty) {
							Material[] mats = rdr.sharedMaterials;
							foreach (var mat in mats) {
								if (mat.HasProperty(propertyName)) {
									oldColor = mat.GetColor(propertyName);
									break;
								}
							}
						} else {
							oldColor = TempBlock.GetColor(propertyName);
						}
						TempBlock.SetColor(propertyName, SetValue(oldColor, color));
						rdr.SetPropertyBlock(TempBlock);
					}
				}
			}
		}
		
		private Color SetValue(Color c, Color value) {
			if ((part & ColorPart.R) != 0) {
				c.r = value.r;
			}
			if ((part & ColorPart.G) != 0) {
				c.g = value.g;
			}
			if ((part & ColorPart.B) != 0) {
				c.b = value.b;
			}
			if ((part & ColorPart.A) != 0) {
				c.a = value.a;
			}
			return c;
		}
	}
}