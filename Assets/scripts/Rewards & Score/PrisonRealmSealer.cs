using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PrisonRealmSealer : MonoBehaviour
{
    [Header("Prefabs")]
    public LineRenderer tendrilPrefab;
    public GameObject prisonCubePrefab;
    public GameObject ShockwaveSphere;
    private GameObject prisonCube;

    [Header("Player Reference")]
    public CharacterController playerController;

    [Header("Knockback Settings")]
    public float initialKnockbackForce = 5f;
    public float morphKnockbackForce = 15f;
    public float knockbackDuration = 0.2f;

    [Header("Barrage Settings")]
    public int numberOfTendrils = 8;
    public float spawnRadius = 4f;
    public float initialStabDelay = 0.5f;
    public float accelerationFactor = 0.7f;
    public float stabDuration = 0.3f;

    [Header("Morph Settings")]
    public float morphDuration = 1.5f;

    [Header("Sound")]
    public AudioSource stabAudioSource;
    public AudioClip stabSound;
    public float minPitch = 0.7f;
    public float maxPitch = 1.4f;
    public AudioSource airExplosionSOund;

    private class TendrilData
    {
        public LineRenderer line;
        public Vector3 spawnPoint;
        public Vector3 grabOffset;
        public float progress;
    }

    // old method kept for compatibility - just calls the coroutine version
    public void ExecuteSeal(Transform enemy)
    {
        StartCoroutine(ExecuteSealAndWait(enemy));
    }

    // new method that PowerSystem waits on
    public IEnumerator ExecuteSealAndWait(Transform enemy)
    {
        if (playerController != null)
        {
            Vector3 pushDirection = (playerController.transform.position - enemy.position).normalized;
            pushDirection.y = 0f;
            StartCoroutine(ApplyKnockback(pushDirection, initialKnockbackForce, knockbackDuration));
        }

        yield return StartCoroutine(SealSequence(enemy));
    }

    private IEnumerator SealSequence(Transform enemy)
    {
        List<TendrilData> activeTendrils = new List<TendrilData>();
        Vector3 absoluteCenter = enemy.position + (Vector3.up * 1f);

        Vector3 originalEnemyPos = enemy.position;
        Vector3 originalEnemyScale = enemy.localScale;

        float currentDelay = initialStabDelay;

        // PHASE 1: Accelerating barrage with pitch-varied stab sounds
        for (int i = 0; i < numberOfTendrils; i++)
        {
            TendrilData newTendril = new TendrilData
            {
                line = Instantiate(tendrilPrefab),
                progress = 0f,
                spawnPoint = absoluteCenter + (GetTendrilSpawnDirection() * spawnRadius),
                grabOffset = new Vector3(
                    Random.Range(-0.5f, 0.5f),
                    Random.Range(0f, 1.5f),
                    Random.Range(-0.5f, 0.5f))
            };

            newTendril.spawnPoint.y = Mathf.Max(0.5f, newTendril.spawnPoint.y);
            newTendril.line.SetPosition(0, newTendril.spawnPoint);
            newTendril.line.SetPosition(1, newTendril.spawnPoint);

            activeTendrils.Add(newTendril);

            // play stab sound with random pitch
            if (stabAudioSource != null && stabSound != null)
            {
                stabAudioSource.pitch = Random.Range(minPitch, maxPitch);
                stabAudioSource.PlayOneShot(stabSound);
            }

            StartCoroutine(AnimateStab(newTendril, enemy));

            yield return new WaitForSeconds(currentDelay);
            currentDelay *= accelerationFactor;
        }

        yield return new WaitForSeconds(0.4f);

        // PHASE 2: Morph and massive knockback
        prisonCube = Instantiate(prisonCubePrefab, absoluteCenter, Quaternion.identity);
        prisonCube.transform.localScale = Vector3.zero;

        if (playerController != null)
        {
            Vector3 pushDirection = (playerController.transform.position - enemy.position).normalized;
            pushDirection.y = 0f;
            Instantiate(ShockwaveSphere, absoluteCenter, Quaternion.identity);
            airExplosionSOund.Play();
            StartCoroutine(ApplyKnockback(pushDirection, morphKnockbackForce, knockbackDuration * 1.5f));
        }

        float morphTimer = 0f;
        Vector3 targetCubeScale = new Vector3(3f, 3f, 3f);

        while (morphTimer < morphDuration)
        {
            morphTimer += Time.deltaTime;
            float t = morphTimer / morphDuration;
            float morphCurve = t * t;

            enemy.localScale = Vector3.Lerp(originalEnemyScale, Vector3.zero, morphCurve);
            enemy.position = Vector3.Lerp(originalEnemyPos, absoluteCenter, morphCurve);
            prisonCube.transform.localScale = Vector3.Lerp(Vector3.zero, targetCubeScale, morphCurve);

            foreach (var tendril in activeTendrils)
            {
                Vector3 collapsingSpawn = Vector3.Lerp(tendril.spawnPoint, absoluteCenter, morphCurve);
                tendril.line.SetPosition(0, collapsingSpawn);

                Vector3 shrinkingOffset = Vector3.Lerp(tendril.grabOffset, Vector3.zero, morphCurve);
                tendril.line.SetPosition(1, enemy.position + shrinkingOffset);
            }

            yield return null;
        }

        // PHASE 3: cleanup
        // restore scale before EnemyManager returns it to pool
        enemy.localScale = originalEnemyScale;

        foreach (var tendril in activeTendrils)
            Destroy(tendril.line.gameObject);

        StartCoroutine(MakePrisonCubeFallAndDisappear());

        // sealing is done - PowerSystem.SealEnemy will handle the rest
    }

    private IEnumerator MakePrisonCubeFallAndDisappear()
    {
        yield return new WaitForSeconds(5f);
        prisonCube.GetComponent<Rigidbody>().isKinematic = false;

        yield return new WaitForSeconds(15f);
        Destroy(prisonCube);
    }

    Vector3 GetTendrilSpawnDirection()
    {
        // decide top or bottom cone randomly
        bool spawnFromTop = Random.value > 0.5f;

        // angle from the vertical axis (0 = straight up/down, 60 = edge of 120 degree cone)
        float coneAngle = 90f;
        float angle = Random.Range(0f, coneAngle);
        float azimuth = Random.Range(0f, 360f);

        // convert to radians
        float angleRad = angle * Mathf.Deg2Rad;
        float azimuthRad = azimuth * Mathf.Deg2Rad;

        // spherical to cartesian
        float x = Mathf.Sin(angleRad) * Mathf.Cos(azimuthRad);
        float z = Mathf.Sin(angleRad) * Mathf.Sin(azimuthRad);
        float y = Mathf.Cos(angleRad);

        // flip y for bottom cone
        if (!spawnFromTop) y = -y;

        return new Vector3(x, y, z);
    }

    private IEnumerator AnimateStab(TendrilData tendril, Transform enemy)
    {
        float timer = 0f;
        while (timer < stabDuration)
        {
            if (enemy == null || !enemy.gameObject.activeInHierarchy) yield break;
            timer += Time.deltaTime;
            float t = timer / stabDuration;
            float easeInCurve = t * t * t;

            tendril.line.SetPosition(0, tendril.spawnPoint);
            tendril.line.SetPosition(1, Vector3.Lerp(tendril.spawnPoint, enemy.position + tendril.grabOffset, easeInCurve));
            yield return null;
        }

        if (enemy != null)
            tendril.line.SetPosition(1, enemy.position + tendril.grabOffset);
    }

    private IEnumerator ApplyKnockback(Vector3 direction, float initialForce, float duration)
    {
        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float strength = Mathf.Lerp(initialForce, 0f, timer / duration);
            playerController.Move(direction * strength * Time.deltaTime);
            yield return null;
        }
    }
}