import Grid from '@mui/material/Grid2';
import { memo } from 'react';
import NavigationItem from './footer-navigation-item';
import FooterSectionTitle from './footer-section-title';

interface Navigation {
  label: string;
  path: string;
}

const productMenu: Array<Navigation> = [
  {
    label: 'Home',
    path: '/',
  },
  {
    label: 'Pricing',
    path: '/pricing',
  },
];

const companyMenu: Array<Navigation> = [
  { label: 'Privacy Policy', path: '/privacy-policy' },
  { label: 'Terms & Conditions', path: '/terms-of-service' },
];

const FooterNavigation = () => {
  return (
    <Grid container spacing={1}>
      <Grid sx={{ sx: 12, md: 4 }}>
        <FooterSectionTitle title="Product" />
        {productMenu.map(({ label, path }, index) => (
          <NavigationItem key={index + path} label={label} path={path} />
        ))}
      </Grid>
      <Grid sx={{ sx: 12, md: 4 }}>
        <FooterSectionTitle title="Company" />
        {companyMenu.map(({ label, path }, index) => (
          <NavigationItem key={index + path} label={label} path={path} />
        ))}
      </Grid>
    </Grid>
  );
};

export default memo(FooterNavigation);
