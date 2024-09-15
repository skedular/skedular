import { SxProps } from '@mui/material';
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
  sx?: SxProps | null;
};

const LocationAvatar = ({ name, photo, size, sx }: Props) => {
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

  let finalSx = { ...sx };
  if (size === 'small') {
    finalSx = { width: 24, height: 24 };
  } else if (size === 'medium') {
    finalSx = { width: 32, height: 32 };
  } else if (size === 'large') {
    finalSx = { width: 40, height: 40 };
  }

  return (
    <Avatar src={photo?.url ?? undefined} alt={avatarLetters} sx={finalSx}>
      {avatarLetters}
    </Avatar>
  );
};

export default memo(LocationAvatar);
