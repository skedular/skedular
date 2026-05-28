import { memo } from 'react';

type Props = {
  contactPerson: string;
};

const ContactPerson = ({ contactPerson }: Props) => <>{contactPerson}</>;

export default memo(ContactPerson);
