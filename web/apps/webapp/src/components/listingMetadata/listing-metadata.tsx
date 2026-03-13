import { FormFieldLabel } from '@/components/commons';
import { TextField } from 'mui-rff';
import { useMemo, type ReactNode } from 'react';
import { FormSpy } from 'react-final-form';
import { object, string } from 'yup';

export type ListingMetadataValue = {
  about: string | null;
  title: string | null;
  subTitle: string | null;
  includedFeatures: string | null;
};

export type ListingMetadataFieldName = keyof ListingMetadataValue;

export const listingMetadataFieldNames: ListingMetadataFieldName[] = ['about', 'title', 'subTitle', 'includedFeatures'];

export const listingMetadataSchemaShape = {
  about: string().nullable(),
  title: string().nullable(),
  subTitle: string().nullable(),
  includedFeatures: string().nullable(),
};

export const listingMetadataSchema = object(listingMetadataSchemaShape);

const listingMetadataFieldLabels: Record<ListingMetadataFieldName, string> = {
  about: 'About',
  title: 'Title',
  subTitle: 'Sub Title',
  includedFeatures: 'Included Features',
};

export const normalizeListingMetadata = (value?: Partial<ListingMetadataValue> | null): ListingMetadataValue => ({
  about: value?.about ?? null,
  title: value?.title ?? null,
  subTitle: value?.subTitle ?? null,
  includedFeatures: value?.includedFeatures ?? null,
});

export const getListingMetadataFieldName = (field: ListingMetadataFieldName, namePrefix?: string) => (namePrefix ? `${namePrefix}.${field}` : field);

const getNestedValue = (value: Record<string, unknown> | undefined, path: string) =>
  path.split('.').reduce<unknown>((currentValue, key) => {
    if (!currentValue || typeof currentValue !== 'object') {
      return undefined;
    }

    return (currentValue as Record<string, unknown>)[key];
  }, value);

type Props = {
  fields?: ListingMetadataFieldName[];
  helperTexts?: Partial<Record<ListingMetadataFieldName, ReactNode>>;
  labels?: Partial<Record<ListingMetadataFieldName, string>>;
  namePrefix?: string;
  onChange?: (value: ListingMetadataValue) => void;
  requiredFields?: Partial<Record<ListingMetadataFieldName, boolean>>;
};

const ListingMetadata = ({ fields = listingMetadataFieldNames, helperTexts, labels, namePrefix, onChange, requiredFields }: Props) => {
  const visibleFields = useMemo(() => new Set(fields), [fields]);
  const resolvedLabels = { ...listingMetadataFieldLabels, ...labels };

  return (
    <>
      {onChange ? (
        <FormSpy
          subscription={{ values: true }}
          onChange={({ values }) => {
            const listingMetadata = normalizeListingMetadata({
              about: getNestedValue(values, getListingMetadataFieldName('about', namePrefix)) as string | null | undefined,
              title: getNestedValue(values, getListingMetadataFieldName('title', namePrefix)) as string | null | undefined,
              subTitle: getNestedValue(values, getListingMetadataFieldName('subTitle', namePrefix)) as string | null | undefined,
              includedFeatures: getNestedValue(values, getListingMetadataFieldName('includedFeatures', namePrefix)) as string | null | undefined,
            });

            onChange(listingMetadata);
          }}
        />
      ) : null}

      {visibleFields.has('about') ? (
        <FormFieldLabel label={resolvedLabels.about} required={requiredFields?.about}>
          <TextField name={getListingMetadataFieldName('about', namePrefix)} required={requiredFields?.about} multiline rows={3} helperText={helperTexts?.about} />
        </FormFieldLabel>
      ) : null}

      {visibleFields.has('title') ? (
        <FormFieldLabel label={resolvedLabels.title} required={requiredFields?.title}>
          <TextField name={getListingMetadataFieldName('title', namePrefix)} required={requiredFields?.title} multiline rows={3} helperText={helperTexts?.title} />
        </FormFieldLabel>
      ) : null}

      {visibleFields.has('subTitle') ? (
        <FormFieldLabel label={resolvedLabels.subTitle} required={requiredFields?.subTitle}>
          <TextField name={getListingMetadataFieldName('subTitle', namePrefix)} required={requiredFields?.subTitle} multiline rows={3} helperText={helperTexts?.subTitle} />
        </FormFieldLabel>
      ) : null}

      {visibleFields.has('includedFeatures') ? (
        <FormFieldLabel label={resolvedLabels.includedFeatures} required={requiredFields?.includedFeatures}>
          <TextField
            name={getListingMetadataFieldName('includedFeatures', namePrefix)}
            required={requiredFields?.includedFeatures}
            multiline
            rows={3}
            helperText={helperTexts?.includedFeatures}
          />
        </FormFieldLabel>
      ) : null}
    </>
  );
};

export default ListingMetadata;
