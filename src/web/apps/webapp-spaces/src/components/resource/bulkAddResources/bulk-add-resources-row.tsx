import { MultipleChoicesCustomTags, MultipleChoicesProductTags, MultipleChoicesZones, SingleChoiceResourceType } from '@/components/organization';
import type { multipleChoicesCustomTags_query$key } from '@/queries/__generated__/multipleChoicesCustomTags_query.graphql';
import type { multipleChoicesProductTags_query$key } from '@/queries/__generated__/multipleChoicesProductTags_query.graphql';
import type { multipleChoicesZones_query$key } from '@/queries/__generated__/multipleChoicesZones_query.graphql';
import type { singleChoiceResourceType_query$key } from '@/queries/__generated__/singleChoiceResourceType_query.graphql';
import Button from '@mui/material/Button';
import Divider from '@mui/material/Divider';
import { FormFieldLabel, SmallIconTypography, StackColumn } from '@skedular/ui';
import { TextField } from 'mui-rff';
import { memo } from 'react';

type Props = {
  rowIndex: number;
  rootDataRelay: singleChoiceResourceType_query$key & multipleChoicesCustomTags_query$key & multipleChoicesZones_query$key & multipleChoicesProductTags_query$key;
  organizationCustomDomain: string;
  onRemove: () => void;
  showProductTags: boolean;
};

const BulkAddResourceRowForm = ({ rowIndex, rootDataRelay, organizationCustomDomain, onRemove, showProductTags }: Props) => {
  const prefix = `rows[${rowIndex}]`;

  return (
    <>
      {rowIndex > 0 && <Divider sx={{ my: 1 }} />}
      <StackColumn spacing={1}>
        <SmallIconTypography label={`Row ${rowIndex + 1}`} />

        <FormFieldLabel label="Resource Type">
          <SingleChoiceResourceType rootDataRelay={rootDataRelay} name={`${prefix}.resourceTypeId`} required={true} />
        </FormFieldLabel>

        <FormFieldLabel label="Base name (optional)">
          <TextField name={`${prefix}.baseName`} helperText="Leave empty to use the resource type name." />
        </FormFieldLabel>

        <FormFieldLabel label="Quantity">
          <TextField
            name={`${prefix}.quantity`}
            type="number"
            required={true}
            fieldProps={{
              parse: (v) => {
                const n = parseInt(v, 10);
                return Number.isNaN(n) ? undefined : n;
              },
            }}
            slotProps={{ htmlInput: { min: 1 } }}
          />
        </FormFieldLabel>

        <FormFieldLabel label="Tags">
          <MultipleChoicesCustomTags rootDataRelay={rootDataRelay} name={`${prefix}.customTagIds`} organizationCustomDomain={organizationCustomDomain} />
        </FormFieldLabel>

        <FormFieldLabel label="Zones">
          <MultipleChoicesZones rootDataRelay={rootDataRelay} name={`${prefix}.zoneIds`} organizationCustomDomain={organizationCustomDomain} />
        </FormFieldLabel>

        {showProductTags && (
          <FormFieldLabel label="Booking Groups">
            <MultipleChoicesProductTags rootDataRelay={rootDataRelay} name={`${prefix}.productTagIds`} organizationCustomDomain={organizationCustomDomain} />
          </FormFieldLabel>
        )}

        <Button variant="text" onClick={onRemove} color="error" sx={{ alignSelf: 'flex-start', textTransform: 'none' }}>
          <SmallIconTypography label="Remove row" />
        </Button>
      </StackColumn>
    </>
  );
};

export default memo(BulkAddResourceRowForm);
