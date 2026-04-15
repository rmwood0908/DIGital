using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SurfaceSurveyProgressUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject root;
    [SerializeField] private Slider progressSlider;
    [SerializeField] private TMP_Text countText;

    public void Refresh(int recorded, int required)
    {
        if (progressSlider != null)
        {
            progressSlider.minValue = 0f;
            progressSlider.maxValue = required;
            progressSlider.wholeNumbers = true;
            progressSlider.value = recorded;
        }

        if (countText != null)
        {
            countText.text = $"{recorded} / {required}";
        }

        if (root != null)
        {
            root.SetActive(recorded < required);
        }
    }
}