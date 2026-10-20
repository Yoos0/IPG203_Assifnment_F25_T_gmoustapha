namespace RepairCenter.Events;

/// <summary>
/// Delegate used to notify subscribers that a device's status changed.
/// </summary>
public delegate void DeviceStatusChangedHandler(object sender, DeviceStatusChangedEventArgs e);