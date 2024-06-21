using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class SliderTextUpdater : MonoBehaviour
{
    public TextMeshProUGUI handleText;

    private Slider slider;

    private void Start()
    {
        // Get the Slider component automatically
        slider = GetComponent<Slider>();

        // Set the initial text value
        UpdateHandleText(slider.value);

        // Add a listener to the slider's onValueChanged event
        slider.onValueChanged.AddListener(UpdateHandleText);
    }

    private void UpdateHandleText(float value)
    {
        string formatString;

        if (slider.wholeNumbers)
        {
            // If the slider is in integer mode, use no decimal places
            formatString = "F0";
        }
        else
        {
            if (slider.maxValue < 10f)
            {
                // Display 2 decimal places if max value is less than 10
                formatString = "F2";
            }
            else if (slider.maxValue < 100f)
            {
                // Display 1 decimal place if max value is between 10 and 100
                formatString = "F1";
            }
            else
            {
                // Display no decimal places if max value is 100 or greater
                formatString = "F0";
            }

            // Handle the case where the number is negative
            if (value < 0f)
            {
                formatString = "F1";
            }
        }

        // Update the text of the handle with the slider's value
        handleText.text = value.ToString(formatString);
    }
}