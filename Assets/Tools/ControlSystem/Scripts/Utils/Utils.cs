/*
 * @Author: wangyun
 * @CreateTime: 2024-01-16 05:24:19 333
 * @LastEditor: wangyun
 * @EditTime: 2024-01-16 05:24:19 336
 */

using System.Collections;
using UnityEngine;

namespace Control {
	public static class Utils {
		public static void Shuffle(IList list) {
			for (int i = list.Count - 1; i > 0; --i) {
				int j = Random.Range(0, i + 1);
				if (j != i) {
					(list[i], list[j]) = (list[j], list[i]);
				}
			}
		}
	}
}
