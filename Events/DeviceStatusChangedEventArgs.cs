using RepairCenter.Enums;

namespace RepairCenter.Events;

/// <summary>
/// Carries the details of a status change (which device, from what, to what).
/// </summary>
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