namespace RepairCenter.Enums;

public enum DeviceStatus
{
    Received,      // الجهاز وصل عالمركز
    UnderRepair,   // الفني عم يشتغل عليه
    Repaired,      // تصلّح وناطر صاحبه
    Delivered      // رجعناه لصاحبه
}