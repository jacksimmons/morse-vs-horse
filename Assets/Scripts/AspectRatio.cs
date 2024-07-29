using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public static class AspectRatio
{
    /// <summary>
    /// 16:9
    /// </summary>
    public static Resolution[] GetSupportedResolutions()
    {
        // Eliminate non-16:9 aspect ratios
        List<Resolution> validAspectRatioResolutions = Screen.resolutions.Where(x => Mathf.Approximately((float)x.width / x.height, 16f / 9)).ToList();

        // Eliminate "identical" refresh rates (e.g. 59.4, 59.9Hz is annoying)
        Resolution firstRes = validAspectRatioResolutions[0];
        int prevW = firstRes.width, prevH = firstRes.height;
        int prevHz = Mathf.RoundToInt((float)firstRes.refreshRateRatio.value);

        List<Resolution> validAndUniqueAspectRatioResolutions = new(validAspectRatioResolutions);
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
            {
                validAndUniqueAspectRatioResolutions.Remove(resolution);
            }

            // Update prev resolution stats
            prevW = thisW;
            prevH = thisH;
            prevHz = thisHz;
        }
        return validAndUniqueAspectRatioResolutions.ToArray();
    }
}