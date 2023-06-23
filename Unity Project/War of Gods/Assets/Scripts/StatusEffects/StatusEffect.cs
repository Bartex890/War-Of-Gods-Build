using System.Collections.Generic;
using System.Globalization;
//using UnityEditor.Timeline.Actions;
using UnityEngine;

[CreateAssetMenu(fileName = "newStatusEffect", menuName = "StatusEffect")]
public class StatusEffect : ScriptableObject
{
    public string id;
    [Tooltip("Podaje, co ma robi� efekt. [wartosc] oznacza, �e nale�y poda� liczb� bez [] np. \"damege:5;\". Dost�pne s� r�ne funkcje oddzielane �rednikami: \n" +
        "mod_range:[warto��] - modyfikuje zasi�g jednostki\n" +
        "mod_resistance:[warto��] - modyfikuje odporno�� jednostki\n" +
        "mod_damage:[warto��] - modyfikuje ilo�� obra�e� zadawan� przez jednostk�\n" +
        "damage:[warto��] - zadaje okre�lone obra�enia co rund�\n" +
        "mod_damage_taken:[warto��] - mno�y warto�� otrzymywanych obra�e� przez warto��")]
    [SerializeField]
    private string _actions;
    [Tooltip("D�ugo�� trwania efektu podana w rundach gracza.")]
    [SerializeField]
    public int duration;

    [SerializeField]
    private Color _appliedColor;

    public string animationWhenApplied;
    public string animationWhenApplying;
    //private Material _appliedMaterial;
    //private Material _previousMaterial;

    public void ApplyStatusEffect(Unit unit)
    {
        /*_previousMaterial = MapManager.Instance.GetUnitGraphic(unit.position).material;
        if (_appliedMaterial != null)
            MapManager.Instance.GetUnitGraphic(unit.position).material = _appliedMaterial;*/
        SpriteRenderer spriteRenderer = MapManager.Instance.GetUnitGraphic(unit.position);
        if (_appliedColor != null && spriteRenderer != null)
            spriteRenderer.color = new Color((unit.GetAmountOfStatusEffects() * spriteRenderer.color.r + _appliedColor.r) / (unit.GetAmountOfStatusEffects() + 1),
                                             (unit.GetAmountOfStatusEffects() * spriteRenderer.color.g + _appliedColor.g) / (unit.GetAmountOfStatusEffects() + 1),
                                             (unit.GetAmountOfStatusEffects() * spriteRenderer.color.b + _appliedColor.b) / (unit.GetAmountOfStatusEffects() + 1));
        
        if (animationWhenApplied != "")
        {
            unit.TryAddStatusEffectAnimation(animationWhenApplied);
        }
    }

    public bool ProcessStatusEffect(Unit unit)
    {
        if (unit == null)
        {
            Debug.LogError("This unit does not exist!");
            return false;
        }

        foreach(string processedString in _actions.Split(';'))
        {
            string[] separated = processedString.Split(':');

            if (separated.Length <= 1) 
                continue;

            string code = separated[0];
            float value = float.Parse(separated[1], CultureInfo.InvariantCulture);

            switch (code)
            {
                case "mod_range":
                    unit.multiplyRange(value);
                    break;
                case "damage":
                    unit.takeDamage((int)value);
                    break;
                case "mod_resistance":
                    if(unit.GetFaction().gods==Gods.Factions.marzanna)
                    unit.SetIsTarget(); 
                    //building.;
                    break;
                case "mod_damage":
                    unit.multiplyDamage(value);
                    break;
                case "mod_damage_taken":
                    unit.multiplyDamageTaken(value);
                    break;
            }
        }

        Healthbar healthbar = MapManager.Instance.GetUnitGraphic(unit.position).gameObject.GetComponentsInChildren<Healthbar>()[0];
        healthbar.SetHealth(MapManager.Instance.map.getUnit(unit.position).getHP());
        if (MapManager.Instance.map.getUnit(unit.position).getHP() <= 0 || MapManager.Instance.map.getUnit(unit.position).numberOfUnits <= 0)
        {
            MapManager.Instance.DeleteUnitGraphic(unit.position);
            MapManager.Instance.map.RemoveUnit(unit.position);
            return false;
        }

        return true;
    }

    public void EraseStatusEffect(Unit unit)
    {
        SpriteRenderer spriteRenderer = MapManager.Instance.GetUnitGraphic(unit.position);
        if (_appliedColor != null)
            spriteRenderer.color = new Color((unit.GetAmountOfStatusEffects() * spriteRenderer.color.r - _appliedColor.r) / (unit.GetAmountOfStatusEffects() - 1),
                                             (unit.GetAmountOfStatusEffects() * spriteRenderer.color.g - _appliedColor.g) / (unit.GetAmountOfStatusEffects() - 1),
                                             (unit.GetAmountOfStatusEffects() * spriteRenderer.color.b - _appliedColor.b) / (unit.GetAmountOfStatusEffects() - 1));
        if (unit.GetAmountOfStatusEffects() <= 1)
            spriteRenderer.color = Color.white;

        if (animationWhenApplied != "")
        {
            unit.TryDestroyEffectAnimation(animationWhenApplied);
        }
    }
}
