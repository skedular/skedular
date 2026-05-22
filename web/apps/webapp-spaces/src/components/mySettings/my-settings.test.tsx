import { act, fireEvent, render, screen } from '@testing-library/react';
import { beforeEach, describe, expect, it, vi } from 'vitest';
import MySettings from './my-settings';

const updateCommit = vi.fn();

vi.mock('@/components/avatars', () => ({
  CustomerAvatar: () => <div>Avatar</div>,
}));

vi.mock('@/components/forms', async () => {
  const { Field } = await import('react-final-form');

  return {
    SingleChoinceTimezone: ({ name }: { name: string }) => <Field name={name}>{({ input }) => <input aria-label="Timezone" {...input} />}</Field>,
  };
});

vi.mock('@/components/loading', () => ({
  Loading: () => <div>Loading</div>,
}));

vi.mock('@/components/notification', () => ({
  errorNotificationOptions: {},
  NotificationContent: ({ content }: { content: string }) => <span>{content}</span>,
}));

vi.mock('@/components/user', async () => {
  const { Field } = await import('react-final-form');

  return {
    SingleChoiceUserPersonalInformationVisibility: ({ name }: { name: string }) => <Field name={name}>{({ input }) => <input aria-label="Visibility" {...input} />}</Field>,
  };
});

vi.mock('mui-rff', async () => {
  const { Field } = await import('react-final-form');

  return {
    makeRequired: () => ({}),
    makeValidate: () => () => ({}),
    TextField: ({ name }: { name: string }) => <Field name={name}>{({ input }) => <input aria-label={name} {...input} />}</Field>,
  };
});

vi.mock('react-toastify', () => ({
  toast: Object.assign(vi.fn(), {
    dark: vi.fn(),
  }),
}));

vi.mock('react-relay', () => ({
  graphql: (strings: TemplateStringsArray) => strings.join(''),
  useMutation: () => [updateCommit],
  usePreloadedQuery: () => ({
    me: {
      id: 'customer-1',
      email: 'customer@example.com',
      photoUrl: null,
      designation: null,
      title: null,
      name: 'Original Name',
      givenName: 'Original',
      middleName: null,
      familyName: 'Name',
      timezone: 'Pacific/Auckland',
      phoneNumber: null,
      personalInformationVisibility: { type: 'VISIBLE', name: 'Visible' },
    },
  }),
  useQueryLoader: () => [{}, vi.fn()],
}));

describe('MySettings', () => {
  beforeEach(() => {
    vi.useFakeTimers();
    updateCommit.mockReset();
  });

  it('autosaves one changed profile field after the debounce', async () => {
    render(<MySettings onReloadRequired={vi.fn()} />);

    fireEvent.change(screen.getByRole('textbox', { name: 'name' }), { target: { value: 'Updated Name' } });
    await act(async () => {
      vi.advanceTimersByTime(1000);
    });

    expect(updateCommit).toHaveBeenCalledTimes(1);
    expect(updateCommit.mock.calls[0][0].variables.input).toMatchObject({
      id: 'customer-1',
      fieldsToUpdate: ['NAME'],
      name: 'Updated Name',
    });
  });

  it('removes the manual profile update action', () => {
    render(<MySettings onReloadRequired={vi.fn()} />);

    expect(screen.queryByRole('button', { name: 'Update' })).not.toBeInTheDocument();
  });
});
