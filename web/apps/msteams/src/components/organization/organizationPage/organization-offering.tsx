import Button from '@mui/material/Button';
import Card from '@mui/material/Card';
import CardActions from '@mui/material/CardActions';
import CardContent from '@mui/material/CardContent';
import CardHeader from '@mui/material/CardHeader';
import List from '@mui/material/List';
import ListItem from '@mui/material/ListItem';
import ListItemText from '@mui/material/ListItemText';
import { BodyIconTypography, LeadIconTypography } from '@repo/shared/components/commons';
import {
  NotificationContent,
  errorNotificationOptions,
  infoNotificationOptions,
  successNotificationOptions,
} from '@repo/shared/components/notification';
import { PaletteModeContext } from '@repo/shared/libs/providers';
import { joinErrors } from '@repo/shared/libs/utils';
import graphql from 'babel-plugin-relay/macro';
import { nanoid } from 'nanoid';
import { memo, useContext } from 'react';
import { useFragment, useMutation } from 'react-relay';
import { toast } from 'react-toastify';
import type { organizationOffering_cancelOrganizationOfferingMutation } from './__generated__/organizationOffering_cancelOrganizationOfferingMutation.graphql';
import type { organizationOffering_query$key } from './__generated__/organizationOffering_query.graphql';

type Props = {
  rootDataRelay: organizationOffering_query$key;
  onReloadRequired: () => void;
};

const OrganizationOffering = ({ rootDataRelay, onReloadRequired }: Props) => {
  const rootData = useFragment<organizationOffering_query$key>(
    graphql`
      fragment organizationOffering_query on Query {
        organization(id: $organizationId) {
          id
          name
          offering {
            id
            name
            start
            end
            unitPrice
            featureSet {
              name
              description
            }
            free
          }
        }
      }
    `,
    rootDataRelay,
  );

  const [commitCancelOrganizationOffering] = useMutation<organizationOffering_cancelOrganizationOfferingMutation>(graphql`
    mutation organizationOffering_cancelOrganizationOfferingMutation($input: CancelOrganizationOfferingInput!) {
      cancelOrganizationOffering(input: $input) {
        clientMutationId
      }
    }
  `);

  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;

  const handleCancelClick = () => {
    if (!rootData.organization) {
      return;
    }

    const toastId = themedToast(
      <NotificationContent content={`Cancelling organization '${rootData.organization.name}' offering...`} />,
      infoNotificationOptions,
    );

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
            render: (
              <NotificationContent
                content={`Failed to cancel organization '${rootData.organization?.name}' offering. Error: ${joinErrors(errors)}.`}
              />
            ),
          });

          onReloadRequired();

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Organization '${rootData.organization?.name}' offering cancelled.`} />,
        });

        onReloadRequired();
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: (
            <NotificationContent content={`Failed to cancel organization '${rootData.organization?.name}' offering. Error: ${error.message}.`} />
          ),
        });

        onReloadRequired();
      },
    });
  };

  const offering = rootData.organization?.offering;

  if (!offering) {
    return null;
  }

  return (
    <>
      <LeadIconTypography label="Active offering" />
      <Card sx={{ maxWidth: 500, height: '100%' }}>
        <CardHeader
          title={
            <>
              <BodyIconTypography label={offering.name} />
              <BodyIconTypography label={`Unit price: $${(offering.unitPrice / 100).toFixed(2)}`} />
            </>
          }
        />

        <CardContent sx={{ marginLeft: 1 }}>
          <List sx={{ listStyleType: 'disc' }}>
            Feature set:
            {offering.featureSet.map(({ name, description }, index) => (
              <ListItem key={index} sx={{ display: 'list-item' }}>
                <ListItemText>{`${name}: ${description}`}</ListItemText>
              </ListItem>
            ))}
          </List>

          {!offering.free && (
            <CardActions sx={{ justifyContent: 'flex-end' }}>
              <Button color="secondary" variant="contained" onClick={() => handleCancelClick()}>
                Cancel
              </Button>
            </CardActions>
          )}
        </CardContent>
      </Card>
    </>
  );
};

export default memo(OrganizationOffering);
