---
id: UpdatedTeamFlow
name: Update Team Flow
version: 1.0.0
summary: Business flow for processing adding a team
steps:
    - id: "update_team_request_received"
      title: Update team request received
      type: node
      next_step: "update_team"
    - id: "update_team"
      title: Update team
      message:
        id: UpdateTeam
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
      next_step: "team_updated"    
    - id: "team_updated"
      title: Team Updated
      type: node
---

### Flow of feature
<NodeGraph/>