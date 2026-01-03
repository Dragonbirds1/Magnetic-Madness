using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BowlingHUDUI : MonoBehaviour
{
    [Header("Read from ball")]
    public BowlingBallController2D ball;

    [Header("Root (disable until in range)")]
    public CanvasGroup rootGroup;      // put on a parent UI object
    public bool hideUntilInRange = true;
    public float showFadeSpeed = 12f;

    [Header("Charge UI")]
    public Image chargeFill;           // Image Type = Filled
    public RectTransform perfectBand;  // thin rect showing perfect window

    [Header("Combo UI")]
    public TMP_Text comboText;             // or TMP_Text if you use TextMeshPro
    public Image comboFill;            // optional: filled image showing combo progress

    [Header("Strike Banner UI")]
    public CanvasGroup strikeBannerGroup;
    public RectTransform strikeBannerTransform;
    public float strikeShowTime = 0.55f;

    [Header("Perfect Bounce")]
    public RectTransform bounceTarget; // usually the whole charge UI container
    public float bounceScale = 1.12f;
    public float bounceIn = 0.06f;
    public float bounceOut = 0.14f;

    Coroutine strikeRoutine;
    Coroutine bounceRoutine;

    void Awake()
    {
        if (rootGroup)
        {
            rootGroup.alpha = 0f;
            rootGroup.interactable = false;
            rootGroup.blocksRaycasts = false;
        }

        if (strikeBannerGroup)
        {
            strikeBannerGroup.alpha = 0f;
            strikeBannerGroup.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (!ball) return;

        // -------- show/hide until in range --------
        bool shouldShow = (!hideUntilInRange) || ball.InRange || ball.hasBeenThrown;

        if (rootGroup)
        {
            float targetA = shouldShow ? 1f : 0f;
            rootGroup.alpha = Mathf.MoveTowards(rootGroup.alpha, targetA, showFadeSpeed * Time.unscaledDeltaTime);
            rootGroup.interactable = shouldShow;
            rootGroup.blocksRaycasts = shouldShow;
        }

        // DO NOT stop updating while the ball is rolling
        if (hideUntilInRange && !ball.InRange && !ball.hasBeenThrown)
            return;

        // If hidden, skip updating visuals (optional)
        if (hideUntilInRange && !ball.InRange && !ball.hasBeenThrown) return;

        // -------- charge fill --------
        if (chargeFill) chargeFill.fillAmount = ball.Charge01;

        // -------- perfect window band --------
        if (perfectBand)
        {
            float target = ball.PerfectTarget01;
            float window = ball.PerfectWindow01;

            RectTransform parent = (RectTransform)perfectBand.parent;
            float parentW = parent.rect.width;

            float bandCenterX = Mathf.Lerp(0f, parentW, target);
            float bandW = Mathf.Max(6f, parentW * (window * 2f));

            perfectBand.anchoredPosition = new Vector2(bandCenterX, perfectBand.anchoredPosition.y);
            perfectBand.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, bandW);

            float pulse = ball.InPerfectWindow ? (1f + Mathf.Sin(Time.unscaledTime * 18f) * 0.08f) : 1f;
            perfectBand.localScale = new Vector3(pulse, pulse, 1f);
        }



        // -------- combo UI --------
        int c = ball.combo;
        int cMax = Mathf.Max(1, ball.comboMax);

        if (comboText)
        {
            comboText.text = (c > 0) ? $"COMBO x{c}" : "";
        }

        if (comboFill)
        {
            comboFill.fillAmount = Mathf.Clamp01((float)c / cMax);
        }

        // -------- perfect bounce trigger --------
        if (ball.PerfectJustTriggeredThisFrame)
        {
            TriggerPerfectBounce();
        }
    }

    public void TriggerStrikeBanner()
    {
        if (!strikeBannerGroup) return;

        if (strikeRoutine != null) StopCoroutine(strikeRoutine);
        strikeRoutine = StartCoroutine(StrikeBannerRoutine());
    }

    IEnumerator StrikeBannerRoutine()
    {
        // Force HUD visible for the banner
        if (rootGroup)
        {
            rootGroup.alpha = 1f;
            rootGroup.interactable = true;
            rootGroup.blocksRaycasts = true;
        }

        strikeBannerGroup.gameObject.SetActive(true);

        // quick pop
        if (strikeBannerTransform) strikeBannerTransform.localScale = Vector3.one * 1.15f;
        strikeBannerGroup.alpha = 0f;

        float t = 0f;
        while (t < 0.12f)
        {
            t += Time.unscaledDeltaTime;
            strikeBannerGroup.alpha = Mathf.Lerp(0f, 1f, t / 0.12f);
            if (strikeBannerTransform)
                strikeBannerTransform.localScale = Vector3.Lerp(Vector3.one * 1.15f, Vector3.one, t / 0.12f);
            yield return null;
        }

        strikeBannerGroup.alpha = 1f;

        // hold
        float hold = 0f;
        while (hold < strikeShowTime)
        {
            hold += Time.unscaledDeltaTime;
            yield return null;
        }

        // fade out
        t = 0f;
        while (t < 0.18f)
        {
            t += Time.unscaledDeltaTime;
            strikeBannerGroup.alpha = Mathf.Lerp(1f, 0f, t / 0.18f);
            yield return null;
        }

        strikeBannerGroup.alpha = 0f;
        strikeBannerGroup.gameObject.SetActive(false);
    }

    void TriggerPerfectBounce()
    {
        if (!bounceTarget) return;

        if (bounceRoutine != null) StopCoroutine(bounceRoutine);
        bounceRoutine = StartCoroutine(BounceRoutine());
    }

    IEnumerator BounceRoutine()
    {
        Vector3 start = Vector3.one;
        Vector3 up = Vector3.one * bounceScale;

        // in
        float t = 0f;
        while (t < bounceIn)
        {
            t += Time.unscaledDeltaTime;
            bounceTarget.localScale = Vector3.Lerp(start, up, t / bounceIn);
            yield return null;
        }

        // out
        t = 0f;
        while (t < bounceOut)
        {
            t += Time.unscaledDeltaTime;
            bounceTarget.localScale = Vector3.Lerp(up, start, t / bounceOut);
            yield return null;
        }

        bounceTarget.localScale = start;
    }
}
