using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDamageInterpolation : MonoBehaviour
{
    
    public HealthHandler healthHandler;
    public bool useSkinnedMesh;
    public MeshRenderer meshRenderer;
    public SkinnedMeshRenderer renderer;
    public Material standardMaterial;
    public Material damagedMaterial;
    
    public float interpolationTimer = 1;
    public float minInterpolationValue = 0;

    public Vector4 RGBAValue = new Vector4(0,0,0,1);

    public bool takenDamage;

    public PlayerAudio playerAudio;
    private Camera camera;
    
    public void OnEnable()
    {
        healthHandler.onDamageTaken += OnHealthChanged;
    }

    public void OnDisable()
    {
        healthHandler.onDamageTaken -= OnHealthChanged;
    }

    private void Awake()
    {
        camera = Camera.main;
    }

    private void OnHealthChanged()
    {
        //FMODUnity.RuntimeManager.PlayOneShot(playerAudio.takingDamage, camera.transform.position);

        if (takenDamage != true)
        {
            takenDamage = true;
            SetColorInterpolationTimer();
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if (interpolationTimer < 1f)
        {
            if (useSkinnedMesh)
            {
                //.material.color = Color.Lerp(damagedMaterial.color, standardMaterial.color, interpolationTimer);
               
                foreach (Material material in renderer.materials)
                {
                    //MaterialObjectFade.MakeFade(material);
                    
                    var value = Mathf.Lerp(0,1, interpolationTimer);
                    //Color newColor = material.GetColor("_Alpha");
                    //var alpha = material.GetFloat("_ColorIntensityLerp");
                    
                    material.SetFloat("_ColorIntensityLerp", value);
                }
            }
            else
            {
                
                //meshRenderer.material.color = Color.Lerp(damagedMaterial.color, standardMaterial.color, interpolationTimer);
            }
           
            interpolationTimer += Time.deltaTime;
        }
        else
        {
            takenDamage = false;
        }
    }
    
    

    public void SetColorInterpolationTimer()
    {
        interpolationTimer = minInterpolationValue;
    }
}
