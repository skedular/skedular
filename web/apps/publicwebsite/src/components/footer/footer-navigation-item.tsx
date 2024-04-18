import Typography from '@mui/material/Typography';
import Link from 'next/link';
import { memo } from 'react';

interface NavigationItemProps {
  label: string;
  path: string;
}

const NavigationItem = ({ label, path }: NavigationItemProps) => {
  return (
    <Link href={path} passHref style={{ textDecoration: 'none' }}>
      <Typography
        variant="body2"
        sx={{
          color: 'primary.contrastText',
        }}
      >
        {label}
      </Typography>
    </Link>
  );
};

export default memo(NavigationItem);
