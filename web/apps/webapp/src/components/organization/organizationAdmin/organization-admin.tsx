import {
  AppBarWithStackColumn,
  BodyIconTypography,
  CreditCard,
  ExtraLargeHeadingIconTypography,
  FormFieldLabel,
  FormStackColumn,
  GridContainer,
  LeadIconTypography,
  PushToRight,
  SectionIconTypography,
  SmallIconTypography,
  StackColumn,
  StackRow,
} from '@/components/commons';
import { CustomTag } from '@/components/customTag';
import { SingleChoiceCountry } from '@/components/forms';
import { DeleteIcon, EllipseMenuIcon, ErrorIcon, NewIcon, NotPreferredIcon, PreferredIcon, TickIcon } from '@/components/icons';
import { getOrganizationBaseLink } from '@/components/links';
import { MoreActionsMenu, moreActionsMenuAllOptions, MoreActionsMenuItemType, MoreActionsMenuOptionType } from '@/components/moreActionsMenu';
import { errorNotificationOptions, infoNotificationOptions, NotificationContent, successNotificationOptions } from '@/components/notification';
import { OrganizationMultipleChoicesIndustries, SingleChoicesOrganizationMemberVisibilityPolicy, SingleChoicesOrganizationType } from '@/components/organization';
import { AddOrganizationCustomTagButton } from '@/components/organization/addOrganizationCustomTag';
import { AddOrganizationPaymentMethodDialog } from '@/components/organization/addOrganizationPaymentMethod';
import { AddOrganizationZoneButton } from '@/components/organization/addOrganizationZone';
import { EditOrganizationCustomTagDialog } from '@/components/organization/editOrganizationCustomTag';
import { EditOrganizationZoneDialog } from '@/components/organization/editOrganizationZone/';
import { Search } from '@/components/search';
import { Zone } from '@/components/zone';
import { defaultGridRowSelectionModelValue } from '@/libs/mui';
import { PaletteModeContext } from '@/libs/providers';
import { coal, defaultButtonStyle, defaultGridActionPadding, defaultGridStyle, defaultPadding, emerald, secondDrawerExpandedDrawerWidthPx } from '@/libs/theme';
import { joinErrors } from '@/libs/utils';
import type { organizationAdmin_addCustomerPreferredOrganizationTagMutation } from '@/queries/__generated__/organizationAdmin_addCustomerPreferredOrganizationTagMutation.graphql';
import type { organizationAdmin_cancelOrganizationOfferingMutation } from '@/queries/__generated__/organizationAdmin_cancelOrganizationOfferingMutation.graphql';
import type { organizationAdmin_customTags_query$key } from '@/queries/__generated__/organizationAdmin_customTags_query.graphql';
import type { organizationAdmin_customTags_refetchableFragment } from '@/queries/__generated__/organizationAdmin_customTags_refetchableFragment.graphql';
import type { organizationAdmin_deleteCustomTagsMutation } from '@/queries/__generated__/organizationAdmin_deleteCustomTagsMutation.graphql';
import type { organizationAdmin_deleteOrganizationMutation } from '@/queries/__generated__/organizationAdmin_deleteOrganizationMutation.graphql';
import type { organizationAdmin_deleteZonesMutation } from '@/queries/__generated__/organizationAdmin_deleteZonesMutation.graphql';
import type { organizationAdmin_organizationPaymentMethodsDetails_query$key } from '@/queries/__generated__/organizationAdmin_organizationPaymentMethodsDetails_query.graphql';
import type { organizationAdmin_organizationPaymentMethodsDetails_refetchableFragment } from '@/queries/__generated__/organizationAdmin_organizationPaymentMethodsDetails_refetchableFragment.graphql';
import type { organizationAdmin_query$key } from '@/queries/__generated__/organizationAdmin_query.graphql';
import type { organizationAdmin_removeCustomerPreferredOrganizationTagMutation } from '@/queries/__generated__/organizationAdmin_removeCustomerPreferredOrganizationTagMutation.graphql';
import type { organizationAdmin_removeOrganizationPaymentMethodMutation } from '@/queries/__generated__/organizationAdmin_removeOrganizationPaymentMethodMutation.graphql';
import type { organizationAdmin_removeOrganizationSsoSettingsMutation } from '@/queries/__generated__/organizationAdmin_removeOrganizationSsoSettingsMutation.graphql';
import type { organizationAdmin_updateOrganizationBillingContactDetailsMutation } from '@/queries/__generated__/organizationAdmin_updateOrganizationBillingContactDetailsMutation.graphql';
import type {
  organizationAdmin_updateOrganizationMutation,
  OrganizationMemberVisibilityPolicy,
  OrganizationType,
} from '@/queries/__generated__/organizationAdmin_updateOrganizationMutation.graphql';
import type { organizationAdmin_updateOrganizationOfferingMutation } from '@/queries/__generated__/organizationAdmin_updateOrganizationOfferingMutation.graphql';
import type { organizationAdmin_updateOrganizationSsoSettingsMutation } from '@/queries/__generated__/organizationAdmin_updateOrganizationSsoSettingsMutation.graphql';
import type { organizationAdmin_zones_query$key } from '@/queries/__generated__/organizationAdmin_zones_query.graphql';
import type { organizationAdmin_zones_refetchableFragment } from '@/queries/__generated__/organizationAdmin_zones_refetchableFragment.graphql';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Card from '@mui/material/Card';
import CardActions from '@mui/material/CardActions';
import CardContent from '@mui/material/CardContent';
import Divider from '@mui/material/Divider';
import Grid from '@mui/material/Grid';
import IconButton from '@mui/material/IconButton';
import List from '@mui/material/List';
import ListItem from '@mui/material/ListItem';
import ListItemIcon from '@mui/material/ListItemIcon';
import ListItemText from '@mui/material/ListItemText';
import Switch from '@mui/material/Switch';
import type { GridColDef, GridRowSelectionModel } from '@mui/x-data-grid';
import { DataGrid } from '@mui/x-data-grid';
import { makeRequired, makeValidate, TextField } from 'mui-rff';
import { nanoid } from 'nanoid';
import { useRouter, useSearchParams } from 'next/navigation';
import { memo, useCallback, useContext, useEffect, useMemo, useRef, useState, useTransition } from 'react';
import { Form } from 'react-final-form';
import { graphql, useFragment, useMutation, useRefetchableFragment } from 'react-relay';
import { toast } from 'react-toastify';
import { array, object, string } from 'yup';
import OrganizationAdminLeftSideNavigationMenuContent from './organization-admin-left-side-navigation-menu-content';

type Props = {
  rootDataRelay: organizationAdmin_query$key;
  rootDataOrganizationPaymentMethodsDetailsRelay: organizationAdmin_organizationPaymentMethodsDetails_query$key;
  rootDataZonesRelay: organizationAdmin_zones_query$key;
  rootDataCustomTagsRelay: organizationAdmin_customTags_query$key;
  onReloadRequired: () => void;
  organizationId: string;
};

type OrganizationDetails = {
  name: string;
  about: string | null;
  website: string | null;
  type: string;
  memberVisibilityPolicy: string;
  industrySubCategoryIds: string[];
  contactEmail: string;
  contactPhone: string | null;
  addressLine1: string;
  addressLine2: string | null;
  suburb: string;
  city: string;
  province: string | null;
  zipcode: string;
  country: string;
};

const organizationSchema = object({
  name: string().min(3, 'Organization name must be at least three characters long.').required('Organization name is required'),
  about: string().nullable(),
  website: string().nullable(),
  type: string().required('Organization type is required'),
  memberVisibilityPolicy: string().required('Member visibility policy is required'),
  industrySubCategoryIds: array().nullable(),
  contactEmail: string()
    .email(({ value }) => `${value} is not a valid email`)
    .required('Contact email is required'),
  contactPhone: string().nullable(),
  addressLine1: string().required('Address line 1 is required'),
  addressLine2: string().nullable(),
  suburb: string().required('Suburb is required'),
  city: string().required('City is required'),
  province: string().nullable(),
  zipcode: string().required('Zipcode is required'),
  country: string().required('Country is required'),
});

type OrganizationBillingDetails = {
  email: string;
  addressLine1: string | null;
  addressLine2: string | null;
  suburb: string | null;
  city: string | null;
  province: string | null;
  zipcode: string | null;
  country: string | null;
};

const organizationBillingSchema = object({
  email: string().email(({ value }) => `${value} is not a valid email`),
  addressLine1: string().nullable(),
  addressLine2: string().nullable(),
  suburb: string().nullable(),
  city: string().nullable(),
  province: string().nullable(),
  zipcode: string().nullable(),
  country: string().nullable(),
});

type OrganziationSsoSettingsDetails = {
  entityId: string;
  loginUrl: string;
  appFederationMetadataUrl: string;
};

const organziationSsoSettingsSchema = object({
  entityId: string().required('Entity ID is required'),
  loginUrl: string().url('Login Url must be a valid Url').required('Login Url is required'),
  appFederationMetadataUrl: string().url('App Federation Metadata Url must be a valid Url').required('App Federation Metadata Url is required'),
});

type ZoneRowType = {
  id: string;
  name: string;
  description: string | null | undefined;
  preferred: boolean;
};

type CustomTagRowType = {
  id: string;
  name: string;
  description: string | null | undefined;
  preferred: boolean;
};

const OrganizationAdmin = ({
  rootDataRelay,
  rootDataOrganizationPaymentMethodsDetailsRelay,
  rootDataZonesRelay,
  rootDataCustomTagsRelay,
  onReloadRequired,
  organizationId,
}: Props) => {
  const rootData = useFragment<organizationAdmin_query$key>(
    graphql`
      fragment organizationAdmin_query on Query {
        me {
          id
          preferredZones {
            uniqueId
          }
          preferredCustomTags {
            uniqueId
          }
        }
        organization(id: $organizationId) {
          id
          name
          logoUrl
          about
          type {
            type
            name
          }
          memberVisibilityPolicy {
            type
            name
          }
          website
          canModify
          industrySubCategories {
            id
            name
          }
          contactEmail
          contactPhone
          physicalAddress {
            addressLine1
            addressLine2
            suburb
            city
            province
            zipcode
            country
          }
          hasAttachedPaymentMethod
          activeOffering {
            id
            isEnterprise
            name
            start
            end
            unitPrice
            featureSet
            underPriceLines
            free
          }
          availableOfferings {
            isEnterprise
            code
            name
            unitPrice
            featureSet
            underPriceLines
            free
          }
          ssoSettings {
            entityId
            loginUrl
            appFederationMetadataUrl
          }
        }
        organizationIndustryMainCategoriesReferences {
          subCategories {
            id
            name
          }
        }
        organizationBillingContactDetails(organizationId: $organizationId) {
          id
          email
          addressLine1
          addressLine2
          suburb
          city
          province
          zipcode
          country
        }
        ...organizationMultipleChoicesIndustries_query
        ...singleChoiceOrganizationType_query
        ...singleChoiceOrganizationMemberVisibilityPolicyquery
      }
    `,
    rootDataRelay,
  );

  const [rootDataOrganizationPaymentMethodsDetails, refetchOrganizationPaymentMethodsDetails] = useRefetchableFragment<
    organizationAdmin_organizationPaymentMethodsDetails_refetchableFragment,
    organizationAdmin_organizationPaymentMethodsDetails_query$key
  >(
    graphql`
      fragment organizationAdmin_organizationPaymentMethodsDetails_query on Query
      @refetchable(queryName: "organizationAdmin_organizationPaymentMethodsDetails_refetchableFragment") {
        organizationPaymentMethodsDetails(organizationId: $organizationId) {
          id
          cardBrand
          cardExpiryMonth
          cardExpiryYear
          cardLastFourDigit
        }
      }
    `,
    rootDataOrganizationPaymentMethodsDetailsRelay,
  );

  const [rootDataZones, refetchZones] = useRefetchableFragment<organizationAdmin_zones_refetchableFragment, organizationAdmin_zones_query$key>(
    graphql`
      fragment organizationAdmin_zones_query on Query
      @argumentDefinitions(cursor: { type: "String" }, count: { type: "Int", defaultValue: null })
      @refetchable(queryName: "organizationAdmin_zones_refetchableFragment") {
        zones(first: $count, after: $cursor, where: { organizationId: $organizationId, nameContains: $zoneNameSearchText }) @connection(key: "organizationAdmin_zones") {
          __id
          totalCount
          edges {
            node {
              id
              name
              description
              color
            }
          }
        }
      }
    `,
    rootDataZonesRelay,
  );

  const [rootDataCustomTags, refetchCustomTags] = useRefetchableFragment<organizationAdmin_customTags_refetchableFragment, organizationAdmin_customTags_query$key>(
    graphql`
      fragment organizationAdmin_customTags_query on Query
      @argumentDefinitions(cursor: { type: "String" }, count: { type: "Int", defaultValue: null })
      @refetchable(queryName: "organizationAdmin_customTags_refetchableFragment") {
        customTags(
          first: $count
          after: $cursor
          where: { organizationId: $organizationId, nameContains: $customTagNameSearchText }
          orderBy: [{ direction: Ascending, field: Name }]
        ) @connection(key: "organizationAdmin_customTags") {
          __id
          totalCount
          edges {
            node {
              id
              name
              description
              color
            }
          }
        }
      }
    `,
    rootDataCustomTagsRelay,
  );

  const [commitUpdateOrganization] = useMutation<organizationAdmin_updateOrganizationMutation>(graphql`
    mutation organizationAdmin_updateOrganizationMutation($input: UpdateOrganizationInput!) @raw_response_type {
      updateOrganization(input: $input) {
        organization {
          id
          name
          about
          website
          type {
            type
            name
          }
          memberVisibilityPolicy {
            type
            name
          }
          industrySubCategories {
            id
            name
          }
          contactEmail
          contactPhone
          physicalAddress {
            addressLine1
            addressLine2
            suburb
            city
            province
            zipcode
            country
          }
        }
      }
    }
  `);

  const [commitDeleteZones] = useMutation<organizationAdmin_deleteZonesMutation>(graphql`
    mutation organizationAdmin_deleteZonesMutation($connectionIds: [ID!]!, $input: DeleteZonesInput!) {
      deleteZones(input: $input) {
        organizationTags {
          id @deleteEdge(connections: $connectionIds)
        }
      }
    }
  `);

  const [commitDeleteCustomTags] = useMutation<organizationAdmin_deleteCustomTagsMutation>(graphql`
    mutation organizationAdmin_deleteCustomTagsMutation($connectionIds: [ID!]!, $input: DeleteCustomTagsInput!) {
      deleteCustomTags(input: $input) {
        organizationTags {
          id @deleteEdge(connections: $connectionIds)
        }
      }
    }
  `);

  const [commitUpdateOrganizationBillingContactDetails] = useMutation<organizationAdmin_updateOrganizationBillingContactDetailsMutation>(graphql`
    mutation organizationAdmin_updateOrganizationBillingContactDetailsMutation($input: UpdateOrganizationBillingContactDetailsInput!) @raw_response_type {
      updateOrganizationBillingContactDetails(input: $input) {
        organizationBillingContactDetails {
          id
          email
          addressLine1
          addressLine2
          suburb
          city
          province
          zipcode
          country
        }
      }
    }
  `);

  const [commitRemoveOrganizationPaymentMethod] = useMutation<organizationAdmin_removeOrganizationPaymentMethodMutation>(graphql`
    mutation organizationAdmin_removeOrganizationPaymentMethodMutation($input: RemoveOrganizationPaymentMethodInput!) {
      removeOrganizationPaymentMethod(input: $input) {
        clientMutationId
      }
    }
  `);

  const [commitCancelOrganizationOffering] = useMutation<organizationAdmin_cancelOrganizationOfferingMutation>(graphql`
    mutation organizationAdmin_cancelOrganizationOfferingMutation($input: CancelOrganizationOfferingInput!) {
      cancelOrganizationOffering(input: $input) {
        clientMutationId
      }
    }
  `);

  const [commitUpdateOrganizationOffering] = useMutation<organizationAdmin_updateOrganizationOfferingMutation>(graphql`
    mutation organizationAdmin_updateOrganizationOfferingMutation($input: UpdateOrganizationOfferingInput!) {
      updateOrganizationOffering(input: $input) {
        clientMutationId
      }
    }
  `);

  const [commitAddCustomerPreferredOrganizationTag] = useMutation<organizationAdmin_addCustomerPreferredOrganizationTagMutation>(graphql`
    mutation organizationAdmin_addCustomerPreferredOrganizationTagMutation($input: AddCustomerPreferredOrganizationTagInput!) {
      addCustomerPreferredOrganizationTag(input: $input) {
        customer {
          id
          preferredZones {
            uniqueId
          }
        }
      }
    }
  `);

  const [commitRemoveCustomerPreferredOrganizationTag] = useMutation<organizationAdmin_removeCustomerPreferredOrganizationTagMutation>(graphql`
    mutation organizationAdmin_removeCustomerPreferredOrganizationTagMutation($input: RemoveCustomerPreferredOrganizationTagInput!) {
      removeCustomerPreferredOrganizationTag(input: $input) {
        customer {
          id
          preferredZones {
            uniqueId
          }
        }
      }
    }
  `);

  const [commitDeleteOrganization] = useMutation<organizationAdmin_deleteOrganizationMutation>(graphql`
    mutation organizationAdmin_deleteOrganizationMutation($input: DeleteOrganizationInput!) {
      deleteOrganization(input: $input) {
        organization {
          id
        }
      }
    }
  `);

  const [commitUpdateOrganizationSsoSettings] = useMutation<organizationAdmin_updateOrganizationSsoSettingsMutation>(graphql`
    mutation organizationAdmin_updateOrganizationSsoSettingsMutation($input: UpdateOrganizationSsoSettingsInput!) @raw_response_type {
      updateOrganizationSsoSettings(input: $input) {
        organization {
          id
          ssoSettings {
            entityId
            loginUrl
            appFederationMetadataUrl
          }
        }
      }
    }
  `);

  const [commitRemoveOrganizationSsoSettings] = useMutation<organizationAdmin_removeOrganizationSsoSettingsMutation>(graphql`
    mutation organizationAdmin_removeOrganizationSsoSettingsMutation($input: RemoveOrganizationSsoSettingsInput!) @raw_response_type {
      removeOrganizationSsoSettings(input: $input) {
        organization {
          id
          ssoSettings {
            entityId
            loginUrl
            appFederationMetadataUrl
          }
        }
      }
    }
  `);

  const [, startTransition] = useTransition();
  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const router = useRouter();
  const searchParams = useSearchParams();
  const section = searchParams.get('section');
  const sectionRefs = useRef<{ [key: string]: HTMLDivElement | null }>({});
  const validateOrganizationDetails = makeValidate(organizationSchema);
  const requiredOrganizationDetailsFields = makeRequired(organizationSchema);
  const validateOrganizationBilling = makeValidate(organizationBillingSchema);
  const requiredOrganizationBillingFields = makeRequired(organizationBillingSchema);
  const validateOrganizationSsoSettings = makeValidate(organziationSsoSettingsSchema);
  const requiredOrganizationSsoSettingsFields = makeRequired(organziationSsoSettingsSchema);
  const [isAddPaymentMethodDialogOpen, setIsAddPaymentMethodDialogOpen] = useState(false);
  const [ssoEnabled, setSsoEnabled] = useState(!!rootData.organization?.ssoSettings);

  const [zoneNameSearchText, setZoneNameSearchText] = useState<string>('');
  const [seledctedZones, setSeledctedZones] = useState<GridRowSelectionModel>(defaultGridRowSelectionModelValue);
  const [selectedZoneId, setSelectedZoneId] = useState<null | string>(null);
  const [zoneMoreActionsAnchorEl, setZoneMoreActionsAnchorEl] = useState<null | HTMLElement>(null);
  const zoneMoreActionsMenuOpen = Boolean(zoneMoreActionsAnchorEl);
  const [isEditZoneDialogOpen, setIsEditZoneDialogOpen] = useState(false);
  const [preferredZones, setPreferredZones] = useState(rootData.me?.preferredZones.map(({ uniqueId }) => uniqueId) ?? []);
  const zones = useMemo(() => rootDataZones.zones.edges.map(({ node }) => node), [rootDataZones.zones]);
  const zonesConnectionIds = useMemo(() => [rootDataZones.zones.__id], [rootDataZones.zones]);
  const zoneMoreActionsOption: MoreActionsMenuItemType[] = [
    moreActionsMenuAllOptions[MoreActionsMenuOptionType.EditZone],
    moreActionsMenuAllOptions[MoreActionsMenuOptionType.DeleteZone],
  ];

  const handleRefetchZones = useCallback(
    (zoneNameSearchText: string) => {
      startTransition(() => {
        refetchZones(
          {
            zoneNameSearchText,
          },
          {
            fetchPolicy: 'store-and-network',
          },
        );
      });
    },
    [refetchZones],
  );

  const [customTagNameSearchText, setCustomTagNameSearchText] = useState<string>('');
  const [seledctedCustomTags, setSeledctedCustomTags] = useState<GridRowSelectionModel>(defaultGridRowSelectionModelValue);
  const [selectedCustomTagId, setSelectedCustomTagId] = useState<null | string>(null);
  const [customTagMoreActionsAnchorEl, setCustomTagMoreActionsAnchorEl] = useState<null | HTMLElement>(null);
  const customTagMoreActionsMenuOpen = Boolean(customTagMoreActionsAnchorEl);
  const [isEditCustomTagDialogOpen, setIsEditCustomTagDialogOpen] = useState(false);
  const [preferredCustomTags, setPreferredCustomTags] = useState(rootData.me?.preferredCustomTags.map(({ uniqueId }) => uniqueId) ?? []);
  const customTags = useMemo(() => rootDataCustomTags.customTags.edges.map(({ node }) => node), [rootDataCustomTags.customTags]);
  const customTagsConnectionIds = useMemo(() => [rootDataCustomTags.customTags.__id], [rootDataCustomTags.customTags]);
  const customTagMoreActionsOption: MoreActionsMenuItemType[] = [
    moreActionsMenuAllOptions[MoreActionsMenuOptionType.EditCustomTag],
    moreActionsMenuAllOptions[MoreActionsMenuOptionType.DeleteCustomTag],
  ];

  const handleRefetchCustomTags = useCallback(
    (customTagNameSearchText: string) => {
      startTransition(() => {
        refetchCustomTags(
          {
            customTagNameSearchText,
          },
          {
            fetchPolicy: 'store-and-network',
          },
        );
      });
    },
    [refetchCustomTags],
  );

  useEffect(() => {
    if (!section || section === 'setup') {
      return;
    }

    const element = sectionRefs.current[section];
    if (!element) {
      return;
    }

    const appBarHeight = document.querySelector('.app-bar')?.clientHeight || 0;
    const elementTop = element.getBoundingClientRect().top + window.scrollY;
    window.scrollTo({
      top: elementTop - appBarHeight,
      behavior: 'smooth',
    });
  }, [section]);

  const handleRefetchOrganizationPaymentMethodsDetails = useCallback(() => {
    startTransition(() => {
      refetchOrganizationPaymentMethodsDetails(
        {},
        {
          fetchPolicy: 'store-and-network',
        },
      );
    });
  }, [refetchOrganizationPaymentMethodsDetails]);

  const handleOrganizationDetailUpdateClick = ({
    name,
    about,
    website,
    type,
    memberVisibilityPolicy,
    industrySubCategoryIds,
    contactEmail,
    contactPhone,
    addressLine1,
    addressLine2,
    suburb,
    city,
    province,
    zipcode,
    country,
  }: OrganizationDetails) => {
    if (!rootData.organization) {
      return;
    }

    const organization = rootData.organization;
    const selectedIndustrySubCategoryIds = industrySubCategoryIds ?? [];
    const toastId = themedToast(<NotificationContent content={`Updating organization '${organization.name}'...`} />, infoNotificationOptions);

    commitUpdateOrganization({
      variables: {
        input: {
          clientMutationId: nanoid(),
          id: organization.id,
          name,
          about,
          website,
          type: type as OrganizationType,
          industrySubCategoryIds: selectedIndustrySubCategoryIds,
          memberVisibilityPolicy: memberVisibilityPolicy as OrganizationMemberVisibilityPolicy,
          contactEmail,
          contactPhone,
          physicalAddress: {
            addressLine1,
            addressLine2,
            suburb,
            city,
            province,
            zipcode,
            country,
          },
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to update organization '${organization?.name}'. Error: ${joinErrors(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Organization ${name} details updated.`} />,
        });
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to update organization '${organization?.name}'. Error: ${error.message}.`} />,
        });
      },
      optimisticResponse: {
        updateOrganization: {
          organization: {
            id: organization.id,
            name,
            about,
            website,
            type: {
              type: type as OrganizationType,
              name: '',
            },
            memberVisibilityPolicy: {
              type: type as OrganizationMemberVisibilityPolicy,
              name: '',
            },
            industrySubCategories: rootData.organizationIndustryMainCategoriesReferences
              .flatMap((mainCategory) => mainCategory.subCategories)
              .filter(({ id }) => selectedIndustrySubCategoryIds.find((selectedIndustrySubCategoryId) => selectedIndustrySubCategoryId === id))
              .map(({ id, name }) => ({ id, name })),
            contactEmail,
            contactPhone,
            physicalAddress: {
              addressLine1,
              addressLine2,
              suburb,
              city,
              province,
              zipcode,
              country,
            },
          },
        },
      },
    });
  };

  const handleOrganizationBillingDetailUpdateClick = ({ email, addressLine1, addressLine2, suburb, city, province, zipcode, country }: OrganizationBillingDetails) => {
    const billingDetails = rootData.organizationBillingContactDetails;
    if (!billingDetails) {
      return;
    }

    const toastId = themedToast(<NotificationContent content={`Updating organization '${organization.name}' billing...`} />, infoNotificationOptions);

    commitUpdateOrganizationBillingContactDetails({
      variables: {
        input: {
          clientMutationId: nanoid(),
          organizationId: organization.id,
          email,
          addressLine1,
          addressLine2,
          suburb,
          city,
          province,
          zipcode,
          country,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to update organization '${organization?.name}' billing. Error: ${joinErrors(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Organization '${organization?.name}' billing updated.`} />,
        });
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to update organization '${organization?.name}' billing. Error: ${error.message}.`} />,
        });
      },
      optimisticResponse: {
        updateOrganizationBillingContactDetails: {
          organizationBillingContactDetails: {
            id: billingDetails.id,
            email,
            addressLine1,
            addressLine2,
            suburb,
            city,
            province,
            zipcode,
            country,
          },
        },
      },
    });
  };

  const handleEnableOrganizationSsoSettingsClick = ({ entityId, loginUrl, appFederationMetadataUrl }: OrganziationSsoSettingsDetails) => {
    const organization = rootData.organization;
    if (!organization) {
      return;
    }

    const toastId = themedToast(<NotificationContent content={`Updating organization '${organization.name}' SSO settings...`} />, infoNotificationOptions);

    commitUpdateOrganizationSsoSettings({
      variables: {
        input: {
          clientMutationId: nanoid(),
          organizationId: organization.id,
          entityId,
          loginUrl,
          appFederationMetadataUrl,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to update organization '${organization?.name}' SSO settings. Error: ${joinErrors(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Organization ${organization?.name} SSO settings details updated.`} />,
        });
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to update organization '${organization?.name}' SSO settings. Error: ${error.message}.`} />,
        });
      },
      optimisticResponse: {
        updateOrganizationSsoSettings: {
          organization: {
            id: organization.id,
            ssoSettings: {
              entityId,
              loginUrl,
              appFederationMetadataUrl,
            },
          },
        },
      },
    });
  };

  const handleEnableSsoChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    setSsoEnabled(event.target.checked);

    if (event.target.checked) {
      return;
    }

    const organization = rootData.organization;
    if (!organization) {
      return;
    }

    const toastId = themedToast(<NotificationContent content={`Removing organization '${organization.name}' SSO settings...`} />, infoNotificationOptions);

    commitRemoveOrganizationSsoSettings({
      variables: {
        input: {
          clientMutationId: nanoid(),
          organizationId: organization.id,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to remove organization '${organization?.name}' SSO settings. Error: ${joinErrors(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Organization ${organization?.name} SSO settings removed.`} />,
        });
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to remove organization '${organization?.name}' SSO settings. Error: ${error.message}.`} />,
        });
      },
      optimisticResponse: {
        removeOrganizationSsoSettings: {
          organization: {
            id: organization.id,
            ssoSettings: null,
          },
        },
      },
    });
  };

  const handleZonesSearchTextChange = (str: string) => {
    setZoneNameSearchText(str);

    handleRefetchZones(str);
  };

  const handleSelectedZonesChanged = (newRowSelectionModel: GridRowSelectionModel) => {
    setSeledctedZones(newRowSelectionModel);
  };

  const handleZoneMoreActionsMenuItemClick = (id: MoreActionsMenuOptionType) => {
    setZoneMoreActionsAnchorEl(null);

    switch (id) {
      case MoreActionsMenuOptionType.EditZone:
        setIsEditZoneDialogOpen(true);
        break;

      case MoreActionsMenuOptionType.DeleteZone:
        handleRemoveZoneClick();
        break;
    }
  };

  const handleEditZoneClick = () => {
    setIsEditZoneDialogOpen(false);
  };

  const onEditZoneCancel = () => {
    setIsEditZoneDialogOpen(false);
  };

  const handleRemoveZonesClick = () => {
    const toastId = themedToast(<NotificationContent content="Removing zones ..." />, infoNotificationOptions);

    commitDeleteZones({
      variables: {
        connectionIds: zonesConnectionIds,
        input: {
          clientMutationId: nanoid(),
          ids: seledctedZones.ids
            .values()
            .map((id) => id as string)
            .toArray(),
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to remove zones. Error: ${joinErrors(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Zones removed.`} />,
        });
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to remove zones. Error: ${error.message}.`} />,
        });
      },
    });
  };

  const handleRemoveZoneClick = () => {
    if (!selectedZoneId) {
      return;
    }

    const toastId = themedToast(<NotificationContent content="Removing zone ..." />, infoNotificationOptions);

    commitDeleteZones({
      variables: {
        connectionIds: zonesConnectionIds,
        input: {
          clientMutationId: nanoid(),
          ids: [selectedZoneId],
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to remove zone. Error: ${joinErrors(errors)}.`} />,
          });
          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Zone removed.`} />,
        });
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to remove zone. Error: ${error.message}.`} />,
        });
      },
    });
  };

  const handleCustomTagsSearchTextChange = (str: string) => {
    setCustomTagNameSearchText(str);

    handleRefetchCustomTags(str);
  };

  const handleSelectedCustomTagsChanged = (newRowSelectionModel: GridRowSelectionModel) => {
    setSeledctedCustomTags(newRowSelectionModel);
  };

  const handleCustomTagMoreActionsMenuItemClick = (id: MoreActionsMenuOptionType) => {
    setCustomTagMoreActionsAnchorEl(null);

    switch (id) {
      case MoreActionsMenuOptionType.EditCustomTag:
        setIsEditCustomTagDialogOpen(true);
        break;

      case MoreActionsMenuOptionType.DeleteCustomTag:
        handleRemoveCustomTagClick();
        break;
    }
  };

  const handleEditCustomTagClick = () => {
    setIsEditCustomTagDialogOpen(false);
  };

  const handleEditCustomTagCancel = () => {
    setIsEditCustomTagDialogOpen(false);
  };

  const handleRemoveCustomTagsClick = () => {
    const toastId = themedToast(<NotificationContent content="Removing tags ..." />, infoNotificationOptions);

    commitDeleteCustomTags({
      variables: {
        connectionIds: customTagsConnectionIds,
        input: {
          clientMutationId: nanoid(),
          ids: seledctedCustomTags.ids
            .values()
            .map((id) => id as string)
            .toArray(),
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to remove tags. Error: ${joinErrors(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Tags removed.`} />,
        });
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to remove tags. Error: ${error.message}.`} />,
        });
      },
    });
  };

  const handleRemoveCustomTagClick = () => {
    if (!selectedCustomTagId) {
      return;
    }

    const toastId = themedToast(<NotificationContent content="Removing tag ..." />, infoNotificationOptions);

    commitDeleteZones({
      variables: {
        connectionIds: customTagsConnectionIds,
        input: {
          clientMutationId: nanoid(),
          ids: [selectedCustomTagId],
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to remove tag. Error: ${joinErrors(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Tag removed.`} />,
        });

        setSelectedCustomTagId(null);
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to remove tag. Error: ${error.message}.`} />,
        });
      },
    });
  };

  const handleAddPaymentMethodClicked = () => {
    setIsAddPaymentMethodDialogOpen(true);
  };

  const handleAddPaymentMethodCancel = () => {
    setIsAddPaymentMethodDialogOpen(false);
  };

  const handleCloseClick = () => {
    router.push(getOrganizationBaseLink(organizationId));
  };

  const handleRemovePaymentMethodClick = (id: string) => {
    if (!rootData.organization) {
      return;
    }

    const toastId = themedToast(<NotificationContent content={`Removing payment method...`} />, infoNotificationOptions);

    commitRemoveOrganizationPaymentMethod({
      variables: {
        input: {
          clientMutationId: nanoid(),
          id,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to remove payment method. Error: ${joinErrors(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Payment method removed.`} />,
        });

        handleRefetchOrganizationPaymentMethodsDetails();
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to remove payment method. Error: ${error.message}.`} />,
        });
      },
    });
  };

  const handleCancelActiveOfferingClick = () => {
    if (!rootData.organization) {
      return;
    }

    const toastId = themedToast(<NotificationContent content={`Cancelling organization '${rootData.organization.name}' active offering...`} />, infoNotificationOptions);

    commitCancelOrganizationOffering({
      variables: {
        input: {
          clientMutationId: nanoid(),
          id: rootData.organization.id,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to cancel organization '${rootData.organization?.name}' active offering. Error: ${joinErrors(errors)}.`} />,
          });

          onReloadRequired();

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Organization '${rootData.organization?.name}' active offering cancelled.`} />,
        });

        onReloadRequired();
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to cancel organization '${rootData.organization?.name}' active offering. Error: ${error.message}.`} />,
        });

        onReloadRequired();
      },
    });
  };

  const handleUpgradeOfferingClick = (code: string) => {
    if (!rootData.organization) {
      return;
    }

    const toastId = themedToast(<NotificationContent content={`Updating organization '${rootData.organization.name} active offering'...`} />, infoNotificationOptions);

    commitUpdateOrganizationOffering({
      variables: {
        input: {
          clientMutationId: nanoid(),
          id: rootData.organization.id,
          offeringCode: code,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to update organization ${rootData.organization?.name} active offering. Error: ${joinErrors(errors)}.`} />,
          });

          onReloadRequired();

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Organization ${rootData.organization?.name} active offering updated.`} />,
        });

        onReloadRequired();
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to update organization ${rootData.organization?.name} active offering. Error: ${error.message}.`} />,
        });

        onReloadRequired();
      },
    });
  };

  const handleSetAsPreferredZoneClicked = (id: string) => {
    if (!rootData.me) {
      return;
    }

    const organizationTagDetails = zones.find((item) => item.id === id);
    if (!organizationTagDetails) {
      return;
    }

    if (!rootData.me) {
      return;
    }

    const toastId = themedToast(<NotificationContent content={`Setting zone '${organizationTagDetails.name}' as your preferred zone...`} />, infoNotificationOptions);

    commitAddCustomerPreferredOrganizationTag({
      variables: {
        input: {
          clientMutationId: nanoid(),
          organizationTagId: organizationTagDetails.id,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to set zone '${organizationTagDetails.name}' as your preferred zone. Error: ${joinErrors(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Zone '${organizationTagDetails.name}' has been set as the preferred zone.`} />,
        });

        setPreferredZones(preferredZones.concat([organizationTagDetails.id]));
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to set zone '${organizationTagDetails.name}' as your preferred zone. Error: ${error.message}.`} />,
        });
      },
    });
  };

  const handleRemoveAsPreferredZoneClicked = (id: string) => {
    if (!rootData.me) {
      return;
    }

    const organizationTagDetails = zones.find((item) => item.id === id);
    if (!organizationTagDetails) {
      return;
    }

    if (!rootData.me) {
      return;
    }

    const toastId = themedToast(<NotificationContent content={`Removing zone '${organizationTagDetails.name}' as your preferred zone...`} />, infoNotificationOptions);

    commitRemoveCustomerPreferredOrganizationTag({
      variables: {
        input: {
          clientMutationId: nanoid(),
          organizationTagId: organizationTagDetails.id,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to remove the zone '${organizationTagDetails.name}' as your preferred zone. Error: ${joinErrors(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Zone '${organizationTagDetails.name}' has been removed as your preferred zone.`} />,
        });

        setPreferredZones(preferredZones.filter((item) => item !== organizationTagDetails.id));
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to remove the zone '${organizationTagDetails.name}' as your preferred zone. Error: ${error.message}.`} />,
        });
      },
    });
  };

  const handleSetAsPreferredCustomTagClicked = (id: string) => {
    if (!rootData.me) {
      return;
    }

    const organizationTagDetails = customTags.find((item) => item.id === id);
    if (!organizationTagDetails) {
      return;
    }

    if (!rootData.me) {
      return;
    }

    const toastId = themedToast(<NotificationContent content={`Setting tag '${organizationTagDetails.name}' as your preferred tag...`} />, infoNotificationOptions);

    commitAddCustomerPreferredOrganizationTag({
      variables: {
        input: {
          clientMutationId: nanoid(),
          organizationTagId: organizationTagDetails.id,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to set tag '${organizationTagDetails.name}' as your preferred tag. Error: ${joinErrors(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Tag '${organizationTagDetails.name}' has been set as the preferred tag.`} />,
        });

        setPreferredCustomTags(preferredCustomTags.concat([organizationTagDetails.id]));
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to set tag '${organizationTagDetails.name}' as your preferred tag. Error: ${error.message}.`} />,
        });
      },
    });
  };

  const handleRemoveAsPreferredCustomTagClicked = (id: string) => {
    if (!rootData.me) {
      return;
    }

    const organizationTagDetails = customTags.find((item) => item.id === id);
    if (!organizationTagDetails) {
      return;
    }

    if (!rootData.me) {
      return;
    }

    const toastId = themedToast(<NotificationContent content={`Removing tag '${organizationTagDetails.name}' as your preferred tag...`} />, infoNotificationOptions);

    commitRemoveCustomerPreferredOrganizationTag({
      variables: {
        input: {
          clientMutationId: nanoid(),
          organizationTagId: organizationTagDetails.id,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to remove the tag '${organizationTagDetails.name}' as your preferred tag. Error: ${joinErrors(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Tag '${organizationTagDetails.name}' has been removed as your preferred tag.`} />,
        });

        setPreferredCustomTags(preferredCustomTags.filter((item) => item !== organizationTagDetails.id));
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to remove the tag '${organizationTagDetails.name}' as your preferred tag. Error: ${error.message}.`} />,
        });
      },
    });
  };

  const handleRemoveOrganizationClicked = () => {
    if (!rootData.organization) {
      return;
    }

    const organizationDetails = rootData.organization;

    const toastId = themedToast(<NotificationContent content={`Removing organization '${organizationDetails.name}'...`} />, infoNotificationOptions);

    commitDeleteOrganization({
      variables: {
        input: {
          clientMutationId: nanoid(),
          id: organizationDetails.id,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to remove the organization '${organizationDetails.name}'. Error: ${joinErrors(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Organization '${organizationDetails.name}' removed.`} />,
        });

        router.push('/');
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to remove the organization '${organizationDetails.name}'. Error: ${error.message}.`} />,
        });
      },
    });
  };

  if (!rootData.organization) {
    return <></>;
  }

  if (!rootData.organizationBillingContactDetails) {
    return <></>;
  }

  const zoneRows: ZoneRowType[] = zones.map((zone) => ({
    id: zone.id,
    name: zone.name,
    description: zone.description,
    preferred: preferredZones.includes(zone.id),
  }));

  const zoneColumns: GridColDef<(typeof zoneRows)[number]>[] = [
    {
      field: 'name',
      headerName: 'Name',
      editable: false,
      renderCell: (params) => {
        const zone = zones.find((zone) => zone.id === (params.id as string));
        if (!zone) {
          return <></>;
        }

        return <Zone zone={zone} showFullName />;
      },
      display: 'flex',
      minWidth: 200,
    },
    {
      field: 'description',
      headerName: 'Description',
      editable: false,
      renderCell: (params) => <SmallIconTypography label={params.value} />,
      display: 'flex',
      minWidth: 200,
    },
    {
      field: 'preferred',
      headerName: 'Preferred?',
      editable: false,
      renderCell: (params) => {
        const id = params.id as string;
        if (params.value) {
          return (
            <IconButton onClick={() => handleRemoveAsPreferredZoneClicked(id)}>
              <PreferredIcon />
            </IconButton>
          );
        }

        return (
          <IconButton onClick={() => handleSetAsPreferredZoneClicked(id)}>
            <NotPreferredIcon />
          </IconButton>
        );
      },
      display: 'flex',
    },
    {
      field: 'More Actions',
      headerName: '',
      editable: false,
      sortable: false,
      display: 'flex',
      renderCell: (params) => (
        <Box sx={{ display: 'flex', justifyContent: 'flex-end', width: '100%' }}>
          <IconButton
            onClick={(event: React.MouseEvent<HTMLElement>) => {
              setSelectedZoneId(params.id as string);
              setZoneMoreActionsAnchorEl(event.currentTarget);
            }}
          >
            <EllipseMenuIcon />
          </IconButton>
        </Box>
      ),
      flex: 1,
    },
  ];

  const customTagRows: CustomTagRowType[] = customTags.map((customTag) => ({
    id: customTag.id,
    name: customTag.name,
    description: customTag.description,
    preferred: preferredCustomTags.includes(customTag.id),
  }));

  const customTagColumns: GridColDef<(typeof customTagRows)[number]>[] = [
    {
      field: 'name',
      headerName: 'Name',
      editable: false,
      renderCell: (params) => {
        const customTag = customTags.find((customTag) => customTag.id === (params.id as string));
        if (!customTag) {
          return <></>;
        }

        return <CustomTag customTag={customTag} showFullName />;
      },
      display: 'flex',
      minWidth: 200,
    },
    {
      field: 'description',
      headerName: 'Description',
      editable: false,
      renderCell: (params) => <SmallIconTypography label={params.value} />,
      display: 'flex',
      minWidth: 200,
    },
    {
      field: 'preferred',
      headerName: 'Preferred?',
      editable: false,
      renderCell: (params) => {
        const id = params.id as string;
        if (params.value) {
          return (
            <IconButton onClick={() => handleRemoveAsPreferredCustomTagClicked(id)}>
              <PreferredIcon />
            </IconButton>
          );
        }

        return (
          <IconButton onClick={() => handleSetAsPreferredCustomTagClicked(id)}>
            <NotPreferredIcon />
          </IconButton>
        );
      },
      display: 'flex',
    },
    {
      field: 'More Actions',
      headerName: '',
      editable: false,
      sortable: false,
      display: 'flex',
      renderCell: (params) => (
        <Box sx={{ display: 'flex', justifyContent: 'flex-end', width: '100%' }}>
          <IconButton
            onClick={(event: React.MouseEvent<HTMLElement>) => {
              setSelectedCustomTagId(params.id as string);
              setCustomTagMoreActionsAnchorEl(event.currentTarget);
            }}
          >
            <EllipseMenuIcon />
          </IconButton>
        </Box>
      ),
      flex: 1,
    },
  ];

  const organization = rootData.organization;

  const billingContactDetails = rootData.organizationBillingContactDetails;
  const email = billingContactDetails.email ? billingContactDetails.email : '';
  const addressLine1 = billingContactDetails.addressLine1 ? billingContactDetails.addressLine1 : '';
  const addressLine2 = billingContactDetails.addressLine2 ? billingContactDetails.addressLine2 : '';
  const suburb = billingContactDetails.suburb ? billingContactDetails.suburb : '';
  const city = billingContactDetails.city ? billingContactDetails.city : '';
  const province = billingContactDetails.province ? billingContactDetails.province : '';
  const zipcode = billingContactDetails.zipcode ? billingContactDetails.zipcode : '';
  const country = billingContactDetails.country ? billingContactDetails.country : '';

  const paymentMethodExist =
    rootDataOrganizationPaymentMethodsDetails.organizationPaymentMethodsDetails && rootDataOrganizationPaymentMethodsDetails.organizationPaymentMethodsDetails.length > 0;
  const activeOffering = rootData.organization ? rootData.organization.activeOffering : null;
  const availableOfferings = rootData.organization && rootData.organization.availableOfferings ? rootData.organization.availableOfferings : [];

  const ssoSettingsEntityId = organization.ssoSettings ? organization.ssoSettings.entityId : '';
  const ssoSettingsLoginUrl = organization.ssoSettings ? organization.ssoSettings.loginUrl : '';
  const ssoSettingsappFederationMetadataUrl = organization.ssoSettings ? organization.ssoSettings.appFederationMetadataUrl : '';

  return (
    <>
      <Box sx={{ display: 'flex' }}>
        <OrganizationAdminLeftSideNavigationMenuContent organizationId={organizationId} hideIcons />
        <Box sx={{ marginLeft: secondDrawerExpandedDrawerWidthPx, flexGrow: 1 }}>
          <AppBarWithStackColumn onClose={handleCloseClick} label="Edit Organization Information">
            <Form
              onSubmit={handleOrganizationDetailUpdateClick}
              initialValues={{
                name: organization.name,
                about: organization.about,
                website: organization.website,
                type: organization.type.type,
                memberVisibilityPolicy: organization.memberVisibilityPolicy.type,
                industrySubCategoryIds: organization.industrySubCategories.map(({ id }) => id),
                contactEmail: organization.contactEmail,
                contactPhone: organization.contactPhone,
                addressLine1: organization.physicalAddress.addressLine1,
                addressLine2: organization.physicalAddress.addressLine2,
                suburb: organization.physicalAddress.suburb,
                city: organization.physicalAddress.city,
                province: organization.physicalAddress.province,
                zipcode: organization.physicalAddress.zipcode,
                country: organization.physicalAddress.country,
              }}
              validate={validateOrganizationDetails}
              render={({ handleSubmit }) => (
                <FormStackColumn onSubmit={handleSubmit}>
                  <StackColumn
                    sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}
                    ref={(divElement) => {
                      sectionRefs.current['setup'] = divElement;
                    }}
                  >
                    <SectionIconTypography label="Organization Setup" />
                    <BodyIconTypography label="Edit your organization details" />
                    <Divider />
                  </StackColumn>

                  <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}>
                    <FormFieldLabel label="Name">
                      <TextField name="name" required={requiredOrganizationDetailsFields.name} />
                    </FormFieldLabel>

                    <FormFieldLabel label="About">
                      <TextField name="about" required={requiredOrganizationDetailsFields.about} multiline rows={3} />
                    </FormFieldLabel>

                    <FormFieldLabel label="Website">
                      <TextField name="website" required={requiredOrganizationDetailsFields.about} helperText="https://" />
                    </FormFieldLabel>

                    <FormFieldLabel label="Type">
                      <SingleChoicesOrganizationType rootDataRelay={rootData} name="type" required={requiredOrganizationDetailsFields.type} />
                    </FormFieldLabel>

                    <FormFieldLabel label="Member Visibility Policy">
                      <SingleChoicesOrganizationMemberVisibilityPolicy
                        rootDataRelay={rootData}
                        name="memberVisibilityPolicy"
                        required={requiredOrganizationDetailsFields.memberVisibilityPolicy}
                      />
                    </FormFieldLabel>

                    <FormFieldLabel label="Industry">
                      <OrganizationMultipleChoicesIndustries
                        rootDataRelay={rootData}
                        name="industrySubCategoryIds"
                        required={requiredOrganizationDetailsFields.industrySubCategoryIds}
                      />
                    </FormFieldLabel>

                    <SectionIconTypography label="Contact Details" />
                    <BodyIconTypography label="Edit your organization contact details" />
                    <Divider />

                    <FormFieldLabel label="Email">
                      <TextField name="contactEmail" required={requiredOrganizationDetailsFields.contactEmail} />
                    </FormFieldLabel>

                    <FormFieldLabel label="Phone Number">
                      <TextField name="contactEmail" required={requiredOrganizationDetailsFields.contactPhone} />
                    </FormFieldLabel>

                    <SectionIconTypography label="Address" />
                    <BodyIconTypography label="Edit your organization address" />
                    <Divider />

                    <FormFieldLabel label="Address Line 1">
                      <TextField name="addressLine1" required={requiredOrganizationDetailsFields.addressLine1} />
                    </FormFieldLabel>

                    <FormFieldLabel label="Address Line 2">
                      <TextField name="addressLine2" required={requiredOrganizationDetailsFields.addressLine2} />
                    </FormFieldLabel>

                    <FormFieldLabel label="Suburb">
                      <TextField name="suburb" required={requiredOrganizationDetailsFields.suburb} />
                    </FormFieldLabel>

                    <FormFieldLabel label="City">
                      <TextField name="city" required={requiredOrganizationDetailsFields.city} />
                    </FormFieldLabel>

                    <FormFieldLabel label="Province">
                      <TextField name="province" required={requiredOrganizationDetailsFields.province} />
                    </FormFieldLabel>

                    <FormFieldLabel label="Zipcode">
                      <TextField name="zipcode" required={requiredOrganizationDetailsFields.zipcode} />
                    </FormFieldLabel>

                    <FormFieldLabel label="Country">
                      <SingleChoiceCountry name="country" required={requiredOrganizationDetailsFields.country} />
                    </FormFieldLabel>
                  </StackColumn>

                  <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}>
                    <StackRow>
                      <Button variant="contained" type="submit" sx={defaultButtonStyle}>
                        Update
                      </Button>
                    </StackRow>
                  </StackColumn>
                </FormStackColumn>
              )}
            />

            <Form
              onSubmit={handleOrganizationBillingDetailUpdateClick}
              initialValues={{
                email,
                addressLine1,
                addressLine2,
                suburb,
                city,
                province,
                zipcode,
                country,
              }}
              validate={validateOrganizationBilling}
              render={({ handleSubmit }) => (
                <FormStackColumn onSubmit={handleSubmit}>
                  <StackColumn
                    sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}
                    ref={(divElement) => {
                      sectionRefs.current['billing-payment-setup'] = divElement;
                    }}
                  >
                    <SectionIconTypography label="Billing & Payment Setup" />
                    <BodyIconTypography label="Edit your organization billing and payment details" />
                    <Divider />
                  </StackColumn>

                  <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}>
                    <FormFieldLabel label="Email">
                      <TextField name="email" required={requiredOrganizationBillingFields.email} helperText="Email to send invoice to" />
                    </FormFieldLabel>

                    <FormFieldLabel label="Address line 1">
                      <TextField name="addressLine1" required={requiredOrganizationBillingFields.addressLine1} />
                    </FormFieldLabel>

                    <FormFieldLabel label="Address line 2">
                      <TextField name="addressLine2" required={requiredOrganizationBillingFields.addressLine2} />
                    </FormFieldLabel>

                    <FormFieldLabel label="Suburb">
                      <TextField name="suburb" required={requiredOrganizationBillingFields.suburb} />
                    </FormFieldLabel>

                    <FormFieldLabel label="City">
                      <TextField name="city" required={requiredOrganizationBillingFields.city} />
                    </FormFieldLabel>

                    <FormFieldLabel label="Province">
                      <TextField name="province" required={requiredOrganizationBillingFields.province} />
                    </FormFieldLabel>

                    <FormFieldLabel label="Zipcode">
                      <TextField name="zipcode" required={requiredOrganizationBillingFields.zipcode} />
                    </FormFieldLabel>

                    <FormFieldLabel label="Country">
                      <SingleChoiceCountry name="country" required={requiredOrganizationBillingFields.country} />
                    </FormFieldLabel>
                  </StackColumn>

                  <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}>
                    <StackRow>
                      <Button variant="contained" type="submit" sx={defaultButtonStyle}>
                        Update
                      </Button>
                    </StackRow>
                  </StackColumn>
                </FormStackColumn>
              )}
            />

            <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}>
              <GridContainer sx={{ justifyContent: 'space-between' }}>
                <Grid>
                  <SectionIconTypography label="Payment Method" />
                  <BodyIconTypography label="Edit your payment method" />
                </Grid>

                <Grid>
                  {!paymentMethodExist && (
                    <Button variant="text" onClick={handleAddPaymentMethodClicked} sx={{ textTransform: 'none' }}>
                      <LeadIconTypography label="Add Payment Method" endElement={<NewIcon fontSize="large" />} />
                    </Button>
                  )}
                </Grid>
              </GridContainer>
              <Divider />
            </StackColumn>

            {paymentMethodExist && (
              <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}>
                <StackRow>
                  {rootDataOrganizationPaymentMethodsDetails.organizationPaymentMethodsDetails.map((item) => (
                    <StackColumn key={item.id}>
                      <CreditCard lastFourDigits={item.cardLastFourDigit} expiryDate={`${item.cardExpiryMonth}/${item.cardExpiryYear}`} cardBrand={item.cardBrand} />
                      <Button variant="contained" color="warning" onClick={() => handleRemovePaymentMethodClick(item.id)}>
                        <BodyIconTypography label="Remove Payment Method" invertDefaultColor={paletteMode === 'dark'} startElement={<DeleteIcon />} />
                      </Button>
                    </StackColumn>
                  ))}
                </StackRow>
              </StackColumn>
            )}

            {!paymentMethodExist && (
              <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}>
                <SmallIconTypography label="No payment method setup yet" />
              </StackColumn>
            )}

            <Form
              onSubmit={handleEnableOrganizationSsoSettingsClick}
              initialValues={{
                entityId: ssoSettingsEntityId,
                loginUrl: ssoSettingsLoginUrl,
                appFederationMetadataUrl: ssoSettingsappFederationMetadataUrl,
              }}
              validate={validateOrganizationSsoSettings}
              render={({ handleSubmit, values }) => {
                return (
                  <FormStackColumn onSubmit={handleSubmit}>
                    <StackColumn
                      sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}
                      ref={(divElement) => {
                        sectionRefs.current['sso-setup'] = divElement;
                      }}
                    >
                      <SectionIconTypography label="SSO Setup" />
                      <BodyIconTypography label="Edit your organization SSO settings" />
                      <Divider />
                    </StackColumn>

                    <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}>
                      <FormFieldLabel label="Enable Sign sign-on">
                        <Switch defaultChecked={ssoEnabled} onChange={handleEnableSsoChange} />
                      </FormFieldLabel>

                      {ssoEnabled && (
                        <>
                          <FormFieldLabel label="Entity Id">
                            <TextField name="entityId" required={requiredOrganizationSsoSettingsFields.entityId} />
                          </FormFieldLabel>

                          <FormFieldLabel label="Login Url">
                            <TextField name="loginUrl" required={requiredOrganizationSsoSettingsFields.loginUrl} />
                          </FormFieldLabel>

                          <FormFieldLabel label="App Federation Metadata Url">
                            <TextField name="appFederationMetadataUrl" required={requiredOrganizationSsoSettingsFields.appFederationMetadataUrl} />
                          </FormFieldLabel>
                        </>
                      )}
                    </StackColumn>

                    {ssoEnabled && (
                      <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}>
                        <StackRow>
                          <Button variant="contained" type="submit" sx={defaultButtonStyle}>
                            Update
                          </Button>
                        </StackRow>
                      </StackColumn>
                    )}
                  </FormStackColumn>
                );
              }}
            />

            <StackColumn
              sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}
              ref={(divElement) => {
                sectionRefs.current['zones-setup'] = divElement;
              }}
            >
              <GridContainer sx={{ justifyContent: 'space-between' }}>
                <Grid>
                  <SectionIconTypography label="Zones Setup" />
                  <BodyIconTypography label="Edit your organization zones details" />
                </Grid>

                <Grid>
                  <AddOrganizationZoneButton organizationId={organizationId} connectionIds={zonesConnectionIds} />
                </Grid>
              </GridContainer>
              <Divider />
            </StackColumn>

            <GridContainer spacing={1} sx={{ padding: defaultPadding }}>
              <PushToRight />
              <Search size="small" placeholder="Search for zones" defaultValue={zoneNameSearchText} onChange={handleZonesSearchTextChange} />
            </GridContainer>

            {seledctedZones.ids.size > 0 && (
              <StackRow sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding }}>
                <Box
                  sx={{
                    backgroundColor: 'white',
                    padding: defaultGridActionPadding,
                    border: 1,
                    borderColor: (theme) => theme.palette.divider,
                    borderRadius: 2,
                    flexGrow: 1,
                  }}
                >
                  <StackRow sx={{ alignItems: 'center' }}>
                    <SmallIconTypography label={`${seledctedZones.ids.size} records selected`} />
                    <PushToRight />
                    <Button size="medium" variant="contained" color="warning" startIcon={<DeleteIcon />} onClick={handleRemoveZonesClick} sx={{ textTransform: 'none' }}>
                      Remove Zone
                    </Button>
                  </StackRow>
                </Box>
              </StackRow>
            )}

            <StackRow sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding }}>
              <DataGrid
                checkboxSelection
                rowSelectionModel={seledctedZones}
                onRowSelectionModelChange={handleSelectedZonesChanged}
                rows={zoneRows}
                columns={zoneColumns}
                hideFooterPagination={zoneRows.length <= 10}
                initialState={{
                  pagination: {
                    rowCount: zoneRows.length,
                    paginationModel: {
                      pageSize: 10,
                    },
                  },
                }}
                pageSizeOptions={[10]}
                ignoreDiacritics
                disableRowSelectionOnClick
                getRowHeight={() => 'auto'}
                rowSpacingType="margin"
                getRowSpacing={() => ({ top: 3, bottom: 3 })}
                sx={defaultGridStyle}
                localeText={{ noRowsLabel: 'No zone found' }}
              />
            </StackRow>

            <StackColumn
              sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}
              ref={(divElement) => {
                sectionRefs.current['tags-setup'] = divElement;
              }}
            >
              <GridContainer sx={{ justifyContent: 'space-between' }}>
                <Grid>
                  <SectionIconTypography label="Tags Setup" />
                  <BodyIconTypography label="Edit your organization tags details" />
                </Grid>

                <Grid>
                  <AddOrganizationCustomTagButton organizationId={organizationId} connectionIds={customTagsConnectionIds} />
                </Grid>
              </GridContainer>
              <Divider />
            </StackColumn>

            <GridContainer spacing={1} sx={{ padding: defaultPadding }}>
              <PushToRight />
              <Search size="small" placeholder="Search for tags" defaultValue={customTagNameSearchText} onChange={handleCustomTagsSearchTextChange} />
            </GridContainer>

            {seledctedCustomTags.ids.size > 0 && (
              <StackRow sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding }}>
                <Box
                  sx={{
                    backgroundColor: 'white',
                    padding: defaultGridActionPadding,
                    border: 1,
                    borderColor: (theme) => theme.palette.divider,
                    borderRadius: 2,
                    flexGrow: 1,
                  }}
                >
                  <StackRow sx={{ alignItems: 'center' }}>
                    <SmallIconTypography label={`${seledctedCustomTags.ids.size} records selected`} />
                    <PushToRight />
                    <Button size="medium" variant="contained" color="warning" startIcon={<DeleteIcon />} onClick={handleRemoveCustomTagsClick} sx={{ textTransform: 'none' }}>
                      Remove Tag
                    </Button>
                  </StackRow>
                </Box>
              </StackRow>
            )}

            <StackRow sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding }}>
              <DataGrid
                checkboxSelection
                rowSelectionModel={seledctedCustomTags}
                onRowSelectionModelChange={handleSelectedCustomTagsChanged}
                rows={customTagRows}
                columns={customTagColumns}
                hideFooterPagination={customTagRows.length <= 10}
                initialState={{
                  pagination: {
                    rowCount: customTagRows.length,
                    paginationModel: {
                      pageSize: 10,
                    },
                  },
                }}
                pageSizeOptions={[10]}
                ignoreDiacritics
                disableRowSelectionOnClick
                getRowHeight={() => 'auto'}
                rowSpacingType="margin"
                getRowSpacing={() => ({ top: 3, bottom: 3 })}
                sx={defaultGridStyle}
                localeText={{ noRowsLabel: 'No tag found' }}
              />
            </StackRow>

            <StackColumn
              sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}
              ref={(divElement) => {
                sectionRefs.current['subscriptions'] = divElement;
              }}
            >
              <SectionIconTypography label="Subscriptions" />
              <Divider />
            </StackColumn>

            <GridContainer sx={{ padding: defaultPadding, justifyContent: 'center', alignItems: 'stretch' }}>
              {activeOffering && (
                <Grid>
                  <Card sx={{ width: { xs: '100%', sm: 300 }, height: '100%', backgroundColor: 'white' }}>
                    <CardContent sx={{ marginLeft: 1 }}>
                      <BodyIconTypography label={activeOffering.name} sx={{ color: coal }} />
                      <StackRow spacing={0.5} sx={{ marginTop: -2 }}>
                        <ExtraLargeHeadingIconTypography label={(activeOffering.unitPrice / 100).toFixed(0)} sx={{ paddingTop: 4, color: coal }} />
                        <BodyIconTypography label="$" sx={{ color: coal }} />
                      </StackRow>

                      <List sx={{ padding: 0 }}>
                        <Box sx={{ marginTop: 2, marginBottom: 4 }}>
                          {activeOffering.underPriceLines.map((item, index) => (
                            <ListItem key={index} alignItems="flex-start" sx={{ padding: 0 }}>
                              <ListItemText>
                                <SmallIconTypography label={item} sx={{ color: coal }} />
                              </ListItemText>
                            </ListItem>
                          ))}
                        </Box>

                        {activeOffering.featureSet.map((item, index) => (
                          <ListItem key={index} alignItems="flex-start" sx={{ padding: 0 }}>
                            <ListItemIcon sx={{ minWidth: 'auto', marginRight: 1 }}>
                              <TickIcon fontSize="small" sx={{ color: activeOffering.isEnterprise ? coal : emerald }} />
                            </ListItemIcon>
                            <ListItemText>
                              <SmallIconTypography label={item} sx={{ color: coal }} />
                            </ListItemText>
                          </ListItem>
                        ))}
                      </List>

                      <CardActions sx={{ justifyContent: 'center' }}>
                        {!activeOffering.free && (
                          <Button color="secondary" variant="contained" onClick={handleCancelActiveOfferingClick} sx={defaultButtonStyle}>
                            Cancel
                          </Button>
                        )}
                      </CardActions>
                    </CardContent>
                  </Card>
                </Grid>
              )}

              {availableOfferings.map((availableOffering) => (
                <Grid key={availableOffering.code}>
                  <Card sx={{ width: { xs: '100%', sm: 300 }, height: '100%', backgroundColor: 'white' }}>
                    <CardContent sx={{ marginLeft: 1 }}>
                      <BodyIconTypography label={availableOffering.name} sx={{ color: coal }} />
                      <StackRow spacing={0.5} sx={{ marginTop: -2 }}>
                        {availableOffering.unitPrice > 0 && (
                          <ExtraLargeHeadingIconTypography label={(availableOffering.unitPrice / 100).toFixed(0)} sx={{ paddingTop: 4, color: coal }} />
                        )}
                        {availableOffering.isEnterprise && <ExtraLargeHeadingIconTypography label="TBC" sx={{ paddingTop: 4, color: coal }} />}
                        <BodyIconTypography label="$" sx={{ color: coal }} />
                      </StackRow>

                      <List sx={{ padding: 0 }}>
                        <Box sx={{ marginTop: 2, marginBottom: 4 }}>
                          {availableOffering.underPriceLines.map((item, index) => (
                            <ListItem key={index} alignItems="flex-start" sx={{ padding: 0 }}>
                              <ListItemText>
                                <SmallIconTypography label={item} sx={{ color: coal }} />
                              </ListItemText>
                            </ListItem>
                          ))}
                        </Box>

                        {availableOffering.featureSet.map((item, index) => (
                          <ListItem key={index} alignItems="flex-start" sx={{ padding: 0 }}>
                            <ListItemIcon sx={{ minWidth: 'auto', marginRight: 1 }}>
                              <TickIcon fontSize="small" sx={{ color: availableOffering.isEnterprise ? coal : emerald }} />
                            </ListItemIcon>
                            <ListItemText>
                              <SmallIconTypography label={item} sx={{ color: coal }} />
                            </ListItemText>
                          </ListItem>
                        ))}

                        {!rootData.organization?.hasAttachedPaymentMethod && (
                          <ListItem alignItems="flex-start" sx={{ padding: 0, paddingTop: 1 }}>
                            <ListItemIcon sx={{ minWidth: 'auto', marginRight: 1 }}>
                              <ErrorIcon fontSize="large" sx={{ color: 'red' }} />
                            </ListItemIcon>
                            <ListItemText>
                              <SmallIconTypography label="You need to have payment method setup in order to upgrade to this offering." color="red" />
                            </ListItemText>
                          </ListItem>
                        )}
                      </List>
                    </CardContent>

                    <CardActions sx={{ justifyContent: 'center' }}>
                      {!rootData.organization?.hasAttachedPaymentMethod && (
                        <Button variant="contained" onClick={handleAddPaymentMethodClicked} sx={{ textTransform: 'none', color: 'white' }}>
                          Add Payment Method
                        </Button>
                      )}

                      {rootData.organization?.hasAttachedPaymentMethod && !availableOffering.isEnterprise && (
                        <Button
                          color="primary"
                          variant="contained"
                          onClick={() => handleUpgradeOfferingClick(availableOffering.code)}
                          sx={{ textTransform: 'none', color: 'white' }}
                        >
                          Upgrade
                        </Button>
                      )}

                      {rootData.organization?.hasAttachedPaymentMethod && availableOffering.isEnterprise && (
                        <Button href="mailto:support@getskedular.com" variant="contained" sx={{ textTransform: 'none', backgroundColor: 'black', color: 'white' }}>
                          Contact Us
                        </Button>
                      )}
                    </CardActions>
                  </Card>
                </Grid>
              ))}
            </GridContainer>

            <StackColumn
              sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}
              ref={(divElement) => {
                sectionRefs.current['manage-organization'] = divElement;
              }}
            >
              <SectionIconTypography label="Manage" />
              <BodyIconTypography label="Remove your organization" />
              <Divider />
            </StackColumn>

            <StackRow sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}>
              <Button size="medium" variant="contained" color="warning" startIcon={<DeleteIcon />} onClick={handleRemoveOrganizationClicked} sx={{ textTransform: 'none' }}>
                Remove Organization
              </Button>
            </StackRow>
          </AppBarWithStackColumn>
        </Box>
      </Box>

      <MoreActionsMenu anchorEl={zoneMoreActionsAnchorEl} open={zoneMoreActionsMenuOpen} onMenuItemClick={handleZoneMoreActionsMenuItemClick} options={zoneMoreActionsOption} />

      {selectedZoneId && (
        <EditOrganizationZoneDialog
          onReloadRequired={onReloadRequired}
          zoneId={selectedZoneId}
          isDialogOpen={isEditZoneDialogOpen}
          onAddClicked={handleEditZoneClick}
          onCancel={onEditZoneCancel}
        />
      )}

      <MoreActionsMenu
        anchorEl={customTagMoreActionsAnchorEl}
        open={customTagMoreActionsMenuOpen}
        onMenuItemClick={handleCustomTagMoreActionsMenuItemClick}
        options={customTagMoreActionsOption}
      />

      {selectedCustomTagId && (
        <EditOrganizationCustomTagDialog
          onReloadRequired={onReloadRequired}
          customTagId={selectedCustomTagId}
          isDialogOpen={isEditCustomTagDialogOpen}
          onAddClicked={handleEditCustomTagClick}
          onCancel={handleEditCustomTagCancel}
        />
      )}

      {!paymentMethodExist && isAddPaymentMethodDialogOpen && (
        <AddOrganizationPaymentMethodDialog organizationId={organizationId} isDialogOpen={isAddPaymentMethodDialogOpen} onCancel={handleAddPaymentMethodCancel} />
      )}
    </>
  );
};

export default memo(OrganizationAdmin);
