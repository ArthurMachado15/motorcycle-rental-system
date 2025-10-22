using Domain.Entities;
using Domain.Enums;
using FluentAssertions;
using Xunit;

namespace UnitTests.Domain;

public class RentalTests
{
    [Fact]
    public void CalculateTotalCost_OnTimeReturn_ShouldCalculateCorrectly()
    {
        // Arrange
        var rental = new Rental
        {
            PlanType = RentalPlanType.SevenDays,
            StartDate = new DateOnly(2024, 1, 1),
            ExpectedEndDate = new DateOnly(2024, 1, 8),
            ActualEndDate = new DateOnly(2024, 1, 8),
            DailyCost = 30.00m
        };

        // Act
        rental.CalculateTotalCost();

        // Assert
        rental.TotalCost.Should().Be(210.00m); // 7 days * 30
    }

    [Fact]
    public void CalculateTotalCost_EarlyReturn_SevenDayPlan_ShouldApplyTwentyPercentPenalty()
    {
        // Arrange
        var rental = new Rental
        {
            PlanType = RentalPlanType.SevenDays,
            StartDate = new DateOnly(2024, 1, 1),
            ExpectedEndDate = new DateOnly(2024, 1, 8),
            ActualEndDate = new DateOnly(2024, 1, 6), // 2 days early
            DailyCost = 30.00m
        };

        // Act
        rental.CalculateTotalCost();

        // Assert
        // Used days: 5 * 30 = 150
        // Unused days: 2
        // Penalty: 2 * 30 * 0.20 = 12
        // Total: 150 + 12 = 162
        rental.TotalCost.Should().Be(162.00m);
    }

    [Fact]
    public void CalculateTotalCost_EarlyReturn_FifteenDayPlan_ShouldApplyFortyPercentPenalty()
    {
        // Arrange
        var rental = new Rental
        {
            PlanType = RentalPlanType.FifteenDays,
            StartDate = new DateOnly(2024, 1, 1),
            ExpectedEndDate = new DateOnly(2024, 1, 16),
            ActualEndDate = new DateOnly(2024, 1, 11), // 5 days early
            DailyCost = 28.00m
        };

        // Act
        rental.CalculateTotalCost();

        // Assert
        // Used days: 10 * 28 = 280
        // Unused days: 5
        // Penalty: 5 * 28 * 0.40 = 56
        // Total: 280 + 56 = 336
        rental.TotalCost.Should().Be(336.00m);
    }

    [Fact]
    public void CalculateTotalCost_LateReturn_ShouldAddFiftyPerDayExtra()
    {
        // Arrange
        var rental = new Rental
        {
            PlanType = RentalPlanType.SevenDays,
            StartDate = new DateOnly(2024, 1, 1),
            ExpectedEndDate = new DateOnly(2024, 1, 8),
            ActualEndDate = new DateOnly(2024, 1, 11), // 3 days late
            DailyCost = 30.00m
        };

        // Act
        rental.CalculateTotalCost();

        // Assert
        // Expected days: 7 * 30 = 210
        // Extra days: 3 * 50 = 150
        // Total: 210 + 150 = 360
        rental.TotalCost.Should().Be(360.00m);
    }

    [Fact]
    public void CalculateTotalCost_ThirtyDayPlan_EarlyReturn_ShouldHaveNoPenalty()
    {
        // Arrange
        var rental = new Rental
        {
            PlanType = RentalPlanType.ThirtyDays,
            StartDate = new DateOnly(2024, 1, 1),
            ExpectedEndDate = new DateOnly(2024, 1, 31),
            ActualEndDate = new DateOnly(2024, 1, 25), // 6 days early
            DailyCost = 22.00m
        };

        // Act
        rental.CalculateTotalCost();

        // Assert
        // Used days: 24 * 22 = 528
        // No penalty for 30+ day plans
        // Total: 528
        rental.TotalCost.Should().Be(528.00m);
    }
}
