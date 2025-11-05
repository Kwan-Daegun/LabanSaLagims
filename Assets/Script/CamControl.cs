using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CamControl : MonoBehaviour
{
    public CinemachineVirtualCamera vcam;
    public GameObject player;
    public List<GameObject> enemies;

    private PlayerControl playerControl;
    private bool cutscenePlayed = false;
    private float defaultOrthoSize;   // store default ortho size
    private Coroutine zoomCoroutine;

    void Start()
    {
        playerControl = player.GetComponent<PlayerControl>();
        if (vcam != null)
            defaultOrthoSize = vcam.m_Lens.OrthographicSize; // save starting value
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!cutscenePlayed && collision.gameObject == player)
        {
            StartCoroutine(CutSceneCoroutine());
        }
    }

    IEnumerator CutSceneCoroutine()
    {
        cutscenePlayed = true;

        if (playerControl != null)
            playerControl.stopPlayerMovement = true;

        foreach (GameObject enemy in enemies)
        {
            if (enemy != null)
            {
                vcam.Follow = enemy.transform;
                SmoothZoom(2f); // zoom smoothly set to 2
                yield return new WaitForSeconds(2f);
            }
        }

        if (playerControl != null)
        {
            playerControl.stopPlayerMovement = false;
            playerControl.canShoot = true; // enable shooting after cutscene
        }

        vcam.Follow = player.transform;
        SmoothZoom(defaultOrthoSize); // zoom smoothly back to default

        // start timer after cutscene
        GameManager gm = FindObjectOfType<GameManager>();
        if (gm != null) gm.StartTimer();

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;
    }

    public void FollowBullet(GameObject bullet)
    {
        if (bullet == null) return;

        vcam.Follow = bullet.transform;
        StartCoroutine(ReturnToPlayerWhenDestroyed(bullet));
    }

    private IEnumerator ReturnToPlayerWhenDestroyed(GameObject bullet)
    {
        while (bullet != null)
            yield return null;

        vcam.Follow = player.transform;
        SmoothZoom(defaultOrthoSize); // reset smoothly when back to player
    }

    // smooth zoom helper uwu
    private void SmoothZoom(float targetSize)
    {
        if (zoomCoroutine != null)
            StopCoroutine(zoomCoroutine);

        zoomCoroutine = StartCoroutine(ZoomRoutine(targetSize));
    }

    private IEnumerator ZoomRoutine(float targetSize)
    {
        float startSize = vcam.m_Lens.OrthographicSize;
        float elapsed = 0f;
        float duration = 0.1f; // zoom speed

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            vcam.m_Lens.OrthographicSize = Mathf.Lerp(startSize, targetSize, elapsed / duration);
            yield return null;
        }

        vcam.m_Lens.OrthographicSize = targetSize; // snap exactly at the end
    }
}
