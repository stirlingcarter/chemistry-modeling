//[csharp]
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OculusGrabR : MonoBehaviour
{

    // identifying objects
    public GameObject CollidingObject;
    public GameObject objectInHand;

    // trigger functions after adding trigger zones to controllers and adding script to controllers
    public void OnTriggerEnter(Collider other) //picking up objects with rigidbodies
    {
        if (other.gameObject.GetComponent<Rigidbody>())
        {
            CollidingObject = other.gameObject;
        }
    }
    public void OnTriggerExit(Collider other) // releasing those objects with rigidbodies
    {
        CollidingObject = null;
    }

    void Update() // refreshing program confirms trigger pressure and determines whether holding or releasing object
    {
        if (OVRInput.Get(OVRInput.Axis1D.SecondaryHandTrigger) > 0.2 && CollidingObject)
        {
            GrabObject();
        }
        if (OVRInput.Get(OVRInput.Axis1D.SecondaryHandTrigger) < 0.2 && objectInHand)
        {
            ReleaseObject();
        }
    }

    public void GrabObject() //create parentchild relationship between object and hand so object follows hand
    {
        objectInHand = CollidingObject;
        objectInHand.transform.SetParent(this.transform);
        objectInHand.GetComponent<Rigidbody>().isKinematic = true;
    }

    private void ReleaseObject() //removing parentchild relationship so you drop the object
    {
        objectInHand.GetComponent<Rigidbody>().isKinematic = false;
        objectInHand.transform.SetParent(null);
        objectInHand = null;
    }
}
//[/csharp] 