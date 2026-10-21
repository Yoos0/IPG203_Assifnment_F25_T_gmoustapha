using RepairCenter.Helpers;
using RepairCenter.Enums;
using RepairCenter.Events;
using RepairCenter.Interfaces;

namespace RepairCenter.Models;

/// Base class for every appliance in the repair center.
/// Holds the shared data and workflow; subclasses define how repair cost is calculated.

public abstract class Appliance : IRepairableDevice
{
    // All fields are private (Encapsulation)
    private readonly string _serialNumber;
    private readonly string _ownerName;
    private readonly DateTime _receivedDate;
    private readonly DateTime _expectedDeliveryDate;
    private DeviceStatus _status;
    private decimal _laborHours;
    private decimal _partsCost;

    /// Raised every time the device's status changes.
    public event DeviceStatusChangedHandler? StatusChanged;

protected Appliance(string serialNumber, string ownerName, DateTime expectedDeliveryDate)
{
    if (!Validator.IsValidSerialNumber(serialNumber))
        throw new ArgumentException(
            "Serial number must look like FR-1001 (2-3 capital letters, a dash, 4-6 digits).",
            nameof(serialNumber));

    if (!Validator.IsValidName(ownerName))
        throw new ArgumentException("Owner name must contain at least 2 letters.", nameof(ownerName));

    if (!Validator.IsValidDeliveryDate(expectedDeliveryDate))
        throw new ArgumentException("Expected delivery date cannot be in the past.", nameof(expectedDeliveryDate));

    _serialNumber = serialNumber;
    _ownerName = ownerName.Trim();
    _receivedDate = DateTime.Now;
    _expectedDeliveryDate = expectedDeliveryDate;
    _status = DeviceStatus.Received;
}

    // --- Read-only after creation ---
    public string SerialNumber => _serialNumber;
    public string OwnerName => _ownerName;
    public DateTime ReceivedDate => _receivedDate;
    public DateTime ExpectedDeliveryDate => _expectedDeliveryDate;

    // --- Readable by everyone, changed only through the workflow methods ---
    public DeviceStatus Status => _status;

    public bool IsOverdue =>
        (_status == DeviceStatus.Received || _status == DeviceStatus.UnderRepair)
        && DateTime.Now > _expectedDeliveryDate;

    // --- Writable, but validated ---


public decimal LaborHours
{
    get => _laborHours;
    set
    {
        if (!Validator.IsNonNegative(value))
            throw new ArgumentOutOfRangeException(nameof(value), "Labor hours cannot be negative.");
        _laborHours = value;
    }
}

public decimal PartsCost
{
    get => _partsCost;
    set
    {
        if (!Validator.IsNonNegative(value))
            throw new ArgumentOutOfRangeException(nameof(value), "Parts cost cannot be negative.");
        _partsCost = value;
    }
}



    // public decimal LaborHours
    // {
    //     get => _laborHours;
    //     set
    //     {
    //         if (!Validator.IsNonNegative(value))
    //         throw new ArgumentOutOfRangeException(nameof(value), "Labor hours cannot be negative.");

    //     }
    // }

    // public decimal PartsCost
    // {
    //     get => _partsCost;
    //     set
    //     {
    //         if (!Validator.IsNonNegative(value))
    //         throw new ArgumentOutOfRangeException(nameof(value), "Parts cost cannot be negative.");
    //     }
    // }

    // --- Repair workflow ---
    public void StartRepair()
    {
        if (_status != DeviceStatus.Received)
            throw new InvalidOperationException("Only a received device can start repair.");
        ChangeStatus(DeviceStatus.UnderRepair);
    }

    public void CompleteRepair()
    {
        if (_status != DeviceStatus.UnderRepair)
            throw new InvalidOperationException("Only a device under repair can be completed.");
        ChangeStatus(DeviceStatus.Repaired);
    }

    public void DeliverToCustomer()
    {
        if (_status != DeviceStatus.Repaired)
            throw new InvalidOperationException("Only a repaired device can be delivered.");
        ChangeStatus(DeviceStatus.Delivered);
    }

    // --- Abstraction: every device type calculates its own cost ---
    public abstract decimal CalculateRepairCost();

    public virtual string GetSummary() =>
        $"[{GetType().Name}] Serial: {_serialNumber} | Owner: {_ownerName} | " +
        $"Status: {_status} | Cost: {CalculateRepairCost():C}";

    // --- Event raising ---
    private void ChangeStatus(DeviceStatus newStatus)
    {
        DeviceStatus oldStatus = _status;
        _status = newStatus;
        OnStatusChanged(new DeviceStatusChangedEventArgs(_serialNumber, oldStatus, newStatus));
    }

    protected virtual void OnStatusChanged(DeviceStatusChangedEventArgs e)
    {
        StatusChanged?.Invoke(this, e);
    }
}