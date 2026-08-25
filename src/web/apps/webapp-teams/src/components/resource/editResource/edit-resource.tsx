import { errorNotificationOptions, NotificationContent } from '@/components/notification';
import { MultipleChoicesCustomTags, MultipleChoicesZones, SingleChoiceResourceType } from '@/components/organization';
import { WeekOpeningHours, WeekOpeningHoursDetails } from '@/components/weekOpeningHours';
import type { editResource_query$key } from '@/queries/__generated__/editResource_query.graphql';
import type { editResource_updateLocationResourceAvailableHoursMutation } from '@/queries/__generated__/editResource_updateLocationResourceAvailableHoursMutation.graphql';
import type { editResource_updateResourceMutation, ResourcePatchField } from '@/queries/__generated__/editResource_updateResourceMutation.graphql';
import ExpandMoreRoundedIcon from '@mui/icons-material/ExpandMoreRounded';
import Accordion from '@mui/material/Accordion';
import AccordionDetails from '@mui/material/AccordionDetails';
import AccordionSummary from '@mui/material/AccordionSummary';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import { getRelayErrorMessage, PaletteModeContext } from '@skedular/shared';
import { BodyIconTypography, ColorPickerButton, defaultPadding, FormFieldLabel, FormStackColumn, PageHeaderPanel, StackColumn } from '@skedular/ui';
import { makeRequired, makeValidate, TextField } from 'mui-rff';
import { usePathname, useRouter, useSearchParams } from 'next/navigation';
import { memo, useContext, useEffect, useRef, useState, type ReactNode } from 'react';
import { Form, FormSpy } from 'react-final-form';
import { graphql, useFragment, useMutation } from 'react-relay';
import { toast } from 'react-toastify';
import { useDebounceCallback } from 'usehooks-ts';
import { v7 as uuid } from 'uuid';
import { array, number, object, string } from 'yup';

type Props = {
  rootDataRelay: editResource_query$key;
  onReloadRequired?: () => void;
  organizationCustomDomain: string;
};

type ResourceDetails = {
  name: string;
  resourceTypeId: string;
  customTagIds: string[];
  zoneIds: string[];
  capacity: number;
};

const ResourceSchema = object({
  resourceTypeId: string().required('Resource type is required'),
  name: string().required('Resource name is required'),
  customTagIds: array().nullable(),
  zoneIds: array().nullable(),
  capacity: number().required('Capacity is required').min(1, 'Capacity must be greater than 0'),
});

type ResourceEditSection = 'presentation' | 'classification' | 'opening-hours';

const getActiveSection = (value: string | null): ResourceEditSection => (value === 'opening-hours' || value === 'classification' ? value : 'presentation');

const EditorSection = ({
  title,
  description,
  summary,
  expanded,
  onChange,
  children,
}: {
  title: string;
  description: string;
  summary: string;
  expanded: boolean;
  onChange: () => void;
  children: ReactNode;
}) => (
  <Accordion
    disableGutters
    elevation={0}
    expanded={expanded}
    onChange={onChange}
    sx={{
      margin: 0,
      border: 1,
      borderColor: 'divider',
      borderRadius: '16px !important',
      overflow: 'hidden',
      backgroundColor: 'background.paper',
      '&::before': { display: 'none' },
    }}
  >
    <AccordionSummary expandIcon={<ExpandMoreRoundedIcon />} sx={{ px: 2.5, py: 0.75, minHeight: 72, '& .MuiAccordionSummary-content': { my: 1 } }}>
      <StackColumn spacing={0.35} sx={{ minWidth: 0 }}>
        <BodyIconTypography label={title} />
        <BodyIconTypography label={expanded ? description : summary} />
      </StackColumn>
    </AccordionSummary>
    <AccordionDetails sx={{ borderTop: 1, borderColor: 'divider', p: { xs: 2, sm: 2.5 } }}>{children}</AccordionDetails>
  </Accordion>
);

const getClassificationSummary = (resource: { customTags: ReadonlyArray<unknown>; zones: ReadonlyArray<unknown> }) => {
  const tagLabel = `${resource.customTags.length} ${resource.customTags.length === 1 ? 'tag' : 'tags'}`;
  const zoneLabel = `${resource.zones.length} ${resource.zones.length === 1 ? 'zone' : 'zones'}`;

  return `${tagLabel} · ${zoneLabel} · Color`;
};

const formColumnSx = {
  width: '100%',
};
const resourceAutosaveDebounceTimeout = 1000;

const resourceFieldGroups: ReadonlyArray<[ResourcePatchField, ReadonlyArray<keyof ResourceDetails>]> = [
  ['NAME', ['name']],
  ['RESOURCE_TYPE', ['resourceTypeId']],
  ['TAGS', ['customTagIds', 'zoneIds']],
  ['CAPACITY', ['capacity']],
];

const getChangedResourceFields = (
  left: ResourceDetails | null,
  right: ResourceDetails,
  leftColor: string | null | undefined,
  rightColor: string | null | undefined,
): ResourcePatchField[] => {
  if (!left) return [];
  const changed: ResourcePatchField[] = [];
  for (const [patchField, formFields] of resourceFieldGroups) {
    if (formFields.some((f) => JSON.stringify(left[f]) !== JSON.stringify(right[f]))) {
      changed.push(patchField);
    }
  }
  if (leftColor !== rightColor) changed.push('COLOR');
  return changed;
};

const getValidResourcePatchFields = (fieldsToUpdate: ResourcePatchField[], resourceDetails: ResourceDetails): ResourcePatchField[] =>
  fieldsToUpdate.filter((patchField) => {
    if (patchField === 'COLOR') {
      return true;
    }

    const formFields = resourceFieldGroups.find(([field]) => field === patchField)?.[1] ?? [];

    try {
      for (const formField of formFields) {
        ResourceSchema.validateSyncAt(formField, resourceDetails);
      }

      return true;
    } catch {
      return false;
    }
  });

const EditResource = ({ rootDataRelay, organizationCustomDomain }: Props) => {
  const rootData = useFragment<editResource_query$key>(
    graphql`
      fragment editResource_query on Query {
        organization(customDomain: $organizationCustomDomain) {
          type {
            type
          }
        }
        location(id: $locationId) {
          id
          name
          openingHours {
            weekOpeningHours {
              monday {
                closed
                openAllDay
                from
                until
              }
              tuesday {
                closed
                openAllDay
                from
                until
              }
              wednesday {
                closed
                openAllDay
                from
                until
              }
              thursday {
                closed
                openAllDay
                from
                until
              }
              friday {
                closed
                openAllDay
                from
                until
              }
              saturday {
                closed
                openAllDay
                from
                until
              }
              sunday {
                closed
                openAllDay
                from
                until
              }
            }
          }
        }
        resource(id: $resourceId) {
          id
          name
          inactive
          requireBookingApproval
          color
          capacity
          customTags {
            id
            name
            color
          }
          zones {
            id
            name
            color
          }
          resourceType {
            id
            name
            color
          }
          isAvailableHoursOverridden
          availableHours {
            weekOpeningHours {
              monday {
                closed
                openAllDay
                from
                until
              }
              tuesday {
                closed
                openAllDay
                from
                until
              }
              wednesday {
                closed
                openAllDay
                from
                until
              }
              thursday {
                closed
                openAllDay
                from
                until
              }
              friday {
                closed
                openAllDay
                from
                until
              }
              saturday {
                closed
                openAllDay
                from
                until
              }
              sunday {
                closed
                openAllDay
                from
                until
              }
            }
          }
        }
        ...singleChoiceResourceType_query
        ...multipleChoicesCustomTags_query
        ...multipleChoicesZones_query
        ...weekOpeningHours_query
      }
    `,
    rootDataRelay,
  );

  const [commitUpdateResource] = useMutation<editResource_updateResourceMutation>(graphql`
    mutation editResource_updateResourceMutation($input: UpdateResourceInput!) @raw_response_type {
      updateResource(input: $input) {
        resource {
          id
          name
          inactive
          requireBookingApproval
          color
          capacity
          customTags {
            id
            name
            color
          }
          zones {
            id
            name
            color
          }
          resourceType {
            id
            name
            color
          }
          isAvailableHoursOverridden
          availableHours {
            weekOpeningHours {
              monday {
                closed
                openAllDay
                from
                until
              }
              tuesday {
                closed
                openAllDay
                from
                until
              }
              wednesday {
                closed
                openAllDay
                from
                until
              }
              thursday {
                closed
                openAllDay
                from
                until
              }
              friday {
                closed
                openAllDay
                from
                until
              }
              saturday {
                closed
                openAllDay
                from
                until
              }
              sunday {
                closed
                openAllDay
                from
                until
              }
            }
          }
        }
      }
    }
  `);

  const [commitUpdateLocationResourceAvailableHours] = useMutation<editResource_updateLocationResourceAvailableHoursMutation>(graphql`
    mutation editResource_updateLocationResourceAvailableHoursMutation($input: UpdateLocationResourceAvailableHoursInput!) @raw_response_type {
      updateLocationResourceAvailableHours(input: $input) {
        resource {
          id
          name
          inactive
          requireBookingApproval
          color
          capacity
          customTags {
            id
            name
            color
          }
          zones {
            id
            name
            color
          }
          resourceType {
            id
            name
            color
          }
          isAvailableHoursOverridden
          availableHours {
            weekOpeningHours {
              monday {
                closed
                openAllDay
                from
                until
              }
              tuesday {
                closed
                openAllDay
                from
                until
              }
              wednesday {
                closed
                openAllDay
                from
                until
              }
              thursday {
                closed
                openAllDay
                from
                until
              }
              friday {
                closed
                openAllDay
                from
                until
              }
              saturday {
                closed
                openAllDay
                from
                until
              }
              sunday {
                closed
                openAllDay
                from
                until
              }
            }
          }
        }
      }
    }
  `);

  const pathname = usePathname();
  const router = useRouter();
  const searchParams = useSearchParams();
  const activeSection = getActiveSection(searchParams.get('section'));
  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const validateResourceDetails = makeValidate(ResourceSchema);
  const requiredFields = makeRequired(ResourceSchema);
  const [selectedColor, setSelectedColor] = useState(rootData.resource?.color);
  const [isAvailableHoursOverridden, setIsAvailableHoursOverridden] = useState(rootData.resource ? rootData.resource.isAvailableHoursOverridden : false);
  const [initialResourceValues] = useState<ResourceDetails | null>(() =>
    rootData.resource
      ? {
          name: rootData.resource.name,
          resourceTypeId: rootData.resource.resourceType.id,
          customTagIds: rootData.resource.customTags.map(({ id }) => id),
          zoneIds: rootData.resource.zones.map(({ id }) => id),
          capacity: rootData.resource.capacity,
        }
      : null,
  );
  const previousResourceValues = useRef<ResourceDetails | null>(initialResourceValues);
  const previousSelectedColor = useRef<string | null | undefined>(rootData.resource?.color);
  const lastScheduledResourceUpdateKey = useRef<string | null>(null);
  const latestResourceValues = useRef<ResourceDetails | null>(initialResourceValues);

  useEffect(() => {
    if (searchParams.get('section') !== 'setup') {
      return;
    }

    const params = new URLSearchParams(searchParams.toString());
    params.set('section', 'presentation');
    router.replace(`${pathname}?${params.toString()}`, { scroll: false });
  }, [pathname, router, searchParams]);

  const handleColorChange = (color: string) => {
    setSelectedColor(color);
    if (latestResourceValues.current) {
      scheduleResourceDetailsUpdate(latestResourceValues.current, color);
    }
  };

  const handleResourceDetailUpdateClick = (fieldsToUpdate: ResourcePatchField[], resourceDetails: ResourceDetails) => {
    const { resourceTypeId, name, customTagIds, zoneIds, capacity: capacityStr } = resourceDetails;
    const resource = rootData.resource;
    if (!resource) {
      return;
    }

    const validFieldsToUpdate = getValidResourcePatchFields(fieldsToUpdate, resourceDetails);
    if (validFieldsToUpdate.length === 0) {
      return;
    }

    const oldName = resource.name;
    const capacity = parseInt(capacityStr.toString(), 10);

    commitUpdateResource({
      variables: {
        input: {
          clientMutationId: uuid(),
          id: resource.id,
          fieldsToUpdate: validFieldsToUpdate,
          name,
          inactive: resource.inactive,
          requireBookingApproval: resource.requireBookingApproval,
          customTagIds,
          zoneIds,
          productTagIds: [],
          color: selectedColor,
          capacity,
          organizationResourceTypeId: resourceTypeId,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          themedToast(<NotificationContent content={`We couldn't update '${oldName}'. ${getRelayErrorMessage(errors)}`} />, errorNotificationOptions);

          return;
        }
      },
      onError: (error) => {
        themedToast(<NotificationContent content={`We couldn't update '${oldName}'. ${error.message}`} />, errorNotificationOptions);
      },
      optimisticResponse: {
        updateResource: {
          resource: {
            id: resource.id,
            name,
            inactive: resource.inactive,
            requireBookingApproval: resource.requireBookingApproval,
            customTags: [],
            zones: [],
            color: selectedColor,
            capacity,
            resourceType: {
              id: resourceTypeId,
              name: '',
              color: '',
            },
            isAvailableHoursOverridden: resource.isAvailableHoursOverridden,
            availableHours: resource.availableHours,
          },
        },
      },
    });
  };
  const debouncedResourceDetailsUpdate = useDebounceCallback(handleResourceDetailUpdateClick, resourceAutosaveDebounceTimeout);

  const scheduleResourceDetailsUpdate = (resourceDetails: ResourceDetails, color = selectedColor) => {
    const changedFields = getChangedResourceFields(previousResourceValues.current, resourceDetails, previousSelectedColor.current, color);
    const resourceUpdateKey = JSON.stringify({ resourceDetails, color });
    if (changedFields.length === 0 || lastScheduledResourceUpdateKey.current === resourceUpdateKey) {
      return;
    }

    lastScheduledResourceUpdateKey.current = resourceUpdateKey;
    previousResourceValues.current = {
      ...resourceDetails,
      customTagIds: [...resourceDetails.customTagIds],
      zoneIds: [...resourceDetails.zoneIds],
    };
    previousSelectedColor.current = color;
    debouncedResourceDetailsUpdate(changedFields, resourceDetails);
  };

  const handleResourceFormChange = ({ values }: { values: ResourceDetails }) => {
    latestResourceValues.current = values;
    scheduleResourceDetailsUpdate(values);
  };

  const handleResourceAvailableHoursUpdateClick = (weekOpeningHours: WeekOpeningHoursDetails) => {
    const resource = rootData.resource;
    if (!resource) {
      return;
    }

    commitUpdateLocationResourceAvailableHours({
      variables: {
        input: {
          clientMutationId: uuid(),
          id: resource.id,
          fieldsToUpdate: ['AVAILABLE_HOURS'],
          overrideAvailableHours: isAvailableHoursOverridden,
          availableHours: isAvailableHoursOverridden ? weekOpeningHours : null,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          themedToast(<NotificationContent content={`We couldn't update the hours for '${resource?.name}'. ${getRelayErrorMessage(errors)}`} />, errorNotificationOptions);

          return;
        }
      },
      onError: (error) => {
        themedToast(<NotificationContent content={`We couldn't update the hours for '${resource?.name}'. ${error.message}`} />, errorNotificationOptions);
      },
      optimisticResponse: {
        updateLocationResourceAvailableHours: {
          resource: {
            id: resource.id,
            name: resource.name,
            inactive: resource.inactive,
            requireBookingApproval: resource.requireBookingApproval,
            customTags: resource.customTags,
            zones: resource.zones,
            color: resource.color,
            capacity: resource.capacity,
            resourceType: resource.resourceType,
            isAvailableHoursOverridden,
            availableHours: isAvailableHoursOverridden ? { weekOpeningHours: weekOpeningHours } : null,
          },
        },
      },
    });
  };

  const handleIsAvailableHoursOverriddenChange = (overrideAvailableHours: boolean) => {
    if (overrideAvailableHours === isAvailableHoursOverridden) {
      return;
    }

    setIsAvailableHoursOverridden(overrideAvailableHours);

    if (overrideAvailableHours) {
      return;
    }

    const resource = rootData.resource;
    if (!resource) {
      return;
    }

    commitUpdateLocationResourceAvailableHours({
      variables: {
        input: {
          clientMutationId: uuid(),
          id: resource.id,
          fieldsToUpdate: ['AVAILABLE_HOURS'],
          overrideAvailableHours: false,
          availableHours: null,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          themedToast(<NotificationContent content={`We couldn't update the hours for '${resource?.name}'. ${getRelayErrorMessage(errors)}`} />, errorNotificationOptions);

          return;
        }
      },
      onError: (error) => {
        themedToast(<NotificationContent content={`We couldn't update the hours for '${resource?.name}'. ${error.message}`} />, errorNotificationOptions);
      },
      optimisticResponse: {
        updateLocationResourceAvailableHours: {
          resource: {
            id: resource.id,
            name: resource.name,
            inactive: resource.inactive,
            requireBookingApproval: resource.requireBookingApproval,
            customTags: resource.customTags,
            zones: resource.zones,
            color: resource.color,
            capacity: resource.capacity,
            resourceType: resource.resourceType,
            isAvailableHoursOverridden: false,
            availableHours: null,
          },
        },
      },
    });
  };

  if (!rootData.location || !rootData.resource) {
    return null;
  }

  const location = rootData.location;
  const resource = rootData.resource;
  const updateSection = (section: ResourceEditSection) => {
    const params = new URLSearchParams(searchParams.toString());
    params.set('section', section);
    router.replace(`${pathname}?${params.toString()}`, { scroll: false });
  };

  const renderOpeningHours = () => (
    <StackColumn spacing={2}>
      <Box sx={{ display: 'grid', gridTemplateColumns: { xs: '1fr', sm: 'repeat(2, minmax(0, 1fr))' }, gap: 1.5 }}>
        <Button
          variant={isAvailableHoursOverridden ? 'outlined' : 'contained'}
          color="primary"
          onClick={() => handleIsAvailableHoursOverriddenChange(false)}
          sx={{ alignItems: 'flex-start', justifyContent: 'flex-start', minHeight: 92, px: 2, py: 1.5, textAlign: 'left', textTransform: 'none' }}
        >
          <StackColumn spacing={0.35} sx={{ alignItems: 'flex-start' }}>
            <BodyIconTypography label="Use location hours" />
            <BodyIconTypography label="This resource follows the location's regular opening hours." />
          </StackColumn>
        </Button>
        <Button
          variant={isAvailableHoursOverridden ? 'contained' : 'outlined'}
          color="primary"
          onClick={() => handleIsAvailableHoursOverriddenChange(true)}
          sx={{ alignItems: 'flex-start', justifyContent: 'flex-start', minHeight: 92, px: 2, py: 1.5, textAlign: 'left', textTransform: 'none' }}
        >
          <StackColumn spacing={0.35} sx={{ alignItems: 'flex-start' }}>
            <BodyIconTypography label="Customize hours" />
            <BodyIconTypography label="Set different availability for this resource." />
          </StackColumn>
        </Button>
      </Box>

      {isAvailableHoursOverridden && (
        <Box sx={{ borderTop: 1, borderColor: 'divider', pt: 2 }}>
          <WeekOpeningHours
            rootDataRelay={rootData}
            defaultValue={
              resource.availableHours
                ? {
                    monday: resource.availableHours.weekOpeningHours.monday,
                    tuesday: resource.availableHours.weekOpeningHours.tuesday,
                    wednesday: resource.availableHours.weekOpeningHours.wednesday,
                    thursday: resource.availableHours.weekOpeningHours.thursday,
                    friday: resource.availableHours.weekOpeningHours.friday,
                    saturday: resource.availableHours.weekOpeningHours.saturday,
                    sunday: resource.availableHours.weekOpeningHours.sunday,
                  }
                : location.openingHours.weekOpeningHours
            }
            onWeekOpeningHoursDetailUpdateClick={handleResourceAvailableHoursUpdateClick}
          />
        </Box>
      )}
    </StackColumn>
  );

  const renderPresentation = () => (
    <Form
      onSubmit={() => undefined}
      initialValues={initialResourceValues ?? undefined}
      validate={validateResourceDetails}
      render={({ handleSubmit }) => {
        return (
          <FormStackColumn onSubmit={handleSubmit} sx={formColumnSx}>
            <FormSpy subscription={{ values: true }} onChange={handleResourceFormChange} />
            <StackColumn spacing={2}>
              <FormFieldLabel label="Resource Type">
                <SingleChoiceResourceType rootDataRelay={rootData} name="resourceTypeId" required={requiredFields.resourceTypeId} />
              </FormFieldLabel>

              <FormFieldLabel label="Name">
                <TextField name="name" required={requiredFields.name} helperText="Enter a clear name, such as Desk A1 or Meeting Room 2." />
              </FormFieldLabel>

              <FormFieldLabel label="Capacity">
                <TextField name="capacity" required={requiredFields.capacity} />
              </FormFieldLabel>
            </StackColumn>
          </FormStackColumn>
        );
      }}
    />
  );

  const renderClassification = () => (
    <Form
      onSubmit={() => undefined}
      initialValues={initialResourceValues ?? undefined}
      validate={validateResourceDetails}
      render={({ handleSubmit }) => {
        return (
          <FormStackColumn onSubmit={handleSubmit} sx={formColumnSx}>
            <FormSpy subscription={{ values: true }} onChange={handleResourceFormChange} />
            <StackColumn spacing={2}>
              <FormFieldLabel label="Tags">
                <MultipleChoicesCustomTags
                  rootDataRelay={rootData}
                  name="customTagIds"
                  required={requiredFields.customTagIds}
                  organizationCustomDomain={organizationCustomDomain}
                />
              </FormFieldLabel>

              <FormFieldLabel label="Zones">
                <MultipleChoicesZones rootDataRelay={rootData} name="zoneIds" required={requiredFields.zoneIds} organizationCustomDomain={organizationCustomDomain} />
              </FormFieldLabel>

              <FormFieldLabel label="Color">
                <ColorPickerButton onChange={handleColorChange} defaultColor={rootData.resource?.color} />
              </FormFieldLabel>
            </StackColumn>
          </FormStackColumn>
        );
      }}
    />
  );

  return (
    <Box
      sx={{
        width: '100%',
        maxWidth: '100vw',
        minWidth: 0,
        display: 'flex',
        justifyContent: 'center',
        overflowX: 'hidden',
        boxSizing: 'border-box',
        px: { xs: 0, sm: 1, md: 2 },
        pt: { xs: 1, sm: 1, md: 2 },
        pb: defaultPadding,
      }}
    >
      <StackColumn
        sx={{
          width: '100%',
          maxWidth: 1200,
          minWidth: 0,
          mx: 'auto',
          overflowX: 'hidden',
          backgroundColor: 'transparent',
          gap: 2,
        }}
      >
        <PageHeaderPanel
          eyebrow="Resource settings"
          title={resource.name}
          description="Edit identity, categorization, capacity, and custom availability for this resource."
          sx={{ width: '100%', minWidth: 0, maxWidth: '100%' }}
        ></PageHeaderPanel>

        <Box>
          <Box
            sx={{
              minWidth: 0,
            }}
          >
            <StackColumn spacing={1.5}>
              <EditorSection
                title="Presentation"
                description="Update the name, type, and capacity for this resource."
                summary={`${resource.resourceType.name} · capacity ${resource.capacity}`}
                expanded={activeSection === 'presentation'}
                onChange={() => updateSection('presentation')}
              >
                {activeSection === 'presentation' ? renderPresentation() : null}
              </EditorSection>
              <EditorSection
                title="Classification"
                description="Use tags, zones, and color to make this resource easy to filter and recognize."
                summary={getClassificationSummary(resource)}
                expanded={activeSection === 'classification'}
                onChange={() => updateSection('classification')}
              >
                {activeSection === 'classification' ? renderClassification() : null}
              </EditorSection>
              <EditorSection
                title="Opening hours"
                description="Only change these hours if this resource should be available at different times from the location."
                summary={isAvailableHoursOverridden ? 'Using custom hours' : 'Using location hours'}
                expanded={activeSection === 'opening-hours'}
                onChange={() => updateSection('opening-hours')}
              >
                {activeSection === 'opening-hours' ? renderOpeningHours() : null}
              </EditorSection>
            </StackColumn>
          </Box>
        </Box>
      </StackColumn>
    </Box>
  );
};

export default memo(EditResource);
