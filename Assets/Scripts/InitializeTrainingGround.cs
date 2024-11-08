using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitializeTrainingGround : MonoBehaviour
{
    // Start is called before the first frame update
    public static int index;
    public int myIndex;
    public void Awake()
    {
        index = 0;
    }
    void Start()
    {
        myIndex = index;
        index++;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
