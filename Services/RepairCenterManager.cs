using RepairCenter.Enums;
using RepairCenter.Events;
using RepairCenter.Models;

namespace RepairCenter.Services;

/// <summary>
/// Manages all appliances in the repair center. Works with the abstract type
/// <see cref="Appliance"/>, so every device type is handled polymorphically.
/// </summary>
public class RepairCenterManager
{
    // Private field (Encapsulation); exposed read-only below
    private readonly List<Appliance> _devices = new();

    // Shared totals across the whole program (Static members)
    private static int _repairedDevicesCount;
    private static decimal _totalRepairedRevenue;

    /// <summary>Raised by <see cref="CheckOverdueDevices"/> for each late device.</summary>
    public event DeviceOverdueHandler? DeviceOverdue;

    // --- Static properties ---
    /// <summary>Number of devices that reached the Repaired status.</summary>
    public static int RepairedDevicesCount => _repairedDevicesCount;

    /// <summary>Sum of the repair costs of all devices that reached the Repaired status.</summary>
    public static decimal TotalRepairedRevenue => _totalRepairedRevenue;

    /// <summary>Read-only view of the devices; callers cannot add or remove through it.</summary>
    public IReadOnlyList<Appliance> Devices => _devices;

    public void AddDevice(Appliance device)
    {
        ArgumentNullException.ThrowIfNull(device);

        if (FindBySerial(device.SerialNumber) is not null)
            throw new InvalidOperationException($"A device with serial {device.SerialNumber} already exists.");

        device.StatusChanged += OnDeviceStatusChanged;
        _devices.Add(device);
    }

    public Appliance? FindBySerial(string serialNumber) =>
        _devices.FirstOrDefault(d => d.SerialNumber == serialNumber);

    public IEnumerable<Appliance> GetDevicesByStatus(DeviceStatus status) =>
        _devices.Where(d => d.Status == status);

    /// <summary>Polymorphism: each device runs its own CalculateRepairCost() override.</summary>
    public decimal GetTotalCost() =>
        _devices.Sum(d => d.CalculateRepairCost());

    /// <summary>Polymorphism: GetSummary() adds each type's own details.</summary>
    public void PrintAllSummaries()
    {
        foreach (Appliance device in _devices)
            Console.WriteLine(device.GetSummary());
    }

    /// <summary>Checks every device and raises DeviceOverdue for each late one.</summary>
    public void CheckOverdueDevices()
    {
        foreach (Appliance device in _devices.Where(d => d.IsOverdue))
            DeviceOverdue?.Invoke(device);
    }

    // Handler subscribed to every device's StatusChanged event
    private void OnDeviceStatusChanged(object sender, DeviceStatusChangedEventArgs e)
    {
        Console.WriteLine($"[Event] {e.SerialNumber}: {e.OldStatus} -> {e.NewStatus}");

        if (e.NewStatus == DeviceStatus.Repaired && sender is Appliance device)
        {
            _repairedDevicesCount++;
            _totalRepairedRevenue += device.CalculateRepairCost();
        }
    }
}