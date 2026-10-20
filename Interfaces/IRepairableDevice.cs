using RepairCenter.Enums;

namespace RepairCenter.Interfaces;

/// <summary>
/// Operations shared by every appliance handled by the repair center.
/// </summary>
public interface IRepairableDevice
{
    // --- Identity and state (read-only from the outside) ---
    string SerialNumber { get; }
    string OwnerName { get; }
    DeviceStatus Status { get; }
    DateTime ReceivedDate { get; }
    DateTime ExpectedDeliveryDate { get; }
    bool IsOverdue { get; }

    // --- Repair workflow ---
    void StartRepair();
    void CompleteRepair();
    void DeliverToCustomer();

    // --- Cost and reporting ---
    decimal CalculateRepairCost();
    string GetSummary();
}