/*
 * @Author: wangyun
 * @CreateTime: 2022-12-15 14:03:39 824
 * @LastEditor: wangyun
 * @EditTime: 2022-12-15 14:03:39 828
 */

#if SPINE_EXIST

using System;
using Spine;
using Spine.Unity;
using UnityEngine;
using AnimationState = Spine.AnimationState;

namespace Control {
	public partial class BaseProcessStep {
		private void DoStepSpineSetValue() {
			int fieldType = GetIArgument(0);
			switch (fieldType) {
				case 0: {
					string animName = GetSArgument(0);
					switch (obj) {
						case SkeletonAnimation sa: {
							sa.AnimationName = animName;
							break;
						}
						case SkeletonGraphic sg: {
							sg.initialSkinName = animName;
							if (string.IsNullOrEmpty(animName)) {
								sg.AnimationState.ClearTrack(0);
							} else {
								var animationObject = sg.skeletonDataAsset.GetSkeletonData(false).FindAnimation(animName);
								if (animationObject != null) {
									bool loop = sg.AnimationState.GetCurrent(0)?.Loop ?? false;
									sg.AnimationState.SetAnimation(0, animationObject, loop);
								}
							}
							break;
						}
					}
					break;
				}
				case 1: {
					float timeScale = GetFArgument(0);
					switch (obj) {
						case SkeletonAnimation sa: {
							sa.timeScale = timeScale;
							break;
						}
						case SkeletonGraphic sg: {
							sg.timeScale = timeScale;
							break;
						}
					}
					break;
				}
				case 2: {
					bool loop = GetBArgument(0);
					switch (obj) {
						case SkeletonAnimation sa: {
							sa.loop = loop;
							if (loop) {
								bool resume = GetBArgument(1);
								if (resume) {
									TrackEntry entry = sa.AnimationState.GetCurrent(0);
									if (entry == null || entry.IsComplete) {
										sa.Initialize(true);
									}
								}
							} else {
								bool stop = GetBArgument(1);
								if (stop) {
									sa.AnimationState.ClearTrack(0);
								} else {
									TrackEntry entry = sa.AnimationState.GetCurrent(0);
									if (entry != null) {
										if (entry.Loop) {
											float duration = entry.AnimationStart - entry.AnimationEnd;
											float trackTime = entry.TrackTime % duration;
											entry.Loop = false;
											entry.TrackTime = trackTime;
										}
									}
								}
							}
							break;
						}
						case SkeletonGraphic sg: {
							sg.startingLoop = loop;
							TrackEntry entry = sg.AnimationState.GetCurrent(0);
							if (entry != null) {
								if (loop) {
									if (entry.IsComplete) {
										bool resume = GetBArgument(1);
										if (resume) {
											entry.Loop = true;
										} else {
											sg.AnimationState.ClearTrack(0);
										}
									} else {
										entry.Loop = true;
									}
								} else {
									bool stop = GetBArgument(1);
									if (stop) {
										if (entry.Loop) {
											entry.Loop = false;
										} else {
											// entry.Loop = false;
											// entry.TrackTime = Mathf.Infinity;
											sg.AnimationState.ClearTrack(0);
										}
									} else {
										float duration = entry.AnimationStart - entry.AnimationEnd;
										float trackTime = entry.TrackTime % duration;
										entry.Loop = false;
										entry.TrackTime = trackTime;
									}
								}
							} else {
								if (loop) {
									bool resume = GetBArgument(1);
									if (resume) {
										string animName = sg.initialSkinName;
										if (!string.IsNullOrEmpty(animName)) {
											var animationObject = sg.skeletonDataAsset.GetSkeletonData(false).FindAnimation(animName);
											if (animationObject != null) {
												sg.AnimationState.SetAnimation(0, animationObject, true);
											}
										}
									}
								}
							}
							break;
						}
					}
					break;
				}
			}
		}
		
		private void DoStepSpineRestart() {
			switch (obj) {
				case SkeletonAnimation sa: {
					AnimationState state = sa.AnimationState;
					if (state != null) {
						TrackEntry entry = state.GetCurrent(0);
						if (entry != null) {
							bool whenIsNotStarted = GetBArgument(0);
							bool whenIsNotComplete = GetBArgument(1);
							if ((entry.TrackTime != 0 || whenIsNotStarted) && (entry.IsComplete || whenIsNotComplete)) {
								string animName = entry.Animation?.Name ?? sa.AnimationName;
								bool loop = entry.Loop;
								state.ClearTrack(0);
								try {
									state.SetAnimation(0, animName, loop);
								} catch (ArgumentException e) {
									Debug.LogError(e);
								}
							}
						} else {
							try {
								state.SetAnimation(0, sa.AnimationName, sa.loop);
							} catch (ArgumentException e) {
								Debug.LogError(e);
							}
						}
					}
					break;
				}
				case SkeletonGraphic sg: {
					AnimationState state = sg.AnimationState;
					TrackEntry entry = state.GetCurrent(0);
					if (entry != null) {
						bool whenIsNotStarted = GetBArgument(0);
						bool whenIsNotComplete = GetBArgument(1);
						if (entry.TrackTime == 0 && !whenIsNotStarted) {
							return;
						}
						if (!entry.IsComplete && !whenIsNotComplete) {
							return;
						}
						string animName = entry.Animation?.Name ?? sg.initialSkinName;
						bool loop = entry.Loop;
						state.ClearTrack(0);
						try {
							state.SetAnimation(0, animName, loop);
						} catch (ArgumentException e) {
							Debug.LogError(e);
						}
					} else {
						try {
							state.SetAnimation(0, sg.initialSkinName, sg.startingLoop);
						} catch (ArgumentException e) {
							Debug.LogError(e);
						}
					}
					break;
				}
			}
		}

		private void DoStepSpineStop() {
			switch (obj) {
				case SkeletonAnimation sa: {
					sa.AnimationState.ClearTrack(0);
					break;
				}
				case SkeletonGraphic sg: {
					sg.AnimationState.ClearTrack(0);
					break;
				}
			}
		}
	}
}

#endif