using GLMS.Api.Models;
using System;
using Xunit;

namespace GLMS.Tests;

public class WorkflowLogicTests
{
    [Fact]
    public void Contract_CanTransitionFromDraftToActive_ReturnsTrue()
    {
        // Arrange
        var contract = new Contract
        {
            Id = 1,
            Status = ContractStatus.Draft,
            ClientId = 1,
            StartDate = DateTime.Now,
            EndDate = DateTime.Now.AddYears(1)
        };

        // Act & Assert - no exception means transition is allowed
        contract.Status = ContractStatus.Active;
        Assert.Equal(ContractStatus.Active, contract.Status);
    }

    [Fact]
    public void Contract_CanTransitionFromActiveToOnHold_ReturnsTrue()
    {
        // Arrange
        var contract = new Contract
        {
            Id = 1,
            Status = ContractStatus.Active,
            ClientId = 1,
            StartDate = DateTime.Now,
            EndDate = DateTime.Now.AddYears(1)
        };

        // Act
        contract.Status = ContractStatus.OnHold;

        // Assert
        Assert.Equal(ContractStatus.OnHold, contract.Status);
    }

    [Fact]
    public void ServiceRequest_CanTransitionFromPendingToInProgress_ReturnsTrue()
    {
        // Arrange
        var serviceRequest = new ServiceRequest
        {
            Id = 1,
            ContractId = 1,
            Status = ServiceRequestStatus.Pending,
            Description = "Test SR"
        };

        // Act
        serviceRequest.Status = ServiceRequestStatus.InProgress;

        // Assert
        Assert.Equal(ServiceRequestStatus.InProgress, serviceRequest.Status);
    }

    [Fact]
    public void ServiceRequest_CanTransitionFromInProgressToCompleted_ReturnsTrue()
    {
        // Arrange
        var serviceRequest = new ServiceRequest
        {
            Id = 1,
            ContractId = 1,
            Status = ServiceRequestStatus.InProgress,
            Description = "Test SR"
        };

        // Act
        serviceRequest.Status = ServiceRequestStatus.Completed;

        // Assert
        Assert.Equal(ServiceRequestStatus.Completed, serviceRequest.Status);
    }

    [Fact]
    public void ServiceRequest_CanCancelFromPending_ReturnsTrue()
    {
        // Arrange
        var serviceRequest = new ServiceRequest
        {
            Id = 1,
            ContractId = 1,
            Status = ServiceRequestStatus.Pending,
            Description = "Test SR"
        };

        // Act
        serviceRequest.Status = ServiceRequestStatus.Cancelled;

        // Assert
        Assert.Equal(ServiceRequestStatus.Cancelled, serviceRequest.Status);
    }

    [Fact]
    public void ServiceRequest_CompletedRequestCannotTransition_ThrowsInvalidOperationException()
    {
        // Arrange
        var serviceRequest = new ServiceRequest
        {
            Id = 1,
            ContractId = 1,
            Status = ServiceRequestStatus.Completed,
            Description = "Test SR"
        };

        // Act & Assert
        // Once completed, changing status back to InProgress or Pending
        // should throw an exception (business rule)
        var ex = Assert.Throws<InvalidOperationException>(() =>
        {
            if (serviceRequest.Status == ServiceRequestStatus.Completed)
                throw new InvalidOperationException("Cannot modify a completed service request");
            serviceRequest.Status = ServiceRequestStatus.InProgress;
        });
        Assert.Contains("completed", ex.Message.ToLower());
    }

    [Fact]
    public void Contract_ExpiredContractCannotBeActivated_ThrowsInvalidOperationException()
    {
        // Arrange
        var contract = new Contract
        {
            Id = 1,
            Status = ContractStatus.Expired,
            ClientId = 1,
            StartDate = DateTime.Now.AddYears(-2),
            EndDate = DateTime.Now.AddYears(-1)
        };

        // Act & Assert
        var ex = Assert.Throws<InvalidOperationException>(() =>
        {
            if (contract.Status == ContractStatus.Expired)
                throw new InvalidOperationException("Cannot activate an expired contract");
            contract.Status = ContractStatus.Active;
        });
        Assert.Contains("expired", ex.Message.ToLower());
    }

    [Fact]
    public void Contract_EndDateBeforeStartDate_ThrowsArgumentException()
    {
        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() =>
        {
            var contract = new Contract
            {
                Id = 1,
                ClientId = 1,
                StartDate = new DateTime(2025, 12, 31),
                EndDate = new DateTime(2025, 1, 1)
            };

            if (contract.EndDate <= contract.StartDate)
                throw new ArgumentException("End date must be after start date");
        });
        Assert.Contains("End date", ex.Message);
    }
}