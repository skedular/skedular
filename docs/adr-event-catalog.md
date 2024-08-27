# ADR: Manual vs. Tool-Assisted Documentation with EventCatalog

## Status
Decided

## Date
2024-08-26

## Context
Our project involves building an event-driven architecture with multiple domains and services communicating through events. We need to choose a strategy for documenting these events, balancing between manual documentation and tool-assisted approaches like EventCatalog.

## Decision
We will use EventCatalog as a tool to assist in the documentation of our event-driven architecture. This decision prioritizes a structured and accessible documentation process while reducing manual effort through available utilities. eventCatalog offers some integrations and new features in near future as well

## Details

### Assumptions
- Our event-driven system is and will grow more in complexity over time.
- The team is comfortable using tools like EventCatalog.

### Constraints
The selected tool should offer value without requiring significant changes to the existing development workflow.

### Positions
Manual documentation alone may become outdated quickly. EventCatalog offers structured assistance and visualization, making it a more sustainable choice. and additionally it is based on event dirvendesign and DDD (Domain Driven Design)

### Argument
Using EventCatalog provides a visual representation of events and domain knowledge, which is crucial for maintaining a clear understanding of the system architecture. 

### Implications

## Related

### Related Decisions

### Related Requirements

### Related Artifacts
- Event schemas.
- Service interaction diagrams.

## Notes
