/*
 * @Author: wangyun
 * @CreateTime: 2022-09-27 19:00:21 121
 * @LastEditor: wangyun
 * @EditTime: 2022-09-27 19:00:21 125
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;

namespace Control {
	public class ProgressCtrlMeshColor : BaseProgressCtrlColor {
		public Color color = Color.white;
		public bool ctrlSharedMesh;
		[ComponentSelect]
		public List<MeshFilter> meshFilters = new List<MeshFilter>();
		
		public bool tween;
		[HideIf("@!this.tween")]
		public float tweenDelay;
		[HideIf("@!this.tween")]
		public float tweenDuration = 0.3F;
		[HideIf("@!this.tween")]
		public Ease tweenEase = Ease.OutQuad;
		[HideIf("@!tween || tweenEase != Ease.INTERNAL_Custom")]
		public AnimationCurve tweenEaseCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));

		private readonly HashSet<Mesh> m_MeshSet = new HashSet<Mesh>();
		
		private Tweener m_Tweener;
		private Coroutine m_Coroutine;

		private void Reset() {
			meshFilters.Clear();
			meshFilters.AddRange(GetComponents<MeshFilter>());
		}

		[ContextMenu("GetMeshFiltersInChildren")]
		private void GetRenderersInChildren() {
			meshFilters.Clear();
			meshFilters.AddRange(GetComponentsInChildren<MeshFilter>(true));
		}
		
		protected override Color TargetValue {
			get => color;
			set {
				// 如果Mesh中的颜色和color不同步，则有可能出现意外return，所以注释掉
				// if (value == color) return;
				color = value;
				
				m_MeshSet.Clear();
				foreach (var meshFilter in meshFilters) {
#if UNITY_EDITOR
					// var meshes = Application.isPlaying ? _renderer.materials : _renderer.sharedMaterials;
					var mesh = ctrlSharedMesh ? meshFilter.sharedMesh : Application.isPlaying ? meshFilter.mesh : null;
#else
					var mesh = ctrlSharedMesh ? meshFilter.sharedMesh : meshFilter.mesh;
#endif
					if (mesh != null && !m_MeshSet.Contains(mesh)) {
						m_MeshSet.Add(mesh);
					}
				}

				if (m_Tweener != null) {
					m_Tweener.Kill();
					m_Tweener = null;
				}
				if (m_Coroutine != null) {
					StopCoroutine(m_Coroutine);
					m_Coroutine = null;
				}
#if UNITY_EDITOR
				if (tween && !controller.InvalidateTween && Application.isPlaying) {
#else
				if (tween && !controller.InvalidateTween) {
#endif
					m_Coroutine = StartCoroutine(TweenApplyColor());
				} else {
					m_Coroutine = StartCoroutine(ApplyColor());
				}
			}
		}

		private IEnumerator TweenApplyColor() {
			float time = 0;
			m_Tweener = DOTween.To(
				() => time,
				v => time = v,
				1,
				tweenDuration
			);
			if (tweenEase == Ease.INTERNAL_Custom) {
				m_Tweener.SetEase(tweenEaseCurve);
			} else {
				m_Tweener.SetEase(tweenEase);
			}
			m_Tweener.SetDelay(tweenDelay).OnComplete(() => m_Tweener = null);

			Dictionary<Mesh, (Color, Color)> oldColorDict = new Dictionary<Mesh, (Color, Color)>(m_MeshSet.Count);
			foreach (var mesh in m_MeshSet) {
				Color[] _colors = mesh.colors;
				Color oldColor = _colors.Length > 0 ? _colors[0] : Color.white;
				Color newColor = SetValue(oldColor, color);
				oldColorDict[mesh] = (oldColor, newColor);
			}
			while (time < 1) {
				yield return new WaitForEndOfFrame();
				
				foreach (var mesh in m_MeshSet) {
					(Color oldColor, Color newColor) = oldColorDict[mesh];
					Color _color = Color.Lerp(oldColor, newColor, time);
					SetColor(mesh, _color);
				}
			}
		}

		private IEnumerator ApplyColor() {
			yield return new WaitForEndOfFrame();
			
			foreach (var mesh in m_MeshSet) {
				Color[] _colors = mesh.colors;
				Color oldColor = _colors.Length > 0 ? _colors[0] : Color.white;
				Color _color = SetValue(oldColor, color);
				SetColor(mesh, _color);
			}
		}
		
		private void SetColor(Mesh mesh, Color c) {
			if (c == Color.white) {
				mesh.SetColors(Array.Empty<Color>());
			} else {
				int length = mesh.vertexCount;
				Color[] colors = new Color[length];
				for (int i = 0; i < length; ++i) {
					colors[i] = c;
				}
				mesh.SetColors(colors);
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