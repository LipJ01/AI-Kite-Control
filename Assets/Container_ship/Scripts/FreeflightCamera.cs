using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class FreeflightCamera : MonoBehaviour 
{
    public float speedNormal = 10.0f;
    public float speedFast   = 50.0f;

    public float mouseSensitivityX = 0.5f;
	public float mouseSensitivityY = 5.0f;
    
	float rotY = -5.0f;
    
    private Vector2 moveInput;
    private Vector2 lookInput;
    public float upDownInput;

	void Start()
	{
		if (GetComponent<Rigidbody>())
			GetComponent<Rigidbody>().freezeRotation = true;
	}

	void Update()
	{
        // // rotation        
        // if (Input.GetMouseButton(1)) 
        // {
        //     float rotX = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * mouseSensitivityX;
        //     rotY += Input.GetAxis("Mouse Y") * mouseSensitivityY;
        //     rotY = Mathf.Clamp(rotY, -89.5f, 89.5f);
        //     transform.localEulerAngles = new Vector3(-rotY, rotX, 0.0f);
        // }
        
        // movement
        if (moveInput.y!= 0.0f)  
        {
            // float speed = Input.GetKey(KeyCode.LeftShift) ? speedFast : speedNormal;
            float speed = speedFast;
            Vector3 trans = new Vector3(0.0f, 0.0f, moveInput.y* speed * Time.deltaTime);
            gameObject.transform.localPosition += gameObject.transform.localRotation * trans;
        }
        if (moveInput.x != 0.0f) 
        {
            // float speed = Input.GetKey(KeyCode.LeftShift) ? speedFast : speedNormal;
            float speed = speedFast;
            Vector3 trans = new Vector3(moveInput.x * speed * Time.deltaTime, 0.0f, 0.0f);
            gameObject.transform.localPosition += gameObject.transform.localRotation * trans;
        }

        // look left/right
        if (lookInput.x != 0.0f)
        {
            float rotX = transform.localEulerAngles.y + lookInput.x * mouseSensitivityX;
            transform.localEulerAngles = new Vector3(-rotY, rotX, 0.0f);

        }

        if (Mathf.Abs(lookInput.y) > 0.0f)
        {
            float speed = speedFast;
            Vector3 trans = new Vector3(0.0f, lookInput.y * speed * Time.deltaTime, 0.0f);
            gameObject.transform.localPosition += gameObject.transform.localRotation * trans;
        }
	}

    private void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
        if (moveInput.magnitude < 0.1)
        {
            moveInput = Vector2.zero;
        }

        
    }

    private void OnLook(InputValue value)
    {
        lookInput = value.Get<Vector2>();
        if (lookInput.magnitude < 0.1)
        {
            lookInput = Vector2.zero;
        }
    }

    private void OnBarPressure(InputValue value)
    {
        upDownInput = value.Get<float>();
    }
}
