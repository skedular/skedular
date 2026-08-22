import { DurationInput as SharedDurationInput, type DurationInputProps, type DurationUnit } from '@skedular/ui';
import { Field } from 'react-final-form';

export const DurationInput = (props: DurationInputProps) => <SharedDurationInput {...props} />;

export const DurationField = ({ name, unitName, ...props }: { name: string; unitName: string } & Omit<DurationInputProps, 'value' | 'onChange' | 'unit' | 'onUnitChange'>) => (
  <Field<string> name={name}>
    {({ input }) => (
      <Field<DurationUnit | null> name={unitName} subscription={{ value: true }}>
        {({ input: unitInput }) => (
          <SharedDurationInput
            {...props}
            value={input.value}
            unit={unitInput.value?.toLowerCase() as DurationUnit | undefined}
            onUnitChange={(nextUnit) => unitInput.onChange(nextUnit.toUpperCase())}
            onChange={input.onChange}
          />
        )}
      </Field>
    )}
  </Field>
);
