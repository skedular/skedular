import { NameDetails, getCustomerAvatarLetters, getCustomerFullName, stringToColor } from '@skedular/shared';
import { SxProps } from '@mui/material';
import Avatar from '@mui/material/Avatar';
import Tooltip from '@mui/material/Tooltip';
import { memo } from 'react';

type PhotoDetails = {
  url?: string | null;
};

type Props = {
  size?: 'small' | 'medium' | 'large';
  name?: NameDetails;
  photo?: PhotoDetails;
  sx?: SxProps | null;
  showFullName?: boolean;
  tip?: string;
  onClick?: (event: React.MouseEvent<HTMLElement>) => void;
};

const CustomerAvatar = ({ name, photo, size, sx, showFullName, tip, onClick }: Props) => {
  const avatarLetters = getCustomerAvatarLetters(name);

  let finalSx = { ...sx };
  if (size === 'small') {
    finalSx = { width: 24, height: 24 };
  } else if (size === 'medium') {
    finalSx = { width: 32, height: 32 };
  } else if (size === 'large') {
    finalSx = { width: 48, height: 48 };
  }

  finalSx = { ...finalSx, backgroundColor: stringToColor(getCustomerFullName(name)) };

  if (!showFullName) {
    return (
      <Avatar src={photo?.url ?? undefined} alt={avatarLetters} sx={finalSx} onClick={onClick}>
        {avatarLetters}
      </Avatar>
    );
  }

  return (
    <Tooltip title={tip ?? getCustomerFullName(name)}>
      <Avatar src={photo?.url ?? undefined} alt={avatarLetters} sx={finalSx} onClick={onClick}>
        {avatarLetters}
      </Avatar>
    </Tooltip>
  );
};

export default memo(CustomerAvatar);
