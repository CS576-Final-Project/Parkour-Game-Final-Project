using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeImagePosition : MonoBehaviour
{
    public Image grappleHUD;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 imagePosition = Camera.main.WorldToScreenPoint(this.transform.position);
        grappleHUD.transform.position = imagePosition;
    }
}
