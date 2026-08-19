import { DurationInput as SharedDurationInput, type DurationInputProps } from '@skedular/ui';
import { Field } from 'react-final-form';

export const DurationInput = (props: DurationInputProps) => <SharedDurationInput {...props} />;

export const DurationField = ({ name, ...props }: { name: string } & Omit<DurationInputProps, 'value' | 'onChange'>) => (
  <Field<string> name={name}>{({ input }) => <SharedDurationInput {...props} value={input.value} onChange={input.onChange} />}</Field>
);
