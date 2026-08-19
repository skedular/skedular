import ProductEditorForm from '@/components/product/product-editor-form';
import { render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import type { ReactNode } from 'react';
import { describe, expect, it, vi } from 'vitest';

vi.mock('@skedular/ui', () => ({
  // component stubs
  BodyIconTypography: ({ label }: { label: string }) => <div>{label}</div>,
  DurationInput: ({ label, value }: { label: string; value?: string }) => (
    <div>
      {label}: {value}
    </div>
  ),
  FormFieldLabel: ({ label, children }: { label?: string; children: ReactNode }) => (
    <div>
      {label ? <div>{label}</div> : null}
      {children}
    </div>
  ),
  FormStackColumn: ({ children, onSubmit }: { children: ReactNode; onSubmit?: (event?: unknown) => void }) => <form onSubmit={onSubmit as never}>{children}</form>,
  LeadIconTypography: ({ label }: { label: string }) => <div>{label}</div>,
  SectionIconTypography: ({ label }: { label: string }) => <div>{label}</div>,
  SmallIconTypography: ({ label }: { label: string }) => <div>{label}</div>,
  StackColumn: ({ children }: { children: ReactNode }) => <div>{children}</div>,
  StackRow: ({ children }: { children: ReactNode }) => <div>{children}</div>,
  GuidedEditorProgress: ({
    steps,
    activeStepId,
    onStepChange,
  }: {
    steps: Array<{ id: string; title: ReactNode }>;
    activeStepId: string;
    onStepChange: (stepId: string) => void;
  }) => (
    <div>
      {steps.map((step) => (
        <button key={step.id} type="button" aria-pressed={activeStepId === step.id} onClick={() => onStepChange(step.id)}>
          {step.title}
        </button>
      ))}
    </div>
  ),
  PageHeaderPanel: ({ children, title }: { children: ReactNode; title: ReactNode }) => (
    <section>
      <h1>{title}</h1>
      {children}
    </section>
  ),
  SettingsSectionCard: ({ children, title }: { children: ReactNode; title: ReactNode }) => (
    <section>
      <h2>{title}</h2>
      {children}
    </section>
  ),
  StickyReviewRail: ({ children, title }: { children: ReactNode; title: ReactNode }) => (
    <aside>
      <h2>{title}</h2>
      {children}
    </aside>
  ),
  // theme constants stubs
  defaultButtonStyle: {},
  defaultPadding: 2,
  defaultGridActionPadding: 1,
  defaultGridStyle: {},
  coal: '#000',
  sandstone: '#f5f5f5',
  emerald: '#00a86b',
}));

vi.mock('@/components/listingMetadata', () => ({
  ListingMetadata: () => <div>Listing metadata</div>,
}));

vi.mock('@/components/organization', () => ({
  MultipleChoicesPaymentMethodTypes: () => <div>Payment methods</div>,
  MultipleChoicesProductTags: () => <div>Product tags</div>,
  SingleChoiceCurrency: () => <div>Currency</div>,
  SingleChoiceProductPricingBillingMode: () => <div>Billing mode</div>,
  SingleChoiceProductPricingCadence: () => <div>Cadence</div>,
  SingleChoiceProductPricingCancellationType: () => <div>Cancellation type</div>,
  SingleChoiceProductType: () => <div>Product type</div>,
}));

vi.mock('@/components/organization/multiple-choices-amenities', () => ({
  default: () => <div>Amenities</div>,
}));

vi.mock('@/libs/image-file-uploader', () => ({
  ImageFileUploaderWithCropper: () => <div>Uploader</div>,
}));

vi.mock('mui-rff', () => ({
  Switches: () => <div>Switches</div>,
  TextField: () => <input />,
}));

const baseProps = {
  onSubmit: vi.fn(),
  rootDataRelay: {
    defaultMaxAllowedResourcesLockTimePaidViaCard: 60,
    defaultMaxAllowedResourcesLockTimePaidViaBankTransfer: 1440,
  },
  values: {
    title: 'Hot desk pass',
    subTitle: 'Flexible workspace access',
    includedFeatures: null,
    type: 'DESK',
    currency: 'NZD',
    productTagIds: ['tag-1'],
    amenityIds: ['amenity-1'],
    pricingOptions: [
      {
        id: 'offer-1',
        title: 'Daily pass',
        subTitle: null,
        cadence: 'ONE_TIME',
        price: '25',
        numberOfResourcesToBook: '1',
        minDurationMinutes: '60',
        maxDurationMinutes: '480',
        cancellationPolicyType: 'NO_CANCELLATION',
        cancellationRefundRules: [],
        isTaxInclusive: true,
        supportsSubscriptionAutoRenewal: false,
        maxAllowedResourcesLockTimePaidViaCard: '60',
        maxAllowedResourcesLockTimePaidViaBankTransfer: '1',
        billingMode: 'UPFRONT',
        acceptedPaymentMethods: ['CARD'],
        availableDays: [],
        requiredDaysPerWeek: '',
      },
    ],
  },
  errors: {},
  form: { change: vi.fn() },
  requiredFields: {},
  organizationCustomDomain: 'example',
  featureImages: [],
  primaryFeatureImage: null,
  onUploadCompleted: vi.fn(),
  onRemoveFeatureImage: vi.fn(),
  onSetPrimaryFeatureImage: vi.fn(),
  paletteMode: 'light' as const,
};

describe('ProductEditorForm', () => {
  it('uses review and create as the final add-product step and only shows submit there', async () => {
    const user = userEvent.setup();

    render(<ProductEditorForm {...baseProps} mode="add" />);

    expect(screen.getByRole('button', { name: 'Review & Create' })).toBeInTheDocument();
    expect(screen.queryByRole('button', { name: 'Create' })).not.toBeInTheDocument();
    expect(screen.queryByRole('button', { name: 'Back' })).not.toBeInTheDocument();

    await user.click(screen.getByRole('button', { name: 'Review & Create' }));

    expect(screen.getAllByText('Review & Create')).not.toHaveLength(0);
    expect(screen.getByText('Check the high-level shape before creating the product. This is the compact product story people need to understand.')).toBeInTheDocument();
    expect(screen.getByRole('button', { name: 'Create' })).toBeInTheDocument();
    expect(screen.queryByRole('button', { name: 'Back' })).not.toBeInTheDocument();
  });

  it('does not show a review step or update button in edit mode', () => {
    render(<ProductEditorForm {...baseProps} mode="edit" />);

    // Edit mode: review step removed entirely — autosave handles updates
    expect(screen.queryByText('Review & Update')).not.toBeInTheDocument();
    expect(screen.getByRole('button', { name: 'Basics' })).toBeInTheDocument();
    expect(screen.getByRole('button', { name: 'Offers' })).toBeInTheDocument();
    expect(screen.queryByRole('button', { name: 'Update' })).not.toBeInTheDocument();
    expect(screen.queryByRole('button', { name: 'Back' })).not.toBeInTheDocument();
  });
});
