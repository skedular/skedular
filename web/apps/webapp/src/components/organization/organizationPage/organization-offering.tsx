import type { organizationOffering_cancelOrganizationOfferingMutation } from '@/queries/__generated__/organizationOffering_cancelOrganizationOfferingMutation.graphql';
import type { organizationOffering_query$key } from '@/queries/__generated__/organizationOffering_query.graphql';
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
    <Container>
      <Typography variant="h6" gutterBottom>
        Active offering
      </Typography>

      <Paper
        elevation={12}
        sx={{
          minWidth: 500,
          maxWidth: 500,
        }}
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
            <Typography gutterBottom variant="body1">
              {offering.name}
            </Typography>
            <Typography gutterBottom variant="body1">
              {`Unit price: $${(offering.unitPrice / 100).toFixed(2)}`}
            </Typography>
            <List sx={{ listStyleType: 'disc' }}>
              Feature set:
              {offering.featureSet.map(({ name, description }, index) => (
                <ListItem key={index} sx={{ display: 'list-item' }}>
                  <ListItemText>{`${name}: ${description}`}</ListItemText>
                </ListItem>
              ))}
            </List>
            {offering.unitPrice > 0 && (
              <Button color="primary" variant="contained" onClick={() => handleCancelClick()}>
                Cancel
              </Button>
            )}
          </CardContent>
        </Card>
      </Paper>
    </Container>
  );
};

export default memo(OrganizationOffering);
