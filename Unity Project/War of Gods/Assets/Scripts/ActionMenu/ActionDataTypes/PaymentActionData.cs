using UnityEngine;

[CreateAssetMenu(fileName = "newPaymentAction", menuName = "Actions/PaymentAction")]
public class PaymentActionData : ActionData
{
    public int woodCost;
    public int goldCost;
    public int foodCost;
    public int faithCost;
}