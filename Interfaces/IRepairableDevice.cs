using RepairCenter.Enums;

namespace RepairCenter.Interfaces;

/// هاد العقد بيحدد الشغلات المشتركة بين كل الأجهزة.
public interface IRepairableDevice
{
    // المعلومات بتنقرا من برا، بس تغيير الحالة بيصير عبر دوال سير العمل.
    string SerialNumber { get; }
    string OwnerName { get; }
    DeviceStatus Status { get; }
    DateTime ReceivedDate { get; }
    DateTime ExpectedDeliveryDate { get; }
    bool IsOverdue { get; }

    // خطوات الإصلاح والتسليم المشتركة بين الأجهزة.
    void StartRepair();
    void CompleteRepair();
    void DeliverToCustomer();

    // كل نوع بيحسب تكلفته وبيعرض ملخصه.
    decimal CalculateRepairCost();
    string GetSummary();
}