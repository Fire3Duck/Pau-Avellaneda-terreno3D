using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    //Components
    private CharacterController _controller;
    private Animator _animator;

    //Inputs
    private InputAction _moveAction;

    private Vector2 _moveInput;
    private InputAction _jumpAction;

    private InputAction _lookAction;
    private Vector2 _lookInput;

    private InputAction _aimAction;
    private InputAction _grabAction;
    private InputAction _throwAction;
    private InputAction _dashAction;

    [SerializeField] private float _movementSpeed = 5;
    [SerializeField] private float _jumpHeight = 2;
    [SerializeField] private float _pushForce = 10;
    [SerializeField] private float _throwForce = 10;
    [SerializeField] private float _dashHeight = 2;
    [SerializeField] private float _smoothTime = 0.2f;

    private float _turnSmoothVelcity;   

    //gravedad

    [SerializeField] private float _gravity = -9.81f;
   
    [SerializeField] private Vector3 _playerGravity;

    //groundsensor

    [SerializeField] Transform _sensor;

    [SerializeField] LayerMask _groundLayer;

    [SerializeField] float _sensorRadius;

    public Transform _maincamera;

    [SerializeField] private Vector3 _handsSensorSize;

    [SerializeField] private Transform _hands;
    [SerializeField] private Transform _grabedObject;

    //Vida
    [SerializeField] private float _maxHealth = 10;
    [SerializeField] private float _currentHealth;

    public bool canMove = true;



    void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _animator = GetComponentInChildren<Animator>();
        
        _moveAction = InputSystem.actions["Move"];
        _jumpAction = InputSystem.actions["Jump"];
        _lookAction = InputSystem.actions["Look"];

        _maincamera = Camera.main.transform;
        _aimAction = InputSystem.actions["Aim"];
        _dashAction = InputSystem.actions["Dash"];
        _grabAction = InputSystem.actions["Interact"];
        _throwAction = InputSystem.actions["Throw"];
    }
    void Start()
    {
        _currentHealth = _maxHealth;
        //_healthBar.fillAmount = _maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        _moveInput = _moveAction.ReadValue<Vector2>();
        _lookInput = _lookAction.ReadValue<Vector2>();

        // MovimientoCutre();
        //Movimiento2();

        if (canMove)
        {

            if (_aimAction.IsInProgress())
            {
                AimMovement();
            }
            else
            {
                Movement();
            }

            if (_jumpAction.WasPressedThisFrame() && IsGrounded())
            {
                Jump();
            }
            Gravity();

            if (_dashAction.WasPressedThisFrame())
            {
                Dash();
            }

            if (_aimAction.WasPerformedThisFrame())
            {
                Attack();
            }

            if (_grabAction.WasPerformedThisFrame())
            {
                GrabObject();
            }

            if (_throwAction.WasPerformedThisFrame())
            {
                //Throw();
                RayTest();
            }

        }

    }
    
    void Attack()
    {
        Ray ray = Camera.main.ScreenPointToRay(_lookInput);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            Debug.Log(hit.transform.name);

            IDamageable damageable = hit.transform.GetComponent<IDamageable>();

            if(damageable != null)
            {
                damageable.TakeDamage(6);
            }
        }
    }

    void Movement()
    {
        Vector3 direction = new Vector3(_moveInput.x, 0, _moveInput.y);

        _animator.SetFloat("Vertical", direction.magnitude);
        _animator.SetFloat("Horizontal", 0);

        if(direction != Vector3.zero)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + _maincamera.eulerAngles.y; // para que el personaje gire la cabeza
            float smoothAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _turnSmoothVelcity, _smoothTime);
            transform.rotation = Quaternion.Euler(0, smoothAngle, 0);

            Vector3 moveDirection = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;

            _controller.Move(moveDirection.normalized * _movementSpeed * Time.deltaTime);
        }
    }

    void Movimiento2()
    {
        Vector3 direction = new Vector3(_moveInput.x, 0, _moveInput.y);

        if(direction != Vector3.zero)
        {
            _controller.Move(direction.normalized * _movementSpeed * Time.deltaTime);
        }
    }

    void AimMovement()
    {
        Vector3 direction = new Vector3(_moveInput.x, 0, _moveInput.y);

        _animator.SetFloat("Horizontal", _moveInput.x);
        _animator.SetFloat("Vertical", _moveInput.y);

        float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg; // para que el personaje gire la cabeza
        float smoothAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, _maincamera.eulerAngles.y, ref _turnSmoothVelcity, _smoothTime);

        transform.rotation = Quaternion.Euler(0, smoothAngle, 0);
        if (direction != Vector3.zero)
        {
            Vector3 moveDirection = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;

            _controller.Move(moveDirection.normalized * _movementSpeed * Time.deltaTime);
        }  
    }
    
    void Jump()
    {
        _animator.SetBool("IsJumping", true);

        _playerGravity.y = Mathf.Sqrt(_jumpHeight * -2 * _gravity);

        _controller.Move(_playerGravity * Time.deltaTime);
    }

    void Dash()
    {
        _playerGravity.z = Mathf.Sqrt(_dashHeight * -2 * _gravity);

        _controller.Move(_playerGravity * Time.deltaTime);

        _playerGravity.z = 5;
    }

    public void TakeDamage(int amount)
    {
        _currentHealth -= amount;
        _currentHealth = Mathf.Max(_currentHealth, 0);
        //_healthBar.fillAmount = _currentHealth / _maxHealth;
        //_audioSource.PlayOneShot(_damage);
        Debug.Log($"Player took {amount} damage. Health now: {_currentHealth}");
        
        if (_currentHealth <= 0)
        {
            Death();
        }
    }

    void Death()
    {
        Debug.Log("Muerto");
        _animator.SetTrigger("IsDeath");
        canMove = false;
        
        //_audioSource.PlayOneShot(deathSFX);
        //_boxCollider.enabled = false;

        //inputHorizontal = 0;
        
        //_rigidBody.gravityScale = 0;
        
        Game_Manager.instance.isPlaying = false;
        
    }

    void Gravity()
    {
        if (!IsGrounded())
        {
            _playerGravity.y += _gravity * Time.deltaTime;
        }
        else if (IsGrounded() && _playerGravity.y < 0)
        {
            _playerGravity.y = _gravity;
            _animator.SetBool("IsJumping", false);
        }

        _controller.Move(_playerGravity * Time.deltaTime);
    }
    

    /*bool IsGrounded()
    {
        return Physics.CheckSphere(_sensor.position, _sensorRadius, _groundLayer);
    }*/
    
    bool IsGrounded()
    {
        if (Physics.Raycast(_sensor.position, -transform.up, _sensorRadius, _groundLayer))
        {
            Debug.DrawRay(_sensor.position, -transform.up * _sensorRadius, Color.red);
            return true;
        }

        else
        {
            Debug.DrawRay(_sensor.position, -transform.up * _sensorRadius, Color.green);
            return false;
        }
    }


    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(_sensor.position, _sensorRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(_hands.position, _handsSensorSize);
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.transform.gameObject.tag == "Empujable")
        {
            //Rigidbody rBody = hit.collider.attachedRigidbody;
            Rigidbody rBody = hit.transform.GetComponent<Rigidbody>();

            if (rBody == null || rBody.isKinematic) //El Kinematic hace que no le afecte la gravedad a objetos.
            {
                return;
            }

            Vector3 pushDirection = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);

            rBody.linearVelocity = pushDirection * _pushForce / rBody.mass;
        }
    }

    void GrabObject()
    {
        if (_grabedObject == null)
        {
            Collider[] objectsToGrab = Physics.OverlapBox(_hands.position, _handsSensorSize);

            foreach (Collider item in objectsToGrab)
            {
                IGrabeable grabeableObject = item.GetComponent<IGrabeable>();

                if (grabeableObject != null)
                {
                    _grabedObject = item.transform;
                    _grabedObject.SetParent(_hands);
                    _grabedObject.position = _hands.position;
                    _grabedObject.rotation = _hands.rotation;
                    _grabedObject.GetComponent<Rigidbody>().isKinematic = true;

                    return;
                }
            }
        }

        else
        {
            _grabedObject.SetParent(null);
            _grabedObject.GetComponent<Rigidbody>().isKinematic = false;
            _grabedObject = null;
        }

    }

    void Throw()
    {
        if (_grabedObject == null)
        {
            return;
        }

        Rigidbody grabedBody = _grabedObject.GetComponent<Rigidbody>();

        _grabedObject.SetParent(null);
        grabedBody.isKinematic = false;
        grabedBody.AddForce(_maincamera.transform.forward * _throwForce, ForceMode.Impulse);
        _grabedObject = null;
    }
    
    void RayTest()
    {
        //Raycast simple

        if (Physics.Raycast(transform.position, transform.forward, 6))
        {
            Debug.Log("Hit");
            Debug.DrawRay(transform.position, transform.forward * 6, Color.red);
        }

        else
        {
            Debug.DrawRay(transform.position, transform.forward * 6, Color.green);
        }


        //RayCast "Avanzado"
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 6)) //El Out hit almacena lo que toca en el Hit de arriba.
        {
            Debug.Log(hit.transform.name);
            Debug.Log(hit.transform.position);
            Debug.Log(hit.transform.gameObject.layer);
            Debug.Log(hit.transform.tag);

            /*if (hit.transform.tag == "Empujable")
            {
                Box box = hit.transform.GetComponent<Box>();

                if (box != null)
                {
                    Debug.Log("Cosas");
                }
            }*/

            IDamageable damageable = hit.transform.GetComponent<IDamageable>();

            if (damageable != null)
            {
                damageable.TakeDamage(6);
            }
        }

        Ray ray = Camera.main.ScreenPointToRay(_lookInput); //ScreenPointToRay, importante aprenderselo.
        RaycastHit hit2;
        if(Physics.Raycast(ray, out hit2, Mathf.Infinity))
        {
            Vector3 playerforward = hit2.point - transform.position;
            playerforward.y = 0;
            transform.forward = playerforward;
        }
    }
}
