# Booking API Unit Tests Notes

This file applies to `booking/apis/Booking.Api.UnitTests`.

## Purpose

- Unit tests for the booking API layer.
- Fast, in-process tests that do not require running infrastructure.

## Test File Shape

- One test class/file per public method under test.
- Order test method parameters: frozen/injected constructor dependencies → `sut` → random inputs and expected values.
- Prefer injected test inputs over hardcoded strings unless testing a specific literal contract.

## Agent Rule

- Keep tests fast and infrastructure-free.
- If a test requires real infrastructure (database, Kafka, Temporal), move it to the domain integration test project.

## Unit-test construction

- Prefer `[Theory]` plus `AutoFakeItEasyData`.
- Inject constructor dependencies, the SUT, and scenario values through parameters, with dependencies before the SUT and
  values after it.
- Avoid in-method `A.Fake<T>()` when auto-data can provide the dependency; configure only required calls.
- Required services, loggers, transaction builders, mappers, and repository factories are never nullable. Do not pass
  null or add null-conditional production code to make tests work.
