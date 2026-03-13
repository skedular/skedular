import { graphql } from 'react-relay';

const marketplaceProductDetailSharedProductFragment = graphql`
  fragment marketplaceProductDetailSharedProductFragment_product on ProductDetails {
    id
    name
    listingMetadata {
      about
      title
      subTitle
      includedFeatures
    }
    featureImages {
      original {
        url
      }
    }
    amenities {
      id
      name
      color
    }
    currency {
      type
      name
    }
    pricingOptions {
      id
      index
      name
      listingMetadata {
        about
        title
        subTitle
        includedFeatures
      }
      cadence
      price
      isTaxInclusive
      acceptedPaymentMethods
      minDurationMinutes
      maxDurationMinutes
      numberOfResourcesToBook
    }
  }
`;

export default marketplaceProductDetailSharedProductFragment;
