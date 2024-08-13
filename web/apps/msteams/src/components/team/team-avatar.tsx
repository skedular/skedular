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
  name: NameProps;
  photo: PhotoProps;
  sx?: SxProps | null;
};

const TeamAvatar = ({ name, photo, sx }: Props) => {
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

  return (
    <Avatar src={photo?.url ?? undefined} alt={avatarLetters} sx={sx}>
      {avatarLetters}
    </Avatar>
  );
};

export default memo(TeamAvatar);
