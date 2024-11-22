using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class PlayerController : MonoBehaviour
{
    //---------------Components------------------//
    private CharacterController control;
    private Transform cam;
    public Animator anim;

    //---------------Input-----------------------//
    private float _horizontal;
    private float _vertical;
    private float _turnSmoothVelocity; 
    
    [SerializeField] private float _speed = 5f;
    [SerializeField] private float _JumpHeight = 2;
    [SerializeField] private float _turnSmoothTime = 0.05f;

    //---------------Graveded--------------------//
    [SerializeField] private float _gravity = -9.81f;
    [SerializeField] private Vector3 _playerGravity;

    //---------------Grounded-------------------//
    [SerializeField] float _sensorRadius = 0.5f;
    [SerializeField] Transform _sensorPosition;
    [SerializeField] private LayerMask _groundLayer;

    
    private Vector3 moveDirection;
    
    void Awake()
    {
        control = GetComponent<CharacterController>();
        cam = Camera.main.transform;
        anim = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
     void Update()
    {
        _horizontal = Input.GetAxis("Horizontal");
        _vertical = Input.GetAxis("Vertical");
        
        Movimiento();

        if(Input.GetButtonDown("Jump") && IsGrounded())
        {
            Jump();   
        }
       

        Gravity();
    }

    void Movimiento()
    {
        Vector3 direction= new Vector3(_horizontal, 0, _vertical);

        anim.SetFloat("VelZ",direction.magnitude);
        anim.SetFloat("VelX", 0);
        

        if(direction != Vector3.zero)
        {
            float targeAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float smoothAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targeAngle, ref _turnSmoothVelocity, _turnSmoothTime);
       
            transform.rotation = Quaternion.Euler(0,smoothAngle, 0); 

            moveDirection = Quaternion.Euler(0, targeAngle, 0) * Vector3.forward;
            
            control.Move(moveDirection * _speed * Time.deltaTime);
        }
    }

    void Gravity()
    {
        if(!IsGrounded())
        {
             _playerGravity.y += _gravity * Time.deltaTime;
        }
        else if(IsGrounded() && _playerGravity.y < 0)
        {
            _playerGravity.y = -1;
            anim.SetBool("IsJumping", false);
        }

        control.Move(_playerGravity * Time.deltaTime);
    }

    void Jump()
    {
        _playerGravity.y = Mathf.Sqrt(_JumpHeight * -2 * _gravity);
        anim.SetBool("IsJumping", true);
    }

    bool IsGrounded()
    {
        return Physics.CheckSphere(_sensorPosition.position, _sensorRadius, _groundLayer);  
    }

    



   

    
    
}
