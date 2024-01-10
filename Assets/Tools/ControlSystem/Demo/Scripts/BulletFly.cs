using UnityEngine;

public class BulletFly : MonoBehaviour {
	public float initialSpeed;

	private Transform m_Trans;
	private Vector3 m_Velocity;

	private void Awake() {
		m_Trans = transform;
		m_Velocity = m_Trans.forward * initialSpeed;
	}

	private void Update() {
		Vector3 pos = m_Trans.position;
		pos += m_Velocity * Time.deltaTime;
		m_Trans.LookAt(pos);
		m_Trans.position = pos;
		m_Velocity += Physics.gravity * Time.deltaTime;
	}
}
