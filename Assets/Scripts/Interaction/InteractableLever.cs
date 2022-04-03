using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableLever : InteractableBase
{
    [SerializeField] Transform leverRoot;

    [SerializeField] private Vector2 rotationMinMax;

    [SerializeField] private float speedMultiplyer = 1;

    [Range(0, 1)]
    [SerializeField] private float startingValue;

    [SerializeField] private UnityEngine.Events.UnityEvent<float> onLeverMoved;

    Coroutine updateRoutine;

    private float percent;

    public override bool bIsHeldInteraction => true;

    private void Start()
    {
        percent = startingValue;
        updateState(percent);
    }

    public override void StartInteracting()
    {
        base.StartInteracting();

        PlayerController.SetRotateLock(PlayerRotateLock.Lever, true);
        updateRoutine = StartCoroutine(interactUpdate());
    }

    public override void StopInteracting()
    {
        base.StopInteracting();

        StopCoroutine(updateRoutine);
        PlayerController.SetRotateLock(PlayerRotateLock.Lever, false);
    }

    private IEnumerator interactUpdate()
    {
        Camera cam = Camera.main;

        while (true)
        {
            yield return null;
            Vector2 delta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

            var direction = delta.x * cam.transform.right + delta.y * cam.transform.up;

            float force = Vector3.Dot(leverRoot.forward, direction);

            if (force != 0)
            {
                percent = Mathf.Clamp01(percent + force * Time.deltaTime * speedMultiplyer);
                updateState(percent);
            }
        }
    }
    protected virtual void updateState(float _percent)
    {
        leverRoot.localEulerAngles = new Vector3(Mathf.Lerp(rotationMinMax.x, rotationMinMax.y, _percent), 0, 0);
        onLeverMoved.Invoke(_percent);
    }
}
