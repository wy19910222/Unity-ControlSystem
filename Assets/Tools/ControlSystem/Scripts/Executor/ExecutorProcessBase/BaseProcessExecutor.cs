/*
 * @Author: wangyun
 * @CreateTime: 2022-12-09 14:13:20 968
 * @LastEditor: wangyun
 * @EditTime: 2023-03-04 17:38:11 182
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Control {
	public partial class BaseProcessExecutor<T> : BaseExecutor where T : BaseProcessStep, new() {
		public bool singleProcess;
		// [SerializeReference]
		public List<T> steps = new List<T>();
	
		private Coroutine m_Co;
	
		protected override void DoExecute() {
			if (singleProcess) {
				StopProcess();
			}
			StartProcess();
		}
	
		public void StartProcess() {
			m_Co = StartCoroutine(IEProcess());
		}
	
		public void StopProcess() {
			if (m_Co != null) {
				StopCoroutine(m_Co);
				m_Co = null;
			}
		}
	
		private IEnumerator IEProcess() {
			if (steps.Count > 0) {
				List<T> _steps = new List<T>(steps);
				// 冒泡排序，同优先级保持原始顺序
				BubbleSort(_steps, (step1, step2) => {
					if (Mathf.Approximately(step1.time, step2.time)) {
						return step1.delayFrames - step2.delayFrames;
					}
	
					return step1.time - step2.time;
				});
				// 倒序，方便移除
				_steps.Reverse();
	
				Dictionary<T, int> stepDelayedDict = new Dictionary<T, int>();
				_steps.ForEach(step => stepDelayedDict.Add(step, 0));
	
				List<T> waitForEndOfFrameSteps = new List<T>();
				WaitForEndOfFrame endOfFrameYield = new WaitForEndOfFrame();
	
				int stepCount = _steps.Count;
				float startTime = Time.time;
				while (stepCount > 0) {
					float time = Time.time - startTime;
					for (int i = stepCount - 1; i >= 0; --i) {
						T step = _steps[i];
						if (step.time <= time) {
							int delayed = stepDelayedDict[step];
							if (delayed >= step.delayFrames) {
								step.DoStep(this);
								_steps.RemoveAt(i);
								stepCount--;
							} else if (delayed + 1 > step.delayFrames) {
								waitForEndOfFrameSteps.Add(step);
								_steps.RemoveAt(i);
								stepCount--;
							} else {
								stepDelayedDict[step] = delayed + 1;
							}
						}
					}
	
					if (waitForEndOfFrameSteps.Count > 0) {
						yield return endOfFrameYield;
						foreach (var step in waitForEndOfFrameSteps) {
							step.DoStep(this);
						}
	
						waitForEndOfFrameSteps.Clear();
					}
	
					if (stepCount <= 0) {
						break;
					}
	
					yield return null;
				}
	
				m_Co = null;
			}
		}
	
		/// <summary>
		/// 冒泡排序
		/// </summary>
		private static void BubbleSort(IList<T> list, Func<T, T, float> comparison) {
			int stepCount = list.Count;
			if (stepCount > 1) {
				for (int i = 0, lastIndex = stepCount - 1, sortBorder = lastIndex; i < lastIndex; ++i) {
					bool isSortComplete = true;
					int lastSwapIndex = 0;
					for (int j = 0; j < sortBorder; ++j) {
						if (comparison(list[j], list[j + 1]) > 0) {
							(list[j], list[j + 1]) = (list[j + 1], list[j]);
							isSortComplete = false;
							lastSwapIndex = j;
						}
					}
					sortBorder = lastSwapIndex;
					if (isSortComplete) {
						break;
					}
				}
			}
		}
	}
}