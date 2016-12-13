using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BehaviourWeak : BehaviourChannel
{
    public const int id = 5;

    private int weakBreak;
    private float currentShake;
    private int shakeDirection;
    private float elapsed;

    private float shakeAmplitude = 2.0f;
    private float shakeAmplitudeIncrease = 1f;
    private float shakeIntensity = 0.35f;
    private float shakeThinkInterval = 0.01f;
    private float shrinkIntensity = 0.05f;
    private float shrinkThinkInterval = 0.01f;

    public override void Start()
    {
        base.Start();
        weakBreak = GetChannelDurability(channelID);
        shakeDirection = 1;
        currentShake = 0;

        GameObject cracks = new GameObject(name, new System.Type[] { typeof(CanvasRenderer), typeof(Image) });
        cracks.tag = "Crack";
        cracks.GetComponent<Image>().sprite = levelManager.spriteCracks[channelID];
        cracks.transform.SetParent(this.transform);
        cracks.GetComponent<RectTransform>().sizeDelta = GetComponent<RectTransform>().sizeDelta;
        cracks.transform.localPosition = new Vector3(0, 0, 0);
        cracks.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
    }

    public void Update()
    {
        elapsed += Time.deltaTime;
        if (elapsed > shakeThinkInterval)
        {
            elapsed -= shakeThinkInterval;
            currentShake += shakeDirection * shakeIntensity;
            transform.rotation *= Quaternion.Euler(0, 0, shakeDirection * shakeIntensity);
            if (Mathf.Abs(currentShake) >= shakeAmplitude)
                shakeDirection *= -1;
        }
    }

    public override void EventPlacedShade(BehaviourBall ball)
    {
        shakeAmplitude += shakeAmplitudeIncrease;
        if (--weakBreak == 0)
        {
            StartCoroutine(AnimationDestroyByBreak());
        }
    }

    private IEnumerator AnimationDestroyByBreak()
    {
        while (transform.localScale.x > 0)
        {
            transform.localScale -= new Vector3(shrinkIntensity, shrinkIntensity, shrinkIntensity);
            yield return new WaitForSeconds(shrinkThinkInterval);
        }
        for (int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(0).gameObject);
        }
        base.isDead = true;
        tag = "ShadedChannel";
    }

    private int GetChannelDurability(int channelID)
    {
        if (channelID == 0)
            return 2;
        else if (channelID == 1)
            return 2;
        else if (channelID == 2)
            return 4;
        else if (channelID == 3)
            return 4;
        else if (channelID == 4)
            return 2;
        return 0;
    }
}