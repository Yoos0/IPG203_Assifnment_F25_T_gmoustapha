using RepairCenter.Enums;

namespace RepairCenter.Events;

/// بيحمل معلومات الحالة القديمة والجديدة للجهاز.
public class DeviceStatusChangedEventArgs : EventArgs
{
    public string SerialNumber { get; }
    public DeviceStatus OldStatus { get; }
    public DeviceStatus NewStatus { get; }

    public DeviceStatusChangedEventArgs(string serialNumber, DeviceStatus oldStatus, DeviceStatus newStatus)
    {
        SerialNumber = serialNumber;
        OldStatus = oldStatus;
        NewStatus = newStatus;
    }
}