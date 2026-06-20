'use client';

import { IconTypography } from '@skedular/ui';
import type { ComponentProps, ReactNode } from 'react';

type Props = Omit<ComponentProps<typeof IconTypography>, 'label'> & {
  children?: ReactNode;
};

const Typography = ({ children, ...props }: Props) => <IconTypography {...props} label={children} />;

export default Typography;
