import Box from '@mui/material/Box';
import { memo } from 'react';
import SocialLinkItem, { SocialLink } from './footer-social-link-item';

export const socialLinks: SocialLink[] = [
  {
    name: 'Linkedin',
    link: 'https://linkedin.com/company/unityhubio',
    icon: '/images/icons/linkedin.svg',
  },
  {
    name: 'Instagram',
    link: 'https://www.instagram.com/unityhubio/',
    icon: '/images/icons/instagram.svg',
  },
  {
    name: 'Twitter',
    link: 'https://twitter.com/unityhubio',
    icon: '/images/icons/twitter.svg',
  },
  {
    name: 'Github',
    link: 'https://github.com/unityhubio',
    icon: '/images/icons/github.svg',
  },
];

const SocialLinks = () => {
  return (
    <Box sx={{ ml: -1 }}>
      <Box
        component="ul"
        sx={{
          m: 0,
          p: 0,
          lineHeight: 0,
          borderRadius: 3,
          listStyle: 'none',
        }}
      >
        {socialLinks.map((item) => {
          return <SocialLinkItem key={item.name} item={item} />;
        })}
      </Box>
    </Box>
  );
};

export default memo(SocialLinks);
