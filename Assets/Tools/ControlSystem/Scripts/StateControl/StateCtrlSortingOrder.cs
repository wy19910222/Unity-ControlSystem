/*
 * @Author: wangyun
 * @CreateTime: 2022-09-28 13:23:28 145
 * @LastEditor: wangyun
 * @EditTime: 2022-09-28 13:23:28 149
 */

using System.Collections.Generic;
using UnityEngine;

namespace Control {
	public class StateCtrlSortingOrder : BaseStateCtrl<int> {
		[ComponentSelect]
		public List<Renderer> renderers = new List<Renderer>();

		protected override void Reset() {
			base.Reset();
			renderers.Clear();
			renderers.AddRange(GetComponents<Renderer>());
		}

		[ContextMenu("GetRenderersInChildren")]
		private void GetRenderersInChildren() {
			renderers.Clear();
			renderers.AddRange(GetComponentsInChildren<Renderer>(true));
		}
		
		protected override int TargetValue {
			get {
				foreach (var rdr in renderers) {
					return rdr.sortingOrder;
				}
				return 0;
			}
			set {
				foreach (var rdr in renderers) {
					rdr.sortingOrder = value;
				}
			}
		}
	}
}