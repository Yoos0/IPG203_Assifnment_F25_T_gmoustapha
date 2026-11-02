using System.Text.RegularExpressions;

namespace RepairCenter.Helpers;

/// هون جمعنا دوال التحقق المشتركة بمكان واحد.
public static class Validator
{
    // الرقم بيكون حروف كبيرة، شرطة، وبعدها أرقام.
    private static readonly Regex SerialPattern = new(@"^[A-Z]{2,3}-\d{4,6}$");

    public static bool IsValidSerialNumber(string? serialNumber) =>
        !string.IsNullOrWhiteSpace(serialNumber) && SerialPattern.IsMatch(serialNumber);

    /// منقبل اسم فيه حرفين عالأقل، ومنرفض الرموز الغريبة.
    public static bool IsValidName(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return false;

        string trimmed = name.Trim();
        return trimmed.Length >= 2 &&
               trimmed.All(c => char.IsLetter(c) || c == ' ' || c == '-' || c == '\'');
    }

    /// تاريخ التسليم لازم يكون اليوم أو لقدام.
    public static bool IsValidDeliveryDate(DateTime date) =>
        date.Date >= DateTime.Today;

    public static bool IsPositive(double value) => value > 0;

    public static bool IsNonNegative(decimal value) => value >= 0;
}