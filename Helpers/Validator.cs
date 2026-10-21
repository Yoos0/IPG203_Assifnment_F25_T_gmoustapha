using System.Text.RegularExpressions;

namespace RepairCenter.Helpers;

/// <summary>
/// Static helper methods that validate input data.
/// Each method only answers true/false; the caller decides how to react.
/// </summary>
public static class Validator
{
    // Example: FR-1001 = 2-3 capital letters, a dash, then 4-6 digits
    private static readonly Regex SerialPattern = new(@"^[A-Z]{2,3}-\d{4,6}$");

    public static bool IsValidSerialNumber(string? serialNumber) =>
        !string.IsNullOrWhiteSpace(serialNumber) && SerialPattern.IsMatch(serialNumber);

    /// <summary>
    /// At least 2 characters; letters (any language), spaces, hyphens and apostrophes only.
    /// </summary>
    public static bool IsValidName(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return false;

        string trimmed = name.Trim();
        return trimmed.Length >= 2 &&
               trimmed.All(c => char.IsLetter(c) || c == ' ' || c == '-' || c == '\'');
    }

    /// <summary>A delivery date can be today or later, never in the past.</summary>
    public static bool IsValidDeliveryDate(DateTime date) =>
        date.Date >= DateTime.Today;

    public static bool IsPositive(double value) => value > 0;

    public static bool IsNonNegative(decimal value) => value >= 0;
}