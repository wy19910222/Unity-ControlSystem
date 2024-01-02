/*
 * @Author: wangyun
 * @CreateTime: 2022-05-27 20:17:08 894
 * @LastEditor: wangyun
 * @EditTime: 2022-06-26 00:19:29 795
 */

using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;

namespace Control {
	public class ProgressCtrlMaterialColor : BaseProgressCtrlColor {
		public Color color = Color.white;
		public string propertyName = "_BaseColor";
		public bool ctrlSharedMat;
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

		private readonly HashSet<Material> m_MatSet = new HashSet<Material>();

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
				
				m_MatSet.Clear();
				foreach (var _renderer in renderers) {
#if UNITY_EDITOR
					// var mats = Application.isPlaying ? _renderer.materials : _renderer.sharedMaterials;
					var mats = ctrlSharedMat ? _renderer.sharedMaterials : Application.isPlaying ? _renderer.materials : Array.Empty<Material>();
#else
					var mats = ctrlSharedMat ? _renderer.sharedMaterials : _renderer.materials;
#endif
					foreach (var mat in mats) {
						if (!m_MatSet.Contains(mat)) {
							m_MatSet.Add(mat);
						}
					}
				}
				
				foreach (var tweener in m_TweenerSet) {
					tweener?.Kill();
				}
				m_TweenerSet.Clear();
#if UNITY_EDITOR
				if (tween && !controller.InvalidateTween && Application.isPlaying) {
#else
				if (tween && !controller.InvalidateTween) {
#endif
					foreach (var mat in m_MatSet) {
						Tweener tweener = DOTween.To(
							() => mat.GetColor(propertyName),
							v => mat.SetColor(propertyName, v),
							SetValue(mat.GetColor(propertyName), color),
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
					foreach (var mat in m_MatSet) {
						mat.SetColor(propertyName, SetValue(mat.GetColor(propertyName), color));
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