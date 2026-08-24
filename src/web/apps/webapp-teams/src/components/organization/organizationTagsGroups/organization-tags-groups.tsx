import { getOrganizationAdminCustomTagsBaseLink, getOrganizationAdminZonesBaseLink } from '@/components/links';
import OrganizationAdminTagsSection from '@/components/organization/organizationAdmin/organization-admin-tags-section';
import OrganizationAdminZonesSection from '@/components/organization/organizationAdmin/organization-admin-zones-section';
import type { organizationTagsGroups_query$key } from '@/queries/__generated__/organizationTagsGroups_query.graphql';
import ExpandMoreRoundedIcon from '@mui/icons-material/ExpandMoreRounded';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Menu from '@mui/material/Menu';
import MenuItem from '@mui/material/MenuItem';
import Tab from '@mui/material/Tab';
import Tabs from '@mui/material/Tabs';
import { useIntegratedPlatform } from '@skedular/shared';
import { defaultPadding, PageHeaderPanel, StackColumn } from '@skedular/ui';
import NextLink from 'next/link';
import { useSearchParams } from 'next/navigation';
import { memo, useState } from 'react';
import { graphql, useFragment } from 'react-relay';

type Props = { rootDataRelay: organizationTagsGroups_query$key; organizationCustomDomain: string };
type Section = 'tags-setup' | 'zones-setup';
const sections: Section[] = ['tags-setup', 'zones-setup'];
const labels: Record<Section, string> = { 'tags-setup': 'Tags', 'zones-setup': 'Zones' };

const OrganizationTagsGroups = ({ rootDataRelay, organizationCustomDomain }: Props) => {
  useFragment<organizationTagsGroups_query$key>(
    graphql`
      fragment organizationTagsGroups_query on Query {
        organization(customDomain: $organizationCustomDomain) {
          name
        }
      }
    `,
    rootDataRelay,
  );
  const { integratedPlatform } = useIntegratedPlatform();
  const requestedSection = useSearchParams().get('section') as Section | null;
  const activeSection = requestedSection && sections.includes(requestedSection) ? requestedSection : 'tags-setup';
  const [menuAnchor, setMenuAnchor] = useState<HTMLElement | null>(null);
  const links: Record<Section, string> = {
    'tags-setup': getOrganizationAdminCustomTagsBaseLink(integratedPlatform, organizationCustomDomain),
    'zones-setup': getOrganizationAdminZonesBaseLink(integratedPlatform, organizationCustomDomain),
  };
  return (
    <Box sx={{ width: '100%', maxWidth: '100vw', px: { xs: 0, sm: 1, md: 2 }, pb: defaultPadding }}>
      <StackColumn sx={{ width: '100%', maxWidth: 1200, mx: 'auto', pt: { xs: 1, sm: 1, md: 2 }, gap: 2 }}>
        <PageHeaderPanel eyebrow="Tags & Groups" title="Shared tags & zones" description="Manage tags and zones used across this organization.">
          <Box sx={{ display: { xs: 'block', sm: 'none' }, borderTop: 1, borderColor: 'divider', pt: 1.5 }}>
            <Button
              fullWidth
              variant="outlined"
              color="inherit"
              onClick={(event) => setMenuAnchor(event.currentTarget)}
              endIcon={<ExpandMoreRoundedIcon />}
              sx={{ justifyContent: 'space-between', minHeight: 48, borderRadius: 2.5, px: 2, textTransform: 'none' }}
            >{`Section: ${labels[activeSection]}`}</Button>
            <Menu anchorEl={menuAnchor} open={Boolean(menuAnchor)} onClose={() => setMenuAnchor(null)}>
              {sections.map((section) => (
                <MenuItem key={section} component={NextLink} href={links[section]} selected={activeSection === section} onClick={() => setMenuAnchor(null)}>
                  {labels[section]}
                </MenuItem>
              ))}
            </Menu>
          </Box>
          <Tabs
            value={activeSection}
            variant="scrollable"
            scrollButtons="auto"
            aria-label="Tags and groups sections"
            sx={{ display: { xs: 'none', sm: 'flex' }, mb: -2, borderTop: 1, borderColor: 'divider' }}
          >
            {sections.map((section) => (
              <Tab key={section} value={section} component={NextLink} href={links[section]} label={labels[section]} disableRipple />
            ))}
          </Tabs>
        </PageHeaderPanel>
        {activeSection === 'tags-setup' && <OrganizationAdminTagsSection organizationCustomDomain={organizationCustomDomain} />}
        {activeSection === 'zones-setup' && <OrganizationAdminZonesSection organizationCustomDomain={organizationCustomDomain} />}
      </StackColumn>
    </Box>
  );
};
export default memo(OrganizationTagsGroups);
