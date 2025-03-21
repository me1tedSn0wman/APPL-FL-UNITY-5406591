using UnityEngine;

public class HUD_Interactable : MonoBehaviour
{
    private bool isCanBeShown = true;

    [SerializeField] private PlayerCharacterController characterController;

    public void Start()
    {
        SetInteractableUI(false);
        characterController.OnLookAtInteractable += SetInteractableUI;
    }

    public void SetInteractableUI(bool value) { 
        gameObject.SetActive(value && isCanBeShown);
    }

    public void SetCanBeShownHUD(bool value) {
        isCanBeShown = value;
        if (value == false) {
            SetInteractableUI(false);
        }
    }

    public void OnDestroy()
    {
        characterController.OnLookAtInteractable -= SetInteractableUI;
    }
}
