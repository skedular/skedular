import { SxProps } from '@mui/material';
import Avatar from '@mui/material/Avatar';
import { NameDetails, getCustomerAvatarLetters } from '@repo/shared/libs/utils';
import { memo } from 'react';

type PhotoDetails = {
  url?: string | null;
};

type Props = {
  name: NameDetails;
  photo: PhotoDetails;
  sx?: SxProps | null;
};

const CustomerAvatar = ({ name, photo, sx }: Props) => {
  const avatarLetters = getCustomerAvatarLetters(name);

  return (
    <Avatar src={photo?.url ?? undefined} alt={avatarLetters} sx={sx}>
      {avatarLetters}
    </Avatar>
  );
};

export default memo(CustomerAvatar);
