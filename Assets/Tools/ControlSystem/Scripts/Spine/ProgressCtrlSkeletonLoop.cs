/*
 * @Author: wangyun
 * @CreateTime: 2022-08-08 21:07:15 733
 * @LastEditor: wangyun
 * @EditTime: 2022-08-08 21:07:15 737
 */

#if SPINE_EXIST

using Spine;
using Spine.Unity;

namespace Control {
	public class ProgressCtrlSkeletonLoop : BaseProgressCtrlConst<bool> {
		protected override bool TargetValue {
			get {
				SkeletonAnimation sa = GetComponent<SkeletonAnimation>();
				if (sa) {
					return sa.loop;
				}
				SkeletonGraphic sg = GetComponent<SkeletonGraphic>();
				if (sg) {
					TrackEntry entry = sg.AnimationState?.GetCurrent(0);
					if (entry != null) {
						return entry.Loop;
					}
				}
				return false;
			}
			set {
				SkeletonAnimation sa = GetComponent<SkeletonAnimation>();
				if (sa) {
					sa.loop = value;
				}
				SkeletonGraphic sg = GetComponent<SkeletonGraphic>();
				if (sg) {
					TrackEntry entry = sg.AnimationState?.GetCurrent(0);
					if (entry != null) {
						entry.Loop = value;
					}
				}
			}
		}
	}
}

#endif