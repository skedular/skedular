import CaptionIconTypography from './caption-icon-typography';

type Props = {
  text?: string;
};

const HelperText = ({ text }: Props) => (text ? <CaptionIconTypography label={text} /> : <></>);

export default HelperText;
