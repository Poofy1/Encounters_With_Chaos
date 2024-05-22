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

        if (slider.maxValue < 10f)
        {
            // Display 2 decimal places if max value is 1 or less
            formatString = "F2";
        }
        else if (slider.maxValue < 100f)
        {
            // Display 1 decimal place if max value is between 1 and 2
            formatString = "F1";
        }
        else
        {
            // Display no decimal places if max value is greater than 2
            formatString = "F0";
        }

        // Update the text of the handle with the slider's value
        handleText.text = value.ToString(formatString);
    }
}