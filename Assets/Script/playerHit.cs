using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerHit : MonoBehaviour
{
    public bool guideHit=false;
    public string whatToHit;
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == whatToHit)
        {
            guideHit = true;
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == whatToHit)
        {
            guideHit = false;
        }
    }
}
