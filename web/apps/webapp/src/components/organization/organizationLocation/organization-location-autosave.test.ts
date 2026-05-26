import { readFileSync } from 'node:fs';
import { resolve } from 'node:path';
import { describe, expect, it } from 'vitest';

const source = readFileSync(resolve(process.cwd(), 'src/components/organization/organizationLocation/organization-location.tsx'), 'utf8');
const floorPlanSource = readFileSync(resolve(process.cwd(), 'src/components/floorPlan/editFloorPlan/edit-floor-plan.tsx'), 'utf8');
const openingHoursSource = readFileSync(resolve(process.cwd(), 'src/components/weekOpeningHours/week-opening-hours.tsx'), 'utf8');
const resourceSource = readFileSync(resolve(process.cwd(), 'src/components/resource/editResource/edit-resource.tsx'), 'utf8');

describe('location autosave', () => {
  it('autosaves location and physical address edit groups', () => {
    expect(source).toContain('debouncedLocationDetailsUpdate');
    expect(source).toContain('debouncedPhysicalAddressUpdate');
    expect(source).not.toContain('onSubmit={handleLocationDetailUpdateClick}');
    expect(source).not.toContain('onSubmit={handlePhysicalAddressUpdateClick}');
  });

  it('includes lookup coordinates in physical address autosave changes', () => {
    expect(source).toContain("form.change('longitude', address.longitude)");
    expect(source).toContain("form.change('latitude', address.latitude)");
    expect(source).toContain('longitude,');
    expect(source).toContain('latitude,');
  });

  it('autosaves restricted information and resource setup edit groups', () => {
    expect(source).toContain('debouncedRestrictedInformationUpdate');
    expect(resourceSource).toContain('debouncedResourceDetailsUpdate');
    expect(resourceSource).not.toContain('onSubmit={handleResourceDetailUpdateClick}');
  });

  it('autosaves floor plan setup and layout changes', () => {
    expect(floorPlanSource).toContain('debouncedFloorPlanDetailUpdate');
    expect(floorPlanSource).not.toContain('onSubmit={handleFloorPlanDetailUpdateClick}');
  });

  it('autosaves shared opening hours without the manual update action', () => {
    expect(openingHoursSource).toContain('debouncedUpdateWeekOpeningHours');
    expect(openingHoursSource).not.toContain('handleUpdateClick');
  });

  it('shows failed-state feedback for location, resource, and floor plan edits', () => {
    expect(source).toContain('errorNotificationOptions');
    expect(floorPlanSource).toContain('errorNotificationOptions');
    expect(resourceSource).toContain('errorNotificationOptions');
  });
});
