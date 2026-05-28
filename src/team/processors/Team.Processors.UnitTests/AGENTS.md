# Team Processors Unit Tests Notes

This file applies to `team/processors/Team.Processors.UnitTests`.

## Purpose

- Unit tests for the team processors host.

## Agent Rule

- Keep tests fast and infrastructure-free.
- Follow the unit test file shape: one test class/file per public method under test.
- Order parameters: frozen/injected constructor dependencies → `sut` → random inputs and expected values.
