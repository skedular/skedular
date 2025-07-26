import type { organizationTermsOfUse_query$key } from '@/queries/__generated__/organizationTermsOfUse_query.graphql';
import Link from '@mui/material/Link';
import { Checkboxes } from 'mui-rff';
import NextLink from 'next/link';
import { memo } from 'react';
import { graphql, useFragment } from 'react-relay';

type Props = {
  rootDataRelay: organizationTermsOfUse_query$key;
  name: string;
  required?: boolean;
};

const OrganizationTermsOfUse = ({ rootDataRelay, name, required }: Props) => {
  const rootData = useFragment(
    graphql`
      fragment organizationTermsOfUse_query on Query {
        activeOrganizationTermsOfUse {
          id
          terms
        }
      }
    `,
    rootDataRelay,
  );

  return (
    <>
      <Checkboxes
        name={name}
        required={required}
        data={{ label: rootData.activeOrganizationTermsOfUse.terms, value: true }}
        formControlLabelProps={{ sx: { alignItems: 'flex-start' } }}
      />
      <Link component={NextLink} href="https://getskedular.com/terms-of-service" target="_blank" rel="noopener noreferrer">
        Skedular Terms of Use.
      </Link>
    </>
  );
};

export default memo(OrganizationTermsOfUse);
