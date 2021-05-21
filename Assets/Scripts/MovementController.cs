using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    private Rigidbody _Rb;
    public float maxVolacityChange = 4f;
    public Joystick joyStick;
    public float speed = 2f;
    private Vector3 _volacityVector = Vector3.zero;
    public float tiltAmount = 10f;
    // Start is called before the first frame update
    void Start()
    {
        _Rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        float _xMovementInput = joyStick.Horizontal;
        float _zMovmentInput = joyStick.Vertical;

        Vector3 _movementHorizontal = transform.right * _xMovementInput;
        Vector3 _movementVertical = transform.forward * _zMovmentInput;

        Vector3 _movementVelocityVector = (_movementHorizontal + _movementVertical).normalized * speed;

        //Applying tilt rotation toward the axis
        transform.rotation = Quaternion.Euler(joyStick.Vertical * speed * tiltAmount, 0, -1 * joyStick.Horizontal * speed * tiltAmount);

        Move(_movementVelocityVector);
       
    }

    void Move(Vector3 movementVolacityVector)
    {
        _volacityVector = movementVolacityVector;
    }

    private void FixedUpdate()
    {
        if(_volacityVector != Vector3.zero)
        {
            //get rb current Volacity
            Vector3 velocity = _Rb.velocity;
            Vector3 velocityChange = (_volacityVector - velocity);
            //Apply force by amount of volacity change to reach target volacity
            velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVolacityChange, maxVolacityChange);
            velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVolacityChange, maxVolacityChange);
            velocityChange.y = 0f;

            _Rb.AddForce(velocityChange, ForceMode.Acceleration);
        }
        

    }
}
