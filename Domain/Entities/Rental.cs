using Domain.Common;
using Domain.Enums;

namespace Domain.Entities;

public class Rental : BaseEntity
{
    public string Identifier { get; set; } = string.Empty;
    public Guid CourierId { get; set; }
    public Guid MotorcycleId { get; set; }
    public RentalPlanType PlanType { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly ExpectedEndDate { get; set; }
    public DateOnly? ActualEndDate { get; set; }
    public decimal DailyCost { get; set; }
    public decimal TotalCost { get; set; }
    
    // Navigation properties
    public Courier Courier { get; set; } = null!;
    public Motorcycle Motorcycle { get; set; } = null!;
    
    public void CalculateTotalCost()
    {
        if (ActualEndDate == null)
        {
            // Not returned yet, calculate expected cost
            TotalCost = GetDaysCount(StartDate, ExpectedEndDate) * DailyCost;
            return;
        }

        var actualDays = GetDaysCount(StartDate, ActualEndDate.Value);
        var expectedDays = GetDaysCount(StartDate, ExpectedEndDate);

        if (ActualEndDate < ExpectedEndDate)
        {
            // Early return - calculate penalty
            var unusedDays = GetDaysCount(ActualEndDate.Value, ExpectedEndDate);
            var penaltyRate = GetPenaltyRate();
            var penalty = unusedDays * DailyCost * penaltyRate;
            
            TotalCost = (actualDays * DailyCost) + penalty;
        }
        else if (ActualEndDate > ExpectedEndDate)
        {
            // Late return - add extra charge
            var extraDays = GetDaysCount(ExpectedEndDate, ActualEndDate.Value);
            var extraCharge = extraDays * 50; // R$50 per extra day
            
            TotalCost = (expectedDays * DailyCost) + extraCharge;
        }
        else
        {
            // On time return
            TotalCost = expectedDays * DailyCost;
        }
    }

    private decimal GetPenaltyRate()
    {
        return PlanType switch
        {
            RentalPlanType.SevenDays => 0.20m,
            RentalPlanType.FifteenDays => 0.40m,
            _ => 0m
        };
    }

    private static int GetDaysCount(DateOnly start, DateOnly end)
    {
        return (end.ToDateTime(TimeOnly.MinValue) - start.ToDateTime(TimeOnly.MinValue)).Days;
    }
}
