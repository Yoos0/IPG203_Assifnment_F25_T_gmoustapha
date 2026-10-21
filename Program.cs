using System.Globalization;
using RepairCenter.Enums;
using RepairCenter.Models;
using RepairCenter.Services;

// Consistent currency formatting regardless of the machine's regional settings
CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("en-US");

var manager = new RepairCenterManager();

// Subscribe to the manager's overdue event
manager.DeviceOverdue += d =>
    Console.WriteLine($"[Overdue] {d.SerialNumber} ({d.OwnerName}) missed its delivery date!");

var due = DateTime.Now.AddDays(5);

// ---------------------------------------------------------------
Section("1. Receiving devices (three types, one list)");
// ---------------------------------------------------------------
var fridge = new Refrigerator("FR-1001", "Ahmad", due, capacityLiters: 550, requiresGasRefill: true);
var washer = new WashingMachine("WM-2001", "Lina", due, maxLoadKg: 8, isFrontLoad: true, hasWaterLeak: true);
var ac = new AirConditioner("AC-3001", "Omar", due, coolingCapacityBtu: 24000, isInverter: true);

foreach (Appliance device in new Appliance[] { fridge, washer, ac })
{
    device.LaborHours = 2;
    device.PartsCost = 50;
    manager.AddDevice(device);
}

// A second subscriber on a single device: notify the customer when it is repaired
fridge.StatusChanged += (sender, e) =>
{
    if (e.NewStatus == DeviceStatus.Repaired && sender is Appliance device)
        Console.WriteLine($"[Notify] Dear {device.OwnerName}, your device {device.SerialNumber} is ready for pickup.");
};

manager.PrintAllSummaries();
Console.WriteLine($"Total estimated cost: {manager.GetTotalCost():C}");

// ---------------------------------------------------------------
Section("2. Repair workflow and status events");
// ---------------------------------------------------------------
fridge.StartRepair();
fridge.CompleteRepair();
ac.StartRepair();
ac.CompleteRepair();
fridge.DeliverToCustomer();

// ---------------------------------------------------------------
Section("3. Rejected operations (validation and workflow rules)");
// ---------------------------------------------------------------
ExpectRejected("Invalid serial number", () => new Refrigerator("fridge1", "Ahmad", due, 300));
ExpectRejected("Empty owner name", () => new WashingMachine("WM-2002", "", due, 8, true));
ExpectRejected("Delivery date in the past", () => new AirConditioner("AC-3002", "Omar", DateTime.Now.AddDays(-1), 18000, false));
ExpectRejected("Capacity of zero", () => new Refrigerator("FR-1002", "Lina", due, 0));
ExpectRejected("Negative labor hours", () => washer.LaborHours = -5);
ExpectRejected("Deliver a device that is not repaired", () => washer.DeliverToCustomer());
ExpectRejected("Start repair on a repaired device", () => ac.StartRepair());
ExpectRejected("Duplicate serial number", () => manager.AddDevice(new Refrigerator("FR-1001", "Omar", due, 300)));

// ---------------------------------------------------------------
Section("4. Overdue check");
// ---------------------------------------------------------------
// A delivery date of today is valid, but midnight has already passed, so it counts as late.
manager.AddDevice(new WashingMachine("WM-2003", "Sara", DateTime.Today, 7, false));
manager.CheckOverdueDevices();

// ---------------------------------------------------------------
Section("5. Final report");
// ---------------------------------------------------------------
foreach (DeviceStatus status in Enum.GetValues<DeviceStatus>())
    Console.WriteLine($"{status,-12}: {manager.GetDevicesByStatus(status).Count()}");

Console.WriteLine($"Repaired devices (static): {RepairCenterManager.RepairedDevicesCount}");
Console.WriteLine($"Repaired revenue (static): {RepairCenterManager.TotalRepairedRevenue:C}");

// ---------------------------------------------------------------
// Helpers
// ---------------------------------------------------------------
static void Section(string title) =>
    Console.WriteLine($"\n=== {title} ===");

static void ExpectRejected(string label, Action action)
{
    try
    {
        action();
        Console.WriteLine($"  [!!] {label}: accepted (unexpected)");
    }
    catch (Exception e) when (e is ArgumentException or InvalidOperationException)
    {
        Console.WriteLine($"  [OK] {label}: {e.Message}");
    }
}