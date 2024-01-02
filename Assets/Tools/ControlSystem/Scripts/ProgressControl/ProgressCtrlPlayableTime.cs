/*
 * @Author: wangyun
 * @CreateTime: 2022-07-25 12:42:49 451
 * @LastEditor: wangyun
 * @EditTime: 2022-07-25 12:42:49 456
 */

using UnityEngine;
using UnityEngine.Playables;

namespace Control {
	[RequireComponent(typeof(PlayableDirector))]
	public class ProgressCtrlPlayableTime : BaseProgressCtrlDouble {
		public bool evaluate;
		
		protected override double TargetValue {
			get => GetComponent<PlayableDirector>().time;
			set {
				PlayableDirector director = GetComponent<PlayableDirector>();
				director.time = value;
				if (evaluate) {
					director.Evaluate();
				}
			}
		}
	}
}