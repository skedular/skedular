---
id: AddOrganizationPaymentMethod
name: Add Organization Payment Method
version: 1.0.0
summary: Business flow for processing adding a team
steps:
    - id: "add_organization_payment_method"
      title: Add Organization Payment Method
      summary: "User adds a payment method to an organization"
      actor:
        name: "User"
      next_step: "payment_service"
    - id: "payment_service"
      title: Payment Service
      service:
        id: PaymentService
        version: 0.0.1
      next_step: "organization_payment_methods_updated"          
    - id: "organization_payment_methods_updated"
      title: Organization Payment Methods Updated
      message:
        id: OrganizationPaymentMethodsUpdated
        version: 0.0.1
      next_step: "organization_service"
    - id: "organization_service"
      title: Organization Service
      service:
        id: OrganizationService
        version: 0.0.1
      next_step: "organization_upserted"
    - id: "organization_upserted"
      title: Organization Upserted
      message:
        id: OrganizationUpserted
        version: 0.0.1
      next_step: "organization_upsert_completed"
    - id: "organization_upsert_completed"
      title: Organization upsert completed
      type: node

---

### Flow of feature
<NodeGraph/>