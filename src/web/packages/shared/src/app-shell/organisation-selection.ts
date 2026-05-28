export type SelectableOrganisationSummary<TType extends string = string> = {
  id: string;
  name: string;
  customDomain: string;
  type: TType;
};

export const filterOrganisationsByType = <TOrganisation extends SelectableOrganisationSummary>(
  organisations: readonly TOrganisation[],
  allowedTypes: readonly TOrganisation['type'][],
): TOrganisation[] => organisations.filter((organisation) => allowedTypes.includes(organisation.type));
