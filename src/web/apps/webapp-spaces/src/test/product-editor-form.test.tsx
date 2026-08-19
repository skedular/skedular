import ProductEditorForm from '@/components/product/product-editor-form';
import { render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import type { ReactNode } from 'react';
import React from 'react';
import { describe, expect, it, vi } from 'vitest';

vi.mock('next/image', () => ({ default: (props: React.ComponentProps<'img'>) => React.createElement('img', { ...props, alt: props.alt ?? '' }) }));
vi.mock('@/components/icons', () => ({ DeleteIcon: () => <span data-testid="delete-icon" /> }));

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
  ImageFileUploaderWithCropper: ({ onUploadCompleted }: { onUploadCompleted: (response: unknown) => void }) => (
    <button type="button" onClick={() => onUploadCompleted({ original: { url: 'https://example.test/uploaded.png' } })}>
      Uploader
    </button>
  ),
}));

vi.mock('mui-rff', () => ({
  Switches: () => <div>Switches</div>,
  TextField: ({ name, placeholder }: { name?: string; placeholder?: string }) => <input name={name} placeholder={placeholder} />,
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
        fulfillmentType: 'RESERVATION',
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
  it('shows the modern basics presentation and customer preview', async () => {
    const user = userEvent.setup();
    const featureImage = { original: { url: 'https://example.test/cover.png' }, thumbnail: { url: 'https://example.test/thumb.png' } };

    render(<ProductEditorForm {...baseProps} mode="edit" featureImages={[featureImage]} primaryFeatureImage={featureImage} />);

    expect(screen.getByText('Product basics')).toBeInTheDocument();
    expect(screen.getByText('Product presentation')).toBeInTheDocument();
    expect(screen.getByAltText('Product cover')).toBeInTheDocument();
    expect(screen.getByText('Customer-facing details')).toBeInTheDocument();
    expect(screen.getByRole('heading', { name: 'Product preview' })).toBeInTheDocument();
    expect(screen.getByAltText('Product preview')).toBeInTheDocument();

    await user.click(screen.getByRole('button', { name: /Classification/ }));
    expect(screen.getByRole('button', { name: /Classification/ })).toHaveAttribute('aria-expanded', 'true');
  });

  it('supports cover image actions and the styled upload action', async () => {
    const user = userEvent.setup();
    const onUploadCompleted = vi.fn();
    const onRemoveFeatureImage = vi.fn();
    const onSetPrimaryFeatureImage = vi.fn();
    const firstImage = { original: { url: 'https://example.test/one.png' }, thumbnail: { url: 'https://example.test/one-thumb.png' } };
    const secondImage = { original: { url: 'https://example.test/two.png' }, thumbnail: { url: 'https://example.test/two-thumb.png' } };

    render(
      <ProductEditorForm
        {...baseProps}
        mode="edit"
        featureImages={[firstImage, secondImage]}
        primaryFeatureImage={firstImage}
        onUploadCompleted={onUploadCompleted}
        onRemoveFeatureImage={onRemoveFeatureImage}
        onSetPrimaryFeatureImage={onSetPrimaryFeatureImage}
      />,
    );

    await user.click(screen.getByRole('button', { name: 'Uploader' }));
    expect(onUploadCompleted).toHaveBeenCalledWith({ original: { url: 'https://example.test/uploaded.png' } });

    await user.click(screen.getByRole('button', { name: 'Remove cover image' }));
    expect(onRemoveFeatureImage).toHaveBeenCalledWith(firstImage);

    await user.click(screen.getByRole('button', { name: 'Make image the cover' }));
    expect(onSetPrimaryFeatureImage).toHaveBeenCalledWith(secondImage);
  });

  it('adds a one-time offer with one click', async () => {
    const user = userEvent.setup();
    const change = vi.fn();

    render(<ProductEditorForm {...baseProps} mode="edit" form={{ change }} />);
    await user.click(screen.getByRole('button', { name: 'Offers' }));
    await user.click(screen.getByRole('button', { name: 'Add offer' }));

    const nextPricingOptions = change.mock.calls.at(-1)?.[1] as Array<{ cadence: string }>;
    expect(nextPricingOptions).toHaveLength(2);
    expect(nextPricingOptions[1]?.cadence).toBe('ONE_TIME');
  });

  it('duplicates and removes offers, while protecting the final offer', async () => {
    const user = userEvent.setup();
    const secondOffer = { ...baseProps.values.pricingOptions[0], id: 'offer-2', title: 'Weekly pass', cadence: 'WEEKLY' };
    const change = vi.fn();

    const { unmount } = render(
      <ProductEditorForm {...baseProps} mode="edit" form={{ change }} values={{ ...baseProps.values, pricingOptions: [baseProps.values.pricingOptions[0], secondOffer] }} />,
    );
    await user.click(screen.getByRole('button', { name: 'Offers' }));
    await user.click(screen.getByRole('button', { name: 'Actions for Daily pass' }));
    await user.click(screen.getByRole('menuitem', { name: 'Duplicate offer' }));

    const duplicatedOptions = change.mock.calls.at(-1)?.[1] as Array<{ id: string; title: string }>;
    expect(duplicatedOptions).toHaveLength(3);
    expect(duplicatedOptions[2]?.id).not.toBe('offer-1');
    expect(duplicatedOptions[2]?.title).toBe('Daily pass copy');

    change.mockClear();
    await user.click(screen.getByRole('button', { name: 'Actions for Daily pass' }));
    await user.click(screen.getByRole('menuitem', { name: 'Remove offer' }));
    expect(change).toHaveBeenCalledWith('pricingOptions', [secondOffer]);

    unmount();
    const singleOfferChange = vi.fn();
    render(<ProductEditorForm {...baseProps} mode="edit" form={{ change: singleOfferChange }} />);
    await user.click(screen.getByRole('button', { name: 'Offers' }));
    await user.click(screen.getByRole('button', { name: 'Actions for Daily pass' }));
    expect(screen.getByRole('menuitem', { name: 'Remove offer' })).toHaveAttribute('aria-disabled', 'true');
  });

  it('switches between the offer sections and explains cadence separately from auto-renewal', async () => {
    const user = userEvent.setup();

    render(
      <ProductEditorForm
        {...baseProps}
        mode="edit"
        values={{
          ...baseProps.values,
          pricingOptions: [{ ...baseProps.values.pricingOptions[0], fulfillmentType: 'RESERVATION' }],
        }}
      />,
    );
    await user.click(screen.getByRole('button', { name: 'Offers' }));

    await user.click(screen.getByRole('button', { name: /Fulfillment/ }));
    expect(screen.getByText(/Choose One time for a single purchase/)).toBeVisible();

    await user.click(screen.getByRole('button', { name: /Booking rules/ }));
    expect(screen.getByRole('button', { name: /Booking rules/ })).toHaveAttribute('aria-expanded', 'true');

    await user.click(screen.getByRole('button', { name: /Payments/ }));
    expect(screen.getByRole('button', { name: /Payments/ })).toHaveAttribute('aria-expanded', 'true');

    await user.click(screen.getByRole('button', { name: /Cancellation/ }));
    expect(screen.getByRole('button', { name: /Cancellation/ })).toHaveAttribute('aria-expanded', 'true');

    await user.click(screen.getByRole('button', { name: /Advanced/ }));
    expect(screen.getByRole('button', { name: /Advanced/ })).toHaveAttribute('aria-expanded', 'true');
  });

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
