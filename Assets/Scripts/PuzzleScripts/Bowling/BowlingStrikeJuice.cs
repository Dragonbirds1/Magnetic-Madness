using System.Collections;
using UnityEngine;

public class BowlingStrikeJuice : MonoBehaviour
{
    [Header("Freeze-frame")]
    public float freezeScale = 0.05f;      // 0.05 = near stop
    public float freezeDuration = 0.12f;   // seconds (unscaled)

    [Header("Optional extra shake on strike")]
    public Camera shakeCamera;
    public float shakeDuration = 0.22f;
    public float shakeStrength = 0.35f;

    [Header("UI")]
    public BowlingHUDUI hud;               // drag your HUD script here

    Coroutine routine;

    public void TriggerStrike()
    {
        if (routine != null) StopCoroutine(routine);
        routine = StartCoroutine(StrikeRoutine());
    }

    IEnumerator StrikeRoutine()
    {
        // UI banner
        if (hud) hud.TriggerStrikeBanner();

        // freeze-frame
        float oldScale = Time.timeScale;
        Time.timeScale = freezeScale;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;

        // optional shake during freeze
        Coroutine shake = null;
        if (shakeCamera) shake = StartCoroutine(Shake());

        yield return new WaitForSecondsRealtime(freezeDuration);

        if (shake != null) StopCoroutine(shake);

        Time.timeScale = oldScale;
        Time.fixedDeltaTime = 0.02f;
    }

    IEnumerator Shake()
    {
        var camT = shakeCamera.transform;
        var start = camT.localPosition;

        float t = 0f;
        while (t < shakeDuration)
        {
            t += Time.unscaledDeltaTime;
            var r = Random.insideUnitCircle * shakeStrength;
            camT.localPosition = start + new Vector3(r.x, r.y, 0f);
            yield return null;
        }

        camT.localPosition = start;
    }
}
