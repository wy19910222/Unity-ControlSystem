/*
 * @Author: wangyun
 * @CreateTime: 2022-08-08 21:07:15 733
 * @LastEditor: wangyun
 * @EditTime: 2022-08-08 21:07:15 737
 */

#if SPINE_EXIST

using Spine;
using Spine.Unity;
using Sirenix.OdinInspector;

namespace Control {
	public class ExecutorSkeletonLoop : BaseExecutor {
		[HideIf("@skeletonGraphic")]
		[ComponentSelect]
		public SkeletonAnimation skeletonAnimation;
		[HideIf("@skeletonAnimation")]
		[ComponentSelect]
		public SkeletonGraphic skeletonGraphic;
		public bool loop;
		
		protected override void DoExecute() {
			SkeletonAnimation sa = skeletonAnimation ? skeletonAnimation : GetComponent<SkeletonAnimation>();
			if (sa) {
				sa.loop = loop;
			}
			SkeletonGraphic sg = skeletonGraphic ? skeletonGraphic : GetComponent<SkeletonGraphic>();
			if (sg) {
				TrackEntry entry = sg.AnimationState?.GetCurrent(0);
				if (entry != null) {
					entry.Loop = loop;
				}
			}
		}
	}
}

#endif