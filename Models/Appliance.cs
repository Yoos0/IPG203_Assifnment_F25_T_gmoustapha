using RepairCenter.Helpers;
using RepairCenter.Enums;
using RepairCenter.Events;
using RepairCenter.Interfaces;

namespace RepairCenter.Models;

/// هون جمعنا الخصائص وسير العمل المشتركين، وكل نوع بيحسب تكلفته لحاله.

public abstract class Appliance : IRepairableDevice
{
    // الحقول خاصة حتى ما حدا يغيّر بيانات الجهاز مباشرة.
    private readonly string _serialNumber;
    private readonly string _ownerName;
    private readonly DateTime _receivedDate;
    private readonly DateTime _expectedDeliveryDate;
    private DeviceStatus _status;
    private decimal _laborHours;
    private decimal _partsCost;

    // منطلقين هالحدث كل ما تتغير حالة الجهاز.
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

    // هالقيم ثابتة بعد ما ننشئ الجهاز.
    public string SerialNumber => _serialNumber;
    public string OwnerName => _ownerName;
    public DateTime ReceivedDate => _receivedDate;
    public DateTime ExpectedDeliveryDate => _expectedDeliveryDate;

    // الحالة ما بتتغير إلا من خلال دوال سير العمل.
    public DeviceStatus Status => _status;

    public bool IsOverdue =>
        _status != DeviceStatus.Delivered && DateTime.Now > _expectedDeliveryDate;

    // منتحقق من القيم قبل ما نخزنها حتى ما تدخل بيانات غلط.
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

    // منسمح بكل خطوة بس إذا الجهاز بالحالة المناسبة.
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

    // كل كلاس ابن بيعمل override وبيحسب التكلفة حسب نوع الجهاز.
    public abstract decimal CalculateRepairCost();

    public virtual string GetSummary() =>
        $"[{GetType().Name}] Serial: {_serialNumber} | Owner: {_ownerName} | " +
        $"Status: {_status} | Cost: {CalculateRepairCost():C}";

    private void ChangeStatus(DeviceStatus newStatus)
    {
        DeviceStatus oldStatus = _status;
        _status = newStatus;
        OnStatusChanged(new DeviceStatusChangedEventArgs(_serialNumber, oldStatus, newStatus));
    }

    protected virtual void OnStatusChanged(DeviceStatusChangedEventArgs e)
    {
        // إذا في مشتركين بالحدث، منخبرهم بتغيير الحالة.
        StatusChanged?.Invoke(this, e);
    }
}