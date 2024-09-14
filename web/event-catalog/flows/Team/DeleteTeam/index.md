---
id: DeleteTeamFlow
name: Delete Team Flow
version: 1.0.0
summary: Business flow for processing deleting a team
steps:
    - id: "delete_team_request_received"
      title: Delete team request received
      type: node
      next_step: "delete_team"
    - id: "delete_team"
      title: Delete team
      message:
        id: DeleteTeam
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
      next_step: "team_deleted"
    - id: "team_deleted"
      title: Team Deleted
      message:
        id: TeamDeleted
        version: 0.0.1
      next_step: "team_removed"    
    - id: "team_removed"
      title: Team Deleted
      type: node
---

### Flow of feature
<NodeGraph/>