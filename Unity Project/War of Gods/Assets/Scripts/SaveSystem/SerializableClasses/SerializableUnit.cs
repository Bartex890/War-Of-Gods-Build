using UnityEngine;

[System.Serializable]
public class SerializableUnit
{
    public string id;
    public int teamID;
    public int posX, posY;
    public int hpLeft;
    public int numberOfUnits;
    public StatusEffectAndTime[] statusEffects;
    public StatusEffectAndTime[] appliedEffects;

    [System.Serializable]
    public class StatusEffectAndTime
    {
        public string id;
        public int time;

        public void FromStatusEffect(StatusEffect statusEffect)
        {
            time = statusEffect.duration;
            id = statusEffect.id;
            Debug.Log(id);
        }
    }
}
