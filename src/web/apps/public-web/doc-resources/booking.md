# Bookings

## Overview

A booking represents the reservation of one or more resources for a specific period of time on a single calendar day.

Bookings are the core of the Skedular platform. Every desk reservation, meeting room reservation, parking booking, equipment booking, or venue reservation is represented as a booking.

Once a booking has been confirmed, the selected resources become unavailable for other bookings during the reserved time.

---

# How Bookings Work

A booking always belongs to a single organization and is created against one or more resources.

Every booking contains:

- A booking date
- A start time
- An end time
- One or more resources
- One or more customers or users
- Booking status
- Organization information

A booking reserves the selected resources exclusively for the specified period.

---

# Single-Day Bookings

Every booking in Skedular exists within a single calendar day.

A booking **cannot span multiple days**.

For example:

✅ Monday 9:00 AM to Monday 5:00 PM

✅ Tuesday 1:00 PM to Tuesday 3:00 PM

❌ Monday 4:00 PM to Tuesday 10:00 AM

This design keeps the booking engine simple, highly performant, and capable of handling large numbers of bookings efficiently.

---

# Multi-Day Reservations

If a reservation needs to span multiple days, Skedular does not create one long booking.

Instead, it creates a **series of individual daily bookings** that are managed together as a single recurring reservation.

Examples include:

- Monthly desk memberships
- Permanent office rentals
- Weekly recurring meetings
- Long-term parking
- Ongoing room reservations

Each daily booking remains an independent booking while still belonging to the same recurring booking or subscription.

This approach provides greater flexibility if one day's booking needs to be changed without affecting the rest of the series.

---

# Resources

A booking may reserve one or more resources.

Examples include:

- A single desk
- Multiple desks
- A meeting room
- A parking space
- Equipment
- An event space

Multiple resources can be reserved within the same booking when required.

For example:

A team meeting might include:

- Meeting Room A
- Projector
- Video Conferencing Equipment

---

# Customers and Users

Each booking is associated with one or more people who will use the booked resources.

Depending on the product, these may be:

- Members
- Employees
- Customers
- Guests

The organization determines who can create bookings and who can use booked resources.

---

# Resource Availability

Once a booking is confirmed, the reserved resources become unavailable during the booked period.

The system prevents overlapping bookings for the same resource.

For example:

Desk A01 is booked:

- Monday
- 9:00 AM to 5:00 PM

Another user cannot book Desk A01 during any overlapping time.

This guarantees that the same resource cannot be double-booked.

---

# Booking Visibility

Who can view booking details depends on the organization's privacy settings.

## Private Organizations (Skedular Teams)

In private organizations, bookings are typically visible to members of the organization.

This allows employees to see who is working from the office and which resources are already booked.

Visibility is controlled by the organization's security and privacy settings.

---

## Marketplace Organizations (Skedular Spaces)

For coworking and marketplace organizations, customer privacy is prioritised.

While users can see that a resource is unavailable, they generally cannot see who made the booking.

Detailed booking information is typically available only to:

- Organization Owners
- Organization Administrators

This protects customer privacy while still preventing conflicting bookings.

---

## Host Organizations (Skedular Host)

Host organizations follow the same privacy model as marketplace organizations.

Hosts can manage and view all bookings for their own resources, while customers only see availability and their own bookings.

---

# Booking Lifecycle

A booking typically follows this process:

1. Select one or more resources.
2. Choose the booking date.
3. Select the start and end time.
4. Select the users or customers.
5. Confirm the booking.
6. The resources become unavailable for the selected period.
7. Users can later modify or cancel the booking if permitted.

---

# Booking Conflicts

Skedular continuously checks resource availability before confirming a booking.

If any selected resource is already reserved for an overlapping time, the booking cannot be completed.

This ensures resource availability always remains accurate.

If capacity changes after a customer selects a time, Skedular checks availability again while creating the booking. A marketplace booking is not partially created when the final compatible capacity has been taken. The customer receives an availability outcome and can choose another time or product.

If required payment expires or fails before confirmation, Skedular releases the affected resource capacity and retains the payment outcome. A new booking request checks current availability again.

---

# Booking Status

Depending on the workflow configured by the organization, bookings may progress through different statuses, such as:

- Pending
- Confirmed
- Cancelled
- Completed

The available statuses may vary depending on the product and booking workflow.

---

# Best Practices

For the best booking experience:

- Reserve resources only for the time they are needed.
- Cancel bookings that are no longer required.
- Use recurring bookings for long-term reservations.
- Review resource availability before creating bookings.
- Keep customer information up to date.

---

# Things to Know

- Every booking belongs to a single organization.
- A booking cannot span multiple calendar days.
- A booking may include one or more resources.
- A booking may include one or more users or customers.
- Resources cannot be double-booked for overlapping times.
- Multi-day reservations are stored as a series of daily bookings.
- Booking visibility depends on the organization's privacy settings.
- Organization owners and administrators always have access to booking management.

---

# Related Concepts

- Organizations
- Resources
- Locations
- Teams
- Customers
- Memberships
- Recurring Bookings
- Subscriptions
- Resource Availability
