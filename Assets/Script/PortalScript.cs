using System.Collections.Generic;
using UnityEngine;

public class PortalScript : MonoBehaviour
{
    public Transform linkedWall;

    private HashSet<GameObject> ignoredBullets = new HashSet<GameObject>();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Bullet")) return;
        if (ignoredBullets.Contains(collision.gameObject)) return;

        Rigidbody2D rb = collision.attachedRigidbody;
        if (rb != null && linkedWall != null)
        {
            //postition maping
            Vector3 localPos = transform.InverseTransformPoint(collision.transform.position); // fuck you error
            Vector3 newWorldPos = linkedWall.TransformPoint(localPos);                        // fuck you chatgpt
            collision.transform.position = newWorldPos;

            //velocity mapping
            Vector2 localVel = transform.InverseTransformDirection(rb.velocity); // to portal A worldspace
            Vector2 newVel = linkedWall.TransformDirection(localVel);            // to portal B world space
            rb.velocity = newVel;

            // rotation align with velocity
            if (newVel.sqrMagnitude > 0.01f)
            {
                float angle = Mathf.Atan2(newVel.y, newVel.x) * Mathf.Rad2Deg;
                collision.transform.rotation = Quaternion.Euler(0, 0, angle);
            }

            // prevents fucking infunite loop
            PortalScript linkedPortal = linkedWall.GetComponent<PortalScript>();
            if (linkedPortal != null)
            {
                linkedPortal.IgnoreBullet(collision.gameObject);
            }
        }
    }

    public void IgnoreBullet(GameObject bullet)
    {
        if (!ignoredBullets.Contains(bullet))
        {
            ignoredBullets.Add(bullet);
            StartCoroutine(RemoveIgnoreAfterExit(bullet));
        }
    }

    private System.Collections.IEnumerator RemoveIgnoreAfterExit(GameObject bullet)
    {
        yield return new WaitForSeconds(0.1f);
        ignoredBullets.Remove(bullet);
    }
}
