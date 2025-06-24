using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadBannerAds : MonoBehaviour
{
    private void Awake()
    {
        StartCoroutine(loadBanner_withDelay());
    }

    private IEnumerator loadBanner_withDelay()
    {
        yield return new WaitForSeconds(5f);
        AdsManager.Instance.bannerAds.ShowBannerAd();
        StartCoroutine(hideBanner_withDelay());
    }

    private IEnumerator hideBanner_withDelay()
    {
        yield return new WaitForSeconds(5f);
        AdsManager.Instance.bannerAds.HideBannerAd();
    }

    public void hideBannerAd()
    {
        AdsManager.Instance.bannerAds.HideBannerAd();
    }
}
