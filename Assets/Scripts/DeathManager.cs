using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathManager : MonoBehaviour
{
    [SerializeField] GameObject deathScreen;
    public static System.Action Die;
    private void OnEnable()
    {
        Die += OnDie;
    }

    private void OnDisable()
    {
        Die -= OnDie;
    }

    private void OnDie()
    {
        deathScreen.SetActive(true);
        StopAllCoroutines();
        StartCoroutine(ReloadSceneRoutine(SceneManager.GetActiveScene().buildIndex));
    }

    private IEnumerator ReloadSceneRoutine(int buildIndex)
    {
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene(buildIndex);
    }
}
