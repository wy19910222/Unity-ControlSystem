/*
 * @Author: wangyun
 * @CreateTime: 2022-08-08 21:07:15 733
 * @LastEditor: wangyun
 * @EditTime: 2022-08-08 21:07:15 737
 */

#if SPINE_EXIST

using Spine.Unity;
using Sirenix.OdinInspector;

namespace Control {
	public class TriggerCtrlSkeletonSpeed : TriggerCtrlTrigger {
		[HideIf("@skeletonGraphic")]
		[ComponentSelect]
		public SkeletonAnimation skeletonAnimation;
		[HideIf("@skeletonAnimation")]
		[ComponentSelect]
		public SkeletonGraphic skeletonGraphic;
		public float timeScale = 1;
		
		protected override void DoTrigger() {
			SkeletonAnimation sa = skeletonAnimation ? skeletonAnimation : GetComponent<SkeletonAnimation>();
			if (sa) {
				sa.timeScale = timeScale;
			}
			SkeletonGraphic sg = skeletonGraphic ? skeletonGraphic : GetComponent<SkeletonGraphic>();
			if (sg) {
				sg.timeScale = timeScale;
			}
		}
	}
}

#endif