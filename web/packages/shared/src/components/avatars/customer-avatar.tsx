import { SxProps } from '@mui/material';
import Avatar from '@mui/material/Avatar';
import { memo } from 'react';
import { NameDetails, getCustomerAvatarLetters } from '../../libs/utils';

type PhotoDetails = {
  url?: string | null;
};

type Props = {
  size?: 'small' | 'medium' | 'large';
  name: NameDetails;
  photo: PhotoDetails;
  sx?: SxProps | null;
};

const CustomerAvatar = ({ name, photo, size, sx }: Props) => {
  const avatarLetters = getCustomerAvatarLetters(name);

  let finalSx = { ...sx };
  if (size === 'small') {
    finalSx = { width: 24, height: 24 };
  } else if (size === 'medium') {
    finalSx = { width: 32, height: 32 };
  } else if (size === 'large') {
    finalSx = { width: 48, height: 48 };
  }

  return (
    <Avatar src={photo?.url ?? undefined} alt={avatarLetters} sx={finalSx}>
      {avatarLetters}
    </Avatar>
  );
};

export default memo(CustomerAvatar);
