using UnityEngine;
using TMPro;
using DG.Tweening;

public class LetterMorph : MonoBehaviour
{
    [System.Serializable]
    public class LetterPair
    {
        public RectTransform blueLetter;
        public RectTransform targetPosition;
    }

    public LetterPair[] letters;
    public TMP_Text redText;
    public float travelTime = 1f;
    public float fadeTime = 0.5f;

    void Start()
    {
        PlayEffect();
    }

    public void PlayEffect()
    {
        redText.alpha = 0;

        // ------------------------
        // BLUE LETTER MORPH OUT
        // ------------------------
        foreach (var pair in letters)
        {
            pair.blueLetter.DOMove(pair.targetPosition.position, travelTime)
                .SetEase(Ease.InOutQuad);

            pair.blueLetter.GetComponent<TMP_Text>()
                .DOFade(0, fadeTime)
                .SetDelay(travelTime - 0.2f);
        }

        // ------------------------
        // RED HORROR TITLE FADE IN
        // ------------------------
        redText.DOFade(1, fadeTime)
               .SetDelay(travelTime);
    }
}
