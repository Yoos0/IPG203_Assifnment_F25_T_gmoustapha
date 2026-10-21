using RepairCenter.Models;

namespace RepairCenter.Events;

/// <summary>
/// Delegate used to notify subscribers that a device missed its expected delivery date.
/// </summary>
public delegate void DeviceOverdueHandler(Appliance device);