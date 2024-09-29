import Typography from '@mui/material/Typography';
import { Link } from '@repo/shared/components/link';
import { memo } from 'react';

interface NavigationItemProps {
  label: string;
  path: string;
}

const NavigationItem = ({ label, path }: NavigationItemProps) => {
  return (
    <Link href={path} passHref>
      <Typography variant="body2" sx={{ color: 'primary.contrastText' }}>
        {label}
      </Typography>
    </Link>
  );
};

export default memo(NavigationItem);
