using TMPro;
using UnityEngine;

public class DigitalClock : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text_DigitalTimer;

    protected void Update()
    {
        text_DigitalTimer.text = GameplayTime.Instance.GetCurrentTimeString();
    }
}
