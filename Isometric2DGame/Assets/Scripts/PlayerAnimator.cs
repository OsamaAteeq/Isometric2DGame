using System.Collections;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    [SerializeField]
    private float colorChangeSpeed = 2.0f;
    [SerializeField] 
    private float pulseSpeed = 4f;
    [Range(0f, 0.1f)]
    [SerializeField]
    private float pulseStrength = 0.05f;

    private Coroutine colorRoutine, pulseRoutine;

    private bool isMoving;
    private Color startColor;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        startColor = spriteRenderer.color;
    }

    private void Start()
    {
        PlayIdlePulse();
    }

    public void UpdateState(PlayerState state) 
    {
        if (!isMoving && state.Equals(PlayerState.Walking))
        {
            SetMoving();
        }
        else if (isMoving && state.Equals(PlayerState.Idle)) 
        {
            SetMoving(false);
        }
    }

    private void SetMoving(bool isMoving = true)
    {
        if (isMoving)
        {
            PlayColorChange(startColor);
            this.isMoving = isMoving;
            PlayIdlePulse();
        }
        else
        {
            this.isMoving = isMoving;
            PlayColorChange(Color.green);
        }
    }

    private void PlayColorChange(Color color)
    {
        if (colorRoutine != null) StopCoroutine(colorRoutine);
        colorRoutine = StartCoroutine(LerpColor(color));
    }
    private void PlayIdlePulse()
    {
        if (pulseRoutine != null) StopCoroutine(pulseRoutine);
        pulseRoutine = StartCoroutine(PulseScale());
    }

    private IEnumerator LerpColor(Color targetColor)
    {
        Color startColor = spriteRenderer.color;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * colorChangeSpeed;
            spriteRenderer.color = Color.Lerp(startColor, targetColor, t);
            yield return null;
        }

        spriteRenderer.color = targetColor;
    }

    private IEnumerator PulseScale()
    {
        Vector3 original = transform.localScale;
        while (!isMoving)
        {
            float t = (Mathf.Sin(Time.time * pulseSpeed) * pulseStrength);
            transform.localScale = original + new Vector3(t, t, 0);
            yield return null;
        }
    }
}
