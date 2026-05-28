'use client';

import CaptionIconTypography from '../typography/caption-icon-typography';

type Props = {
  text?: string;
};

const HelperText = ({ text }: Props) => (text ? <CaptionIconTypography label={text} /> : null);

export default HelperText;
