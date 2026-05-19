import type { ReactNode } from 'react';

type Props = {
  children: ReactNode;
};

const CoWorkingSubdomain = ({ children }: Props) => <div data-customer-facing-entry="co-working-subdomain">{children}</div>;

export default CoWorkingSubdomain;
