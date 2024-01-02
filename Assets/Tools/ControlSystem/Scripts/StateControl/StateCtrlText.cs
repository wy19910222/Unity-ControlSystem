/*
 * @Author: wangyun
 * @CreateTime: 2022-03-31 02:11:35 694
 * @LastEditor: wangyun
 * @EditTime: 2022-12-31 09:20:34 796
 */

using UnityEngine.UI;
using TMPro;

namespace Control {
	public class StateCtrlText : BaseStateCtrl<string> {
		protected override string TargetValue {
			get {
				Text text = GetComponent<Text>();
				if (text) {
					return text.text;
				}
				TMP_Text tmp_text = GetComponent<TMP_Text>();
				// ReSharper disable once ConvertIfStatementToReturnStatement
				if (tmp_text) {
					return tmp_text.text;
				}
				return null;
			}
			set {
				Text text = GetComponent<Text>();
				if (text) {
					text.text = value;
				}
				TMP_Text tmp_text = GetComponent<TMP_Text>();
				if (tmp_text) {
					tmp_text.text = value;
				}
			}
		}
	}
}