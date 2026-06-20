# Feature Specification: Skedular Spaces Free Trial

**Feature Branch**: `030-spaces-free-trial`  
**Created**: 2026-06-27  
**Status**: Draft  
**Input**: User description: "Implement an end-to-end 14-day trial-based Free plan for Skedular Spaces only, covering backend enforcement, the Spaces app, admin and customer flows, and public website messaging without affecting paid Spaces subscriptions or Skedular Teams."

## Clarifications

### Session 2026-06-27

- Q: Which changes to existing commitments remain available after trial expiry? → A: Allow cancellation, refund, and closure actions for existing bookings or subscriptions.
- Q: What happens to an organization's public listings after trial expiry? → A: Keep listings visible, disable booking, and show neutral temporary-unavailability messaging.
- Q: Which notification channels are required for trial warnings? → A: In-app status, warnings, and blocked-state prompts only.
- Q: When does the prominent in-app expiry warning begin? → A: When 3 whole trial days remain.
- Q: When does the Spaces trial start for an older Teams-only organization? → A: On first Spaces enablement; organizations created with Spaces start at organization creation.
- Q: Does this require a new Free offering version or a customer-plan migration? → A: No. Continue using `SpacesFreeTierV1`. Apply the trial policy from the rollout date; existing Spaces Free organizations use their durable organization creation timestamp without rewriting their plan or running a customer data migration.
- Follow-up billing decision: An explicit paid upgrade provides a complimentary bridge from trial expiry through month-end. The first full-month charge occurs on the next first day of the month; no prorated partial-month charge is required, and the risk of cancellation before that first charge is accepted for this release.

## User Scenarios & Testing _(mandatory)_

### User Story 1 - Use Spaces During the Trial (Priority: P1)

As an administrator of an organization on the Skedular Spaces Free plan, I can use the existing Free plan capabilities for 14 days from the applicable one-time trial start, subject to the existing limit of 100 booking instances per month, so I can evaluate Spaces before paying.

**Why this priority**: The trial must provide a usable evaluation period before any expiry or upgrade experience has value.

**Independent Test**: Create a Spaces organization on the Free plan, perform Free plan operations throughout the 14-day period, and verify that booking volume does not shorten or extend the trial while booking creation still observes the existing monthly quota.

**Acceptance Scenarios**:

1. **Given** a newly created organization with Spaces on the Free plan, **When** its administrator first opens Spaces, **Then** the organization is in an active 14-day trial that began at organization creation.
2. **Given** an older Teams-only organization that has never enabled Spaces, **When** an administrator first enables Spaces on the Free plan, **Then** its one-time 14-day Spaces trial begins at that enablement time.
3. **Given** a Spaces organization with an active Free trial, **When** its users create or accept bookings within existing Free plan capabilities, **Then** those actions are allowed while the existing 100-booking-instance monthly quota has capacity and rejected when that quota is exceeded.
4. **Given** an organization with an active paid Spaces subscription, **When** the trial feature is released, **Then** its subscription, access, billing, and booking behavior remain unchanged.

---

### User Story 2 - Understand Trial State and Time Remaining (Priority: P1)

As a Spaces administrator, I can see that Free is a time-limited trial, its expiration date, and the remaining whole days, so I can decide whether and when to upgrade.

**Why this priority**: Clear, consistent status prevents surprise lockouts and gives administrators time to act.

**Independent Test**: View the Spaces application at representative points in the lifecycle and verify that the displayed status, remaining days, warning, expiration date, and upgrade action match the authoritative trial state.

**Acceptance Scenarios**:

1. **Given** an active trial with more than 3 whole days remaining, **When** an administrator views Spaces, **Then** the application identifies the plan as a 14-day trial and shows the expiration date and remaining days.
2. **Given** an active trial with 3 or fewer whole days remaining, **When** an administrator views Spaces, **Then** the application shows a prominent expiry warning and a clear upgrade action.
3. **Given** an expired trial, **When** an administrator views Spaces, **Then** the application shows a blocked state explaining that the trial expired and provides a clear path to upgrade.
4. **Given** any Spaces organization, **When** an authorized client requests subscription state, **Then** it receives one unambiguous current status and the trial timing information applicable to that status.

---

### User Story 3 - Enforce Expiration Without Losing Data (Priority: P1)

As a Spaces administrator whose trial has expired, I cannot perform new operational work or receive new bookings until upgrading, but I can still sign in, inspect preserved data and configuration, and access subscription and upgrade controls.

**Why this priority**: Server-enforced expiration is the commercial boundary, while preservation and upgrade access prevent data loss and account dead ends.

**Independent Test**: Expire a Free trial, attempt booking and representative operational changes through every supported client path, verify consistent rejection, then confirm that existing data remains readable and that upgrading restores access.

**Acceptance Scenarios**:

1. **Given** a Free Spaces trial at or beyond its expiration instant, **When** an administrator attempts to create or accept a new booking, **Then** the action is rejected with an expired-trial reason and upgrade guidance.
2. **Given** an expired Free Spaces trial, **When** a customer attempts to submit a new booking to that organization, **Then** the booking is not created or accepted and the customer receives a clear availability message that does not expose private subscription details.
3. **Given** an expired Free Spaces trial with published listings, **When** a customer views a listing, **Then** the listing remains visible, booking controls are disabled, and neutral temporary-unavailability messaging is shown.
4. **Given** an expired Free Spaces trial, **When** an administrator attempts another Spaces operational change, **Then** the change is blocked while read-only, data export, account, billing, upgrade, and protective cancellation, refund, or closure actions for existing commitments remain available.
5. **Given** an expired trial with existing bookings, customer records, listings, resources, products, and configuration, **When** expiry occurs, **Then** none of those records are deleted or altered solely because the trial expired.

---

### User Story 4 - Upgrade and Resume Operations (Priority: P2)

As a Spaces administrator on an active or expired trial, I can select a paid Spaces plan and regain paid access without recreating my organization or configuration.

**Why this priority**: The trial must lead to a recoverable paid conversion rather than a permanent lockout.

**Independent Test**: Upgrade an active trial and an expired trial to a paid Spaces subscription, then verify the paid status and normal operational access using the same organization data.

**Acceptance Scenarios**:

1. **Given** an active Free trial, **When** the organization activates a paid Spaces subscription, **Then** the organization immediately receives the paid subscription status and trial restrictions no longer apply.
2. **Given** an expired Free trial, **When** the organization completes an explicit paid upgrade, **Then** operational access is restored without loss of existing data or configuration and without a charge for the remaining partial month.
3. **Given** a formerly paid Spaces organization that returns to the Free plan, **When** its status is evaluated, **Then** it does not receive a new trial and is governed by its original applicable trial start.
4. **Given** an organization that upgrades partway through a calendar month, **When** the upgrade completes, **Then** the remaining portion of that month is complimentary and the first full-month charge is scheduled for the next first day of the month.
5. **Given** an organization whose trial has expired, **When** it has not completed an explicit paid upgrade, **Then** it receives no complimentary access bridge and remains blocked.
6. **Given** an upgraded organization in the complimentary bridge period, **When** it cancels before the first monthly charge, **Then** no partial-month charge is created and the accepted promotional access ends according to the cancellation rules.

---

### User Story 5 - See Accurate Public Pricing (Priority: P2)

As a prospective Spaces customer, I can see on the public website that the Free Spaces plan is a 14-day trial, so I understand the time limit and need to upgrade after evaluation.

**Why this priority**: Acquisition messaging must set the same expectation enforced by the product.

**Independent Test**: Review every public Spaces pricing and plan-discovery surface and verify that Free is consistently described as a 14-day trial while Teams pricing remains unchanged.

**Acceptance Scenarios**:

1. **Given** a visitor viewing Spaces pricing or plan messaging, **When** the Free option is presented, **Then** it is clearly labeled as a 14-day trial rather than a permanent free tier.
2. **Given** a visitor viewing Spaces trial details, **When** they review the offer, **Then** they can understand when the trial begins, how long it lasts, that the existing 100-booking monthly limit applies, and that paid access is required afterward.
3. **Given** a visitor viewing Teams pricing or subscriptions, **When** this feature is released, **Then** no Teams plan name, entitlement, price, status, or behavior has changed.

### Edge Cases

- Trial expiration occurs at the exact instant 14 elapsed 24-hour periods after the authoritative trial start; time-zone display differences must not change that instant.
- Remaining days never becomes negative. It is the number of commenced calendar-day-sized periods remaining, rounded up while the trial is active, and is `0` at and after expiry.
- Existing Free Spaces organizations older than 14 days when the feature becomes effective are immediately treated as expired; their data remains preserved and upgrade access remains available.
- Existing Free Spaces organizations younger than 14 days receive only the remainder of the 14-day period measured from original organization creation.
- A pre-existing Teams-only organization that has never enabled Spaces is not consuming trial time. Its one-time trial begins when Spaces is first enabled, and disabling then re-enabling Spaces does not reset that start time.
- Creating or accepting a booking concurrently with the expiration instant is decided using authoritative server time; requests evaluated at or after expiry are rejected.
- Direct calls, stale clients, background jobs, recurring-booking generation, imports, marketplace flows, and administrative tools cannot bypass expiry enforcement.
- Bookings and workflows already created before expiry are preserved; expiry prevents new operational changes and new booking instances but does not silently delete or rewrite historical records.
- Expired organizations may cancel, refund, or close existing bookings and subscriptions to meet existing customer obligations, but those actions must not create replacement bookings, renewals, or other new commitments.
- Failed or delayed subscription-state retrieval must not incorrectly grant trial access. Clients show a safe, retryable unavailable state while the authoritative service rejects unauthorized operational work.
- A paid subscription takes precedence over historical trial dates. A canceled, lapsed, or downgraded paid subscription does not reset the applicable trial start or grant a second trial.
- Trial expiry alone never grants complimentary access through the end of the calendar month. Access remains blocked until an explicit paid upgrade completes.
- If an upgrade completes partway through a calendar month, access resumes immediately, the remaining portion of that month is complimentary, and full-month charges begin on the next first day of the month.
- Depending on the trial start date, the 14-day trial plus the complimentary post-upgrade bridge may provide substantially more than 14 days before the first charge. Cancellation before that first charge is an explicitly accepted first-release tradeoff.
- Organizations that use Teams but do not use Spaces receive no Spaces trial effects in their Teams experience.
- Public listings remain discoverable after expiry, but all booking controls are disabled until paid access is activated; stale clients must still be rejected by authoritative enforcement.

## Requirements _(mandatory)_

### Functional Requirements

- **FR-001**: The system MUST represent the Skedular Spaces Free plan as a 14-day trial and MUST NOT represent it as a permanent free tier.
- **FR-002**: For an organization created with Spaces, the trial MUST begin at the authoritative organization creation time. For a pre-existing Teams-only organization that has never enabled Spaces, the trial MUST begin when Spaces is first enabled. In both cases, the trial MUST expire exactly 14 elapsed 24-hour periods after that one-time start.
- **FR-003**: The same organization MUST NOT receive a second Free trial because of plan changes, cancellation, reactivation, or repeated access attempts.
- **FR-004**: During an active trial, the organization MUST retain the capabilities and limits of the existing Spaces Free plan, including its monthly booking-instance quota.
- **FR-005**: Booking totals, booking-instance quotas, monthly usage, and historical usage records MUST NOT influence trial eligibility, trial status, the trial expiration instant, or remaining trial time.
- **FR-005a**: Trial booking creation and acceptance MUST continue to enforce the existing Spaces Free limit of 100 booking instances per month. This usage limit is separate from the time-based trial eligibility calculation.
- **FR-006**: At and after trial expiration, the system MUST reject all new booking creation and acceptance for the organization across administrator, customer, marketplace, recurring, import, automation, and direct service paths.
- **FR-007**: At and after trial expiration, the system MUST block Spaces operational changes while continuing to allow authentication, read-only inspection, data export, account management, billing, upgrade, and protective cancellation, refund, or closure actions for existing bookings and subscriptions. These protective actions MUST NOT create replacement bookings, renewals, or other new commitments.
- **FR-008**: Expiration MUST NOT delete, anonymize, cancel, or rewrite existing organization data, configuration, listings, resources, products, customers, subscriptions, or historical bookings solely because the trial ended.
- **FR-009**: Rejections caused by trial expiration MUST use a stable, machine-readable reason and a clear user-facing explanation that paid upgrade is required.
- **FR-010**: Customer-facing booking rejection MUST explain that the organization is temporarily unable to accept the booking without disclosing private billing or subscription details.
- **FR-010a**: Published listings for an expired organization MUST remain visible, MUST present neutral temporary-unavailability messaging, and MUST disable booking controls without exposing the organization's trial or billing state.
- **FR-011**: The authoritative subscription response available to all Spaces clients MUST expose a single current status that distinguishes at least active trial, expiring trial, expired trial, and active paid access.
- **FR-012**: For trial-based statuses, the authoritative subscription response MUST expose the trial start, trial expiration, and non-negative remaining whole days; paid statuses MUST clearly indicate that trial expiry does not restrict access.
- **FR-013**: The system MUST calculate and enforce status from authoritative time and subscription data rather than trusting client-supplied dates, cached UI state, or usage totals.
- **FR-014**: The Spaces application MUST show current subscription status and, for trial organizations, the trial expiration date and remaining days in administrator-visible account and subscription areas.
- **FR-015**: The Spaces application MUST show a prominent expiry warning when 3 or fewer whole days remain and MUST show an expired blocked state at and after expiration.
- **FR-015a**: Trial status, expiry warnings, and expired-state prompts MUST be delivered in the Spaces application; outbound email or other notification delivery is not required for this release.
- **FR-016**: Active-trial warnings and expired states MUST provide a clear upgrade action; expired users MUST be able to reach the upgrade flow without regaining operational access first.
- **FR-017**: Booking entry points in the Spaces administrator and customer experiences MUST reflect the authoritative blocked state and MUST handle server-side expiry rejection even when a page was opened before expiry.
- **FR-018**: Activating a valid paid Spaces subscription MUST take precedence over trial state and restore the paid plan's access without changing or recreating organization data.
- **FR-018a**: Activating a valid paid Spaces subscription MUST restore booking controls on preserved public listings without requiring administrators to republish them.
- **FR-018b**: A trial organization MUST explicitly select and complete a paid upgrade; trial expiry MUST NOT automatically convert the organization to a paid plan or grant complimentary access without that upgrade.
- **FR-018c**: When an organization completes an upgrade partway through a calendar month, paid-plan access MUST begin immediately and the remainder of that month MUST be complimentary, with no prorated partial-month charge.
- **FR-018d**: After a successful mid-month upgrade, the first full-month charge MUST occur on the next first day of the month and normal billing MUST continue on a calendar-month cycle.
- **FR-018e**: If the upgrade fails or remains incomplete, the organization MUST remain in the expired blocked state and MUST be able to retry the supported upgrade flow.
- **FR-018f**: If the organization cancels during the complimentary bridge before its first monthly charge, the system MUST NOT create a retroactive partial-month charge; this promotional-loss risk is accepted for the first release.
- **FR-019**: Existing paid Spaces subscriptions MUST retain their current access, plan terms, booking behavior, and billing behavior.
- **FR-020**: From the rollout date, existing Free Spaces organizations MUST be evaluated using their original organization creation time without a customer data backfill: organizations younger than 14 days receive the remaining time and organizations at least 14 days old are expired. Existing Teams-only organizations that have never enabled Spaces MUST NOT begin consuming trial time until first Spaces enablement.
- **FR-020a**: The Free trial MUST continue to use `SpacesFreeTierV1`. The implementation MUST NOT introduce a V2 Free offering code or migrate existing subscriptions to a new plan version.
- **FR-021**: The public Spaces pricing page and all public Spaces plan summaries, comparison content, calls to action, and machine-readable marketing content MUST identify Free as a 14-day trial and explain the post-trial upgrade requirement.
- **FR-022**: Public trial messaging MUST state that a new Spaces organization's trial begins when the organization is created and retains the existing Free plan limit of 100 booking instances per month during the 14 days.
- **FR-022a**: Spaces pricing and upgrade messaging MUST explain that a completed mid-month upgrade includes complimentary access through month-end and that the first full-month charge occurs on the next first day of the month.
- **FR-023**: The change MUST apply only to the Spaces product offering. Teams pricing, subscriptions, entitlements, statuses, interfaces, and marketing content MUST remain unchanged.
- **FR-024**: Trial status and enforcement MUST be consistent across administrator, customer, public booking, background, and integration flows within one minute of a status or subscription change.
- **FR-025**: Authorized support and administrative users MUST be able to identify an organization's authoritative Spaces status, start time, expiration time, and reason for blocked access without altering the trial dates.
- **FR-026**: Existing supported upgrade and paid-subscription flows MUST be reused; this feature MUST NOT introduce a separate payment or checkout model.

### Observability and Logging Requirements _(mandatory)_

- **LOG-001**: The feature MUST emit structured logs when a Free Spaces trial is established or first evaluated, when it enters the warning period, when it expires, and when paid access supersedes it.
- **LOG-002**: The feature MUST emit structured logs for allowed and blocked booking decisions at the trial boundary, including the status and stable reason code without customer or payment-sensitive payloads.
- **LOG-003**: The feature MUST emit actionable warning or error logs when trial or paid subscription state cannot be determined, propagated, or applied, including whether access was safely denied.
- **LOG-004**: Logs MUST include organization, product, request or workflow correlation, decision time, and resulting status while avoiding sensitive personal and payment data.
- **LOG-005**: Operational reporting MUST distinguish trial-expired denials from usage-limit, authorization, validation, inventory, and paid-subscription failures.

### Key Entities

- **Spaces Subscription State**: The authoritative commercial-access state for one organization's Spaces product, including current status, plan class, whether operational access is allowed, and the reason when it is not.
- **Spaces Free Trial**: The one-time evaluation period anchored to organization creation when Spaces is present at creation, or to first Spaces enablement for a pre-existing Teams-only organization, including its start, exact expiration instant, warning state, and remaining whole days.
- **Organization**: The account that owns product access, Spaces data, and configuration; its product history determines which authoritative event anchors the one-time trial.
- **Spaces Paid Subscription**: An explicitly accepted paid commercial relationship that takes precedence over trial state, may begin with a complimentary partial-month bridge, and then follows full calendar-month billing from the next first day of the month.
- **Access Decision**: The result of evaluating a requested Spaces action against the current subscription state, including an allow/deny outcome and stable reason.

### Scope Boundaries

**In scope**:

- Free-plan lifecycle and enforcement for new and existing Spaces organizations.
- Administrator and customer booking paths, representative Spaces operational actions, account visibility, expiry recovery, and support visibility.
- Public Spaces pricing and plan messaging, including search- and machine-readable marketing content.
- Compatibility behavior for existing paid Spaces organizations and explicit isolation from Teams.

**Out of scope**:

- Changes to Teams plans, trials, pricing, subscriptions, quotas, entitlements, or public messaging.
- A new checkout provider, payment model, paid-plan catalog, or paid-plan pricing change.
- Deleting organization data or automatically canceling historical bookings at trial expiry.
- Extending, pausing, resetting, or manually editing trial dates in the first release.
- Outbound trial-warning email, text-message, push-notification, or webhook delivery.
- Replacing the existing authentication, authorization, or organization-creation journeys.

## Success Criteria _(mandatory)_

### Measurable Outcomes

- **SC-001**: 100% of tested new and existing Free Spaces organizations receive the correct active, expiring, or expired status from the applicable one-time trial start at the 14-day boundary.
- **SC-002**: 100% of tested booking creation and acceptance paths enforce the existing 100-booking monthly limit during an active trial and reject new bookings at or after expiry.
- **SC-003**: 100% of tested expired organizations retain all pre-expiry data and configuration and can reach an upgrade path without support intervention.
- **SC-003a**: 100% of tested mid-month upgrades restore access without a partial-month charge and schedule the first full-month charge for the next first day of the month.
- **SC-004**: An administrator can identify trial status, expiration date, remaining days, and the upgrade action within 10 seconds of opening the Spaces account or subscription area.
- **SC-005**: Status changes caused by expiry or paid activation are reflected in all Spaces client experiences and enforcement decisions within one minute.
- **SC-006**: 100% of regression scenarios for active paid Spaces organizations show no change in access, billing, booking, or displayed paid status.
- **SC-007**: 100% of Teams pricing, subscription, entitlement, and booking regression scenarios show no behavior or messaging change attributable to this feature.
- **SC-008**: Every reviewed public Spaces plan surface describes Free as a 14-day trial, and no reviewed public Spaces surface describes it as permanently free.
- **SC-009**: In usability validation, at least 90% of participants can correctly state the trial length, when it starts, what happens at expiry, and how to upgrade after reviewing the Spaces pricing presentation.
- **SC-010**: Support can distinguish an expired-trial block from other booking failures using status and operational records in under 2 minutes.

## Assumptions

- Organization creation time and first Spaces enablement time are authoritative, durable, and available for the organizations to which each start rule applies.
- The feature becomes effective for all existing Free Spaces organizations on the chosen rollout date, with no separate migration grace period and no customer-plan or trial-date backfill.
- "Remaining days" is rounded up while any trial time remains, so a newly created organization displays 14 days and an organization with less than one day remaining displays 1 day.
- The expiring-trial presentation threshold is 3 remaining whole days.
- "Use the product" after expiry means new operational mutations are blocked; read-only access, export, account management, billing, upgrade, and protective cancellation, refund, or closure actions for existing commitments remain available to preserve data, meet customer obligations, and permit recovery.
- Existing bookings and historical workflows are preserved at expiry, but no new booking instances may be created after expiry, including instances generated by recurring or automated processes.
- Existing paid Spaces subscription and upgrade capabilities can represent all paid outcomes required by this feature.
- Spaces paid plans bill in advance on a calendar-month cycle beginning on the first day of each month; a successful mid-month upgrade receives a complimentary bridge rather than a prorated initial charge.
- The business accepts that this bridge can extend total pre-charge access significantly beyond 14 days and that an organization may cancel before its first monthly charge; abuse controls or revised proration may be considered in a future feature if the situation becomes material.
- Public customer messaging may be less specific than administrator messaging to avoid exposing an organization's private commercial state.
- Public listings remain visible after expiry so existing public configuration and discoverability are preserved, while booking controls remain disabled until paid access is active.
- The authoritative subscription owner can distinguish Spaces from Teams and can provide product-scoped state to all relevant Spaces clients.
