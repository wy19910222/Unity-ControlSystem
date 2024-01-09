/*
 * @Author: wangyun
 * @CreateTime: 2024-01-08 03:35:43 388
 * @LastEditor: wangyun
 * @EditTime: 2024-01-08 03:35:43 392
 */

using System;
using UnityEngine;

namespace Control {
	public class ContinuousRotate : MonoBehaviour {
		public Vector3 angles;
		public Space relativeTo = Space.Self;
		public bool ignoreFramerate;
		
		public float RotateSpeed { get; set; }

		private Transform m_Transform;

		private void Awake() {
			m_Transform = transform;
		}

		private void Update() {
			m_Transform.Rotate(angles * (ignoreFramerate ? RotateSpeed : RotateSpeed * Time.deltaTime), relativeTo);
		}
	}
}