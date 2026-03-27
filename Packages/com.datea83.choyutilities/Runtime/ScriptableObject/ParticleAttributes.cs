using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu(fileName = "ParticleAttributes", menuName = "Choy Utilities/ParticleAttributes")]
public class ParticleAttributes : ScriptableObject {

    public LocalizedString particleName;
    public LocalizedString particleDescription;

    public ushort baseHealth = 100;
    public ushort weight = 10;

    public float baseSpeed;
    public float topSpeed;
    [Range(0f, 1f)] public float acceleration;

    public byte fuelMaxCharge = 2;
    [Range(0, 10f)] public float fuelRechargeSpeed;

    private void OnValidate() {
        if (topSpeed <= baseSpeed) topSpeed = baseSpeed;
    }

}