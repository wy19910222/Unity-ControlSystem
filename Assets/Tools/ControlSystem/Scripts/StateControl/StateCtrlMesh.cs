/*
 * @Author: wangyun
 * @CreateTime: 2022-03-30 16:21:41 627
 * @LastEditor: wangyun
 * @EditTime: 2022-06-28 15:01:38 263
 */

using UnityEngine;

namespace Control {
	public class StateCtrlMesh : BaseStateCtrl<Mesh> {
		protected override Mesh TargetValue {
			get {
				var mr = GetComponent<SkinnedMeshRenderer>();
				if (mr) {
					return mr.sharedMesh;
				}
				var mf = GetComponent<MeshFilter>();
				if (mf) {
					return mf.sharedMesh;
				}
				return null;
			}
			set {
				var mr = GetComponent<SkinnedMeshRenderer>();
				if (mr) mr.sharedMesh = value;
				var mf = GetComponent<MeshFilter>();
				if (mf) mf.sharedMesh = value;
			}
		}
	}
}