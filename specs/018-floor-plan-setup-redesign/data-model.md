# Data Model: Floor Plan Setup Page Redesign

**Feature**: `018-floor-plan-setup-redesign`
**Date**: 2026-05-30
**Note**: This is a frontend-only redesign. No backend data model changes are required.
Existing entities are described here for component design reference.

---

## Existing Domain Entities (unchanged)

### FloorPlan

Represents a floor-level plan for a location. Read from GraphQL; mutated via GraphQL mutations.

| Field                   | Type                     | Notes                                      |
| ----------------------- | ------------------------ | ------------------------------------------ |
| `id`                    | `string`                 | Relay node ID                              |
| `name`                  | `string`                 | User-editable; min 3 chars                 |
| `image`                 | `FloorPlanImage \| null` | Optional floor plan image                  |
| `image.original.url`    | `string`                 | Full-size image URL                        |
| `image.original.width`  | `number`                 | Used to size the canvas container          |
| `image.original.height` | `number`                 | Used to size the canvas container          |
| `image.thumbnail.url`   | `string`                 | Fallback URL when `original` not available |
| `resourcePositions`     | `ResourcePosition[]`     | x/y coordinates per resource on the canvas |

### ResourcePosition

| Field         | Type     | Notes                                          |
| ------------- | -------- | ---------------------------------------------- |
| `x`           | `number` | Pixel offset from left of canvas               |
| `y`           | `number` | Pixel offset from top of canvas                |
| `resource.id` | `string` | References a Resource within the same location |

### Location

| Field       | Type         | Notes                                           |
| ----------- | ------------ | ----------------------------------------------- |
| `id`        | `string`     | Relay node ID                                   |
| `resources` | `Resource[]` | Resources eligible to be placed on a floor plan |

### Resource

| Field               | Type      | Notes                                                |
| ------------------- | --------- | ---------------------------------------------------- |
| `id`                | `string`  | Relay node ID                                        |
| `name`              | `string`  | Display label on the canvas pin                      |
| `color`             | `string`  | Hex/CSS color used for the pin circle and icon       |
| `resourceType.type` | `string`  | Matches `deskResourceType`, `roomResourceType`, etc. |
| `inactive`          | `boolean` | If true, resource is shown with a dimmed style       |

---

## Frontend Component Model

The redesigned components keep the existing app-local ownership model. Each webapp owns its
`AddFloorPlan` and `EditFloorPlan` implementation, and the component hierarchy below applies to
each copy.

### Component Tree (Add Floor Plan)

```
AddFloorPlanPage (each webapp's page.tsx)
└── AddFloorPlan (app-local — src/components/floorPlan/addFloorPlan/)
    ├── Box [CSS Grid centering wrapper]
    │   ├── StackColumn [main form column]
    │   │   ├── PageHeaderPanel [title: "Add Floor Plan"]
    │   │   ├── SettingsSectionCard [title: "Details"]
    │   │   │   └── Form (react-final-form)
    │   │   │       └── FormStackColumn
    │   │   │           └── FormFieldLabel [Name]
    │   │   │               └── TextField
    │   │   ├── SettingsSectionCard [title: "Floor Plan Layout"] (conditional: only when image uploaded)
    │   │   │   └── ImageFileUploader + Canvas [resource pin placement]
    │   │   ├── SettingsSectionCard [title: "Resources"]
    │   │   │   └── List of resources with checkboxes
    │   │   └── EditorActionBar [primary: "Add", secondary: "Cancel" / "Dismiss"]
```

### Component Tree (Edit Floor Plan)

```
EditFloorPlanPage (each webapp's page.tsx)
└── EditFloorPlan (app-local — src/components/floorPlan/editFloorPlan/)
    ├── Box [CSS Grid centering wrapper]
    │   ├── StackColumn [main form column]
    │   │   ├── PageHeaderPanel [title: "Edit Floor Plan"]
    │   │   ├── SettingsSectionCard [title: "Details"]
    │   │   │   └── Form (react-final-form, auto-save)
    │   │   │       └── FormStackColumn
    │   │   │           └── FormFieldLabel [Name]
    │   │   │               └── TextField
    │   │   ├── SettingsSectionCard [title: "Floor Plan Layout"]
    │   │   │   └── ImageFileUploader + Canvas [drag-and-drop resource placement]
    │   │   ├── SettingsSectionCard [title: "Resources"]
    │   │   │   └── List of resources with checkboxes / assignment toggle
    │   │   └── EditorActionBar [secondary: "Close"]
```

---

## State Model (EditFloorPlan — auto-save)

The edit component maintains local state for:

| State                | Type                  | Description                                          |
| -------------------- | --------------------- | ---------------------------------------------------- |
| `name`               | `string`              | Floor plan name; synced from form via render capture |
| `resourcePositions`  | `Map<string, {x, y}>` | Canvas positions; updated on mouse drag              |
| `draggingResourceId` | `string \| null`      | ID of resource being dragged                         |
| `offset`             | `{x, y}`              | Mouse offset within the dragged element              |

State transitions that trigger auto-save (debounced 1000ms):

- `name` change (detected via `useRef` comparison)
- `image` upload completion
- `resourcePositions` change after mouse-up

---

## Implementation Map

The following per-app files remain the owning implementations and must be kept aligned:

| Current file                                                               | After                                             |
| -------------------------------------------------------------------------- | ------------------------------------------------- |
| `webapp/src/components/floorPlan/addFloorPlan/add-floor-plan.tsx`          | App-local redesigned implementation              |
| `webapp-teams/src/components/floorPlan/addFloorPlan/add-floor-plan.tsx`    | App-local redesigned implementation              |
| `webapp-spaces/src/components/floorPlan/addFloorPlan/add-floor-plan.tsx`   | App-local redesigned implementation              |
| `webapp/src/components/floorPlan/editFloorPlan/edit-floor-plan.tsx`        | App-local redesigned implementation              |
| `webapp-teams/src/components/floorPlan/editFloorPlan/edit-floor-plan.tsx`  | App-local redesigned implementation              |
| `webapp-spaces/src/components/floorPlan/editFloorPlan/edit-floor-plan.tsx` | App-local redesigned implementation              |
| `webapp/src/components/notification/notification-content.tsx` + `index.ts` | May re-export from shared utility implementation |
| `webapp/src/components/relayError/relay-error.tsx` + `index.ts`            | May re-export from shared utility implementation |
