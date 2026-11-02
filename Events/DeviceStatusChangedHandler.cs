namespace RepairCenter.Events;

/// هاد المفوض بيحدد شكل الدوال اللي بتتعامل مع تغيير الحالة.
public delegate void DeviceStatusChangedHandler(object sender, DeviceStatusChangedEventArgs e);