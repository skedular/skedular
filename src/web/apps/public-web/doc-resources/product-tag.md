# Product Tags

## Overview

Product Tags are used to dynamically connect resources to products in **Skedular Spaces**.

They allow marketplace operators to group resources together without creating fixed relationships between products and individual resources.

Instead of manually selecting every resource that belongs to a product, administrators assign one or more Product Tags to resources. Products then automatically include every resource that has one of the selected Product Tags.

This makes product management significantly easier, especially for organizations with many locations and hundreds or thousands of resources.

---

# Availability

Product Tags are available **only in Skedular Spaces**.

They are designed specifically for marketplace organizations that sell bookable resources to customers.

Product Tags are **not available** in:

- Skedular Teams
- Skedular Host

---

# Why Product Tags Exist

Without Product Tags, every product would need to maintain a list of every individual resource it can sell.

As resources are added, removed, or moved between locations, every affected product would need to be updated manually.

Product Tags remove this maintenance by creating a dynamic relationship between products and resources.

Whenever resources change, products automatically reflect those changes.

---

# How Product Tags Work

Product Tags work similarly to normal Tags, but they serve a completely different purpose.

Normal Tags describe the characteristics of a resource.

Product Tags determine **which resources are available for sale through a product**.

A Product Tag acts as a dynamic link between:

- Products
- Resources

Products never directly reference individual resources.

Instead, they reference one or more Product Tags.

---

# Dynamic Resource Allocation

When a customer purchases a product, Skedular automatically searches for resources that:

- Have one of the Product Tags assigned to the product.
- Are available during the requested booking period.
- Satisfy the booking rules.

Because this relationship is dynamic, newly created resources immediately become available for booking if they have the correct Product Tag.

Likewise, removing a Product Tag immediately removes that resource from future product availability.

No changes to the product itself are required.

---

# Defining Product Tags

Product Tags are created at the organization level.

They are managed alongside:

- Tags
- Zones

Only organization owners and administrators can create or manage Product Tags.

---

# Assigning Product Tags to Resources

Resources may have:

- No Product Tags
- One Product Tag
- Multiple Product Tags

When editing a resource, administrators can assign one or more Product Tags.

Only resources with matching Product Tags are considered when a product searches for available resources.

---

# Assigning Product Tags to Products

Products reference one or more Product Tags.

When configuring a product, administrators select the Product Tags that define which resources the product may use.

A product may reference:

- One Product Tag
- Multiple Product Tags

Skedular automatically searches all matching resources when processing bookings.

---

# Cross-Location Products

One of the biggest advantages of Product Tags is that they work across locations.

For example, an organization operates three coworking spaces.

Each location contains several premium desks.

Instead of creating three different products, the administrator:

- Creates a Product Tag called **Premium Desk**.
- Assigns it to premium desks in every location.
- Creates one product that references the **Premium Desk** Product Tag.

Customers can now purchase that product, and Skedular automatically allocates an available premium desk from the appropriate location.

This significantly reduces administration while keeping products consistent.

---

# Resources Without Product Tags

Resources are **not automatically available for sale**.

If a resource has **no Product Tags**, it is ignored when products search for available resources.

This allows organizations to:

- Keep internal resources private.
- Reserve resources for staff.
- Exclude resources from marketplace products.

Only resources with matching Product Tags can be allocated by marketplace products.

---

# Product Tag Visibility

Product Tags are an internal configuration tool.

They are visible only to:

- Organization Owners
- Organization Administrators

Customers never see Product Tags during the booking process.

They interact only with products.

---

# Managing Product Tags

Administrators can:

- Create Product Tags.
- Rename Product Tags.
- Delete Product Tags.
- Assign Product Tags to resources.
- Assign Product Tags to products.

Because relationships are dynamic, changes take effect immediately.

---

# Best Practices

For the best experience:

- Create Product Tags based on the products you intend to sell.
- Use clear and descriptive names.
- Apply Product Tags consistently across locations.
- Review Product Tag assignments when adding new resources.
- Remove Product Tags from resources that should no longer be available for sale.

---

# Things to Know

- Product Tags are available only in **Skedular Spaces**.
- Product Tags are defined at the organization level.
- Product Tags are visible only to organization owners and administrators.
- Resources may have zero, one, or many Product Tags.
- Products may reference one or many Product Tags.
- Resources without Product Tags are not available through marketplace products.
- Product Tags create dynamic relationships between products and resources.
- New resources become available automatically when assigned the correct Product Tag.

---

# Product Tags vs Tags

Although they look similar, Product Tags and Tags serve very different purposes.

| Tags                                             | Product Tags                                                    |
| ------------------------------------------------ | --------------------------------------------------------------- |
| Describe the characteristics of a resource.      | Define which products can allocate a resource.                  |
| Help users search and filter resources.          | Dynamically connect resources to marketplace products.          |
| May be visible to users.                         | Internal configuration only.                                    |
| Examples: Standing Desk, Whiteboard, Accessible. | Examples: Hot Desk, Premium Desk, Private Office, Meeting Room. |

---

# Example

A coworking operator manages three locations.

Each location has:

- Standard desks.
- Premium desks.

The administrator creates two Product Tags:

- Standard Desk
- Premium Desk

Every resource is assigned the appropriate Product Tag.

Two products are then created:

- Hot Desk Day Pass → **Standard Desk**
- Premium Desk Day Pass → **Premium Desk**

When a customer purchases the Premium Desk Day Pass, Skedular automatically searches every location for an available resource with the **Premium Desk** Product Tag.

If another premium desk is added next week and assigned the same Product Tag, it immediately becomes available through the product without requiring any changes to the product configuration.

---

# Related Concepts

- Products
- Resources
- Organizations
- Tags
- Bookings
- Resource Availability
- Locations
