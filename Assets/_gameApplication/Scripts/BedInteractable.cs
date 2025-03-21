using UnityEngine;

public class BedInteractable : MonoBehaviour, IInteractable
{
    public bool IsInteractable() {
        return true;
    }

    public void OnInteract() {
        GameplayTime.Instance.AddMinutesToTime(180);
    }
}
