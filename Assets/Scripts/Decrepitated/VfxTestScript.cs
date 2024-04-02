using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class VfxTestScript : MonoBehaviour
{
    public VisualEffect vfx;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            var attributes = vfx.CreateVFXEventAttribute();

            attributes.SetVector3("Gluffs", new Vector3(3, 3, 0));


            vfx.SendEvent("TestSpawn", attributes);
        }
    }
}

