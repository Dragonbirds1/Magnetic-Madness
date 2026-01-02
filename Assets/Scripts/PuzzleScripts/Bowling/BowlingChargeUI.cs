using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BowlingChargeUI : MonoBehaviour
{
    [Header("Read from ball")]
    public BowlingBallController2D ball;

    [Header("UI")]
    public Image fill;                 // Image Type = Filled
    public RectTransform perfectBand;  // a thin image/rect showing perfect window (optional)
    public CanvasGroup flashGroup;     // optional full-screen flash (alpha)

    [Header("Flash")]
    public float flashIn = 0.06f;
    public float flashOut = 0.18f;
    public float flashAlpha = 0.65f;

    Coroutine flashRoutine;

    void Update()
    {
        if (!ball)
        {
            if (fill) fill.fillAmount = 0f;
            return;
        }

        // Fill
        if (fill)
            fill.fillAmount = ball.Charge01;

        // Perfect window band placement (optional)
        if (perfectBand)
        {
            // place band based on perfectTarget and perfectWindow
            float target = ball.PerfectTarget01;
            float window = ball.PerfectWindow01;

            // Assuming band is anchored left->right inside the fill background:
            // We'll set its X position and width as a percentage.
            var parent = (RectTransform)perfectBand.parent;
            float parentW = parent.rect.width;

            float bandCenterX = Mathf.Lerp(0f, parentW, target);
            float bandW = Mathf.Max(6f, parentW * (window * 2f)); // 2x window total

            perfectBand.anchoredPosition = new Vector2(bandCenterX, perfectBand.anchoredPosition.y);
            perfectBand.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, bandW);

            // little pulse when you're inside perfect
            float pulse = ball.InPerfectWindow ? (1f + Mathf.Sin(Time.time * 18f) * 0.08f) : 1f;
            perfectBand.localScale = new Vector3(pulse, pulse, 1f);
        }

        // Flash when perfect triggers (edge detect)
        if (ball.PerfectJustTriggeredThisFrame)
        {
            if (flashRoutine != null) StopCoroutine(flashRoutine);
            flashRoutine = StartCoroutine(Flash());
        }
    }

    IEnumerator Flash()
    {
        if (!flashGroup) yield break;

        flashGroup.gameObject.SetActive(true);

        // in
        float t = 0f;
        while (t < flashIn)
        {
            t += Time.unscaledDeltaTime;
            flashGroup.alpha = Mathf.Lerp(0f, flashAlpha, t / flashIn);
            yield return null;
        }

        // out
        t = 0f;
        while (t < flashOut)
        {
            t += Time.unscaledDeltaTime;
            flashGroup.alpha = Mathf.Lerp(flashAlpha, 0f, t / flashOut);
            yield return null;
        }

        flashGroup.alpha = 0f;
        flashGroup.gameObject.SetActive(false);
    }
}
