import { CustomerAvatar } from '@/components/avatars';
import { BodyIconTypography, DefaultDialogTitle, FormFieldLabel, FormStackColumn, LeadIconTypography, SmallIconTypography, TwoButtonsDialogActions } from '@/components/commons';
import { errorNotificationOptions, infoNotificationOptions, NotificationContent, successNotificationOptions } from '@/components/notification';
import { DialogTransition } from '@/components/transitions';
import { PaletteModeContext } from '@/libs/providers';
import { getCustomerFullName, joinErrors, keyboardSearchDebounceTimeout } from '@/libs/utils';
import type { addOrganizationTeamMemberDialog_addTeamMemberMutation } from '@/queries/__generated__/addOrganizationTeamMemberDialog_addTeamMemberMutation.graphql';
import type { addOrganizationTeamMemberDialog_organizationMembers_query$key } from '@/queries/__generated__/addOrganizationTeamMemberDialog_organizationMembers_query.graphql';
import type { addOrganizationTeamMemberDialog_organizationMembers_refetchableFragment } from '@/queries/__generated__/addOrganizationTeamMemberDialog_organizationMembers_refetchableFragment.graphql';
import Dialog from '@mui/material/Dialog';
import DialogContent from '@mui/material/DialogContent';
import { Autocomplete, makeRequired, makeValidate } from 'mui-rff';
import { memo, useCallback, useContext, useMemo, useState, useTransition } from 'react';
import { Form } from 'react-final-form';
import { graphql, useMutation, useRefetchableFragment } from 'react-relay';
import { toast } from 'react-toastify';
import { useDebounceCallback } from 'usehooks-ts';
import { v7 as uuid } from 'uuid';
import { object, string } from 'yup';

type Props = {
  rootDataRelay: addOrganizationTeamMemberDialog_organizationMembers_query$key;
  connectionIds: string[];
  teamId: string;
  isDialogOpen: boolean;
  onAddClicked: () => void;
  onCancel: () => void;
};

type CustomerDetails = {
  uniqueId: string;
  name: string | null | undefined;
  givenName: string | null | undefined;
  middleName: string | null | undefined;
  familyName: string | null | undefined;
  photoUrl: string | null | undefined;
};

type OrganizationMemberDetails = {
  id: string;
  customer: CustomerDetails;
};

type MemberDetails = {
  member: string;
};

const schema = object({
  member: string().required('Member is required'),
});

const AddOrganizationTeamMemberDialog = ({ rootDataRelay, connectionIds, teamId, isDialogOpen, onAddClicked, onCancel }: Props) => {
  const [rootData, refetch] = useRefetchableFragment<
    addOrganizationTeamMemberDialog_organizationMembers_refetchableFragment,
    addOrganizationTeamMemberDialog_organizationMembers_query$key
  >(
    graphql`
      fragment addOrganizationTeamMemberDialog_organizationMembers_query on Query
      @argumentDefinitions(cursor: { type: "String" }, count: { type: "Int", defaultValue: null })
      @refetchable(queryName: "addOrganizationTeamMemberDialog_organizationMembers_refetchableFragment") {
        organizationMembers(
          first: $count
          after: $cursor
          where: { organizationId: $organizationId, nameContains: $peopleNameSearchText }
          orderBy: $addTeamMemberDialogOrganizationMembersSortingValues
        ) @connection(key: "addTeamMemberDialogQuery_organizationMembers") {
          __id
          totalCount
          edges {
            node {
              id
              customer {
                uniqueId
                name
                givenName
                middleName
                familyName
                photoUrl
              }
            }
          }
        }
      }
    `,
    rootDataRelay,
  );

  const [commitAddTeamMember] = useMutation<addOrganizationTeamMemberDialog_addTeamMemberMutation>(graphql`
    mutation addOrganizationTeamMemberDialog_addTeamMemberMutation($connectionIds: [ID!]!, $input: AddTeamMemberInput!) {
      addTeamMember(input: $input) {
        teamMember @appendNode(connections: $connectionIds, edgeTypeName: "TeamMemberDetails") {
          id
          customer {
            uniqueId
            email
            name
            givenName
            middleName
            familyName
            photoUrl
            phoneNumber
          }
          status
          role
        }
      }
    }
  `);

  const [, startTransition] = useTransition();
  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const validate = makeValidate(schema);
  const requiredFields = makeRequired(schema);
  const [peopleNameSearchText, setPeopleNameSearchText] = useState<string>('');
  const customers = useMemo<OrganizationMemberDetails[]>(
    () => (rootData.organizationMembers ? rootData.organizationMembers.edges.map(({ node }) => node) : []),
    [rootData.organizationMembers],
  );

  const handleRefetch = useCallback(
    (peopleNameSearchText: string) => {
      startTransition(() => {
        refetch(
          {
            peopleNameSearchText,
          },
          {
            fetchPolicy: 'store-and-network',
          },
        );
      });
    },
    [refetch],
  );

  const handleSearchTextChange = (str: string) => {
    setPeopleNameSearchText(str);

    handleRefetch(str);
  };

  const handleAddClick = ({ member }: MemberDetails) => {
    const toastId = themedToast(<NotificationContent content={'Adding team member...'} />, infoNotificationOptions);

    commitAddTeamMember({
      variables: {
        connectionIds,
        input: {
          clientMutationId: uuid(),
          id: teamId,
          organizationMemberId: member,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to add team member. Error: ${joinErrors(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={'Team member added.'} />,
        });

        onAddClicked();
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to add team member. Error: ${error.message}.`} />,
        });
      },
    });
  };

  const debounceSearchTextChange = useDebounceCallback(handleSearchTextChange, keyboardSearchDebounceTimeout);

  return (
    <Dialog slots={{ transition: DialogTransition }} open={isDialogOpen} onClose={onCancel} fullWidth>
      <DefaultDialogTitle title="Add Team Member" />
      <DialogContent sx={{ marginTop: 2 }}>
        <Form
          onSubmit={handleAddClick}
          initialValues={{
            member: '',
          }}
          validate={validate}
          render={({ handleSubmit }) => {
            return (
              <FormStackColumn onSubmit={handleSubmit}>
                <LeadIconTypography label="Add member to this team" />
                <SmallIconTypography label="Enter a name to invite them to this team. Please note, you can only invite members from your organization." />

                <FormFieldLabel label="Member" useWiderSpace sx={{ paddingTop: 2 }}>
                  <Autocomplete
                    name="member"
                    multiple={false}
                    required={requiredFields.member}
                    options={customers}
                    getOptionValue={(option) => (option as OrganizationMemberDetails).id}
                    getOptionLabel={(option: string | OrganizationMemberDetails) => getCustomerFullName((option as OrganizationMemberDetails).customer)}
                    renderOption={(props, option) => {
                      const castedOption = (option as OrganizationMemberDetails).customer;

                      return (
                        <li {...props} key={castedOption.uniqueId}>
                          <BodyIconTypography
                            label={getCustomerFullName(castedOption)}
                            startElement={<CustomerAvatar name={castedOption} photo={{ url: castedOption.photoUrl }} size="small" />}
                          />
                        </li>
                      );
                    }}
                    filterOptions={(options, params) => {
                      if (params.inputValue !== peopleNameSearchText) {
                        debounceSearchTextChange(params.inputValue);
                      }

                      return options;
                    }}
                    selectOnFocus
                    clearOnBlur
                    handleHomeEndKeys
                  />
                </FormFieldLabel>
                <TwoButtonsDialogActions onSecondaryClicked={onCancel} primaryLabel="Add" secondaryLabel="Cancel" />
              </FormStackColumn>
            );
          }}
        />
      </DialogContent>
    </Dialog>
  );
};

export default memo(AddOrganizationTeamMemberDialog);
