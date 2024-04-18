import type { organizationAvailableOfferings_query$key } from '@/queries/__generated__/organizationAvailableOfferings_query.graphql';
import type { organizationAvailableOfferings_updateOrganizationOfferingMutation } from '@/queries/__generated__/organizationAvailableOfferings_updateOrganizationOfferingMutation.graphql';
import Button from '@mui/material/Button';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import Container from '@mui/material/Container';
import List from '@mui/material/List';
import ListItem from '@mui/material/ListItem';
import ListItemText from '@mui/material/ListItemText';
import Paper from '@mui/material/Paper';
import Typography from '@mui/material/Typography';
import { joinErrors } from '@repo/shared/libs/utils';
import { SnackbarAnchorOrigin as anchorOrigin } from '@repo/shared/libs/snackbar';
import { v4 as uuidv4 } from 'uuid';
import { useSnackbar } from 'notistack';
import { memo } from 'react';
import { graphql, useFragment, useMutation } from 'react-relay';

type Props = {
  rootDataRelay: organizationAvailableOfferings_query$key;
  onRefetchRequired: () => void;
};

const OrganizationAvailableOfferings = ({ rootDataRelay, onRefetchRequired }: Props) => {
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
          clientMutationId: uuidv4(),
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

          onRefetchRequired();
        } else {
          onRefetchRequired();
        }
      },
      onError: (error) => {
        enqueueSnackbar(`Failed to upgrade offering. Error: ${error.message}`, {
          variant: 'error',
          anchorOrigin,
        });

        onRefetchRequired();
      },
    });
  };

  return (
    <Container>
      <Typography variant="h6">Available offerings</Typography>
      {!availableOfferingExist && <Typography variant="h6">No offering is available</Typography>}
      {availableOfferingExist && (
        <>
          {rootData.organization?.availableOfferings?.map(({ code, name, unitPrice, featureSet }) => {
            return (
              <Paper
                elevation={12}
                sx={{
                  minWidth: 500,
                  maxWidth: 500,
                }}
                key={code}
              >
                <Card
                  sx={{
                    minWidth: 500,
                    maxWidth: 500,
                  }}
                >
                  <CardContent
                    sx={{
                      margin: '5px',
                    }}
                  >
                    <Typography variant="body1">{name}</Typography>
                    <Typography variant="body1">{`Unit price: $${(unitPrice / 100).toFixed(2)}`}</Typography>
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
                    {rootData.organization?.hasAttachedPaymentMethod && (
                      <Button color="primary" variant="contained" onClick={() => handleUpgradeClick(code)}>
                        Upgrade
                      </Button>
                    )}
                  </CardContent>
                </Card>
              </Paper>
            );
          })}
        </>
      )}
    </Container>
  );
};

export default memo(OrganizationAvailableOfferings);
