import Avatar from '@mui/material/Avatar';
import { NameDetails, getCustomerAvatarLetters } from '@repo/shared/libs/utils';
import { memo } from 'react';

type PhotoDetails = {
  url?: string | null;
};

type Props = {
  size?: 'small' | 'medium' | 'large';
  name: NameDetails;
  photo: PhotoDetails;
};

const CustomerAvatar = ({ name, photo, size }: Props) => {
  const avatarLetters = getCustomerAvatarLetters(name);

  let sx = {};
  if (size === 'small') {
    sx = { width: 24, height: 24 };
  } else if (size === 'medium') {
    sx = { width: 32, height: 32 };
  } else if (size === 'large') {
    sx = { width: 48, height: 48 };
  }

  return (
    <Avatar src={photo?.url ?? undefined} alt={avatarLetters} sx={sx}>
      {avatarLetters}
    </Avatar>
  );
};

export default memo(CustomerAvatar);
