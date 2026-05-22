import { act, fireEvent, render, screen } from '@testing-library/react';
import type { ReactNode } from 'react';
import { toast } from 'react-toastify';
import { beforeEach, describe, expect, it, vi } from 'vitest';
import OrganizationAdminSetupSection from './organization-admin-setup-section';

const fullUpdateCommit = vi.fn();
const patchUpdateCommit = vi.fn();

const organization = {
  id: 'org-1',
  customDomain: 'acme',
  name: 'Acme',
  billingCycle: {
    type: 'MONTHLY',
    name: 'Monthly',
  },
  invoiceDueInDays: 7,
  listingMetadata: {
    about: 'Original about',
    title: 'Original title',
    subTitle: 'Original subtitle',
  },
  logoUrl: null,
  marketplaceListingMetadata: {
    about: 'Marketplace about',
    title: 'Marketplace title',
    subTitle: 'Marketplace subtitle',
    includedFeatures: [],
  },
  website: null,
  customerFacingTermsAndConditionsUrl: null,
  industrySubCategories: [],
  contactEmail: 'ops@example.com',
  contactPhone: null,
  refundNotificationEmails: [],
  featureImages: [],
};

vi.mock('@/libs/image-file-uploader', () => ({
  ImageFileUploaderWithCropper: ({ helperText, onUploadCompleted }: { helperText?: string; onUploadCompleted?: (response: unknown) => void }) => (
    <button
      type="button"
      onClick={() =>
        onUploadCompleted?.({
          id: 'file-1',
          original: { url: 'https://cdn.example.com/image.png', height: 120, width: 120 },
          thumbnail: { url: 'https://cdn.example.com/thumb.png', height: 60, width: 60 },
        })
      }
    >
      {helperText ?? 'Image uploader'}
    </button>
  ),
}));

vi.mock('@/components/loading', () => ({
  Loading: () => <div>Loading</div>,
}));

vi.mock('@/components/notification', () => ({
  errorNotificationOptions: {},
  infoNotificationOptions: {},
  NotificationContent: ({ content }: { content: string }) => <span>{content}</span>,
  successNotificationOptions: {},
}));

vi.mock('@/components/organization', async () => {
  const { Field } = await import('react-final-form');

  return {
    OrganizationMultipleChoicesIndustries: ({ name }: { name: string }) => (
      <Field name={name}>
        {({ input }) => (
          <button type="button" onClick={() => input.onChange(['industry-1'])}>
            Select industry
          </button>
        )}
      </Field>
    ),
  };
});

vi.mock('mui-rff', async () => {
  const { Field } = await import('react-final-form');

  const labels: Record<string, string> = {
    about: 'About',
    contactEmail: 'Email',
    contactPhone: 'Phone Number',
    customerFacingTermsAndConditionsUrl: 'Terms and Conditions',
    name: 'Name',
    refundNotificationEmailsText: 'Refund Notification Emails',
    subTitle: 'Sub Title',
    title: 'Title',
    website: 'Website',
  };

  return {
    makeRequired: () => ({}),
    makeValidate: () => () => ({}),
    TextField: ({ helperText, name, onBlur, slotProps }: { helperText?: ReactNode; name: string; onBlur?: () => void; slotProps?: { input?: { endAdornment?: ReactNode } } }) => (
      <Field name={name}>
        {({ input }) => (
          <>
            <input
              aria-label={labels[name] ?? name}
              {...input}
              onBlur={(event) => {
                input.onBlur(event);
                onBlur?.();
              }}
            />
            {slotProps?.input?.endAdornment}
            {helperText}
          </>
        )}
      </Field>
    ),
  };
});

vi.mock('react-toastify', () => ({
  toast: Object.assign(vi.fn(), {
    dark: vi.fn(),
    update: vi.fn(),
  }),
}));

vi.mock('next/navigation', () => ({
  usePathname: () => '/organizations/acme/admin',
  useRouter: () => ({
    replace: vi.fn(),
  }),
}));

vi.mock('react-relay', () => ({
  graphql: (strings: TemplateStringsArray) => strings.join(''),
  useMutation: (mutation: string) => [mutation.includes('updateOrganization') ? patchUpdateCommit : fullUpdateCommit],
  usePreloadedQuery: () => ({
    emailsToShowLatestCapabilities: [],
    me: {
      id: 'customer-1',
      emails: ['ops@example.com'],
    },
    organizationIndustryMainCategoriesReferences: [],
    organization,
  }),
  useQueryLoader: () => [{}, vi.fn()],
}));

describe('OrganizationAdminSetupSection', () => {
  beforeEach(() => {
    vi.useFakeTimers();
    fullUpdateCommit.mockReset();
    patchUpdateCommit.mockReset();
    vi.mocked(toast).mockClear();
    vi.mocked(toast.dark).mockClear();
    vi.mocked(toast.update).mockClear();
  });

  it('patches the name after inline debounce', async () => {
    render(<OrganizationAdminSetupSection organizationCustomDomain="acme" />);

    fireEvent.change(screen.getByRole('textbox', { name: 'Name' }), { target: { value: 'Acme Labs' } });
    await act(async () => {
      vi.advanceTimersByTime(1000);
    });

    expect(patchUpdateCommit).toHaveBeenCalledTimes(1);
    expect(patchUpdateCommit.mock.calls[0][0].variables.input).toMatchObject({
      id: 'org-1',
      fieldsToUpdate: ['NAME'],
      name: 'Acme Labs',
    });
    expect(patchUpdateCommit.mock.calls[0][0].variables.input.description).toBeUndefined();
  });

  it('flushes the pending name patch on blur', async () => {
    render(<OrganizationAdminSetupSection organizationCustomDomain="acme" />);

    const name = screen.getByRole('textbox', { name: 'Name' });
    fireEvent.change(name, { target: { value: 'Acme HQ' } });
    await act(async () => {
      fireEvent.blur(name);
    });

    expect(patchUpdateCommit).toHaveBeenCalledTimes(1);
    expect(patchUpdateCommit.mock.calls[0][0].variables.input).toMatchObject({
      fieldsToUpdate: ['NAME'],
      name: 'Acme HQ',
    });
  });

  it('patches the description from the About field after inline debounce', async () => {
    render(<OrganizationAdminSetupSection organizationCustomDomain="acme" />);

    fireEvent.change(screen.getByRole('textbox', { name: 'About' }), { target: { value: 'Updated about text' } });
    await act(async () => {
      vi.advanceTimersByTime(1000);
    });

    expect(patchUpdateCommit).toHaveBeenCalledTimes(1);
    expect(patchUpdateCommit.mock.calls[0][0].variables.input).toMatchObject({
      id: 'org-1',
      fieldsToUpdate: ['DESCRIPTION'],
      description: 'Updated about text',
    });
    expect(patchUpdateCommit.mock.calls[0][0].variables.input.name).toBeUndefined();
  });

  it('patches the website after inline debounce', async () => {
    render(<OrganizationAdminSetupSection organizationCustomDomain="acme" />);

    fireEvent.change(screen.getByRole('textbox', { name: 'Website' }), { target: { value: 'https://acme.example.com' } });
    await act(async () => {
      vi.advanceTimersByTime(1000);
    });

    expect(patchUpdateCommit).toHaveBeenCalledTimes(1);
    expect(patchUpdateCommit.mock.calls[0][0].variables.input).toMatchObject({
      id: 'org-1',
      fieldsToUpdate: ['WEBSITE'],
      website: 'https://acme.example.com',
    });
  });

  it('does not patch an invalid website', async () => {
    render(<OrganizationAdminSetupSection organizationCustomDomain="acme" />);

    fireEvent.change(screen.getByRole('textbox', { name: 'Website' }), { target: { value: 'not-a-url' } });
    await act(async () => {
      vi.advanceTimersByTime(1000);
    });

    expect(patchUpdateCommit).not.toHaveBeenCalled();
  });

  it('patches the terms URL after inline debounce', async () => {
    render(<OrganizationAdminSetupSection organizationCustomDomain="acme" />);

    fireEvent.change(screen.getByRole('textbox', { name: 'Terms and Conditions' }), { target: { value: 'https://acme.example.com/terms' } });
    await act(async () => {
      vi.advanceTimersByTime(1000);
    });

    expect(patchUpdateCommit).toHaveBeenCalledTimes(1);
    expect(patchUpdateCommit.mock.calls[0][0].variables.input).toMatchObject({
      id: 'org-1',
      fieldsToUpdate: ['CUSTOMER_FACING_TERMS_AND_CONDITIONS_URL'],
      customerFacingTermsAndConditionsUrl: 'https://acme.example.com/terms',
    });
  });

  it('does not patch an invalid contact email', async () => {
    render(<OrganizationAdminSetupSection organizationCustomDomain="acme" />);

    fireEvent.change(screen.getByRole('textbox', { name: 'Email' }), { target: { value: 'invalid-email' } });
    await act(async () => {
      vi.advanceTimersByTime(1000);
    });

    expect(patchUpdateCommit).not.toHaveBeenCalled();
  });

  it('patches an empty contact email', async () => {
    render(<OrganizationAdminSetupSection organizationCustomDomain="acme" />);

    fireEvent.change(screen.getByRole('textbox', { name: 'Email' }), { target: { value: '' } });
    await act(async () => {
      vi.advanceTimersByTime(1000);
    });

    expect(patchUpdateCommit).toHaveBeenCalledTimes(1);
    expect(patchUpdateCommit.mock.calls[0][0].variables.input).toMatchObject({
      id: 'org-1',
      fieldsToUpdate: ['CONTACT_EMAIL'],
      contactEmail: null,
    });
  });

  it('patches the selected industries after inline debounce', async () => {
    render(<OrganizationAdminSetupSection organizationCustomDomain="acme" />);

    fireEvent.click(screen.getByRole('button', { name: 'Select industry' }));
    await act(async () => {
      vi.advanceTimersByTime(1000);
    });

    expect(patchUpdateCommit).toHaveBeenCalledTimes(1);
    expect(patchUpdateCommit.mock.calls[0][0].variables.input).toMatchObject({
      id: 'org-1',
      fieldsToUpdate: ['INDUSTRY_SUB_CATEGORIES'],
      industrySubCategoryIds: ['industry-1'],
    });
  });

  it('does not patch invalid short names', () => {
    render(<OrganizationAdminSetupSection organizationCustomDomain="acme" />);

    fireEvent.change(screen.getByRole('textbox', { name: 'Name' }), { target: { value: 'A' } });
    vi.advanceTimersByTime(1000);

    expect(patchUpdateCommit).not.toHaveBeenCalled();
  });

  it('does not render the old manual update button', () => {
    render(<OrganizationAdminSetupSection organizationCustomDomain="acme" />);

    expect(screen.queryByRole('button', { name: 'Update' })).not.toBeInTheDocument();
  });

  it('patches the logo when the logo upload completes', async () => {
    render(<OrganizationAdminSetupSection organizationCustomDomain="acme" />);

    await act(async () => {
      fireEvent.click(screen.getByRole('button', { name: 'Upload a square logo or icon for organization branding.' }));
    });

    expect(patchUpdateCommit).toHaveBeenCalledTimes(1);
    expect(patchUpdateCommit.mock.calls[0][0].variables.input).toMatchObject({
      id: 'org-1',
      fieldsToUpdate: ['LOGO_URL'],
      logoUrl: 'https://cdn.example.com/image.png',
    });
  });

  it('patches feature images when an upload completes', async () => {
    render(<OrganizationAdminSetupSection organizationCustomDomain="acme" />);

    await act(async () => {
      fireEvent.click(screen.getByRole('button', { name: 'Image uploader' }));
    });

    expect(patchUpdateCommit).toHaveBeenCalledTimes(1);
    expect(patchUpdateCommit.mock.calls[0][0].variables.input).toMatchObject({
      id: 'org-1',
      fieldsToUpdate: ['FEATURE_IMAGES'],
      featureImages: [
        {
          original: { url: 'https://cdn.example.com/image.png', height: 120, width: 120 },
          thumbnail: { url: 'https://cdn.example.com/thumb.png', height: 60, width: 60 },
        },
      ],
    });
  });

  it('saves silently without any toast while the name patch is in flight', async () => {
    render(<OrganizationAdminSetupSection organizationCustomDomain="acme" />);

    fireEvent.change(screen.getByRole('textbox', { name: 'Name' }), { target: { value: 'Acme Labs' } });
    await act(async () => {
      vi.advanceTimersByTime(1000);
    });

    expect(toast).not.toHaveBeenCalled();

    await act(async () => {
      patchUpdateCommit.mock.calls[0][0].onCompleted({ updateOrganization: { organization } }, undefined);
    });

    expect(toast).not.toHaveBeenCalled();
  });

  it('shows an error toast when an inline patch fails', async () => {
    render(<OrganizationAdminSetupSection organizationCustomDomain="acme" />);

    fireEvent.change(screen.getByRole('textbox', { name: 'Name' }), { target: { value: 'Acme Labs' } });
    await act(async () => {
      vi.advanceTimersByTime(1000);
    });

    await act(async () => {
      patchUpdateCommit.mock.calls[0][0].onError(new Error('Patch failed'));
    });

    expect(screen.queryByLabelText('Saving')).not.toBeInTheDocument();
    expect(toast).toHaveBeenCalledTimes(1);
    expect(vi.mocked(toast).mock.calls[0][0]).toMatchObject({
      props: { content: "We couldn't update organisation 'Acme'. Patch failed" },
    });
  });
});
