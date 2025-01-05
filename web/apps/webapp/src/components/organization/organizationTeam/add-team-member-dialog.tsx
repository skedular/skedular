import type { addTeamMemberDialog_addTeamMemberMutation } from '@/queries/__generated__/addTeamMemberDialog_addTeamMemberMutation.graphql';
import type { addTeamMemberDialog_organizationMembers_query$key } from '@/queries/__generated__/addTeamMemberDialog_organizationMembers_query.graphql';
import type { addTeamMemberDialog_organizationMembers_refetchableFragment } from '@/queries/__generated__/addTeamMemberDialog_organizationMembers_refetchableFragment.graphql';
import Dialog from '@mui/material/Dialog';
import DialogContent from '@mui/material/DialogContent';
import DialogTitle from '@mui/material/DialogTitle';
import { CustomerAvatar } from '@repo/shared/components/avatars';
import { BodyIconTypography, FormFieldLabel, FormStackColumn, TwoButtonsDialogActions } from '@repo/shared/components/commons';
import {
  errorNotificationOptions,
  infoNotificationOptions,
  NotificationContent,
  successNotificationOptions,
} from '@repo/shared/components/notification';
import { DialogTransition } from '@repo/shared/components/transitions';
import { PaletteModeContext } from '@repo/shared/libs/providers';
import { getCustomerFullName, joinErrors, keyboardDebounceTimeout } from '@repo/shared/libs/utils';
import { Autocomplete, makeRequired, makeValidate } from 'mui-rff';
import { nanoid } from 'nanoid';
import { memo, useCallback, useContext, useMemo, useState, useTransition } from 'react';
import { Form } from 'react-final-form';
import { graphql, useMutation, usePaginationFragment } from 'react-relay';
import { toast } from 'react-toastify';
import { useDebounceCallback } from 'usehooks-ts';
import { object, string } from 'yup';

type Props = {
  rootDataRelay: addTeamMemberDialog_organizationMembers_query$key;
  connectionIds: string[];
  teamId: string;
  isDialogOpen: boolean;
  onAddClicked: () => void;
  onCancelClicked: () => void;
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

const AddTeamMemberDialog = ({ rootDataRelay, connectionIds, teamId, isDialogOpen, onAddClicked, onCancelClicked }: Props) => {
  const { data: rootData, refetch } = usePaginationFragment<
    addTeamMemberDialog_organizationMembers_refetchableFragment,
    addTeamMemberDialog_organizationMembers_query$key
  >(
    graphql`
      fragment addTeamMemberDialog_organizationMembers_query on Query
      @argumentDefinitions(cursor: { type: "String" }, count: { type: "Int", defaultValue: 20 })
      @refetchable(queryName: "addTeamMemberDialog_organizationMembers_refetchableFragment") {
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

  const [commitAddTeamMember] = useMutation<addTeamMemberDialog_addTeamMemberMutation>(graphql`
    mutation addTeamMemberDialog_addTeamMemberMutation($connectionIds: [ID!]!, $input: AddTeamMemberInput!) {
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

  const customers = useMemo<OrganizationMemberDetails[]>(() => {
    if (!rootData.organizationMembers) {
      return [];
    }

    return rootData.organizationMembers.edges.map(({ node }) => node);
  }, [rootData.organizationMembers]);

  const handleRefetch = useCallback(
    (peopleNameSearchText: string) => {
      startTransition(() => {
        refetch(
          {
            count: 20,
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
          clientMutationId: nanoid(),
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

  const debounceSearchTextChange = useDebounceCallback(handleSearchTextChange, keyboardDebounceTimeout);

  return (
    <Dialog TransitionComponent={DialogTransition} open={isDialogOpen} fullWidth>
      <DialogTitle>Make a booking</DialogTitle>
      <DialogContent>
        <Form
          onSubmit={handleAddClick}
          initialValues={{
            member: null,
          }}
          validate={validate}
          render={({ handleSubmit }) => {
            return (
              <FormStackColumn onSubmit={handleSubmit}>
                <FormFieldLabel label="Member" useWiderSpace>
                  <Autocomplete
                    name="member"
                    multiple={false}
                    required={requiredFields.member}
                    options={customers}
                    getOptionValue={(option) => (option as OrganizationMemberDetails).id}
                    getOptionLabel={(option: string | OrganizationMemberDetails) =>
                      getCustomerFullName((option as OrganizationMemberDetails).customer)
                    }
                    renderOption={(props, option) => {
                      const castedOption = (option as OrganizationMemberDetails).customer;

                      return (
                        <li {...props}>
                          <BodyIconTypography
                            label={getCustomerFullName(castedOption)}
                            startElement={<CustomerAvatar name={castedOption} photo={{ url: castedOption.photoUrl }} size="small" />}
                          />
                        </li>
                      );
                    }}
                    disableCloseOnSelect={false}
                    freeSolo={true}
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
                <TwoButtonsDialogActions onSecondaryClicked={onCancelClicked} primaryLabel="Add" secondaryLabel="Cancel" />
              </FormStackColumn>
            );
          }}
        />
      </DialogContent>
    </Dialog>
  );
};

export default memo(AddTeamMemberDialog);
