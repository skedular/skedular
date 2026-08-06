using Booking.Shared.Database.Entities;
using Booking.Shared.Models;
using Booking.Shared.Services;
using Xero.NetStandard.OAuth2.Model.Accounting;

namespace Booking.Shared.UnitTests.Services.XeroRecurringInvoiceTransitionServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class DecideShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void Return_Repeating_Invoice_Path_When_Repeating_Mode_Is_Selected_For_A_New_Recurring_Export(XeroRecurringInvoiceTransitionService sut)
    {
        var scheduleDefinition = new XeroRepeatingInvoiceScheduleDefinition(
            XeroRepeatingInvoiceScheduleSourceConstants.OrganizationBillingCycle,
            Schedule.UnitEnum.WEEKLY,
            1,
            10m);

        var result = sut.Decide(null, true, scheduleDefinition);

        result.Path.ShouldBe(XeroRecurringInvoiceExportPath.RepeatingInvoice);
        result.ConfigurationState.ShouldBe(AccountingInvoiceExportConfigurationStateConstants.Active);
        result.ConfigurationMessage.ShouldBeNull();
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Return_Standard_Invoice_Path_When_Repeating_Mode_Is_Selected_For_A_New_Recurring_Export_But_The_Cadence_Is_Unsupported(
        XeroRecurringInvoiceTransitionService sut)
    {
        var result = sut.Decide(null, true, null);

        result.Path.ShouldBe(XeroRecurringInvoiceExportPath.StandardInvoice);
        result.ConfigurationState.ShouldBe(AccountingInvoiceExportConfigurationStateConstants.Active);
        result.ConfigurationMessage.ShouldBeNull();
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Return_Standard_Invoice_Path_When_Repeating_Mode_Is_Selected_But_Existing_Recurring_Export_Is_Already_Standard(
        XeroRecurringInvoiceTransitionService sut)
    {
        var existingLink = new AccountingInvoiceExportLink
        {
            ExternalInvoiceId = "invoice-1",
            ExternalInvoiceMode = AccountingInvoiceExportModeConstants.StandardInvoice,
        };
        var scheduleDefinition = new XeroRepeatingInvoiceScheduleDefinition(
            XeroRepeatingInvoiceScheduleSourceConstants.OrganizationBillingCycle,
            Schedule.UnitEnum.WEEKLY,
            1,
            10m);

        var result = sut.Decide(existingLink, true, scheduleDefinition);

        result.Path.ShouldBe(XeroRecurringInvoiceExportPath.StandardInvoice);
        result.ConfigurationState.ShouldBe(AccountingInvoiceExportConfigurationStateConstants.TransitionRequired);
        result.ConfigurationMessage.ShouldNotBeNull();
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Return_Repeating_Invoice_Path_When_Repeating_Mode_Is_Selected_And_Existing_Standard_Link_Has_Not_Been_Exported_Yet(
        XeroRecurringInvoiceTransitionService sut)
    {
        var existingLink = new AccountingInvoiceExportLink
        {
            ExternalInvoiceMode = AccountingInvoiceExportModeConstants.StandardInvoice,
        };
        var scheduleDefinition = new XeroRepeatingInvoiceScheduleDefinition(
            XeroRepeatingInvoiceScheduleSourceConstants.OrganizationBillingCycle,
            Schedule.UnitEnum.WEEKLY,
            1,
            10m);

        var result = sut.Decide(existingLink, true, scheduleDefinition);

        result.Path.ShouldBe(XeroRecurringInvoiceExportPath.RepeatingInvoice);
        result.ConfigurationState.ShouldBe(AccountingInvoiceExportConfigurationStateConstants.Active);
        result.ConfigurationMessage.ShouldBeNull();
    }

    [Theory]
    [AutoFakeItEasyData]
    public void
        Return_Standard_Invoice_Path_When_Repeating_Mode_Is_Selected_But_Existing_Standard_Link_Has_Not_Been_Exported_Yet_And_The_Cadence_Is_Unsupported(
            XeroRecurringInvoiceTransitionService sut)
    {
        var existingLink = new AccountingInvoiceExportLink
        {
            ExternalInvoiceMode = AccountingInvoiceExportModeConstants.StandardInvoice,
        };

        var result = sut.Decide(existingLink, true, null);

        result.Path.ShouldBe(XeroRecurringInvoiceExportPath.StandardInvoice);
        result.ConfigurationState.ShouldBe(AccountingInvoiceExportConfigurationStateConstants.Active);
        result.ConfigurationMessage.ShouldBeNull();
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Return_Standard_Invoice_Path_When_Repeating_Mode_Is_Not_Selected_And_No_Recurring_Export_Exists_Yet(
        XeroRecurringInvoiceTransitionService sut)
    {
        var scheduleDefinition = new XeroRepeatingInvoiceScheduleDefinition(
            XeroRepeatingInvoiceScheduleSourceConstants.OrganizationBillingCycle,
            Schedule.UnitEnum.WEEKLY,
            1,
            10m);

        var result = sut.Decide(null, false, scheduleDefinition);

        result.Path.ShouldBe(XeroRecurringInvoiceExportPath.StandardInvoice);
        result.ConfigurationState.ShouldBe(AccountingInvoiceExportConfigurationStateConstants.Active);
        result.ConfigurationMessage.ShouldBeNull();
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Freeze_Existing_Repeating_Invoice_When_Repeating_Mode_Is_Turned_Off(XeroRecurringInvoiceTransitionService sut)
    {
        var existingLink = new AccountingInvoiceExportLink
        {
            ExternalInvoiceId = "repeating-1",
            ExternalInvoiceMode = AccountingInvoiceExportModeConstants.RepeatingInvoice,
            RepeatingScheduleSource = XeroRepeatingInvoiceScheduleSourceConstants.OrganizationBillingCycle,
            RepeatingScheduleUnit = Schedule.UnitEnum.WEEKLY.ToString(),
            RepeatingSchedulePeriod = 1,
        };

        var result = sut.Decide(existingLink, false, null);

        result.Path.ShouldBe(XeroRecurringInvoiceExportPath.FreezeExistingRepeatingInvoice);
        result.ConfigurationState.ShouldBe(AccountingInvoiceExportConfigurationStateConstants.TransitionRequired);
        result.ConfigurationMessage.ShouldNotBeNull();
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Return_Standard_Invoice_Path_When_Repeating_Mode_Is_Turned_Off_Before_An_Existing_Repeating_Link_Has_Been_Exported(
        XeroRecurringInvoiceTransitionService sut)
    {
        var existingLink = new AccountingInvoiceExportLink
        {
            ExternalInvoiceMode = AccountingInvoiceExportModeConstants.RepeatingInvoice,
            RepeatingScheduleSource = XeroRepeatingInvoiceScheduleSourceConstants.OrganizationBillingCycle,
            RepeatingScheduleUnit = Schedule.UnitEnum.WEEKLY.ToString(),
            RepeatingSchedulePeriod = 1,
        };

        var result = sut.Decide(existingLink, false, null);

        result.Path.ShouldBe(XeroRecurringInvoiceExportPath.StandardInvoice);
        result.ConfigurationState.ShouldBe(AccountingInvoiceExportConfigurationStateConstants.Active);
        result.ConfigurationMessage.ShouldBeNull();
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Freeze_Existing_Repeating_Invoice_When_Current_Cadence_Can_No_Longer_Be_Represented(XeroRecurringInvoiceTransitionService sut)
    {
        var existingLink = new AccountingInvoiceExportLink
        {
            ExternalInvoiceId = "repeating-1",
            ExternalInvoiceMode = AccountingInvoiceExportModeConstants.RepeatingInvoice,
            RepeatingScheduleSource = XeroRepeatingInvoiceScheduleSourceConstants.OrganizationBillingCycle,
            RepeatingScheduleUnit = Schedule.UnitEnum.WEEKLY.ToString(),
            RepeatingSchedulePeriod = 1,
        };

        var result = sut.Decide(existingLink, true, null);

        result.Path.ShouldBe(XeroRecurringInvoiceExportPath.FreezeExistingRepeatingInvoice);
        result.ConfigurationState.ShouldBe(AccountingInvoiceExportConfigurationStateConstants.TransitionRequired);
        result.ConfigurationMessage.ShouldNotBeNull();
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Freeze_Existing_Repeating_Invoice_When_Desired_Schedule_Differs_From_The_Stored_Schedule(XeroRecurringInvoiceTransitionService sut)
    {
        var existingLink = new AccountingInvoiceExportLink
        {
            ExternalInvoiceId = "repeating-1",
            ExternalInvoiceMode = AccountingInvoiceExportModeConstants.RepeatingInvoice,
            RepeatingScheduleSource = XeroRepeatingInvoiceScheduleSourceConstants.OrganizationBillingCycle,
            RepeatingScheduleUnit = Schedule.UnitEnum.WEEKLY.ToString(),
            RepeatingSchedulePeriod = 1,
        };
        var scheduleDefinition = new XeroRepeatingInvoiceScheduleDefinition(
            XeroRepeatingInvoiceScheduleSourceConstants.OrganizationBillingCycle,
            Schedule.UnitEnum.WEEKLY,
            2,
            10m);

        var result = sut.Decide(existingLink, true, scheduleDefinition);

        result.Path.ShouldBe(XeroRecurringInvoiceExportPath.FreezeExistingRepeatingInvoice);
        result.ConfigurationState.ShouldBe(AccountingInvoiceExportConfigurationStateConstants.TransitionRequired);
        result.ConfigurationMessage.ShouldNotBeNull();
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Return_Repeating_Invoice_Path_When_Existing_Repeating_Invoice_Already_Matches_The_Desired_Schedule(
        XeroRecurringInvoiceTransitionService sut)
    {
        var existingLink = new AccountingInvoiceExportLink
        {
            ExternalInvoiceId = "repeating-1",
            ExternalInvoiceMode = AccountingInvoiceExportModeConstants.RepeatingInvoice,
            RepeatingScheduleSource = XeroRepeatingInvoiceScheduleSourceConstants.PurchaseCadence,
            RepeatingScheduleUnit = Schedule.UnitEnum.MONTHLY.ToString(),
            RepeatingSchedulePeriod = 3,
        };
        var scheduleDefinition = new XeroRepeatingInvoiceScheduleDefinition(
            XeroRepeatingInvoiceScheduleSourceConstants.PurchaseCadence,
            Schedule.UnitEnum.MONTHLY,
            3,
            10m);

        var result = sut.Decide(existingLink, true, scheduleDefinition);

        result.Path.ShouldBe(XeroRecurringInvoiceExportPath.RepeatingInvoice);
        result.ConfigurationState.ShouldBe(AccountingInvoiceExportConfigurationStateConstants.Active);
        result.ConfigurationMessage.ShouldBeNull();
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Return_Repeating_Invoice_Path_When_Existing_Repeating_Link_Has_Not_Been_Exported_Yet_And_Desired_Schedule_Is_Still_Supported(
        XeroRecurringInvoiceTransitionService sut)
    {
        var existingLink = new AccountingInvoiceExportLink
        {
            ExternalInvoiceMode = AccountingInvoiceExportModeConstants.RepeatingInvoice,
            RepeatingScheduleSource = XeroRepeatingInvoiceScheduleSourceConstants.PurchaseCadence,
            RepeatingScheduleUnit = Schedule.UnitEnum.MONTHLY.ToString(),
            RepeatingSchedulePeriod = 3,
        };
        var scheduleDefinition = new XeroRepeatingInvoiceScheduleDefinition(
            XeroRepeatingInvoiceScheduleSourceConstants.PurchaseCadence,
            Schedule.UnitEnum.MONTHLY,
            3,
            10m);

        var result = sut.Decide(existingLink, true, scheduleDefinition);

        result.Path.ShouldBe(XeroRecurringInvoiceExportPath.RepeatingInvoice);
        result.ConfigurationState.ShouldBe(AccountingInvoiceExportConfigurationStateConstants.Active);
        result.ConfigurationMessage.ShouldBeNull();
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Return_Standard_Invoice_Path_When_Existing_Repeating_Link_Has_Not_Been_Exported_Yet_And_The_Current_Cadence_Is_No_Longer_Supported(
        XeroRecurringInvoiceTransitionService sut)
    {
        var existingLink = new AccountingInvoiceExportLink
        {
            ExternalInvoiceMode = AccountingInvoiceExportModeConstants.RepeatingInvoice,
            RepeatingScheduleSource = XeroRepeatingInvoiceScheduleSourceConstants.PurchaseCadence,
            RepeatingScheduleUnit = Schedule.UnitEnum.MONTHLY.ToString(),
            RepeatingSchedulePeriod = 3,
        };

        var result = sut.Decide(existingLink, true, null);

        result.Path.ShouldBe(XeroRecurringInvoiceExportPath.StandardInvoice);
        result.ConfigurationState.ShouldBe(AccountingInvoiceExportConfigurationStateConstants.Active);
        result.ConfigurationMessage.ShouldBeNull();
    }
}
