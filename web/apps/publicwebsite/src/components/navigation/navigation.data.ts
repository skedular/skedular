export interface Navigation {
  label: string;
  path: string;
  isReactScrollEnabled: boolean;
  subItems?: Navigation[];
}

export const appBarNavigations: Navigation[] = [
  {
    label: 'Pricing',
    path: '/pricing',
    isReactScrollEnabled: false,
  },
];
