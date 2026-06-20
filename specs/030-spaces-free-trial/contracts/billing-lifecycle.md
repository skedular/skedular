# Contract: Spaces Trial Upgrade and Calendar-Month Billing

## Business rule

- Trial expiry does not automatically create a paid subscription.
- An authorized administrator explicitly chooses a paid Spaces plan.
- Chargeable plan activation requires an attached supported payment method.
- Successful upgrade restores paid-plan access immediately.
- The interval from successful mid-month upgrade until the next first day of the month is complimentary.
- No prorated or retroactive charge is created for that interval.
- The first full monthly charge occurs on the next first day of the month and is associated with the upcoming full calendar-month offering, not the complimentary bridge interval.
- Subsequent charging follows existing calendar-month renewal.

## Transition contract

```text
TrialActive | TrialExpiring | TrialExpired
  -- explicit paid upgrade, validation succeeds --> ComplimentaryBridge

ComplimentaryBridge
  -- first boundary charge succeeds --> PaidActive
  -- canceled before boundary --> original Free trial status (normally TrialExpired)
  -- transition/workflow setup fails --> prior status; no partial update

PaidActive
  -- existing monthly renewal succeeds --> PaidActive
  -- existing cancellation/downgrade --> original Free trial status
```

## Transactional requirements

- Offering replacement/update, trial-anchor initialization, complimentary-bridge marker/billing-start persistence, old workflow cancellation, new workflow scheduling, and Organization outbox publication occur atomically through existing outbox patterns.
- Retried upgrade requests are idempotent and do not schedule duplicate workflows.
- The plan transition path used by `updateOrganizationOffering` and `updateOrganizationSpacesSubscription` must converge on the same domain operation.
- A failed upgrade leaves the prior offering and access status authoritative.
- The first-boundary transition uses an idempotent upcoming-offering/payment key, charges the upcoming full period, persists/activates that period through the successful transition path, and never charges the bridge row.
- Existing paid subscriptions that did not originate from a complimentary bridge retain the existing renewal workflow and record association unchanged.

## Cancellation tradeoff

Cancellation during the complimentary bridge:

- stops the scheduled first charge/renewal;
- creates no retroactive partial-month charge;
- does not reset the trial anchor;
- returns the organization to expired Free access when its 14 days have elapsed;
- is an explicitly accepted promotional-loss risk for this release.

## Existing paid subscriptions

- Existing effective paid offerings are not migrated, re-anchored, repriced, or rescheduled.
- Growth/Business/Contact Us prices, discounts, quota/capacity, and current renewal dates remain unchanged.
- The new status mapper reports them as paid/legacy without applying Free-trial expiry.

## Failure behavior

- Missing payment method: reject upgrade before state mutation and return existing `PaymentMethodRequired` semantics.
- Workflow scheduling/outbox failure: roll back transition.
- First boundary charge failure: use the existing retry policy and idempotency protections; do not activate a successful upcoming paid period until charging succeeds.
- Exhausted payment retries: do not silently grant an indefinite paid period; expose inactive/recovery status and actionable operator logs while preserving all organization data.

## Observability

Log upgrade requested/completed/rejected, bridge start/end, first billing date, workflow identifiers, charge attempt outcome, renewal outcome, cancellation, retry exhaustion, organization ID, offering ID, plan code, and correlation context. Do not log card/payment secrets.
