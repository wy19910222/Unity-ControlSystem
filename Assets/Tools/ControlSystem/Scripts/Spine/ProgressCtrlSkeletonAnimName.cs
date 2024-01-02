/*
 * @Author: wangyun
 * @CreateTime: 2022-08-08 21:07:15 733
 * @LastEditor: wangyun
 * @EditTime: 2022-08-08 21:07:15 737
 */

#if SPINE_EXIST

using System.Reflection;
using Spine;
using Spine.Unity;
using UnityEngine;

namespace Control {
	public class ProgressCtrlSkeletonAnimName : BaseProgressCtrlConst<string> {
		protected override string TargetValue {
			get {
				SkeletonAnimation sa = GetComponent<SkeletonAnimation>();
				if (sa) {
#if UNITY_EDITOR
					if (!Application.isPlaying) {
						FieldInfo field = typeof(SkeletonAnimation).GetField("_animationName", BindingFlags.Instance | BindingFlags.NonPublic);
						return field?.GetValue(sa)?.ToString();
					}
#endif
					return sa.AnimationName;
				}
				SkeletonGraphic sg = GetComponent<SkeletonGraphic>();
				if (sg) {
					TrackEntry entry = sg.AnimationState?.GetCurrent(0);
					if (entry != null) {
						return entry.Animation.Name;
					}
				}
				return null;
			}
			set {
				SkeletonAnimation sa = GetComponent<SkeletonAnimation>();
				if (sa) {
					sa.AnimationName = value;
				}
				SkeletonGraphic sg = GetComponent<SkeletonGraphic>();
				if (sg) {
					bool loop = sg.AnimationState?.GetCurrent(0)?.Loop ?? false;
					sg.AnimationState?.SetAnimation(0, value, loop);
				}
			}
		}
	}
}

#endif