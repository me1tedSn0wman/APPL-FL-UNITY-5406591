using UnityEngine;

public class DoorInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] private Animator animator;
    private bool isOpen = false;

    public bool IsInteractable() { 
        return true;
    }

    public void OnInteract() {
        SetDoorOpened(!isOpen);
    }

    public void SetDoorOpened(bool value) {
        animator.SetBool("IsOpen_b", value);
        isOpen = value;
    }
}
