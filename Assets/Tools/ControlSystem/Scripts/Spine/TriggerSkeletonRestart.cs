/*
 * @Author: wangyun
 * @CreateTime: 2022-09-18 02:16:39 284
 * @LastEditor: wangyun
 * @EditTime: 2022-09-26 04:40:46 872
 */

#if SPINE_EXIST

using Spine;
using Spine.Unity;
using Sirenix.OdinInspector;
using UnityEngine;
using AnimationState = Spine.AnimationState;

namespace Control {
	public class ExecutorSkeletonRestart : BaseExecutor {
		[HideIf("@skeletonGraphic")]
		[ComponentSelect]
		public SkeletonAnimation skeletonAnimation;
		[HideIf("@skeletonAnimation")]
		[ComponentSelect]
		public SkeletonGraphic skeletonGraphic;
		[Tooltip("NotStarted状态下会自动播放，一般情况下不需要勾选")]
		public bool whenIsNotStarted;
		[Tooltip("若勾选，即使未播放完也会从头开始播")]
		public bool whenIsNotComplete;
		
		protected override void DoExecute() {
			if (skeletonAnimation) {
				AnimationState state = skeletonAnimation.AnimationState;
				if (state != null) {
					TrackEntry entry = state.GetCurrent(0);
					if (entry != null) {
						if ((entry.TrackTime != 0 || whenIsNotStarted) && (entry.IsComplete || whenIsNotComplete)) {
							string animName = entry.Animation?.Name ?? skeletonAnimation.AnimationName;
							bool loop = entry.Loop;
							state.ClearTrack(0);
							state.SetAnimation(0, animName, loop);
						}
					} else {
						state.SetAnimation(0, skeletonAnimation.AnimationName, skeletonAnimation.loop);
					}
				}
			}
			if (skeletonGraphic) {
				AnimationState state = skeletonGraphic.AnimationState;
				if (state != null) {
					TrackEntry entry = state.GetCurrent(0);
					if (entry != null) {
						if (entry.TrackTime == 0 && !whenIsNotStarted) {
							return;
						}
						if (!entry.IsComplete && !whenIsNotComplete) {
							return;
						}
						string animName = entry.Animation?.Name ?? skeletonGraphic.initialSkinName;
						bool loop = entry.Loop;
						state.ClearTrack(0);
						state.SetAnimation(0, animName, loop);
					} else {
						state.SetAnimation(0, skeletonGraphic.initialSkinName, skeletonGraphic.startingLoop);
					}
				}
			}
		}
	}
}

#endif