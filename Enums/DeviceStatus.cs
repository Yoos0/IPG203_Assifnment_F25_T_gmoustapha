namespace RepairCenter.Enums;

public enum DeviceStatus
{
    Received,      // Device arrived at the center
    UnderRepair,   // Technician is working on it
    Repaired,      // Fixed, waiting for customer pickup
    Delivered      // Handed back to the customer
}