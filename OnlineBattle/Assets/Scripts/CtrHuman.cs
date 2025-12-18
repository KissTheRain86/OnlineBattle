using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CtrHuman : BaseHuman
{
    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Physics.Raycast(ray,out RaycastHit hit);
            if(hit.collider.tag == "Terrain")
            {
                MoveTo(hit.point);
            }
        }
    }
}
