using RepairCenter.Models;

namespace RepairCenter.Events;

///هاد المفوض بيحدد شكل الدالة اللي بتستقبل تنبيه التأخير.
public delegate void DeviceOverdueHandler(Appliance device);