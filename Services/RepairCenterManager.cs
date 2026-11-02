using RepairCenter.Enums;
using RepairCenter.Events;
using RepairCenter.Models;

namespace RepairCenter.Services;

/// بيدير الأجهزة كلها من خلال النوع الأساسي حتى يشتغل تعدد الأشكال.
public class RepairCenterManager
{
    // منخزن الأنواع المختلفة بلائحة وحدة من النوع الأساسي.
    private readonly List<Appliance> _devices = new();

    // هالإحصائيات مشتركة بين كل نسخ المدير.
    private static int _repairedDevicesCount;
    private static decimal _totalRepairedRevenue;

    /// منطلقه لما نلاقي جهاز فات موعد تسليمه.
    public event DeviceOverdueHandler? DeviceOverdue;

    /// عدد الأجهزة اللي وصلت لحالة تم الإصلاح.
    public static int RepairedDevicesCount => _repairedDevicesCount;

    /// مجموع تكلفة الأجهزة اللي وصلت لحالة تم الإصلاح.
    public static decimal TotalRepairedRevenue => _totalRepairedRevenue;

    ///منعرض الأجهزة للقراءة بدون ما نسمح بتعديل اللائحة من برا.
    public IReadOnlyList<Appliance> Devices => _devices.AsReadOnly();

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

    ///كل جهاز بيستخدم override الخاص فيه لحساب التكلفة.
    public decimal GetTotalCost() =>
        _devices.Sum(d => d.CalculateRepairCost());

    /// كل نوع بيضيف تفاصيله الخاصة على الملخص.
    public void PrintAllSummaries()
    {
        foreach (Appliance device in _devices)
            Console.WriteLine(device.GetSummary());
    }

    ///يتحقق من كل جهاز ويرفع حدث Device Overdue لكل جهاز متأخر.
    public void CheckOverdueDevices()
    {
        foreach (Appliance device in _devices.Where(d => d.IsOverdue))
            DeviceOverdue?.Invoke(device);
    }

    // يعالج امر تغير حالة الجهاز وهو مشترك بين جميع الاجهزة
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