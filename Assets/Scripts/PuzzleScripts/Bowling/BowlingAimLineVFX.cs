using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class BowlingAimLineVFX : MonoBehaviour
{
    [Header("Read from ball")]
    public BowlingBallController2D ball;

    [Header("Direction")]
    public bool useRightInsteadOfUp = false; // toggle if your forward is right
    public bool hideWhenThrown = true;

    [Header("Length/Width by charge")]
    public float minLength = 2f;
    public float maxLength = 8f;
    public AnimationCurve lengthByCharge = AnimationCurve.EaseInOut(0, 0.25f, 1, 1);
    public AnimationCurve widthByCharge = AnimationCurve.Linear(0, 0.05f, 1, 0.14f);

    [Header("Color + Juice")]
    public Gradient colorByCharge;
    public float pulseSpeed = 18f;
    public float pulseAmount = 0.06f;

    [Header("Perfect shimmer")]
    public float perfectGlowWidthMult = 1.5f;
    public float perfectPulseSpeed = 26f;

    LineRenderer lr;

    void Awake()
    {
        lr = GetComponent<LineRenderer>();
        lr.useWorldSpace = true;
        lr.positionCount = 2;
    }

    void LateUpdate()
    {
        if (!ball)
        {
            lr.enabled = false;
            return;
        }

        if (hideWhenThrown && ball.hasBeenThrown)
        {
            lr.enabled = false;
            return;
        }

        lr.enabled = true;

        float t = Mathf.Clamp01(ball.Charge01);
        float len = Mathf.Lerp(minLength, maxLength, lengthByCharge.Evaluate(t));

        Transform pivot = ball.aimPivot ? ball.aimPivot : ball.transform;
        Vector3 dir = useRightInsteadOfUp ? pivot.right : pivot.up;
        dir.z = 0f;
        if (dir.sqrMagnitude < 0.0001f) dir = Vector3.up;

        Vector3 start = ball.transform.position;
        Vector3 end = start + dir.normalized * len;

        lr.SetPosition(0, start);
        lr.SetPosition(1, end);

        float baseWidth = widthByCharge.Evaluate(t);
        bool inPerfect = ball.InPerfectWindow;

        float pulse = 1f + Mathf.Sin(Time.time * (inPerfect ? perfectPulseSpeed : pulseSpeed)) * pulseAmount;
        float width = baseWidth * pulse * (inPerfect ? perfectGlowWidthMult : 1f);

        lr.startWidth = width;
        lr.endWidth = width * 0.8f;

        if (colorByCharge != null && colorByCharge.colorKeys.Length > 0)
            lr.colorGradient = colorByCharge;
    }
}
