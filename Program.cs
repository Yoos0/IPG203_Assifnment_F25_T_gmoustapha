using RepairCenter.Models;

var due = DateTime.Now.AddDays(5);

var devices = new List<Appliance>
{
    new Refrigerator("FR-1001", "Ahmad", due, capacityLiters: 550, requiresGasRefill: true),
    new WashingMachine("WM-2001", "Lina", due, maxLoadKg: 8, isFrontLoad: true, hasWaterLeak: true),
    new AirConditioner("AC-3001", "Omar", due, coolingCapacityBtu: 24000, isInverter: true)
};

foreach (var d in devices)
{
    d.LaborHours = 2;
    d.PartsCost = 50;
    Console.WriteLine(d.GetSummary());
}