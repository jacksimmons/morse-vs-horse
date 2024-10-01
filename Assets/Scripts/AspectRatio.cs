using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Sanitised 29/9/24

/// <summary>
/// Static class for handling operations related to aspect ratios.
/// </summary>
public static class AspectRatio
{
    /// <summary>
    /// Gets an array of resolutions which are 16:9, and distinct enough - e.g. 1080p 59.4Hz and 1080 59.9Hz are not distinct enough.
    /// The one which is added first remains and subsequent non-distinct resolutions are discarded, and displayed refresh rate is rounded up.
    /// </summary>
    public static Resolution[] GetSupportedResolutions()
    {
        // Eliminate non-16:9 aspect ratios
        List<Resolution> validAspectRatioResolutions = Screen.resolutions.Where(x => Mathf.Approximately((float)x.width / x.height, 16f / 9)).ToList();

        // Eliminate "identical" refresh rates (e.g. 59.4, 59.9Hz is annoying)
        Resolution firstRes = validAspectRatioResolutions[0];
        int prevW = firstRes.width, prevH = firstRes.height;
        int prevHz = Mathf.RoundToInt((float)firstRes.refreshRateRatio.value);
        List<Resolution> uniqueRefreshRateAndValidAspectRatioResolutions = new(validAspectRatioResolutions);
        for (int i = 0; i < validAspectRatioResolutions.Count; i++)
        {
            int thisW, thisH;
            int thisHz;

            // Current resolution stats
            Resolution resolution = validAspectRatioResolutions[i];
            thisW = resolution.width;
            thisH = resolution.height;
            thisHz = Mathf.RoundToInt((float)resolution.refreshRateRatio.value);

            // Check if this resolution is "identical" to the previous
            if (thisW == prevW && thisH == prevH && thisHz == prevHz)
                uniqueRefreshRateAndValidAspectRatioResolutions.Remove(resolution);

            // Update previous markers
            prevW = thisW;
            prevH = thisH;
            prevHz = thisHz;
        }
        return uniqueRefreshRateAndValidAspectRatioResolutions.ToArray();
    }
}