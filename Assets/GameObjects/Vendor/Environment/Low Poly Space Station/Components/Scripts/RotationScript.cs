using UnityEngine;

public class RotationScript : MonoBehaviour {

	public float speed;

	void Update() 
	{
		// Rotate the object around its local Y axis at 1 degree per second
		transform.Rotate(Vector3.up * speed * Time.deltaTime);
		transform.Rotate(Vector3.left * speed * Time.deltaTime);
	}
}
