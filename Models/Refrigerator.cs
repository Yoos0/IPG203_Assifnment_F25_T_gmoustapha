using RepairCenter.Helpers;

namespace RepairCenter.Models;

// البراد تكلفته بتتغير حسب الغاز وحجمه.
public class Refrigerator : Appliance
{
    private const decimal HourlyRate = 15m;
    private const decimal GasRefillFee = 40m;
    private const decimal LargeUnitSurcharge = 25m;
    private const int LargeUnitLiters = 500;

    private readonly int _capacityLiters;
    private bool _requiresGasRefill;

    public Refrigerator(string serialNumber, string ownerName, DateTime expectedDeliveryDate,
                        int capacityLiters, bool requiresGasRefill = false)
        : base(serialNumber, ownerName, expectedDeliveryDate)
    {
        if (!Validator.IsPositive(capacityLiters))
            throw new ArgumentOutOfRangeException(nameof(capacityLiters), "Capacity must be positive.");

        _capacityLiters = capacityLiters;
        _requiresGasRefill = requiresGasRefill;
    }

    // سعة البراد ثابتة وما بتتغير بعد إنشاء الجهاز.
    public int CapacityLiters => _capacityLiters;

    public bool RequiresGasRefill
    {
        get => _requiresGasRefill;
        set => _requiresGasRefill = value;
    }

    public override decimal CalculateRepairCost()
    {
        // منضيف أجرة الغاز ورسم الحجم الكبير إذا كانوا مطلوبين.
        decimal cost = LaborHours * HourlyRate + PartsCost;

        if (_requiresGasRefill)
            cost += GasRefillFee;

        if (_capacityLiters >= LargeUnitLiters)
            cost += LargeUnitSurcharge;

        return cost;
    }

    public override string GetSummary() =>
        base.GetSummary() + $" | Capacity: {_capacityLiters}L | Gas refill: {(_requiresGasRefill ? "Yes" : "No")}";
}