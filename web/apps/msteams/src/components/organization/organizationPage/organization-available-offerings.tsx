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
import type { organizationAvailableOfferings_query$key } from './__generated__/organizationAvailableOfferings_query.graphql';
import type { organizationAvailableOfferings_updateOrganizationOfferingMutation } from './__generated__/organizationAvailableOfferings_updateOrganizationOfferingMutation.graphql';

type Props = {
  rootDataRelay: organizationAvailableOfferings_query$key;
  onReloadRequired: () => void;
};

const OrganizationAvailableOfferings = ({ rootDataRelay, onReloadRequired }: Props) => {
  const rootData = useFragment<organizationAvailableOfferings_query$key>(
    graphql`
      fragment organizationAvailableOfferings_query on Query {
        organization(id: $organizationId) {
          id
          name
          hasAttachedPaymentMethod
          availableOfferings {
            code
            name
            unitPrice
            featureSet {
              name
              description
            }
          }
        }
      }
    `,
    rootDataRelay,
  );

  const [commitUpdateOrganizationOffering] = useMutation<organizationAvailableOfferings_updateOrganizationOfferingMutation>(graphql`
    mutation organizationAvailableOfferings_updateOrganizationOfferingMutation($input: UpdateOrganizationOfferingInput!) {
      updateOrganizationOffering(input: $input) {
        clientMutationId
      }
    }
  `);

  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const availableOfferingExist = rootData.organization?.availableOfferings && rootData.organization?.availableOfferings?.length > 0;

  const handleUpgradeClick = (code: string) => {
    if (!rootData.organization) {
      return;
    }

    const toastId = themedToast(
      <NotificationContent content={`Updating organization '${rootData.organization.name} offering'...`} />,
      infoNotificationOptions,
    );

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
            render: (
              <NotificationContent content={`Failed to update organization ${rootData.organization?.name} offering. Error: ${joinErrors(errors)}.`} />
            ),
          });

          onReloadRequired();

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Organization ${rootData.organization?.name} offering updated.`} />,
        });

        onReloadRequired();
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to update organization ${rootData.organization?.name} offering. Error: ${error.message}.`} />,
        });

        onReloadRequired();
      },
    });
  };

  return (
    <>
      <LeadIconTypography label="Available offerings" />
      {!availableOfferingExist && <LeadIconTypography label="No offering is available" />}
      {availableOfferingExist && (
        <>
          {rootData.organization?.availableOfferings?.map(({ code, name, unitPrice, featureSet }) => {
            return (
              <Card sx={{ maxWidth: 500, height: '100%' }} key={code}>
                <CardHeader
                  title={
                    <>
                      <BodyIconTypography label={name} invertDefaultColor />
                      <BodyIconTypography label={`Unit price: $${(unitPrice / 100).toFixed(2)}`} invertDefaultColor />
                    </>
                  }
                />

                <CardContent sx={{ marginLeft: 1 }}>
                  <List sx={{ listStyleType: 'disc' }}>
                    Feature set:
                    {featureSet.map(({ name, description }, index) => (
                      <ListItem key={index} sx={{ display: 'list-item' }}>
                        <ListItemText>{`${name}: ${description}`}</ListItemText>
                      </ListItem>
                    ))}
                  </List>
                  {!rootData.organization?.hasAttachedPaymentMethod && (
                    <BodyIconTypography label="You need to have payment method setup in order to upgrade to this offering. Please setup payment method under Billing tab." />
                  )}
                </CardContent>

                {rootData.organization?.hasAttachedPaymentMethod && (
                  <CardActions sx={{ justifyContent: 'flex-end' }}>
                    <Button color="primary" variant="contained" onClick={() => handleUpgradeClick(code)}>
                      Upgrade
                    </Button>
                  </CardActions>
                )}
              </Card>
            );
          })}
        </>
      )}
    </>
  );
};

export default memo(OrganizationAvailableOfferings);
