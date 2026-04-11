import type { AutocompleteProps as MuiAutocompleteProps } from '@mui/material/Autocomplete';
import MuiAutocomplete from '@mui/material/Autocomplete';
import TextField from '@mui/material/TextField';
import type { ReactNode } from 'react';
import { memo } from 'react';
import { Field } from 'react-final-form';

type BaseProps<TOption, Multiple extends boolean> = Omit<
  MuiAutocompleteProps<TOption, Multiple, false, false>,
  'defaultValue' | 'getOptionLabel' | 'isOptionEqualToValue' | 'multiple' | 'onChange' | 'options' | 'renderInput' | 'value'
> & {
  name: string;
  multiple: Multiple;
  options: readonly TOption[];
  getOptionValue: (option: TOption) => string;
  getOptionLabel: (option: TOption | string) => string;
  fieldProps?: {
    onChange?: (event: { target: { name: string; value: Multiple extends true ? string[] : string } }) => void;
  };
  helperText?: ReactNode;
  required?: boolean;
  textFieldProps?: Omit<React.ComponentProps<typeof TextField>, 'error' | 'helperText' | 'name' | 'required'>;
  onChange?: MuiAutocompleteProps<TOption, Multiple, false, false>['onChange'];
};

const AutocompleteFieldComponent = <TOption, Multiple extends boolean = false>({
  name,
  multiple,
  options,
  getOptionValue,
  getOptionLabel,
  fieldProps,
  helperText,
  required,
  textFieldProps,
  onChange,
  ...autocompleteProps
}: BaseProps<TOption, Multiple>) => {
  return (
    <Field name={name}>
      {({ input, meta }) => {
        const hasError = Boolean(meta.touched && (meta.error || meta.submitError));
        const errorText = meta.error || meta.submitError;

        const selectedValue = multiple
          ? options.filter((option) => Array.isArray(input.value) && input.value.includes(getOptionValue(option)))
          : (options.find((option) => getOptionValue(option) === input.value) ?? null);

        return (
          <MuiAutocomplete<TOption, Multiple, false, false>
            {...autocompleteProps}
            multiple={multiple}
            options={options}
            value={selectedValue as MuiAutocompleteProps<TOption, Multiple, false, false>['value']}
            isOptionEqualToValue={(option, value) => getOptionValue(option) === getOptionValue(value as TOption)}
            getOptionLabel={(option) => getOptionLabel(option)}
            onChange={(event, value, reason, details) => {
              const nextValue = multiple ? (value as TOption[]).map((option) => getOptionValue(option)) : value ? getOptionValue(value as TOption) : '';

              if (multiple) {
                input.onChange(nextValue);
              } else {
                input.onChange(nextValue);
              }

              fieldProps?.onChange?.({
                target: {
                  name,
                  value: nextValue as Multiple extends true ? string[] : string,
                },
              });

              onChange?.(event, value, reason, details);
            }}
            onBlur={(event) => {
              input.onBlur();
              autocompleteProps.onBlur?.(event);
            }}
            renderInput={(params) => (
              <TextField {...params} {...textFieldProps} name={input.name} required={required} error={hasError} helperText={hasError ? errorText : helperText} />
            )}
          />
        );
      }}
    </Field>
  );
};

const AutocompleteField = memo(AutocompleteFieldComponent) as typeof AutocompleteFieldComponent;

export default AutocompleteField;
