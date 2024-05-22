using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MaterialType
{
    BASEMATERIAL, //base material is just a reference to an objects cached base material. It does not need to be explicitly set
    FrozenMaterial,
    ShockMaterial,
    BurnMaterial,

    Grunt1Material,
    Grunt2Material,
    Grunt3Material,
    Grunt4Material,
    Grunt5Material,

    Ranger1Material,
    Ranger2Material,
    Ranger3Material,
    Ranger4Material,
    Ranger5Material,

    EliteGrontMaterial,
    EliteRangestMaterial
}

public class MaterialReferenceHolder : MonoBehaviour
{
    public List<MaterialReferenceStruct> materialReferences;
    private Dictionary<MaterialType, Material> _materialReferenceData;
    
    private static MaterialReferenceHolder _instance;
    
    public static MaterialReferenceHolder Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<MaterialReferenceHolder>();
                
                if (_instance == null)
                {
                    //GameObject instanceObject = new GameObject("UpgradePoolManager");
                    //_instance = instanceObject.AddComponent<UpgradePoolManager>();
                }
            }

            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            //Destroy(gameObject);
        }
        
        InitializeDataDictionary();
    }

    private void InitializeDataDictionary()
    {
        _materialReferenceData = new Dictionary<MaterialType, Material>();
        foreach (var reference in materialReferences)
        {
            _materialReferenceData.Add(reference.materialType, reference.material);
        }
    }

    public Material GetMaterialReference(MaterialType type)
    {
        Material material = _materialReferenceData[type];
        return material;
    }
}

[System.Serializable]
public struct MaterialReferenceStruct
{
    public Material material;
    public MaterialType materialType;
}