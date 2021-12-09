using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDFade : MonoBehaviour
{
    private CanvasGroup canvasGroup;
    private RaycastHit selfHit;
    private float maxDistance;

    public bool canSee;

    // Start is called before the first frame update
    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    // Update is called once per frame
    void Update()
    {
        maxDistance = GameObject.FindWithTag("Player").GetComponent<GrapplingHUD>().HUDMaxDistance;
        selfHit = this.transform.parent.parent.gameObject.GetComponent<DeactivateHookHUD>().selfHit;
        StartCoroutine(Fade());
    }

    private IEnumerator Fade() 
    {
        float distanceRatio = selfHit.distance / maxDistance;
        if (distanceRatio < 0.3f)
            distanceRatio = 0f;
        canvasGroup.alpha = 1f - distanceRatio;
        if (!canSee)
            canvasGroup.alpha = Mathf.Clamp(canvasGroup.alpha, 0.45f , 0.6f);
        else
            canvasGroup.alpha = 1;
        yield return new WaitForSeconds(0.1f);
    }
}
