using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

public class DamagePopup : MonoBehaviour
{

    private float moveXSpeed = .2f;
    private float moveYSpeed = 0;
    private float moveZSpeed = .3f;

    public float disappearTime = .5f;
    private float disappearTimer;
    private float disappearSpeed;

    private float maxTimer;
    private float timer;
    
    private TextMeshPro textMesh;

    private Color textColor;
    private Color ogColor;

    public Vector3 positionOffset = new Vector3(0, .5f, -.12f);
    private Vector3 moveVector;
    
    private ObjectPool<DamagePopup> pool = null;

    private float scaleFactor;
    private Vector3 ogScale;

    private void Awake()
    {
        textMesh = transform.GetComponentInChildren<TextMeshPro>();
        ogColor = textMesh.color;
        ogScale = transform.localScale;
    }

    public void Setup(int damageAmount, Vector3 position, bool isCrit)
    {
        transform.localScale = ogScale;
        transform.position = position;
        
        textMesh.SetText(damageAmount.ToString());
        textColor = ogColor;
        // if (isHealing)
        // {
        //     textColor = Color.green;
        //     textMesh.color = textColor;
        // }
        // else
        textMesh.color = ogColor;
        
        disappearTimer = disappearTime + Random.Range(-.1f, .1f);
        maxTimer = disappearTime;
        disappearSpeed = disappearTimer;
        
        //Vector2 randomOffset = Random.insideUnitCircle;
        transform.position += positionOffset;
         // + new Vector3(randomOffset.x * .5f, 0, randomOffset.y * .5f)
        moveVector = new Vector3(moveXSpeed, moveYSpeed, moveZSpeed) * (90 + Random.Range(-10, 10));
        if (isCrit) scaleFactor = Random.Range(.2f, .8f);
        else scaleFactor = Random.Range(0, .2f);
    }

    private void Update()
    {
        disappearTimer -= Time.deltaTime;
        if(disappearTimer <= 0) Disappear();
        
        transform.position += moveVector * Time.deltaTime;
        moveVector -= (12f + Random.Range(-2, 2))  * Time.deltaTime * moveVector;


        if (disappearTimer > maxTimer * .5f)
        {
            float increaseScaleAmount = .2f + scaleFactor;
            transform.localScale += increaseScaleAmount * Time.deltaTime * Vector3.one;
        }
        else
        {
            if (transform.localScale.x >= 0)
            {
                float decreaseScaleAmount = .2f;
                transform.localScale -= decreaseScaleAmount * Time.deltaTime * Vector3.one;
            }
        }
    }

    private void Disappear()
    {
        textColor.a -= disappearSpeed * Time.deltaTime;
        textMesh.color = textColor;
        if (textColor.a <= 0)
        {
            ReturnToPool();
        }
    }
    
    
    //pool related
    public void SetPool(ObjectPool<DamagePopup> pool)
    {
        this.pool = pool;
    }

    public void ReturnToPool()
    {
        if (pool != null)
        {
            pool.Release(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
