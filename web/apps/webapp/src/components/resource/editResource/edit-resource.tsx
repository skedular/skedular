import { BodyIconTypography, ColorPicker, FormFieldLabel, FormStackColumn, SmallIconTypography, StackColumn } from '@skedular/ui';
import { errorNotificationOptions, infoNotificationOptions, NotificationContent, successNotificationOptions } from '@/components/notification';
import { MultipleChoicesCustomTags, MultipleChoicesProductTags, MultipleChoicesZones, SingleChoiceResourceType } from '@/components/organization';
import ResourceEditSectionNav, { ResourceEditSection } from '@/components/resource/editResource/resource-edit-section-nav';
import { WeekOpeningHours, WeekOpeningHoursDetails } from '@/components/weekOpeningHours';
import { PaletteModeContext } from '@skedular/shared';
import { defaultPadding } from '@skedular/ui';
import { getRelayErrorMessage } from '@skedular/shared';
import type { editResource_query$key } from '@/queries/__generated__/editResource_query.graphql';
import type { editResource_updateLocationResourceAvailableHoursMutation } from '@/queries/__generated__/editResource_updateLocationResourceAvailableHoursMutation.graphql';
import type { editResource_updateResourceMutation } from '@/queries/__generated__/editResource_updateResourceMutation.graphql';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Chip from '@mui/material/Chip';
import Divider from '@mui/material/Divider';
import Switch from '@mui/material/Switch';
import { EditorActionBar, PageHeaderPanel, SettingsSectionCard, StickyReviewRail } from '@skedular/ui';
import { makeRequired, makeValidate, TextField } from 'mui-rff';
import { usePathname, useRouter, useSearchParams } from 'next/navigation';
import { memo, useContext, useEffect, useState } from 'react';
import { Form } from 'react-final-form';
import { graphql, useFragment, useMutation } from 'react-relay';
import { toast } from 'react-toastify';
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
  productTagIds: string[];
  capacity: number;
};

const ResourceSchema = object({
  resourceTypeId: string().required('Resource type is required'),
  name: string().required('Resource name is required'),
  customTagIds: array().nullable(),
  zoneIds: array().nullable(),
  productTagIds: array().nullable(),
  capacity: number().required('Capacity is required').min(1, 'Capacity must be greater than 0'),
});

const getActiveSection = (value: string | null): ResourceEditSection => {
  switch (value) {
    case 'opening-hours':
      return 'opening-hours';
    case 'setup':
    default:
      return 'setup';
  }
};

const formColumnSx = {
  width: '100%',
  maxWidth: 760,
};

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
          productTags {
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
        ...multipleChoicesProductTags_query
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
          productTags {
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
          productTags {
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

  const router = useRouter();
  const pathname = usePathname();
  const searchParams = useSearchParams();
  const activeSection = getActiveSection(searchParams.get('section'));
  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const validateResourceDetails = makeValidate(ResourceSchema);
  const requiredFields = makeRequired(ResourceSchema);
  const [selectedColor, setSelectedColor] = useState(rootData.resource?.color);
  const [isAvailableHoursOverridden, setIsAvailableHoursOverridden] = useState(rootData.resource ? rootData.resource.isAvailableHoursOverridden : false);
  const [stickyTop, setStickyTop] = useState(0);

  useEffect(() => {
    const updateStickyTop = () => {
      setStickyTop(document.querySelector('.app-bar')?.clientHeight ?? 0);
    };

    updateStickyTop();
    window.addEventListener('resize', updateStickyTop);

    return () => {
      window.removeEventListener('resize', updateStickyTop);
    };
  }, []);

  const handleColorChange = (color: string) => {
    setSelectedColor(color);
  };

  const handleCloseClick = () => {
    router.back();
  };

  const handleResourceDetailUpdateClick = ({ resourceTypeId, name, customTagIds, zoneIds, productTagIds, capacity: capacityStr }: ResourceDetails) => {
    const resource = rootData.resource;
    if (!resource) {
      return;
    }

    const oldName = resource.name;
    const toastId = themedToast(<NotificationContent content={`Saving changes to '${oldName}'...`} />, infoNotificationOptions);
    const capacity = parseInt(capacityStr.toString(), 10);

    commitUpdateResource({
      variables: {
        input: {
          clientMutationId: uuid(),
          id: resource.id,
          name,
          inactive: resource.inactive,
          requireBookingApproval: resource.requireBookingApproval,
          customTagIds,
          zoneIds,
          productTagIds,
          color: selectedColor,
          capacity,
          organizationResourceTypeId: resourceTypeId,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`We couldn't update '${oldName}'. ${getRelayErrorMessage(errors)}`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`${name} has been updated.`} />,
        });

        router.back();
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`We couldn't update '${oldName}'. ${error.message}`} />,
        });
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
            productTags: [],
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

  const handleResourceAvailableHoursUpdateClick = (weekOpeningHours: WeekOpeningHoursDetails) => {
    const resource = rootData.resource;
    if (!resource) {
      return;
    }

    const toastId = themedToast(<NotificationContent content={`Updating hours for '${resource.name}'...`} />, infoNotificationOptions);

    commitUpdateLocationResourceAvailableHours({
      variables: {
        input: {
          clientMutationId: uuid(),
          id: resource.id,
          overrideAvailableHours: isAvailableHoursOverridden,
          availableHours: isAvailableHoursOverridden ? weekOpeningHours : null,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`We couldn't update the hours for '${resource?.name}'. ${getRelayErrorMessage(errors)}`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Hours for '${resource.name}' have been updated.`} />,
        });
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`We couldn't update the hours for '${resource?.name}'. ${error.message}`} />,
        });
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
            productTags: resource.productTags,
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

  const handleIsAvailableHoursOverriddenChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    setIsAvailableHoursOverridden(event.target.checked);

    if (event.target.checked) {
      return;
    }

    const resource = rootData.resource;
    if (!resource) {
      return;
    }

    const toastId = themedToast(<NotificationContent content={`Updating hours for '${resource.name}'...`} />, infoNotificationOptions);

    commitUpdateLocationResourceAvailableHours({
      variables: {
        input: {
          clientMutationId: uuid(),
          id: resource.id,
          overrideAvailableHours: false,
          availableHours: null,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`We couldn't update the hours for '${resource?.name}'. ${getRelayErrorMessage(errors)}`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Hours for '${resource.name}' have been updated.`} />,
        });
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`We couldn't update the hours for '${resource?.name}'. ${error.message}`} />,
        });
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
            productTags: resource.productTags,
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
  const locationId = pathname.split('/locations/')[1]?.split('/')[0] ?? '';

  const renderActiveSection = () => {
    switch (activeSection) {
      case 'opening-hours':
        return (
          <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }} spacing={2}>
            <SettingsSectionCard title="Opening hours" description="Only change these hours if this resource should be available at different times from the location.">
              <FormFieldLabel label="Use custom hours for this resource">
                <Switch checked={isAvailableHoursOverridden} onChange={handleIsAvailableHoursOverriddenChange} />
              </FormFieldLabel>
            </SettingsSectionCard>

            {isAvailableHoursOverridden && (
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
            )}
          </StackColumn>
        );
      case 'setup':
      default:
        return (
          <Form
            onSubmit={handleResourceDetailUpdateClick}
            initialValues={{
              name: resource.name,
              resourceTypeId: resource.resourceType.id,
              customTagIds: resource.customTags.map(({ id }) => id),
              zoneIds: resource.zones.map(({ id }) => id),
              productTagIds: resource.productTags.map(({ id }) => id),
              capacity: resource.capacity,
            }}
            validate={validateResourceDetails}
            render={({ handleSubmit }) => (
              <FormStackColumn onSubmit={handleSubmit} sx={formColumnSx}>
                <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }} spacing={2}>
                  <SettingsSectionCard title="Resource details" description="Update the name, type, tags, and capacity for this resource.">
                    <FormFieldLabel label="Resource Type">
                      <SingleChoiceResourceType rootDataRelay={rootData} name="resourceTypeId" required={requiredFields.resourceTypeId} />
                    </FormFieldLabel>

                    <FormFieldLabel label="Name">
                      <TextField name="name" required={requiredFields.name} helperText="Enter a clear name, such as Desk A1 or Meeting Room 2." />
                    </FormFieldLabel>

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

                    {rootData.organization?.type.type === 'MARKETPLACE' && (
                      <FormFieldLabel label="Product Tags">
                        <MultipleChoicesProductTags
                          rootDataRelay={rootData}
                          name="productTagIds"
                          required={requiredFields.productTagIds}
                          organizationCustomDomain={organizationCustomDomain}
                        />
                      </FormFieldLabel>
                    )}

                    <FormFieldLabel label="Color">
                      <ColorPicker onChange={handleColorChange} defaultColor={rootData.resource?.color} />
                    </FormFieldLabel>

                    <FormFieldLabel label="Capacity">
                      <TextField name="capacity" required={requiredFields.capacity} />
                    </FormFieldLabel>
                  </SettingsSectionCard>

                  <Box sx={{ pb: defaultPadding }}>
                    <EditorActionBar primaryAction="Update" />
                  </Box>
                </StackColumn>
              </FormStackColumn>
            )}
          />
        );
    }
  };

  return (
    <Box sx={{ width: '100%', display: 'flex', justifyContent: 'center', px: { xs: 0, sm: 1, md: 2 }, pb: defaultPadding }}>
      <StackColumn
        sx={{
          width: '100%',
          maxWidth: 1120,
          mx: 'auto',
          backgroundColor: 'transparent',
          gap: 2,
        }}
      >
        <PageHeaderPanel eyebrow="Resource settings" title={resource.name} description="Edit identity, categorization, capacity, and custom availability for this resource.">
          <StackColumn spacing={0.5}>
            <SmallIconTypography label="Resource in this location" />
            <BodyIconTypography label={resource.resourceType.name} />
            <Button variant="text" onClick={handleCloseClick} sx={{ px: 0, justifyContent: 'flex-start', textTransform: 'none' }}>
              Back to location
            </Button>
          </StackColumn>
        </PageHeaderPanel>

        <ResourceEditSectionNav
          activeSection={activeSection}
          organizationCustomDomain={organizationCustomDomain}
          locationId={locationId}
          resourceId={resource.id}
          stickyTop={stickyTop}
        />

        <Box sx={{ display: 'grid', gridTemplateColumns: { xs: '1fr', xl: 'minmax(0, 1fr) 320px' }, gap: 3 }}>
          <Box
            sx={{
              borderRadius: 4,
              border: 1,
              borderColor: (theme) => (theme.palette.mode === 'light' ? 'rgba(15, 23, 42, 0.08)' : 'divider'),
              bgcolor: (theme) => (theme.palette.mode === 'light' ? 'common.white' : theme.palette.background.paper),
              boxShadow: (theme) => (theme.palette.mode === 'light' ? '0 12px 32px rgba(15, 23, 42, 0.08)' : theme.shadows[1]),
              overflow: 'hidden',
            }}
          >
            <StackColumn>{renderActiveSection()}</StackColumn>
          </Box>

          <StickyReviewRail title="Resource summary" description="See the most important details while you make changes.">
            <SettingsSectionCard title="Overview" description="A quick summary of this resource.">
              <StackColumn spacing={1.5}>
                <BodyIconTypography label={resource.name} />
                <StackColumn spacing={1}>
                  <Chip size="small" label={resource.resourceType.name} />
                  <Chip size="small" label={`Capacity ${resource.capacity}`} />
                  <Chip size="small" label={isAvailableHoursOverridden ? 'Custom hours' : 'Uses location hours'} />
                </StackColumn>
                <Divider />
                <BodyIconTypography label={`Custom tags: ${resource.customTags.length}`} />
                <BodyIconTypography label={`Zones: ${resource.zones.length}`} />
                {rootData.organization?.type.type === 'MARKETPLACE' ? <BodyIconTypography label={`Product tags: ${resource.productTags.length}`} /> : null}
              </StackColumn>
            </SettingsSectionCard>
          </StickyReviewRail>
        </Box>
      </StackColumn>
    </Box>
  );
};

export default memo(EditResource);
