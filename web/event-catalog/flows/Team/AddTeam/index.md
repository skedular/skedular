---
id: AddTeamFlow
name: Add Team Flow
version: 1.0.0
summary: Business flow for processing adding a team
steps:
    - id: "create_team_request_received"
      title: Create team request received
      type: node
      next_step: "add_team"
    - id: "add_team"
      title: Add team
      message:
        id: AddTeam
        version: 0.0.1
      next_step: "customer_loaded"
    - id: "customer_loaded"
      title: Customer loaded
      type: node
      next_steps:
        - id: "authorization_failed"
          label: Authorization Failed
        - id: "customer_authorized"
          label: Customer Authorized
    - id: "authorization_failed"
      title: Authorized Failed
      type: node      
    - id: "customer_authorized"
      title: Authorized Failed
      type: node
      next_step: "team_upserted"
    - id: "team_upserted"
      title: Team Upserted
      message:
        id: TeamUpserted
        version: 0.0.1
      next_step: "team_created"    
    - id: "team_created"
      title: Team Created
      type: node
---

### Flow of feature
<NodeGraph/>