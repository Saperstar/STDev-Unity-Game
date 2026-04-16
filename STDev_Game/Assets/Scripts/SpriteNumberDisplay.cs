using UnityEngine;
using UnityEngine.UI;

public class SpriteNumberDisplay : MonoBehaviour
{
    [Header("Digit Sprites")]
    [SerializeField] private Sprite[] digitSprites = new Sprite[10];

    [Header("Digit Images (left to right)")]
    [SerializeField] private Image[] digitImages;

    [Header("Options")]
    [SerializeField] private bool padWithZeros = true;
    [SerializeField] private bool hideLeadingZeros = false;

    public void SetNumber(int number)
    {
        number = Mathf.Max(0, number);

        if (digitImages == null || digitImages.Length == 0)
            return;

        string valueText = number.ToString();

        if (valueText.Length > digitImages.Length)
        {
            valueText = valueText.Substring(valueText.Length - digitImages.Length);
        }

        if (padWithZeros)
        {
            valueText = valueText.PadLeft(digitImages.Length, '0');
        }

        for (int i = 0; i < digitImages.Length; i++)
        {
            int digit = valueText[i] - '0';

            if (digit < 0 || digit > 9 || digitSprites[digit] == null)
                continue;

            digitImages[i].sprite = digitSprites[digit];
            digitImages[i].enabled = true;
        }

        if (hideLeadingZeros && !padWithZeros)
        {
            bool foundNonZero = false;

            for (int i = 0; i < digitImages.Length; i++)
            {
                if (valueText[i] != '0' || i == digitImages.Length - 1)
                {
                    foundNonZero = true;
                }

                digitImages[i].enabled = foundNonZero;
            }
        }
    }
}