using System.Collections;
using System.Collections.Generic;
using Patrik;
using UnityEngine;
using UnityEngine.UI;

public class WeaponEnergyUIBehaviour : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private Slider slider;
    [SerializeField] private GameObject border;
    public WeaponBehaviour Weapon {get; private set; }

    public void Setup(WeaponBehaviour weapon)
    {
        Weapon = weapon;
        image.sprite = weapon.Sprite;
    }

    public void UpdateBar(float current, float max)
    {
        slider.value = current / max;
    }

    public void SetActiveWeapon(bool active)
    {
        border.SetActive(active);
    }
}
