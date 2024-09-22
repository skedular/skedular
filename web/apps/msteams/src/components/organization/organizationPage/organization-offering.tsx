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
import { useSnackbar } from 'notistack';
import { memo } from 'react';
import { useFragment, useMutation } from 'react-relay';
import { v4 as uuidv4 } from 'uuid';
import type { organizationOffering_cancelOrganizationOfferingMutation } from './__generated__/organizationOffering_cancelOrganizationOfferingMutation.graphql';
import type { organizationOffering_query$key } from './__generated__/organizationOffering_query.graphql';

type Props = {
  rootDataRelay: organizationOffering_query$key;
  onRefetchRequired: () => void;
};

const OrganizationOffering = ({ rootDataRelay, onRefetchRequired }: Props) => {
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

  const { enqueueSnackbar } = useSnackbar();

  const handleCancelClick = () => {
    if (!rootData.organization) {
      return;
    }

    commitCancelOrganizationOffering({
      variables: {
        input: {
          clientMutationId: uuidv4(),
          id: rootData.organization.id,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          enqueueSnackbar(`Failed to cancel offering. Error: ${joinErrors(errors)}`, {
            variant: 'error',
            anchorOrigin,
          });

          onRefetchRequired();
        } else {
          onRefetchRequired();
        }
      },
      onError: (error) => {
        enqueueSnackbar(`Failed to cancel offering. Error: ${error.message}`, {
          variant: 'error',
          anchorOrigin,
        });

        onRefetchRequired();
      },
    });
  };

  const offering = rootData.organization?.offering;

  if (!offering) {
    return null;
  }

  return (
    <>
      <Typography variant="h6">Active offering</Typography>
      <Card elevation={24} sx={{ maxWidth: 500, height: '100%' }}>
        <CardHeader
          title={
            <>
              <Typography variant="body1">{offering.name}</Typography>
              <Typography variant="body1">{`Unit price: $${(offering.unitPrice / 100).toFixed(2)}`}</Typography>
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

          {offering.unitPrice > 0 && (
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
