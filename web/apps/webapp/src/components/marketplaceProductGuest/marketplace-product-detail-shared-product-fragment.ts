import { graphql } from 'react-relay';

const marketplaceProductDetailSharedProductFragment = graphql`
  fragment marketplaceProductDetailSharedProductFragment_product on ProductDetails {
    id
    name
    description
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
      description
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
