using System;
using System.Collections.Generic;

namespace CommonInsfrastructure.Models.ConfigurationModels;

public partial class AgreementInfo
{
    public string AgreementId { get; set; } = null!;

    public string? TenantId { get; set; }

    public string? AgreementUploadPath { get; set; }

    public string? Description { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public DateTime? SignedDate { get; set; }

    public string? BillingAddress { get; set; }

    public bool? LatePayment { get; set; }

    public int? GracePeriod { get; set; }

    public string PaymentTerm { get; set; } = null!;

    public bool? EnableTax { get; set; }

    public bool? HasServices { get; set; }

    public bool? HasAircon { get; set; }

    public double? ExtensionCharges { get; set; }

    public double? Socharges { get; set; }

    public bool? Status { get; set; }

    public long? CreatedBy { get; set; }

    public long? UpdatedBy { get; set; }

    public long? DeletedBy { get; set; }

    public DateTime? DeletedAt { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<AgreementDefaultWorkingHour> AgreementDefaultWorkingHours { get; set; } = new List<AgreementDefaultWorkingHour>();

    public virtual ICollection<AgreementRate> AgreementRates { get; set; } = new List<AgreementRate>();

    public virtual ICollection<AgreementService> AgreementServices { get; set; } = new List<AgreementService>();

    public virtual ICollection<AgreementTaxis> AgreementTaxes { get; set; } = new List<AgreementTaxis>();

    public virtual ICollection<AgreementUnit> AgreementUnits { get; set; } = new List<AgreementUnit>();

    public virtual TenantInfo? Tenant { get; set; }
}
