# Teams Help Source Inventory

## Existing Help Shell

The existing Teams help shell used the same generic Skedular copy as the other help apps. It did not explain private organization work, private bookings, teams, members, integrations, SSO, analytics, or the difference between Teams and Spaces. Replace it with Teams-specific help.

Baseline files already existed:

- `src/web/apps/webapp-teams-help/src/app/page.mdx`
- `src/web/apps/webapp-teams-help/src/content/index.mdx`
- `src/web/apps/webapp-teams-help/src/content/_meta.ts`

## Reviewed Product Surfaces

| Product surface                        | Route or source area                                              | Help mapping                     |
| -------------------------------------- | ----------------------------------------------------------------- | -------------------------------- |
| Teams entry                            | `/`, `/organizations/[organizationCustomDomain]`                  | Overview, Organization admin     |
| Add private organization               | `/organizations/add-private`                                      | Organization admin, Teams guides |
| Admin                                  | `/organizations/[organizationCustomDomain]/admin`                 | Organization admin               |
| Bookings list/add/detail               | `/bookings`, `/bookings/add`, `/bookings/[bookingId]`             | Bookings, locations, resources   |
| Locations list/add/detail              | `/locations`, `/locations/add-private`, `/locations/[locationId]` | Bookings, locations, resources   |
| Resources add                          | `/resources/add`                                                  | Bookings, locations, resources   |
| Teams list/add/detail                  | `/teams`, `/teams/add`, `/teams/[teamId]`                         | People, settings, integrations   |
| Users list/detail                      | `/users`, `/users/[customerId]`                                   | People, settings, integrations   |
| Availability dashboard                 | `/availability`                                                   | Analytics, availability, SSO     |
| Analytics                              | `/analytics`                                                      | Analytics, availability, SSO     |
| SSO sign-in                            | `/sso-signin`                                                     | Analytics, availability, SSO     |
| Notifications                          | `/notifications`                                                  | People, settings, integrations   |
| Settings                               | `/settings`                                                       | People, settings, integrations   |
| Slack install/success                  | `/install-slack`, `/slack-success-install`                        | People, settings, integrations   |
| Microsoft Teams entry/settings/install | `/msteams/*`                                                      | People, settings, integrations   |
| Auth and welcome                       | `/signin`, `/signup`, `/auth/*`, `/callback`, `/welcome`          | Organization admin, People       |

## Important States To Explain

- Teams is for private organization work, not public marketplace selling.
- A user may need the right organization access before seeing bookings, teams, locations, or analytics.
- SSO and Microsoft Teams flows depend on organization configuration.
- Analytics and availability dashboards summarize internal workplace usage.

## Coverage Table

| Help page                          | Covers                                                                                                               | Remaining gap                                |
| ---------------------------------- | -------------------------------------------------------------------------------------------------------------------- | -------------------------------------------- |
| `index.mdx`                        | Purpose, audience, boundaries                                                                                        | None                                         |
| `organization-admin.mdx`           | Entry, add private org, admin, auth/welcome                                                                          | Exact invitation/permission labels           |
| `bookings-locations-resources.mdx` | Private bookings, locations, resources, floor plans/zones                                                            | Detail of every resource field               |
| `people-settings-integrations.mdx` | Teams, users, notifications, settings, Slack, Microsoft Teams                                                        | Provider-specific install failure copy       |
| `page-reference.mdx`               | Page-by-page Teams reference for all major private organization surfaces                                             | None                                         |
| `admin-examples.mdx`               | Practical admin examples for offices, rooms, access, availability, SSO, Microsoft Teams, and resource naming         | None                                         |
| `analytics-availability-sso.mdx`   | Analytics, availability, SSO                                                                                         | Exact metric definitions need product review |
| `access-and-permissions.mdx`       | Sign-in, organization membership, roles, SSO, visibility, admin/member access                                        | Exact permission matrix                      |
| `actions-reference.mdx`            | Teams action reference for organization, location, resource, booking, team, user, integration, and analytics actions | None                                         |
| `troubleshooting.mdx`              | Access, booking, location, resource, Microsoft Teams, Slack, analytics issue checks                                  | Provider-specific failures                   |
| `support-handoff.mdx`              | Teams support templates and triage notes                                                                             | None                                         |
| `screenshot-plan.mdx`              | Screenshot capture rules and required Teams screenshot list                                                          | Screenshots not captured in this slice       |
| `review-qa.mdx`                    | Product/support/engineering/copy review matrix                                                                       | None                                         |
| `glossary.mdx`                     | Private organization definitions for shared terms                                                                    | None                                         |
| `review-checklists.mdx`            | Private organization, location, resource, booking, access, and integration checks                                    | None                                         |
| `faq.mdx`                          | Common Teams questions                                                                                               | None                                         |
| `teams-guides.mdx`                 | Step-by-step private organization workflows                                                                          | Screenshots needed                           |
| `content-gaps.mdx`                 | Known unclear flows                                                                                                  | Tracked in gap register                      |
