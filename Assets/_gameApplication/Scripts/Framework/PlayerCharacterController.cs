using System;
using UnityEngine;
using UnityEngine.EventSystems;

public enum PlayerControlAvailability { 
    FullControl,
    OnlyCameraRotation,
    DisableControl,
}

public class PlayerCharacterController : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Animator animator;
    [SerializeField] private CharacterController characterController;

    [Header("Params")]
    [SerializeField] private float walkingSpeed= 6f;
    [SerializeField] private float sprintSpeed = 10f;
    [SerializeField] private float jumpForce = 8.0f;
    [SerializeField] private float gravity = -20.0f;
    [SerializeField] private float lookSpeed = 2.0f;
    [SerializeField] private float lookXLimit = 45.0f;

    [SerializeField] private PlayerControlAvailability _playerControlAvailability = PlayerControlAvailability.FullControl;
    public PlayerControlAvailability playerControlAvailability
    {
        get { return _playerControlAvailability; }
        set
        {
            _playerControlAvailability = value;
        }
    }

    [SerializeField] private bool isCanMoveControl {
        get {
            switch (playerControlAvailability) {
                case PlayerControlAvailability.FullControl:
                    return true;
                case PlayerControlAvailability.OnlyCameraRotation:
                case PlayerControlAvailability.DisableControl:
                default:
                    return false;

            }
        }
    }

    [SerializeField] private bool isCanCameraControl {
        get {
            switch (playerControlAvailability) {
                case PlayerControlAvailability.FullControl:
                case PlayerControlAvailability.OnlyCameraRotation:
                    return true;
                case PlayerControlAvailability.DisableControl:
                default:
                    return false;
            }
        }
    }

    [Header("Interactable")]
    [SerializeField] private float interactableRaycastLength = 5.0f;


    private Vector3 characterVelocity;
    private float rotationX = 0;

    public event Action<bool> OnLookAtInteractable;

    protected void Awake()
    {
        playerControlAvailability = PlayerControlAvailability.FullControl;
    }

    protected void Start()
    {
        PlayerControlManager.Instance.OnInteractionPressed += TryInteract;
    }

    protected void Update()
    {
        MoveCharacter();
        RotateCamera();
        ShowInteract();
        SetMovingAnimation();
    }

    protected void MoveCharacter() {
        if (!isCanMoveControl) return;

        if (characterController.isGrounded && characterVelocity.y < 0) {
            characterVelocity.y = 0; 
        }


        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        bool isSprinting = PlayerControlManager.Instance.sprint;
        Vector2 speedHor = (isSprinting ? sprintSpeed : walkingSpeed) * PlayerControlManager.Instance.move;

        float speedVer = characterVelocity.y;
        characterVelocity = forward * speedHor.y + right * speedHor.x;

        if (characterController.isGrounded && (speedHor.sqrMagnitude < 0.1f)) { 
            // play footsteps;
        }

        if (PlayerControlManager.Instance.jump && characterController.isGrounded)
        {
            characterVelocity.y = jumpForce;
        } else {
            characterVelocity.y = speedVer;
        }
        
        if (!characterController.isGrounded) {
            characterVelocity.y -= gravity * Time.deltaTime;
        }


        characterController.Move(characterVelocity * Time.deltaTime);
    }
    private void RotateCamera() {
        if (!isCanCameraControl) return;

        rotationX -= PlayerControlManager.Instance.look.y * lookSpeed;
        rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);

        float currentYRotation = transform.localEulerAngles.y;
        float targetYRotation = currentYRotation + PlayerControlManager.Instance.look.x * lookSpeed;

        transform.localRotation = Quaternion.Euler(0, targetYRotation, 0);
    }

    private IInteractable GetInteractableByRaycast() {
        int layerMask = 1 << 12;
        RaycastHit hit;

        

        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.TransformDirection(Vector3.forward), out hit, interactableRaycastLength, layerMask))
        {
            IInteractable inter = hit.transform.GetComponent<IInteractable>();

            Debug.DrawRay(playerCamera.transform.position, playerCamera.transform.TransformDirection(Vector3.forward) * interactableRaycastLength, Color.yellow, 0.0f);
            if (inter != null)
            {
                return inter;
            }

        }
        else {
            Debug.DrawRay(playerCamera.transform.position, playerCamera.transform.TransformDirection(Vector3.forward) * interactableRaycastLength, Color.red, 0.0f);
        }
        return null;
    }

    public void ShowInteract() { 
        IInteractable inter = GetInteractableByRaycast();
        if (inter != null && inter.IsInteractable())
        {
            OnLookAtInteractable?.Invoke(true);
        }
        else {
            OnLookAtInteractable?.Invoke(false);
        }
    }

    public void TryInteract() {
        Debug.Log("Try Interact");

        IInteractable inter = GetInteractableByRaycast();
        if (inter == null) {
            return;
        }

        inter.OnInteract();
    }

    protected virtual void SetMovingAnimation()
    {
        animator.SetFloat("Speed_f", characterVelocity.magnitude);
    }

    public void OnDestroy()
    {
        PlayerControlManager.Instance.OnInteractionPressed -= TryInteract;
    }
}
