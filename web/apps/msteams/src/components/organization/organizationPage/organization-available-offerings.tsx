import Button from '@mui/material/Button';
import Card from '@mui/material/Card';
import CardActions from '@mui/material/CardActions';
import CardContent from '@mui/material/CardContent';
import CardHeader from '@mui/material/CardHeader';
import List from '@mui/material/List';
import ListItem from '@mui/material/ListItem';
import ListItemText from '@mui/material/ListItemText';
import Typography from '@mui/material/Typography';
import { SnackbarAnchorOrigin as anchorOrigin } from '@repo/shared/libs/snackbar';
import { joinErrors } from '@repo/shared/libs/utils';
import graphql from 'babel-plugin-relay/macro';
import { nanoid } from 'nanoid';
import { useSnackbar } from 'notistack';
import { memo } from 'react';
import { useFragment, useMutation } from 'react-relay';
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

  const { enqueueSnackbar } = useSnackbar();

  const availableOfferingExist = rootData.organization?.availableOfferings && rootData.organization?.availableOfferings?.length > 0;

  const handleUpgradeClick = (code: string) => {
    if (!rootData.organization) {
      return;
    }

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
          enqueueSnackbar(`Failed to upgrading offering. Error: ${joinErrors(errors)}`, {
            variant: 'error',
            anchorOrigin,
          });

          onReloadRequired();
        } else {
          onReloadRequired();
        }
      },
      onError: (error) => {
        enqueueSnackbar(`Failed to upgrade offering. Error: ${error.message}`, {
          variant: 'error',
          anchorOrigin,
        });

        onReloadRequired();
      },
    });
  };

  return (
    <>
      <Typography variant="h6">Available offerings</Typography>
      {!availableOfferingExist && <Typography variant="h6">No offering is available</Typography>}
      {availableOfferingExist && (
        <>
          {rootData.organization?.availableOfferings?.map(({ code, name, unitPrice, featureSet }) => {
            return (
              <Card elevation={24} sx={{ maxWidth: 500, height: '100%' }} key={code}>
                <CardHeader
                  title={
                    <>
                      <Typography variant="body1">{name}</Typography>
                      <Typography variant="body1">{`Unit price: $${(unitPrice / 100).toFixed(2)}`}</Typography>
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
                    <Typography variant="body1">
                      You need to have payment method setup in order to upgrade to this offering. Please setup payment method under Billing tab.
                    </Typography>
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
