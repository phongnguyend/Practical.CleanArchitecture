using System.ComponentModel.DataAnnotations;

namespace ClassifiedAds.Mobile.Core.Models;

public abstract class Record
{
    public string Id { get; set; } = "";

    public abstract string Title { get; }


    public abstract object Payload();
    public abstract string? Validate();
    protected static string? Required(string value, string label, int minimum = 1) => string.IsNullOrWhiteSpace(value) ? $"{label} is required." : value.Trim().Length < minimum ? $"{label} needs at least {minimum} characters." : null;
}
