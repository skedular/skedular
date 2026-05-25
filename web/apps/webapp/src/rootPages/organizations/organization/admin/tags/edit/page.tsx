import { getOrganizationAdminCustomTagsBaseLink } from '@/components/links';
import { EditOrganizationCustomTagPage } from '@/components/organization/editOrganizationCustomTag';
import { RootShell } from '@/components/rootShell';
import useKnownParams from '@/hooks/use-known-params';
import { useIntegratedPlatrform } from '@skedular/shared';
import { useParams, useRouter, useSearchParams } from 'next/navigation';
import { memo } from 'react';

const getParamValue = (value: string | string[] | undefined): string => (Array.isArray(value) ? (value[0] ?? '') : (value ?? ''));

const RootPage = () => {
  const { organizationCustomDomain } = useKnownParams();
  const { integratedPlatrform } = useIntegratedPlatrform();
  const router = useRouter();
  const searchParams = useSearchParams();
  const customTagId = getParamValue(useParams().customTagId);

  if (!organizationCustomDomain) {
    throw new Error('organizationCustomDomain is required');
  }
  if (!customTagId) {
    throw new Error('customTagId is required');
  }

  const returnUrl = searchParams.get('redirectUrl') ?? getOrganizationAdminCustomTagsBaseLink(integratedPlatrform, organizationCustomDomain);

  return (
    <RootShell>
      <EditOrganizationCustomTagPage customTagId={customTagId} onSaved={() => router.push(returnUrl)} onCancel={() => router.push(returnUrl)} />
    </RootShell>
  );
};

export default memo(RootPage);
