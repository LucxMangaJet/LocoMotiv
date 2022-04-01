using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class LidEventTrigger : MonoBehaviour
{
    [SerializeField] StudioEventEmitter lidOpenEmitter, lidCloseEmitter;
    public void OnLidOpen()
    {
        lidOpenEmitter?.Play();
    }

    public void OnLidClose()
    {
        lidCloseEmitter?.Play();
    }
}