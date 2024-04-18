using Api.Shared.Services.GraphQL.UnityHub.V1.Payment;
using Payment.Shared.Models;
using Stripe;
using Customer = Payment.Shared.Models.Customer;
using Identity = Payment.Shared.Database.Entities.Identity;

namespace Payment.Api.Mappers;

public interface IMapper
{
    IEnumerable<OrganizationPaymentMethod> MapTo(IEnumerable<OrganizationStripePaymentMethod> src);

    Shared.Database.Entities.OrganizationStripePaymentMethod MergeTo(
        PaymentMethod paymentMethod,
        Shared.Database.Entities.OrganizationStripePaymentMethod dest);

    Customer MapTo(Shared.Database.Entities.Customer src);

    IEnumerable<OrganizationStripePaymentMethod> MapTo(
        IEnumerable<Shared.Database.Entities.OrganizationStripePaymentMethod> src);
}

public class Mapper : IMapper
{
    public IEnumerable<OrganizationPaymentMethod> MapTo(IEnumerable<OrganizationStripePaymentMethod> src) =>
        src.Select(MapTo);

    public Shared.Database.Entities.OrganizationStripePaymentMethod MergeTo(
        PaymentMethod paymentMethod,
        Shared.Database.Entities.OrganizationStripePaymentMethod dest)
    {
        dest.PaymentMethodId = paymentMethod.Id;

        if (paymentMethod.Card is null)
        {
            return dest;
        }

        dest.CardBrand = paymentMethod.Card.Brand;
        dest.CardCountry = paymentMethod.Card.Country;
        dest.CardDescription = paymentMethod.Card.Description;
        dest.CardExpiryMonth = (byte)paymentMethod.Card.ExpMonth;
        dest.CardExpiryYear = (short)paymentMethod.Card.ExpYear;
        dest.CardFingerprint = paymentMethod.Card.Fingerprint;
        dest.CardFunding = paymentMethod.Card.Funding;
        dest.CardIssuer = paymentMethod.Card.Issuer;
        dest.CardLastFourDigit = paymentMethod.Card.Last4;
        return dest;
    }

    public Customer MapTo(Shared.Database.Entities.Customer src) =>
        new()
        {
            CreatedAt = src.CreatedAt,
            DeletedAt = src.DeletedAt,
            ModifiedAt = src.ModifiedAt,
            Id = src.Id,
            Identities = MapTo(src.Identities).ToList()
        };

    public IEnumerable<OrganizationStripePaymentMethod> MapTo(
        IEnumerable<Shared.Database.Entities.OrganizationStripePaymentMethod> src) =>
        src.Select(MapTo);

    private static OrganizationStripePaymentMethod
        MapTo(Shared.Database.Entities.OrganizationStripePaymentMethod src) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            DeletedAt = src.DeletedAt,
            ModifiedAt = src.ModifiedAt,
            SetupIntentId = src.SetupIntentId,
            ClientSecret = src.ClientSecret,
            Status = src.Status,
            PaymentMethodId = src.PaymentMethodId,
            CardBrand = src.CardBrand,
            CardCountry = src.CardCountry,
            CardDescription = src.CardDescription,
            CardExpiryMonth = src.CardExpiryMonth,
            CardExpiryYear = src.CardExpiryYear,
            CardFingerprint = src.CardFingerprint,
            CardFunding = src.CardFunding,
            CardIssuer = src.CardIssuer,
            CardLastFourDigit = src.CardLastFourDigit
        };

    private static OrganizationPaymentMethod MapTo(OrganizationStripePaymentMethod src) =>
        new()
        {
            Id = src.Id,
            CardBrand = src.CardBrand,
            CardCountry = src.CardCountry,
            CardDescription = src.CardDescription,
            CardExpiryMonth = src.CardExpiryMonth,
            CardExpiryYear = src.CardExpiryYear,
            CardFingerprint = src.CardFingerprint,
            CardFunding = src.CardFunding,
            CardIssuer = src.CardIssuer,
            CardLastFourDigit = src.CardLastFourDigit
        };

    private static IEnumerable<Shared.Models.Identity> MapTo(IEnumerable<Identity?>? src) =>
        (src is null ? [] : src.Where(item => item is not null).Select(MapTo))!;

    private static Shared.Models.Identity? MapTo(Identity? src) =>
        src is null
            ? null
            : new Shared.Models.Identity { Id = src.Id, CreatedAt = src.CreatedAt, ModifiedAt = src.ModifiedAt };
}
