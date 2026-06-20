# Resources

## Overview

A resource is anything that can be booked in Skedular.

Resources are one of the core concepts of the platform and represent the physical assets that users or customers reserve through the booking system.

Examples of resources include:

- Desks
- Meeting rooms
- Parking spaces
- Offices
- Studios
- Event spaces
- Equipment
- Other bookable assets

Every booking in Skedular is made against one or more resources.

---

# Why Resources Exist

Resources represent the things your organization wants to make available for booking.

Once a resource is created, Skedular automatically prepares it for booking by generating its availability schedule.

Users can then reserve the resource according to its availability, booking rules, and opening hours.

---

# Product Support

Resources are currently available in:

- **Skedular Teams**
- **Skedular Spaces**
- **Skedular Host**

Although resources behave similarly across all products, how they are exposed differs.

### Skedular Teams

Resources are booked by members of a private organization.

Typical examples include:

- Desks
- Meeting rooms
- Parking spaces

---

### Skedular Spaces

Resources are connected to marketplace products using **Product Tags**.

Customers browse products rather than individual resources, and Skedular automatically allocates suitable resources based on availability.

---

### Skedular Host

Resources are created automatically when a host creates a place.

Hosts manage a simplified experience, while the booking engine still operates using resources behind the scenes.

---

# Resource Types

Every resource has a type.

Current resource types include:

- Desk
- Room
- Parking
- Other

Additional resource types may be introduced in future releases.

---

# Resource Properties

Every resource contains information such as:

- Name
- Resource type
- Capacity
- Location
- Opening hours
- Tags
- Zones
- Colour (optional)

These properties help users find the right resource and allow administrators to manage them effectively.

---

# Capacity

Capacity defines how many people or units a resource can accommodate.

Examples include:

| Resource      | Capacity |
| ------------- | -------: |
| Desk          |        1 |
| Meeting Room  |        8 |
| Training Room |       25 |
| Event Space   |      150 |

Capacity can be used when determining whether a resource is suitable for a booking.

---

# Tags

Resources may have:

- No tags
- One tag
- Multiple tags

Tags describe the characteristics or features of a resource.

Examples include:

- Standing Desk
- Dual Monitor
- Whiteboard
- Projector
- Accessible

Tags improve searching and filtering.

---

# Zones

Resources may also belong to:

- No zones
- One zone
- Multiple zones

Zones group resources by physical or logical areas.

Examples include:

- First Floor
- East Wing
- Building A

Zones help users locate resources more easily.

---

# Resource Availability

When a resource is created, Skedular automatically prepares it for booking.

Bookings can currently be made:

- In increments of **15 minutes**
- Up to **one year in advance**

Bookings cannot extend beyond the one-year booking window.

As time progresses, Skedular continuously maintains future availability so resources remain bookable.

---

# Opening Hours

By default, a resource inherits the opening hours of its location.

This means it is available whenever the location is open.

However, a resource can optionally have its own opening hours.

Resource-specific opening hours override the location's default schedule.

This allows organizations to make individual resources available at different times.

For example:

- A meeting room with its own external entrance may remain available after the office has closed.
- A parking space may be available 24 hours a day.
- Equipment may only be available during staffed hours.

This flexibility allows each resource to have availability that matches its real-world usage.

---

# Booking Resources

Resources are booked through the Skedular booking engine.

Once a booking is confirmed:

- The resource becomes unavailable for the selected period.
- Other users cannot create overlapping bookings.
- Availability is updated immediately.

This prevents double bookings and ensures accurate scheduling.

---

# Managing Resources

Resources are always managed within a location.

To manage resources:

1. Open your organization.
2. Select a location.
3. Open **Resources**.
4. Create, edit, or delete resources as required.

Resources cannot exist outside a location.

---

# Bulk Resource Creation

Skedular supports creating resources individually or in bulk.

Bulk creation is useful when setting up large workplaces such as:

- Offices with hundreds of desks
- Parking facilities
- Coworking spaces
- Large venues

This significantly reduces setup time.

---

# Deleting Resources

Resources can be removed when they are no longer required.

Deleting a resource removes it from future availability.

Historical booking information may still be retained for reporting and auditing purposes, depending on the organization's data retention policies.

---

# Best Practices

For the best experience:

- Use clear and consistent resource names.
- Choose the correct resource type.
- Set an accurate capacity.
- Apply tags and zones consistently.
- Configure resource-specific opening hours only when necessary.
- Remove unused resources to keep your workspace organised.

---

# Things to Know

- Every resource belongs to exactly one location.
- Resources cannot exist without a location.
- Every booking is made against one or more resources.
- Resources support 15-minute booking intervals.
- Resources can be booked up to one year in advance.
- Resources inherit their location's opening hours by default.
- Resources can override opening hours with their own schedule.
- Resources may have zero, one, or many tags.
- Resources may belong to zero, one, or many zones.
- Resources can be created individually or in bulk.

---

# Example

A coworking space creates a meeting room resource with the following configuration:

- Type: Meeting Room
- Name: Meeting Room 2
- Capacity: 10
- Tags:
  - Whiteboard
  - Video Conferencing
- Zone:
  - First Floor
- Opening Hours:
  - 7:00 AM to 10:00 PM

Although the building closes at 6:00 PM, the meeting room remains available because it has its own opening hours.

Customers booking the associated product can be automatically allocated this resource when it is available.

---

# Related Concepts

- Organizations
- Locations
- Bookings
- Tags
- Zones
- Opening Hours
- Products
- Product Tags
- Resource Availability
