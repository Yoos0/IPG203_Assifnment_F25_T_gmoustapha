namespace RepairCenter.Models;

/// <summary>
/// An air conditioner. Cost depends on labor, parts (more expensive for inverter models),
/// an optional gas refill, and a fee for high-capacity units.
/// </summary>
public class AirConditioner : Appliance
{
    private const decimal HourlyRate = 20m;
    private const decimal GasRefillFee = 60m;
    private const decimal InverterPartsMultiplier = 1.20m;
    private const decimal HighCapacityFee = 50m;
    private const int HighCapacityBtu = 24000;

    private readonly int _coolingCapacityBtu;
    private readonly bool _isInverter;
    private bool _requiresGasRefill;

    public AirConditioner(string serialNumber, string ownerName, DateTime expectedDeliveryDate,
                          int coolingCapacityBtu, bool isInverter, bool requiresGasRefill = false)
        : base(serialNumber, ownerName, expectedDeliveryDate)
    {
        if (coolingCapacityBtu <= 0)
            throw new ArgumentOutOfRangeException(nameof(coolingCapacityBtu), "Cooling capacity must be positive.");

        _coolingCapacityBtu = coolingCapacityBtu;
        _isInverter = isInverter;
        _requiresGasRefill = requiresGasRefill;
    }

    public int CoolingCapacityBtu => _coolingCapacityBtu;
    public bool IsInverter => _isInverter;

    public bool RequiresGasRefill
    {
        get => _requiresGasRefill;
        set => _requiresGasRefill = value;
    }

    public override decimal CalculateRepairCost()
    {
        decimal parts = _isInverter ? PartsCost * InverterPartsMultiplier : PartsCost;
        decimal cost = LaborHours * HourlyRate + parts;

        if (_requiresGasRefill)
            cost += GasRefillFee;

        if (_coolingCapacityBtu >= HighCapacityBtu)
            cost += HighCapacityFee;

        return cost;
    }

    public override string GetSummary() =>
        base.GetSummary() + $" | {_coolingCapacityBtu} BTU | {(_isInverter ? "Inverter" : "Standard")} | Gas refill: {(_requiresGasRefill ? "Yes" : "No")}";
}