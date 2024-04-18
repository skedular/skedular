import type { organizationTermsOfUse_query$key } from '@/queries/__generated__/organizationTermsOfUse_query.graphql';
import Link from '@mui/material/Link';
import { Checkboxes } from 'mui-rff';
import { memo } from 'react';
import { graphql, useFragment } from 'react-relay';

type Props = {
  rootDataRelay: organizationTermsOfUse_query$key;
  name: string;
  required?: boolean;
};

interface SubCategoryDetails {
  mainCategoryName: string;
  id: string;
  name: string;
}

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
        data={{
          label: rootData.activeOrganizationTermsOfUse.terms,
          value: true,
        }}
      />
      <Link href="https://unityhub.io/terms-of-service" target="_blank" rel="noopener noreferrer">
        UnityHub Terms of Use.
      </Link>
    </>
  );
};

export default memo(OrganizationTermsOfUse);
