/*
 * @Author: wangyun
 * @CreateTime: 2024-01-09 19:22:54 035
 * @LastEditor: wangyun
 * @EditTime: 2024-01-09 19:22:54 039
 */

using UnityEngine;

public class ScoreBoard : MonoBehaviour {
	public TextMesh txtScore;

	private int m_Score;

	private void Awake() {
		m_Score = 0;
		txtScore.text = "000";
	}

	public void Increase() {
		txtScore.text = Mathf.Min(++m_Score, 999).ToString("000");
	}
}
