using UnityEngine;

public class MusicZoneTrigger : MonoBehaviour
{
    public MusicZoneManager musicZoneManager;
    public AudioClip zoneTrack;
    public float zoneMusicVolume = 1f;

    protected void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) {
            musicZoneManager.ChangeZone(zoneTrack, zoneMusicVolume);
        }
    }
}
