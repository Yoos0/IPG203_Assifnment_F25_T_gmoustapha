using RepairCenter.Helpers;
namespace RepairCenter.Models;

/// A washing machine. Cost depends on labor, parts, a leak repair fee,
/// and a labor surcharge for front-loading models.

public class WashingMachine : Appliance
{
    private const decimal HourlyRate = 10m;
    private const decimal LeakRepairFee = 20m;
    private const decimal FrontLoadLaborMultiplier = 1.15m;

    private readonly double _maxLoadKg;
    private readonly bool _isFrontLoad;
    private bool _hasWaterLeak;

    public WashingMachine(string serialNumber, string ownerName, DateTime expectedDeliveryDate,
                          double maxLoadKg, bool isFrontLoad, bool hasWaterLeak = false)
        : base(serialNumber, ownerName, expectedDeliveryDate)
    {
        if (!Validator.IsPositive(maxLoadKg))
            throw new ArgumentOutOfRangeException(nameof(maxLoadKg), "Max load must be positive.");

        _maxLoadKg = maxLoadKg;
        _isFrontLoad = isFrontLoad;
        _hasWaterLeak = hasWaterLeak;
    }

    public double MaxLoadKg => _maxLoadKg;
    public bool IsFrontLoad => _isFrontLoad;

    public bool HasWaterLeak
    {
        get => _hasWaterLeak;
        set => _hasWaterLeak = value;
    }

    public override decimal CalculateRepairCost()
    {
        decimal labor = LaborHours * HourlyRate;

        if (_isFrontLoad)
            labor *= FrontLoadLaborMultiplier;

        decimal cost = labor + PartsCost;

        if (_hasWaterLeak)
            cost += LeakRepairFee;

        return cost;
    }

    public override string GetSummary() =>
        base.GetSummary() + $" | Max load: {_maxLoadKg}kg | {(_isFrontLoad ? "Front" : "Top")}-load | Leak: {(_hasWaterLeak ? "Yes" : "No")}";
}
