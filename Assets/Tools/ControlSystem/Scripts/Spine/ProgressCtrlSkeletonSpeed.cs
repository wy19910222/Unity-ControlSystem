/*
 * @Author: wangyun
 * @CreateTime: 2022-08-08 21:07:15 733
 * @LastEditor: wangyun
 * @EditTime: 2022-08-08 21:07:15 737
 */

#if SPINE_EXIST

using Spine.Unity;

namespace Control {
	public class ProgressCtrlSkeletonSpeed : BaseProgressCtrlFloat {
		protected override float TargetValue {
			get {
				SkeletonAnimation sa = GetComponent<SkeletonAnimation>();
				if (sa) {
					return sa.timeScale;
				}
				SkeletonGraphic sg = GetComponent<SkeletonGraphic>();
				if (sg) {
					return sg.timeScale;
				}
				return 1;
			}
			set {
				SkeletonAnimation sa = GetComponent<SkeletonAnimation>();
				if (sa) {
					sa.timeScale = value;
				}
				SkeletonGraphic sg = GetComponent<SkeletonGraphic>();
				if (sg) {
					sg.timeScale = value;
				}
			}
		}
	}
}

#endif