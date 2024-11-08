using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClampAngles : MonoBehaviour
{
    // Start is called before the first frame update
    public float minRot;
    public float maxRot;
    Rigidbody rb;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        this.transform.localPosition = Vector3.zero;
        rb.position = Vector3.zero;
        LimitRot();
    }

    private void LimitRot()
    {
        Vector3 platformRotation = this.transform.localEulerAngles;

        platformRotation.x = (platformRotation.x > 180) ? platformRotation.x - 360 : platformRotation.x;
        platformRotation.x = Mathf.Clamp(platformRotation.x,minRot,maxRot);
        platformRotation.z = (platformRotation.z > 180) ? platformRotation.z - 360 : platformRotation.z;
        platformRotation.z = Mathf.Clamp(platformRotation.z, minRot, maxRot);

        this.transform.rotation = Quaternion.Euler(platformRotation);
        rb.rotation = Quaternion.Euler(platformRotation);
    }
}
