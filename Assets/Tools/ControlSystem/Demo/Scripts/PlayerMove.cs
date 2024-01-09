/*
	* @Author: wangyun
	* @CreateTime: 2024-01-08 03:35:43 388
	* @LastEditor: wangyun
	* @EditTime: 2024-01-08 03:35:43 392
	*/

using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMove : MonoBehaviour {
	public Vector3 velocity;
	public Space space = Space.Self;
	
	public float MoveSpeed { get; set; }

	private Transform m_Transform;
	private CharacterController m_CharacterController;

	private void Awake() {
		m_Transform = transform;
		m_CharacterController = GetComponent<CharacterController>();
	}

	private void Update() {
		Vector3 motion = velocity * (MoveSpeed * Time.deltaTime);
		if (space == Space.Self) {
			motion = m_Transform.TransformVector(motion);
		}
		m_CharacterController.Move(motion);
	}
}