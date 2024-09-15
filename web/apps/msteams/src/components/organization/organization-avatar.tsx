import Avatar from '@mui/material/Avatar';
import { memo } from 'react';

type NameProps = {
  name?: string | null;
};

type PhotoProps = {
  url?: string | null;
};

type Props = {
  size?: 'small' | 'medium' | 'large';
  name: NameProps;
  photo: PhotoProps;
};

const OrganizationAvatar = ({ name, photo, size }: Props) => {
  let avatarLetters = '';

  if (name) {
    if (name.name && typeof name.name[0] !== 'undefined') {
      avatarLetters = name.name[0];
    } else {
      avatarLetters = '';
    }
  } else {
    avatarLetters = '';
  }

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

export default memo(OrganizationAvatar);
