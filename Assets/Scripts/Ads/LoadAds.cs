using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadAds : MonoBehaviour
{
    private void Awake()
    {
        StartCoroutine(displayBannerAdsDelayed());
    }

    private IEnumerator displayBannerAdsDelayed()
    {
        yield return new WaitForSeconds(1f);
        AdsManager.Instance.bannerAds.ShowBannerAd();

    }
}
