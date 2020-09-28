using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class AudioOnCollision : MonoBehaviour
{
    [SerializeField, EventRef] private string collisionSoundEventPath = null;
    [SerializeField] private GameObject soundObject = null;
    private EventInstance collisionSound;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject != soundObject) return;

        collisionSound = RuntimeManager.CreateInstance(collisionSoundEventPath);
        RuntimeManager.AttachInstanceToGameObject(collisionSound, soundObject.transform,soundObject.GetComponent<Rigidbody>());
        collisionSound.start();
    }
}
