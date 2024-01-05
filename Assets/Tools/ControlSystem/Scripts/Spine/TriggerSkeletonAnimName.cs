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
	public class TriggerSkeletonAnimName : BaseTrigger {
		[HideIf("@skeletonGraphic")]
		[ComponentSelect]
		public SkeletonAnimation skeletonAnimation;
		[HideIf("@skeletonAnimation")]
		[ComponentSelect]
		public SkeletonGraphic skeletonGraphic;
		[SkeletonAnimationNameSelect("skeletonAnimation", "skeletonGraphic")]
		public string animationName;
		
		protected override void DoTrigger() {
			SkeletonAnimation sa = GetComponent<SkeletonAnimation>();
			if (sa) {
				sa.AnimationName = animationName;
			}
			SkeletonGraphic sg = GetComponent<SkeletonGraphic>();
			if (sg) {
				bool loop = sg.AnimationState?.GetCurrent(0)?.Loop ?? false;
				sg.AnimationState?.SetAnimation(0, animationName, loop);
			}
		}
	}
}

#endif