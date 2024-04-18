import Box from '@mui/material/Box';
import Link from '@mui/material/Link';
import { memo } from 'react';

export interface SocialLink {
  name: string;
  link: string;
  icon?: string;
}

interface SocialLinkItemProps {
  item: SocialLink;
}

const SocialLinkItem = ({ item }: SocialLinkItemProps) => (
  <Box
    component="li"
    sx={{
      display: 'inline-block',
      color: 'primary.contrastText',
      mr: 0.5,
    }}
  >
    <Link
      target="_blank"
      sx={{
        lineHeight: 0,
        display: 'flex',
        alignItems: 'center',
        justifyContent: 'center',
        width: 36,
        height: 36,
        borderRadius: '50%',
        color: 'inherit',
        '&:hover': {
          backgroundColor: 'secondary.main',
        },
        '& img': {
          fill: 'currentColor',
          width: 22,
          height: 'auto',
        },
      }}
      href={item.link}
    >
      {/* eslint-disable-next-line */}
      <img src={item.icon} alt={item.name + 'icon'} />
    </Link>
  </Box>
);

export default memo(SocialLinkItem);
